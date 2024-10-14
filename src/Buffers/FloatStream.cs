/* Author:  Leonardo Trevisan Silio
 * Date:    14/10/2024
 */
namespace Radiance.Buffers;

public class FloatStream : IBufferedData
{
    private float[] data;
    public float[] Data => data;

    public int Vertices => throw new System.NotImplementedException();

    public Buffer? Buffer { get; set; }

    public TrianguleBuffer Triangules => throw new System.NotImplementedException();
}