/* Author:  Leonardo Trevisan Silio
 * Date:    29/12/2024
 */
namespace Radiance.Internal;

/// <summary>
/// Represents a HalfEdge with start and end.
/// </summary>
public class HalfEdge(int origin, int end)
{
    public readonly int OriginId = origin;
    public readonly int EndId = end;
    
    public HalfEdge? Twin { get; set; }
    public HalfEdge? Next { get; set; }
    public HalfEdge? Previous { get; set; }
}