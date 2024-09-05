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
        
        if (!TryRegisterCall(poly, data))
        {
            result = Curry(args);
            return true;
        }
        
        result = null;
        return true;
    }

    public void RenderWith(Polygon poly, object[] data)
    {
        foreach (var (obj, dependence) in data.Zip(dependenceList))
            dependence.UpdateData(obj);
        
        context.Render(poly, data);
    }

    private bool TryRegisterCall(Polygon poly, object[] data)
    {
        var pipeline = PipelineContext.GetContext();
        if (pipeline is null)
            return false;
        
        LoadContextIfFirstRun();
        pipeline.RegisterRenderCall(this, poly, data);
        return true;
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
}