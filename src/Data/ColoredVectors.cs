/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Dependencies;
using ShaderSupport.Objects;

/// <summary>
/// Represents a group of Colored Vectors.
/// </summary>
public class ColoredVectors : IData<Vec3ShaderObject, Vec4ShaderObject>, ICollection<ColoredVector>
{
    #region ICollection Members

    private List<ColoredVector> vectors = new List<ColoredVector>();
    public int Count => vectors.Count;
    public bool IsReadOnly => false;

    public void Add(ColoredVector item)
        => this.vectors.Add(item);

    public void Clear()
        => this.vectors.Clear();

    public bool Contains(ColoredVector item)
        => this.vectors.Contains(item);

    public void CopyTo(ColoredVector[] array, int arrayIndex)
        => this.vectors.CopyTo(array, arrayIndex);

    public IEnumerator<ColoredVector> GetEnumerator()
        => this.vectors.GetEnumerator();

    public bool Remove(ColoredVector item)
        => this.vectors.Remove(item);

    IEnumerator IEnumerable.GetEnumerator()
        => this.vectors.GetEnumerator();

    #endregion

    #region Data Members

    public Vec3ShaderObject VertexObject
        => new PositionBufferDependence(this.GetBuffer(), 0);

    public Vec4ShaderObject FragmentObject
        => new ColorBufferDependence(this.GetBuffer(), 1);

    public IEnumerable<ShaderOutput> Outputs => ShaderOutput.Empty;

    public int Size => 7 * this.vectors.Count;

    public int Elements => this.vectors.Count;

    public IEnumerable<int> Sizes => new int[] { 3, 4 };

    public void SetData(float[] arr, ref int indexoff)
    {
        foreach (var vector in this.vectors)
            vector.SetData(arr, ref indexoff);
    }

    public float[] GetBuffer()
    {
        float[] buffer = new float[this.Size];

        int indexoff = 0;
        this.SetData(buffer, ref indexoff);
        
        return buffer;
    }

    #endregion
}