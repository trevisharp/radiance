/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2023
 */
namespace Radiance.Data;

public class Vector : Data
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
    
    public override int Size => 3;
    public override int SetData(float[] arr, int indexoff)
    {
        arr[indexoff] = this.x;
        arr[indexoff + 1] = this.y;
        arr[indexoff + 2] = this.z;

        return indexoff + 3;
    }
    public override string GetLayout => 
        $"layout(location = 0) in vec3 position;";
    public override string GetName => "position";
}