/* Author:  Leonardo Trevisan Silio
 * Date:    23/08/2023
 */
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

/// <summary>
/// Represents a group of Vectors.
/// </summary>
public class Vectors : IData<Vec3ShaderObject>, ICollection<Vector>
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

    private PositionBufferDependence dep =>
        new PositionBufferDependence(this);

    public Vec3ShaderObject VertexObject => dep;

    public Vec4ShaderObject FragmentObject => Color.White;
    
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

    public int Size => 3 * this.vectors.Count;
    
    public int Elements => this.vectors.Count;

    public IEnumerable<ShaderOutput> Outputs => ShaderOutput.Empty;

    public IEnumerable<int> Sizes => new int[] { 3 };

    public Vec3ShaderObject Data1 => dep;

    #endregion
}