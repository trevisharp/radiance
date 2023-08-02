namespace SharpGL;

/// <summary>
/// Represents a tridimensional point with axes x, y and z.
/// </summary>
public record Vertex(float x, float y, float z)
{
    public Vertex Translate(float x, float y, float z)
        => new (this.x + x, this.y + y, this.z + z);
    
    public Vertex Scale(float ax, float ay, float az)
        => new (this.x * ax, this.y * ay, this.z * az);

    public Vertex RotateX(float cosa, float sina)
        => new (x, y * cosa - z * sina, y * sina + z * cosa);

    public Vertex RotateY(float cosa, float sina)
        => new (x * cosa + z * sina, y, z * cosa - x * sina);

    public Vertex RotateZ(float cosa, float sina)
        => new (x * cosa - y * sina, y * cosa + x * sina, z);

    public override string ToString()
        => $"({x:N3}, {y:N3}, {z:N3})";

    public static Vertex operator +(Vertex p, Vector v)
        => new (p.x + v.x, p.y + v.y, p.z + v.z);
    
    public static Vertex operator -(Vertex p, Vector v)
        => new (p.x - v.x, p.y - v.y, p.z - v.z);
    
    public static Vertex operator +(Vector v, Vertex p)
        => new (p.x + v.x, p.y + v.y, p.z + v.z);

    public static Vector operator -(Vertex p, Vertex q)
        => new (p.x - q.x, p.y - q.y, p.z - q.z);
        
    public static implicit operator Vertex((float x, float y, float z) tuple)
        => new (tuple.x, tuple.y, tuple.z);
        
    public static implicit operator Vertex((float x, float y) tuple)
        => new (tuple.x, tuple.y, 0);
    
    public static readonly Vertex Origin;
    static Vertex()
        => Origin = new(0, 0, 0);
}