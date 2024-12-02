/* Author:  Leonardo Trevisan Silio
 * Date:    02/12/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// A interface for all bufferd data that contains
/// (x, y, z) points and can make triangulation.
/// </summary>
public interface IPolygon : IBufferedData
{
    /// <summary>
    /// The triangule buffer of triangules that can fill this polygon.
    /// </summary>
    Vec3Buffer Triangules { get; }

    /// <summary>
    /// The line buffer of lines that can draw bounds of this polygon. 
    /// </summary>
    Vec3Buffer Lines { get; }

    /// <summary>
    /// The line buffer of points that can draw bounds of this polygon. 
    /// </summary>
    Vec3Buffer Points { get; }
}