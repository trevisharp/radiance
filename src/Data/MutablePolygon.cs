/* Author:  Leonardo Trevisan Silio
 * Date:    30/08/2024
 */
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Data;

/// <summary>
/// Represents a data that can managed by GPU and drawed in screen.
/// </summary>
public class MutablePolygon : Polygon, ICollection<Vec3>
{
    readonly LinkedList<float> data = [];
    public override IEnumerable<float> Data => data;

    public int Count => data.Count / 3;

    public bool IsReadOnly => false;

    protected override void AddPoint(float x, float y, float z)
    {
        data.AddLast(x);
        data.AddLast(y);
        data.AddLast(z);
    }

    public override Polygon Clone()
    {
        var copy = new MutablePolygon();
        foreach (var el in data)
            copy.data.AddLast(el);
        return copy;
    }

    void ICollection<Vec3>.Add(Vec3 item)
        => Add(item);

    public void Clear()
        => data.Clear();

    public bool Contains(Vec3 item)
        => data.Chunk(3).Any(arr => (arr[0], arr[1], arr[2]) == item);

    public void CopyTo(Vec3[] array, int arrayIndex)
        => data
            .Chunk(3)
            .Select(arr => new Vec3(arr[0], arr[1], arr[2]))
            .ToArray()
            .CopyTo(array, arrayIndex);

    public bool Remove(Vec3 item)
    {
        var it = data.First;
        while (it != null)
        {
            var vec = (
                it.Value,
                it.Next?.Value ?? float.NaN,
                it.Next?.Next?.Value ?? float.NaN
            );

            if (vec == item)
            {
                data.Remove(it.Next!.Next!);
                data.Remove(it.Next);
                data.Remove(it);
                return true;
            }

            it = it.Next!.Next;
        }
        return false;
    }

    IEnumerator<Vec3> IEnumerable<Vec3>.GetEnumerator()
        => data
            .Chunk(3)
            .Select(arr => new Vec3(arr[0], arr[1], arr[2]))
            .GetEnumerator();
}