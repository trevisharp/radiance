/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// A base class to all data layouts.
/// </summary>
public interface IData
{
    Vec3ShaderObject VertexObject { get; }
    Vec4ShaderObject FragmentObject { get; }
    
    IEnumerable<ShaderOutput> Outputs { get; } 

    int Size { get;}
    int Elements { get; }
    IEnumerable<int> Sizes { get; }

    void SetData(float[] arr, ref int indexoff);
    float[] GetBuffer();
}

/// <summary>
/// A base class to all data layouts.
/// </summary>
public interface IData<D> : IData
    where D : ShaderObject, new()
{
    D Data1 { get; }
}

/// <summary>
/// A base class to all data layouts.
/// </summary>
public interface IData<D1, D2> : IData
    where D1 : ShaderObject, new()
    where D2 : ShaderObject, new()
{
    D1 Data1 { get; }
    D2 Data2 { get; }
}