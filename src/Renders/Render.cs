/* Author:  Leonardo Trevisan Silio
 * Date:    23/01/2024
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
public class Render : DynamicObject, ICurryable
{
    private RenderContext ctx;
    private readonly int extraParameterCount;
    private List<ShaderDependence> dependenceList;
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

    public dynamic Curry(params object[] parameters)
        => new CurryingRender(this, parameters);

    public override bool TryInvoke(
        InvokeBinder binder, object[] args, out object result)
    {
        if (args.Length == 0)
            throw new MissingPolygonException();

        var poly = args[0] as Polygon;
        if (poly is null)
            throw new MissingPolygonException();

        if (args.Length < extraParameterCount + 1)
        {
            result = this.Curry(args);
            return true;
        }

        var data = getArgs(args[1..]);

        foreach (var pair in data.Zip(dependenceList))
            pair.Second.UpdateData(pair.First);

        ctx.Render(poly, data);
        
        result = null;
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
            var dep = new UniformFloatDependence(parameter.Name);
            this.dependenceList.Add(dep);

            return new FloatShaderObject(
                parameter.Name, ShaderOrigin.Global, [dep]
            );
        }

        if (parameter.ParameterType == typeof(Sampler2DShaderObject))
        {
            var dep = new TextureDependence(parameter.Name);
            this.dependenceList.Add(dep);
            
            return new Sampler2DShaderObject(
                parameter.Name, ShaderOrigin.Global, [dep]
            );
        }
        
        return null;
    }

    private void initRender()
    {
        this.ctx = RenderContext.CreateContext();

        var bufferDep = new BufferDependence();
        ctx.Position = new ("pos", ShaderOrigin.VertexShader, [bufferDep]);

        ctx.Color = new("vec4(0.0, 0.0, 0.0, 1.0)", ShaderOrigin.FragmentShader, []);
    }

    private object[] getArgs(object[] args)
    {
        int index = 0;
        var result = new object[extraParameterCount];

        foreach (var arg in args)
            index = setArgs(arg, result, index);
        
        if (index < args.Length)
            throw new MissingParametersException();
        
        return result;
    }

    private int setArgs(object arg, object[] arr, int index)
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
            
            case Texture img:
                add(img);
                break;
        }
        return index;

        void add(object value)
        {
            if (index >= arr.Length)
                throw new SurplusParametersException();
            arr[index++] = value;
        }
    }
}