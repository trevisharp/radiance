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
    public static IData<V, F> colorize<V, F>(
        this IData<V, F> data,
        Func<F, F> transformation
    )
        where V : ShaderObject, new()
        where F : ShaderObject, new()
        => new FragmentTransformedData<V, F, F>(data, transformation);
    
    /// <summary>
    /// Apply color in data
    /// </summary>
    public static IData<V, Vec4ShaderObject> colorize<V, F>(
        this IData<V, F> data,
        Color color
    )
        where V : ShaderObject, new()
        where F : ShaderObject, new()
        => new FragmentTransformedData<V, F, Vec4ShaderObject>(data, _ => color);
}