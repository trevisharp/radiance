/* Author:  Leonardo Trevisan Silio
 * Date:    21/06/2025
 */
using System.Globalization;

namespace Radiance.Internal;

/// <summary>
/// Represents a Planar Vertex.
/// </summary>
public record Vertex(float X, float Y, float Z)
{
    public override string ToString()
        => $"({X.ToString("0.000", CultureInfo.InvariantCulture)}, {Y.ToString("0.000", CultureInfo.InvariantCulture)})";
    
    public static bool operator >(Vertex v, Vertex u)
        => v.Y > u.Y || (v.Y == u.Y && v.X > u.X);
    
    public static bool operator <(Vertex v, Vertex u)
        => v.Y < u.Y || (v.Y == u.Y && v.X < u.X);
}