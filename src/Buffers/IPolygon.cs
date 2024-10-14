/* Author:  Leonardo Trevisan Silio
 * Date:    14/10/2024
 */
using System.Collections.ObjectModel;

namespace Radiance.Buffers;

/// <summary>
/// A interface for all bufferd data that contains
/// (x, y, z) points and can make triangulation.
/// </summary>
public interface IPolygon : IBufferedData
{
    /// <summary>
    /// The readonly data for this polygon.
    /// </summary>
    ReadOnlyCollection<float> Data { get; }

    /// <summary>
    /// The triangule buffer associated with this polygon.
    /// </summary>
    TrianguleBuffer Triangules { get; }
}