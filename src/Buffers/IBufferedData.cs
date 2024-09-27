/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represent a object data that can be sended to a GPU Buffer.
/// </summary>
public interface IBufferedData
{
    float[] Data { get; }
}