/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
using System;

namespace Radiance;

using Data;
using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// Extension class of util operations with Data.
/// </summary>
public static class DataExtension
{
    /// <summary>
    /// Transform original data
    /// </summary>
    public static IData<D> transform<D>(
        this IData<D> data,
        Func<D, Vec3ShaderObject> transformation
    )
        where D : ShaderObject, new()
        => new VertexTransformedData<D>(data, transformation);
        
    /// <summary>
    /// Transform original data
    /// </summary>
    public static IData<D1, D2> transform<D1, D2>(
        this IData<D1, D2> data,
        Func<D1, D2, Vec3ShaderObject> transformation
    )
        where D1 : ShaderObject, new()
        where D2 : ShaderObject, new()
        => new VertexTransformedData<D1, D2>(data, transformation);

    /// <summary>
    /// Apply color in data
    /// </summary>
    public static IData<D> colorize<D>(
        this IData<D> data,
        Func<D, Vec4ShaderObject> transformation
    )
        where D : ShaderObject, new()
        => new FragmentTransformedData<D>(data, transformation);
        
    /// <summary>
    /// Apply color in data
    /// </summary>
    public static IData<D1, D2> colorize<D1, D2>(
        this IData<D1, D2> data,
        Func<D1, D2, Vec4ShaderObject> transformation
    )
        where D1 : ShaderObject, new()
        where D2 : ShaderObject, new()
        => new FragmentTransformedData<D1, D2>(data, transformation);

    /// <summary>
    /// Apply color in data
    /// </summary>
    public static IData<D> colorize<D>(
        this IData<D> data,
        Color color
    )
        where D : ShaderObject, new()
        => new FragmentTransformedData<D>(data, _ => color);
        
    /// <summary>
    /// Apply color in data
    /// </summary>
    public static IData<D1, D2> colorize<D1, D2>(
        this IData<D1, D2> data,
        Color color
    )
        where D1 : ShaderObject, new()
        where D2 : ShaderObject, new()
        => new FragmentTransformedData<D1, D2>(data, (_, _) => color);
        
    /// <summary>
    /// Apply color in data
    /// </summary>
    public static IData<D> colorize<D>(
        this IData<D> data,
        FloatShaderObject r,
        FloatShaderObject g,
        FloatShaderObject b
    )
        where D : ShaderObject, new()
        => new FragmentTransformedData<D>(data, _ => (r, g, b, 1.0));
        
    /// <summary>
    /// Apply color in data
    /// </summary>
    public static IData<D1, D2> colorize<D1, D2>(
        this IData<D1, D2> data,
        FloatShaderObject r,
        FloatShaderObject g,
        FloatShaderObject b
    )
        where D1 : ShaderObject, new()
        where D2 : ShaderObject, new()
        => new FragmentTransformedData<D1, D2>(data, (_, _) => (r, g, b, 1.0));
}