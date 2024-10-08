/* Author:  Leonardo Trevisan Silio
 * Date:    02/10/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represent a object data that can be sended to a GPU Buffer.
/// </summary>
public interface IBufferedData
{
    /// <summary>
    /// Get the data values.
    /// </summary>
    float[] Data { get; }
    
    /// <summary>
    /// Get the Count of the Vertices;
    /// </summary>
    int Vertices { get; }

    /// <summary>
    /// Get the associated buffer.
    /// </summary>
    Buffer? Buffer { get; set; }

    /// <summary>
    /// Get the triangulation of this data.
    /// </summary>
    IBufferedData Triangulation { get; }
}