/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
namespace Radiance.Data;

using ShaderSupport;

/// <summary>
/// A base class to all data layouts.
/// </summary>
public abstract class Data<D>
    where D : ShaderDependence
{
    public abstract int Size { get; }
    public abstract int Elements { get; }
    public abstract D ToDependence { get; }
    
    public abstract int SetData(float[] arr, int indexoff);
    public float[] GetBuffer()
    {
        float[] buffer = new float[this.Size];

        this.SetData(buffer, 0);
        
        return buffer;
    }

    public static implicit operator D(Data<D> data)
        => data.ToDependence;
}