/* Author:  Leonardo Trevisan Silio
 * Date:    14/10/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represent a object data that can be sended to a GPU Buffer.
/// </summary>
public interface IMutableData : IBufferedData
{
    void Fill(float[] data);

    void Changed();
}