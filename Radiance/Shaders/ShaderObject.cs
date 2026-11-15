/* Author:  Leonardo Trevisan Silio
 * Date:    04/10/2024
 */
using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Radiance.Shaders;

using Dependencies;
using static ShaderOrigin;

using Exceptions;

/// <summary>
/// Represents any data in a shader implementation.
/// </summary>
public abstract class ShaderObject(
    ShaderType type,
    string expression,
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> dependencies
) {
    public readonly ShaderType Type = type;
    public readonly string Expression = expression;
    public readonly ShaderOrigin Origin = origin;
    public readonly IEnumerable<ShaderDependence> Dependencies = dependencies;

    public override string ToString()
        => Expression;
    
    public static R Union<R>(string newExpression, params ShaderObject[] objs)
        where R : ShaderObject
    {
        var deps = objs.SelectMany(x => x.Dependencies);
        var originInfo = UnionOrigin(objs.Select(x => x.Origin));

        if (originInfo.hasConflitct)
        {
            foreach (var vertObj in objs.Where(x => x.Origin == VertexShader))
            {
                var output = new OutputDependence(vertObj);
                newExpression = ReplaceExpression(newExpression, vertObj.Expression, output.Name);
                deps = deps.Append(output);
            }
        }

        var newObj = Activator.CreateInstance(
            typeof(R), newExpression, originInfo.origin, deps
        ) as R;
        
        return newObj!;
    }

    public static R MergeOrigin<R>(R originalObject, ShaderOrigin extraOrigin)
        where R : ShaderObject
    {
        var unionInfo = UnionOrigin([originalObject.Origin, extraOrigin]);
        if (!unionInfo.hasConflitct)
            return originalObject;
        
        var output = new OutputDependence(originalObject);
        ShaderDependence[] deps = [ output ];

        var newObj = Activator.CreateInstance(
            typeof(R), output.Name, unionInfo.origin, deps
        ) as R;

        return newObj!;
    }

    /// <summary>
    /// Replace an expression inside other without matching part of a larger identifier.
    /// </summary>
    static string ReplaceExpression(string text, string expression, string replacement)
    {
        var start = char.IsLetterOrDigit(expression[0]) || expression[0] == '_' ? @"(?<![\w.])" : "";
        var end = char.IsLetterOrDigit(expression[^1]) || expression[^1] == '_' ? @"(?!\w)" : "";
        return Regex.Replace(text, start + Regex.Escape(expression) + end, _ => replacement);
    }

    static (ShaderOrigin origin, bool hasConflitct) UnionOrigin(IEnumerable<ShaderOrigin> origins)
    {
        var nonGlobal =
            from origin in origins
            where origin != Global
            select origin;
        
        var hasVertex = nonGlobal.Contains(VertexShader);
        var hasFragment = nonGlobal.Contains(FragmentShader);

        if (hasFragment)
            return (FragmentShader, hasVertex);
        
        if (hasVertex)
            return (VertexShader, false);
        
        return (Global, false);
    }

    public static R Transform<T, R>(string newExpression, T obj)
        where T : ShaderObject
        where R : ShaderObject
    {
        var newObj = Activator.CreateInstance(
            typeof(R), newExpression,
            obj.Origin, obj.Dependencies
        ) as R;
        return newObj!;
    }

    public static string ToShaderExpression(object? obj)
    {
        return obj switch
        {
            float value => value.ToString(CultureInfo.InvariantCulture),
            double value => value.ToString(CultureInfo.InvariantCulture),
            int value => value.ToString(CultureInfo.InvariantCulture),
            bool value => value ? "true" : "false",
            _ => throw new InvalidShaderExpressionException(obj)
        };
    }
}