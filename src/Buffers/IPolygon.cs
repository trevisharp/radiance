/* Author:  Leonardo Trevisan Silio
 * Date:    16/10/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// A interface for all bufferd data that contains
/// (x, y, z) points and can make triangulation.
/// </summary>
public interface IPolygon : IBufferedData
{
    /// <summary>
    /// The triangule buffer associated with this polygon.
    /// </summary>
    Vec3Buffer Triangules { get; }
}