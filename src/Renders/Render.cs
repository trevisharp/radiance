/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Data;
using Pipelines;
using Exceptions;

using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;

/// <summary>
/// Represents a function that can used by GPU to draw in the screen.
/// </summary>
public class Render(Delegate function) : DynamicObject, ICurryable
{
    private RenderContext context = null;
    private readonly List<ShaderDependence> dependenceList = [];
    public readonly int ExtraParameterCount = function.Method.GetParameters().Length;

    public dynamic Curry(params object[] parameters)
        => new CurryingRender(this, parameters);

    public override bool TryInvoke(
        InvokeBinder binder, object[] args, out object result)
    {
        if (args.Length == 0)
            throw new MissingPolygonException();

        if (args[0] is not Polygon poly)
            throw new MissingPolygonException();

        int argCount = MeasureParameters(args);
        if (argCount < ExtraParameterCount + 1)
        {
            result = Curry(args);
            return true;
        }

        var data = new object[ExtraParameterCount];
        DisplayParameters(data, args[1..]);

        foreach (var (obj, dependence) in data.Zip(dependenceList))
            dependence.UpdateData(obj);
        
        RenderData(poly, data);
        
        result = null;
        return true;
    }

    private void RenderData(Polygon poly, object[] data)
    {
        LoadContextIfFirstRun();

        var pipeline = PipelineContext.GetContext()
            ?? throw new IlegalRenderMomentException();
        pipeline.RegisterRenderCall(context, poly, data);
    }

    private void LoadContextIfFirstRun()
    {
        if (context is not null)
            return;
        
        context = GlobalRenderContext.CreateContext();
        CallWithShaderObjects(function);
    }

    private void CallWithShaderObjects(Delegate func)
    {
        var parameters = func.Method.GetParameters();
        
        var objs = parameters
            .Select(GenerateDependence)
            .ToArray();
        
        if (objs.Any(obj => obj is null))
            throw new InvalidRenderException();

        func.DynamicInvoke(objs);
    }

    private ShaderObject GenerateDependence(ParameterInfo parameter)
    {
        if (parameter.ParameterType == typeof(FloatShaderObject))
        {
            var dep = new UniformFloatDependence(parameter.Name);
            dependenceList.Add(dep);

            return new FloatShaderObject(
                parameter.Name, ShaderOrigin.Global, [dep]
            );
        }

        if (parameter.ParameterType == typeof(Sampler2DShaderObject))
        {
            var dep = new TextureDependence(parameter.Name);
            dependenceList.Add(dep);
            
            return new Sampler2DShaderObject(
                parameter.Name, ShaderOrigin.FragmentShader, [dep]
            );
        }
        
        return null;
    }

    /// <summary>
    /// Fill parameters data on a vector to run a function.
    /// </summary>
    private static void DisplayParameters(object[] result, object[] args)
    {
        int index = 0;
        foreach (var arg in args)
            index = DisplayParameters(arg, result, index);
        
        if (index < args.Length)
            throw new MissingParametersException();
    }

    /// <summary>
    /// Measure the size of parameters args when displayed based on size.
    /// </summary>
    private static int MeasureParameters(object[] args)
        => args.Sum(arg => arg switch
        {
            Vec2 => 2,
            Vec3 => 3,
            Vec4 => 4,
            float[] arr => arr.Length,
            _ => 1
        });
    
    /// <summary>
    /// Set arr data from a index based on arg data size.
    /// </summary>
    private static int DisplayParameters(object arg, object[] arr, int index)
    {
        switch (arg)
        {
            case float num:
                verify(1);
                add(num);
                break;
            
            case int num:
                verify(1);
                add((float)num);
                break;
            
            case double num:
                verify(1);
                add((float)num);
                break;
                
            case Vec2 vec:
                verify(2);
                add(vec.X);
                add(vec.Y);
                break;
                
            case Vec3 vec:
                verify(3);
                add(vec.X);
                add(vec.Y);
                add(vec.Z);
                break;
                
            case Vec4 vec:
                verify(4);
                add(vec.X);
                add(vec.Y);
                add(vec.Z);
                add(vec.W);
                break;
            
            case float[] subArray:
                verify(subArray.Length);
                foreach (var value in subArray)
                    add(value);
                break;
            
            case Texture img:
                verify(1);
                add(img);
                break;
        }
        return index;

        void verify(int size)
        {
            if (index + size <= arr.Length)
                return;
            
            throw new SurplusParametersException();
        }

        void add(object value)
            => arr[index++] = value;
    }
}