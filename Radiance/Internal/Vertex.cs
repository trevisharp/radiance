/* Author:  Leonardo Trevisan Silio
 * Date:    12/06/2025
 */
using System.Globalization;

namespace Radiance.Internal;

/// <summary>
/// Represents a Planar Vertex.
/// </summary>
public record Vertex(int Id, float X, float Y, float Z)
{
    public override string ToString()
        => $"({X.ToString("0.000", CultureInfo.InvariantCulture)}, {Y.ToString("0.000", CultureInfo.InvariantCulture)})";
}