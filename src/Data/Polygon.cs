/* Author:  Leonardo Trevisan Silio
 * Date:    15/01/2024
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Radiance.Data;

/// <summary>
/// Represents a data that can be sended to a shader and drawed.
/// </summary>
public abstract class Polygon
{
    private List<LayoutInfo> layouts = new();
    private LinkedList<float> data = new();

    public Polygon()
        => AppendLayout(3, "vec3", "pos");

    public Polygon Add(float x, float y, float z)
    {
        data.AddLast(x);
        data.AddLast(y);
        data.AddLast(z);

        for (int i = 1; i < layouts.Count; i++)
        {
            var layout = layouts[i];
            var layoutData =
                layout.definition is null ?
                new float[layout.size] :
                layout.definition(new float[] { x, y, z });
            
            foreach (var value in layoutData)
                data.AddLast(value);
        }

        return this;
    }

    internal void AppendLayout(int size, string type, string name)
        => this.layouts.Add(new(size, type, name, null));
    
    internal string GetHeader()
    {
        StringBuilder sb = new StringBuilder();

        int location = 0;
        foreach (var layout in layouts)
            sb.AppendLine($"layout (location = {location}) in {layout.type} {layout.name};");

        return sb.ToString();
    }

    internal float[] GetData()
        => data.ToArray();

    private record LayoutInfo(
        int size,
        string type,
        string name,
        Func<float[], float[]> definition
    );
}