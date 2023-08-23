/* Author:  Leonardo Trevisan Silio
 * Date:    23/08/2023
 */
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Dependencies;
using ShaderSupport.Objects;

public class Vector : IData<Vec3ShaderObject>
{
    public Vector(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public static implicit operator Vector((float x, float y, float z) data)
        => new Vector(data.x, data.y, data.z);

    public static Vector operator +(Vector v)
        => new Vector(v.x, v.y, v.z);
        
    public static Vector operator -(Vector v)
        => new Vector(-v.x, -v.y, -v.z);
        
    public static Vector operator +(Vector v, Vector u)
        => new Vector(v.x + u.x, v.y + u.y, v.z + u.z);
    
    public static Vector operator -(Vector v, Vector u)
        => new Vector(v.x - u.x, v.y - u.y, v.z - u.z);
    
    public static Vector operator *(Vector v, float a)
        => new Vector(a *  v.x, a * v.y, a * v.z);
    
    public static Vector operator *(float a, Vector v)
        => new Vector(a *  v.x, a * v.y, a * v.z);
    
    public static Vector operator /(Vector v, float a)
        => new Vector(v.x / a, v.y / a, v.z / a);
    
    public static ColoredVector operator |(Vector v, Color c)
        => new ColoredVector()
        {
            Vector = v,
            Color = c
        };

    #region Data Members

    private PositionBufferDependence dep =>
        new PositionBufferDependence(this);

    public Vec3ShaderObject VertexObject => dep;

    public Vec4ShaderObject FragmentObject => Color.White;

    public IEnumerable<ShaderOutput> Outputs
        => ShaderOutput.Empty;

    public int Size => 3;
    public int Elements => 1;
    public IEnumerable<int> Sizes => new int[] { 3 };

    public Vec3ShaderObject Data1 => dep;

    public void SetData(float[] arr, ref int indexoff)
    {
        arr[indexoff + 0] = x;
        arr[indexoff + 1] = y;
        arr[indexoff + 2] = z;
        indexoff += 3;
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