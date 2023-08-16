/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
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
    public static Data<D, T> transform<D, T>(
        this Data<D, T> data,
        Func<T, T> transformation
    )
        where D : ShaderDependence<T>
        where T : ShaderObject, new()
        => new TransformedData<D, T>(data, transformation);
    
    /// <summary>
    /// Aply color in data
    /// </summary>
    public static Data<D, T> colorize<D, T>(
        this Data<D, T> data,
        Vec4ShaderObject color
    )
        where D : ShaderDependence<T>
        where T : ShaderObject, new()
        => new ColoredData<D, T>(data, () => color);
    
    /// <summary>
    /// Aply color in data
    /// </summary>
    public static Data<D, T> colorize<D, T>(
        this Data<D, T> data,
        FloatShaderObject r,
        FloatShaderObject g,
        FloatShaderObject b,
        FloatShaderObject a
    )
        where D : ShaderDependence<T>
        where T : ShaderObject, new()
        => new ColoredData<D, T>(data, () => (r, g, b, a));
}