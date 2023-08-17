/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport.Dependencies;
using ShaderSupport.Objects;

/// <summary>
/// Represents a group of Vectors.
/// </summary>
public class Vectors : Data<PositionBufferDependence, Vec3ShaderObject>, ICollection<Vector>
{
    #region ICollection Members

    private List<Vector> vectors = new List<Vector>();
    public int Count => vectors.Count;
    public bool IsReadOnly => false;

    public void Add(Vector item)
        => this.vectors.Add(item);

    public void Clear()
        => this.vectors.Clear();

    public bool Contains(Vector item)
        => this.vectors.Contains(item);

    public void CopyTo(Vector[] array, int arrayIndex)
        => this.vectors.CopyTo(array, arrayIndex);

    public IEnumerator<Vector> GetEnumerator()
        => this.vectors.GetEnumerator();

    public bool Remove(Vector item)
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

    public override int Size => 3 * this.vectors.Count;

    public override PositionBufferDependence ToDependence
    {
        get
        {
            var bufferDependence = new PositionBufferDependence(
                this.GetBuffer()
            );
            return bufferDependence;
        }
    }
    
    public override int Elements => this.vectors.Count;

    #endregion
}