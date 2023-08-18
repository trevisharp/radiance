/* Author:  Leonardo Trevisan Silio
 * Date:    18/08/2023
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
    /// Transform original data
    /// </summary>
    public static Data<D1, D2, T1, T2> transform<D1, D2, T1, T2>(
        this Data<D1, D2, T1, T2> data,
        Func<T1, T2, (T1, T2)> transformation
    )
        where D1 : ShaderDependence<T1>
        where D2 : ShaderDependence<T2>
        where T1 : ShaderObject, new()
        where T2 : ShaderObject, new()
        => new TransformedData<D1, D2, T1, T2>(data, transformation);
    
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
    
    
    /// <summary>
    /// Aply color in data
    /// </summary>
    public static Data<D1, D2, T1, T2> colorize<D1, D2, T1, T2>(
        this Data<D1, D2, T1, T2> data,
        Func<T2, Vec4ShaderObject> color
    )
        where D1 : ShaderDependence<T1>
        where D2 : ShaderDependence<T2>
        where T1 : ShaderObject, new()
        where T2 : ShaderObject, new()
        => new ColoredData<D1, D2, T1, T2>(data, color);
}