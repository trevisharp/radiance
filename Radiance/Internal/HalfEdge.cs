/* Author:  Leonardo Trevisan Silio
 * Date:    30/12/2024
 */
namespace Radiance.Internal;

/// <summary>
/// Represents a Edge between two vertex.
/// </summary>
public class HalfEdge(int id, int from, int to, int face)
{
    public readonly int Id = id;
    public readonly int From = from;
    public readonly int To = to;
    public readonly int FaceId = face;
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
        Previous.Next = this;
    }
}