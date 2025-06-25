/* Author:  Leonardo Trevisan Silio
 * Date:    30/12/2024
 */
namespace Radiance.Internal;

/// <summary>
/// Represents a Edge between two vertex.
/// </summary>
public class HalfEdge(int id, Vertex from, Vertex to)
{
    public readonly int Id = id;
    public readonly Vertex From = from;
    public readonly Vertex To = to;
    public HalfEdge? Next { get; private set; }
    public HalfEdge? Previous { get; private set; }

    public void SetNext(HalfEdge next)
    {
        Next = next;
        next.Previous = this;
    }

    public void SetPrevious(HalfEdge prev)
    {
        Previous = prev;
        prev.Next = this;
    }

    public override string ToString()
        => $"{From} > {To}";
}