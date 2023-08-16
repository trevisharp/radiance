/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
using System;

namespace Radiance.Data;

using ShaderSupport;

/// <summary>
/// Extension class of util operations with Data.
/// </summary>
public static class DataExtension
{
    public static Data<D, T> transform<D, T>(
        this Data<D, T> data,
        Func<T, T> transformation
    )
        where D : ShaderDependence<T>
        where T : ShaderObject, new()
        => new TransformedData<D, T>(data, transformation);
    
    
}