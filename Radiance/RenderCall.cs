/* Author:  Leonardo Trevisan Silio
 * Date:    16/11/2026
 */
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Radiance;

using Internal;
using Bufferings;
using Primitives;
using Exceptions;

public partial class Render
{
    readonly Dictionary<int, RenderCall> calls = [];
    RenderCall? lastCall;

    /// <summary>
    /// Get the call plan for the signature of the received arguments, creating it if needed.
    /// </summary>
    RenderCall GetCall(object?[] args)
    {
        if (lastCall is not null && lastCall.Matches(args))
            return lastCall;

        var hash = RenderCall.GetSignatureHash(args);
        calls.TryGetValue(hash, out var first);
        for (var call = first; call is not null; call = call.Next)
        {
            if (call.Matches(args))
                return lastCall = call;
        }

        var created = RenderCall.Create(this, args);
        created.Next = first;
        calls[hash] = created;
        return lastCall = created;
    }

    /// <summary>
    /// Represents a compiled plan to execute a render for a specific signature of arguments.
    /// </summary>
    sealed class RenderCall
    {
        readonly Render render;
        readonly Type[] types;
        readonly int[] lengths;
        readonly Action<object?[], float[], object?[]> unpack;
        readonly float[] floats;
        readonly object?[] refs;
        readonly int[] bufferSlots;
        readonly int[] valueSlots;
        readonly int[] textureSlots;
        readonly object[] drawArgs;
        readonly int[] depths;
        int[] lastDepths = [];
        CallInfo? lastInfo;

        public RenderCall? Next { get; set; }

        RenderCall(Render render, object?[] args, object[] slots, Action<object?[], float[], object?[]> unpack)
        {
            this.render = render;
            this.unpack = unpack;
            types = new Type[args.Length];
            lengths = new int[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = args[i]!.GetType();
                lengths[i] = GetLength(args[i]!);
            }

            floats = new float[slots.Length];
            refs = new object?[slots.Length];
            depths = new int[slots.Length];
            List<int> buffers = [], values = [], textures = [];
            for (int i = 0; i < slots.Length; i++)
            {
                switch (slots[i])
                {
                    case IBufferedData:
                        buffers.Add(i);
                        break;

                    case Texture:
                        textures.Add(i);
                        depths[i] = -1;
                        break;

                    default:
                        values.Add(i);
                        break;
                }
            }
            bufferSlots = [ ..buffers ];
            valueSlots = [ ..values ];
            textureSlots = [ ..textures ];
            drawArgs = new object[bufferSlots.Length];

            var curried = render.arguments;
            for (int i = 0; i < curried.Length; i++)
            {
                if (IsEmpty(curried[i]))
                    continue;

                if (curried[i] is float value)
                    floats[i] = value;
                else refs[i] = curried[i];
            }
        }

        /// <summary>
        /// Execute the render with arguments of this signature.
        /// </summary>
        public void Execute(object?[] args)
        {
            unpack(args, floats, refs);

            int instances = -1;
            for (int i = 0; i < bufferSlots.Length; i++)
            {
                var slot = bufferSlots[i];
                var data = (IBufferedData)refs[slot]!;
                var count = data.Instances;
                if (instances != -1 && count != instances)
                    throw new InvalidDataDepthsException(data, count, instances);

                instances = count;
                depths[slot] = count;
                drawArgs[i] = data;
            }

            var info = GetInfo(args);
            var targets = info.Targets;
            foreach (var slot in valueSlots)
            {
                var target = targets[slot - 1];
                target.Float?.SetArgument(target.Component, floats[slot]);
            }

            foreach (var slot in textureSlots)
                targets[slot - 1].Texture?.UpdateData(refs[slot]!);

            info.Context.Render(drawArgs);
        }

        /// <summary>
        /// Get the shader analysis for the current instances of the buffers.
        /// </summary>
        CallInfo GetInfo(object?[] args)
        {
            if (lastInfo is not null && depths.AsSpan().SequenceEqual(lastDepths))
                return lastInfo;

            var info = render.infos.Get(depths);
            if (info is null)
            {
                var slots = DisplayArguments(render.arguments, args);
                info = AnalisysInvoke(render.function, RemovePolygonParameter(slots));
                render.infos.Add(depths, info);
            }

            lastDepths = [ ..depths ];
            lastInfo = info;
            return info;
        }

        /// <summary>
        /// Returns true if the arguments have the signature of this call.
        /// </summary>
        public bool Matches(object?[] args)
        {
            if (args.Length != types.Length)
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg is null || arg.GetType() != types[i])
                    return false;

                if (lengths[i] != -1 && GetLength(arg) != lengths[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Get a hash of the signature of a sequence of arguments.
        /// </summary>
        public static int GetSignatureHash(object?[] args)
        {
            var hash = new HashCode();
            foreach (var arg in args)
            {
                hash.Add(arg?.GetType());
                if (arg is not null)
                    hash.Add(GetLength(arg));
            }

            return hash.ToHashCode();
        }

        /// <summary>
        /// Create a call plan validating the arguments and compiling the function that
        /// places each argument on its parameter slot.
        /// </summary>
        public static RenderCall Create(Render render, object?[] args)
        {
            var slots = DisplayArguments(render.arguments, args);
            if (slots[0] is not IBufferedData and not SkipCurryingParameter)
                throw new MissingPolygonException();

            if (HasSkipValues(slots))
                throw new InvalidCurryPhaseException();

            if (slots[0] is not IPolygon)
                throw new MissingPolygonException();

            var unpack = CompileUnpack(render.arguments, args);
            return new RenderCall(render, args, slots, unpack);
        }

        /// <summary>
        /// Compile a function that writes each argument, already split by size,
        /// on the empty slots of the render.
        /// </summary>
        static Action<object?[], float[], object?[]> CompileUnpack(object[] curried, object?[] args)
        {
            var argsParam = Expression.Parameter(typeof(object?[]), "args");
            var floatsParam = Expression.Parameter(typeof(float[]), "floats");
            var refsParam = Expression.Parameter(typeof(object?[]), "refs");
            List<ParameterExpression> variables = [];
            List<Expression> body = [ Expression.Empty() ];

            List<int> emptySlots = [];
            for (int i = 0; i < curried.Length; i++)
            {
                if (IsEmpty(curried[i]))
                    emptySlots.Add(i);
            }

            int next = 0;
            for (int i = 0; i < args.Length; i++)
            {
                var arg = Expression.ArrayIndex(argsParam, Expression.Constant(i));
                switch (args[i])
                {
                    case BufferedDataArray array:
                        var arrayVar = Local(typeof(BufferedDataArray), arg);
                        for (int j = 0; j < array.Count; j++)
                            SetRef(Expression.Property(arrayVar, "Item", Expression.Constant(j)));
                        break;

                    case IBufferedData or Texture:
                        SetRef(arg);
                        break;

                    case string:
                        var get = typeof(TextPolygonCache).GetMethod(nameof(TextPolygonCache.Get))!;
                        SetRef(Expression.Call(get, Expression.Convert(arg, typeof(string))));
                        break;

                    case Vec2:
                        var vec2Var = Local(typeof(Vec2), arg);
                        SetFloat(Expression.Property(vec2Var, nameof(Vec2.X)));
                        SetFloat(Expression.Property(vec2Var, nameof(Vec2.Y)));
                        break;

                    case Vec3:
                        var vec3Var = Local(typeof(Vec3), arg);
                        SetFloat(Expression.Property(vec3Var, nameof(Vec3.X)));
                        SetFloat(Expression.Property(vec3Var, nameof(Vec3.Y)));
                        SetFloat(Expression.Property(vec3Var, nameof(Vec3.Z)));
                        break;

                    case Vec4:
                        var vec4Var = Local(typeof(Vec4), arg);
                        SetFloat(Expression.Property(vec4Var, nameof(Vec4.X)));
                        SetFloat(Expression.Property(vec4Var, nameof(Vec4.Y)));
                        SetFloat(Expression.Property(vec4Var, nameof(Vec4.Z)));
                        SetFloat(Expression.Property(vec4Var, nameof(Vec4.W)));
                        break;

                    case float:
                        SetFloat(Expression.Unbox(arg, typeof(float)));
                        break;

                    case int:
                        SetFloat(Expression.Convert(Expression.Unbox(arg, typeof(int)), typeof(float)));
                        break;

                    case double:
                        SetFloat(Expression.Convert(Expression.Unbox(arg, typeof(double)), typeof(float)));
                        break;

                    case float[] values:
                        var valuesVar = Local(typeof(float[]), arg);
                        for (int j = 0; j < values.Length; j++)
                            SetFloat(Expression.ArrayIndex(valuesVar, Expression.Constant(j)));
                        break;

                    default:
                        throw new InvalidPrimitiveException(args[i]);
                }
            }

            var lambda = Expression.Lambda<Action<object?[], float[], object?[]>>(
                Expression.Block(variables, body),
                argsParam, floatsParam, refsParam
            );
            return lambda.Compile();

            ParameterExpression Local(Type type, Expression value)
            {
                var variable = Expression.Variable(type);
                variables.Add(variable);
                body.Add(Expression.Assign(variable, Expression.Convert(value, type)));
                return variable;
            }

            void SetFloat(Expression value)
            {
                var slot = Expression.Constant(emptySlots[next++]);
                body.Add(Expression.Assign(Expression.ArrayAccess(floatsParam, slot), value));
            }

            void SetRef(Expression value)
            {
                var slot = Expression.Constant(emptySlots[next++]);
                body.Add(Expression.Assign(Expression.ArrayAccess(refsParam, slot), Expression.Convert(value, typeof(object))));
            }
        }

        static bool IsEmpty(object? slot)
            => slot is null or SkipCurryingParameter;

        static int GetLength(object arg) => arg switch
        {
            BufferedDataArray array => array.Count,
            float[] values => values.Length,
            _ => -1
        };
    }
}
