/* Author:  Leonardo Trevisan Silio
 * Date:    12/06/2025
 */
namespace Radiance.Internal;

/// <summary>
/// Represents a Planar Vertex.
/// </summary>
public record Vertex(int Id, float X, float Y, float Z)
{
    public override string ToString()
        => $"({X}, {Y})";
}