/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
using System;
using System.Dynamic;

namespace Radiance.Renders;

using Data;
using Internal;
using Exceptions;

using ShaderSupport.Objects;

public class Render : DynamicObject
{
    private Delegate function;

    private readonly int extraParameterCount;
    public int ExtraParameterCount => extraParameterCount;

    public Render(Action function)
    {
        this.extraParameterCount = 0;
        this.function = function;
    }
    public Render(Action<FloatShaderObject> function)
    {
        this.extraParameterCount = 1;
        this.function = function;
    }
    public Render(Action<FloatShaderObject, FloatShaderObject> function)
    {
        this.extraParameterCount = 2;
        this.function = function;
    }
    public Render(Action<FloatShaderObject, FloatShaderObject, FloatShaderObject> function)
    {
        this.extraParameterCount = 3;
        this.function = function;
    }
    public Render(Action<FloatShaderObject, FloatShaderObject, FloatShaderObject, FloatShaderObject> function)
    {
        this.extraParameterCount = 4;
        this.function = function;
    }

    public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
    {
        if (args.Length == 0)
            throw new MissingPolygonException();

        var poly = args[0] as Polygon;
        if (poly is null)
            throw new MissingPolygonException();
        
        var ctx = RenderContext.GetContext();
        ctx.Data = poly;
        ctx.Operation = null; // TODO
        var data = getArgs(args[1..]);
        this.function.DynamicInvoke(data);

        result = true;
        return true;
    }

    private float[] getArgs(object[] args)
    {
        int index = 0;
        float[] result = new float[extraParameterCount];

        foreach (var arg in args)
            index = setArgs(arg, result, index);
        
        if (index < args.Length)
            throw new MissingParametersException();
        
        return result;
    }

    private int setArgs(object arg, float[] arr, int index)
    {
        switch (arg)
        {
            case float num:
                add(num);
                break;
                
            case Vec2 vec:
                add(vec.X);
                add(vec.Y);
                break;
                
            case Vec3 vec:
                add(vec.X);
                add(vec.Y);
                add(vec.Z);
                break;
                
            case Vec4 vec:
                add(vec.X);
                add(vec.Y);
                add(vec.Z);
                add(vec.W);
                break;
        }
        return index;

        void add(float value)
        {
            if (index >= arr.Length)
                throw new SurplusParametersException();
            arr[index++] = value;
        }
    }
}