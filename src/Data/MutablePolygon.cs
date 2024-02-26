/* Author:  Leonardo Trevisan Silio
 * Date:    26/02/2024
 */
using System;
using System.Collections.Generic;

namespace Radiance.Data;

/// <summary>
/// Represents a data that can managed by GPU and drawed in screen.
/// </summary>
public class MutablePolygon : Polygon
{
    private LinkedList<float> data = new();
    public override IEnumerable<float> Data => data;
    
    protected override void add(float x, float y, float z)
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
}