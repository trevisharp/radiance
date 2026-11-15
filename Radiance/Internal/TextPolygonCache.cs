/* Author:  Leonardo Trevisan Silio
 * Date:    14/11/2026
 */
using System.Collections.Generic;

namespace Radiance.Internal;

using Bufferings;

/// <summary>
/// A cache of the last texts drawn as strings.
/// </summary>
internal static class TextPolygonCache
{
    const int capacity = 256;
    static readonly Dictionary<string, LinkedListNode<(string Text, Polygon Polygon)>> map = [];
    static readonly LinkedList<(string Text, Polygon Polygon)> recent = [];

    /// <summary>
    /// Get the triangulated polygon of a text.
    /// </summary>
    public static Polygon Get(string text)
    {
        if (map.TryGetValue(text, out var node))
        {
            recent.Remove(node);
            recent.AddFirst(node);
            return node.Value.Polygon;
        }

        var polygon = Polygon.FromText(text);
        _ = polygon.Triangules;

        map[text] = recent.AddFirst((text, polygon));
        if (map.Count > capacity)
        {
            var oldest = recent.Last!;
            recent.RemoveLast();
            map.Remove(oldest.Value.Text);
            oldest.Value.Polygon.Dispose();
        }

        return polygon;
    }
}
