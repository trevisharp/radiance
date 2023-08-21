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
public interface IData<V, F>
    where V : ShaderObject, new()
    where F : ShaderObject, new()
{
    V VertexObject { get; }
    F FragmentObject { get; }

    IEnumerable<ShaderOutput> Outputs { get; } 

    int Size { get;}
    int Elements { get; }
    IEnumerable<int> Sizes { get; }

    void SetData(float[] arr, ref int indexoff);
    float[] GetBuffer();
}