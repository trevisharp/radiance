/* Author:  Leonardo Trevisan Silio
 * Date:    18/08/2023
 */
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport.Dependencies;
using ShaderSupport.Objects;

/// <summary>
/// Represents a group of Colored Vectors.
/// </summary>
public class ColoredVectors : Data
    <PositionBufferDependence,
    ColorBufferDependence,
    Vec3ShaderObject,
    Vec3ShaderObject>, ICollection<ColoredVector>
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

    public override void SetData(float[] arr, ref int indexoff)
    {
        foreach (var vector in this.vectors)
            vector.SetData(arr, ref indexoff);
    }

    public override int Size1 => 3 * this.vectors.Count;
    public override int Size2 => 3 * this.vectors.Count;

    public override PositionBufferDependence ToDependence1
        => new PositionBufferDependence(this.GetBuffer());
        
    public override ColorBufferDependence ToDependence2
        => new ColorBufferDependence(this.GetBuffer());
    
    public override int Elements => this.vectors.Count;

    #endregion
}