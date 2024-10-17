/* Author:  Leonardo Trevisan Silio
 * Date:    16/10/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// A buffered data with a polygon repeated many times.
/// </summary>
public class RepeatPolygon(IPolygon polygon, int times) : IPolygon
{
    public int Count => polygon.Count * times;

    public int Size => polygon.Size;

    public Buffer? Buffer { get; set; }

    Vec3Buffer? triangulationPair = null;
    public Vec3Buffer Triangules
        => triangulationPair ??= new(BuildTriangules());

    float[]? data = null;
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