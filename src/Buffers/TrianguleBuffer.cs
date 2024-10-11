/* Author:  Leonardo Trevisan Silio
 * Date:    11/10/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represents a simple triagules buffer with many data on vertices.
/// </summary>
public class TrianguleBuffer(float[] buffer, int vertexDataSize) : IBufferedData
{
    public int Count => buffer.Length;

    public float[] Data => buffer;

    public int Vertices => Count / vertexDataSize;

    public Buffer? Buffer { get; set; }

    public TrianguleBuffer Triangules => this;
}