/* Author:  Leonardo Trevisan Silio
 * Date:    17/02/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Data;
using Exceptions;
using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;

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
            this.ctx = RenderContext.CreateContext();
            callWithShaderObjects(function);
            RenderContext.ClearContext();
        });
    }

    public dynamic Curry(params object[] parameters)
        => new CurryingRender(this, parameters);

    public override bool TryInvoke(
        InvokeBinder binder, object[] args, out object result)
    {
        if (args.Length == 0)
            throw new MissingPolygonException();

        var poly = args[0] as MutablePolygon;
        if (poly is null)
            throw new MissingPolygonException();

        int argCount = countArgs(args);
        if (argCount < extraParameterCount + 1)
        {
            result = Curry(args);
            return true;
        }

        var data = getArgs(args[1..]);

        foreach (var pair in data.Zip(dependenceList))
            pair.Second.UpdateData(pair.First);
        
        if (ctx is null)
            throw new IlegalRenderMomentException();

        ctx.Render(poly, data);
        
        result = null;
        return true;
    }

    private void callWithShaderObjects(Delegate func)
    {
        var parameters = func.Method.GetParameters();
        
        var objs = parameters
            .Select(generateDependence)
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
                parameter.Name, ShaderOrigin.FragmentShader, [dep]
            );
        }
        
        return null;
    }

    private int countArgs(object[] args)
        => args.Sum(arg => arg switch
        {
            Vec2 => 2,
            Vec3 => 3,
            Vec4 => 4,
            float[] arr => arr.Length,
            _ => 1
        });

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
            
            case float[] subarr:
                for (int i = 0; i < subarr.Length; i++)
                    add(subarr[i]);
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