/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// A buffered data with a polygon repeated many times.
/// </summary>
public class RepeatPolygon : IPolygon
{
    float[]? data = null;
    readonly IPolygon polygon;
    readonly int times;

    public RepeatPolygon(IPolygon polygon, int times)
    {
        this.polygon = polygon;
        this.times = times;
        Buffer = Buffer.From(this);
    }

    public int Count => polygon.Count * times;

    public int Size => polygon.Size;

    public Buffer Buffer { get; private set; }

    Vec3Buffer? triangulationPair = null;
    public Vec3Buffer Triangules
        => triangulationPair ??= new(BuildTriangules());
    
    public float[] GetBufferData()
        => data ??= BuildData();
    
    float[] BuildData()
        => Repeat(polygon.GetBufferData(), times);

    float[] BuildTriangules()
        => Repeat(polygon.Triangules.GetBufferData(), times);
    
    static float[] Repeat(float[] data, int times)
    {
        var len = data.Length;
        var buffer = new float[len * times];

        for (int i = 0; i < times; i++)
            data.CopyTo(buffer, len * i);
        
        return buffer;
    }
}