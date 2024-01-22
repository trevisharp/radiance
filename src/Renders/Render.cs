/* Author:  Leonardo Trevisan Silio
 * Date:    21/01/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;
using Data;
using Internal;
using Exceptions;

/// <summary>
/// Represents a function that can used by GPU to draw in the screen.
/// </summary>
public class Render : DynamicObject
{
    private OpenGLManager manager;
    private readonly int extraParameterCount;
    private List<UniformParameterDependence> dependenceList;
    public int ExtraParameterCount => extraParameterCount;

    public Render(Delegate function)
    {
        this.extraParameterCount = 
            function.Method.GetParameters().Length;
        this.dependenceList = new();
        Window.RunOrSchedule(() => {
            initRender();
            callWithShaderObjects(function);
        });
    }

    public override bool TryInvoke(
        InvokeBinder binder, object[] args, out object result)
    {
        if (args.Length == 0)
            throw new MissingPolygonException();

        var poly = args[0] as Polygon;
        if (poly is null)
            throw new MissingPolygonException();

        var data = getArgs(args[1..]);
        foreach (var pair in data.Zip(dependenceList))
            pair.Second.SetValue(pair.First);

        manager.Render(poly, data);

        result = true;
        return true;
    }

    private void callWithShaderObjects(Delegate func)
    {
        var parameters = func.Method.GetParameters();
        
        var objs = parameters
            .Select(p => generateDependence(p))
            .ToArray();
        
        if (objs.Any(obj => obj is null))
            throw new InvalidRenderException();

        func.DynamicInvoke(objs);
    }

    private ShaderObject generateDependence(ParameterInfo parameter)
    {
        if (parameter.ParameterType == typeof(FloatShaderObject))
        {
            var dep = new UniformParameterDependence(
                parameter.Name, "float"
            );
            this.dependenceList.Add(dep);
            return new FloatShaderObject(parameter.Name, dep);
        }
        
        return null;
    }

    private void initRender()
    {
        var ctx = RenderContext.CreateContext();
        ctx.Position = new BufferDependence<Vec3ShaderObject>(
            "pos", null, 0
        );
        ctx.Color = new Vec4ShaderObject("(0.0, 0.0, 0.0, 1.0)");
        this.manager = ctx.Manager = new OpenGLManager();
    }

    private float[] getArgs(object[] args)
    {
        int index = 0;
        var result = new float[extraParameterCount];

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
            
            case int num:
                add((float)num);
                break;
            
            case double num:
                add((float)num);
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