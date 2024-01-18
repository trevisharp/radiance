/* Author:  Leonardo Trevisan Silio
 * Date:    15/01/2024
 */
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Data;

/// <summary>
/// Represents a data that can be sended to a shader and drawed.
/// </summary>
public abstract class Polygon
{
    private int elementSize = 0;
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
            if (layout.definition is null)
            {
                for (int k = 0; k < layout.size; k++)
                    data.AddLast(0);
                continue;
            }

            foreach (var def in layout.definition)
                data.AddLast(def(x, y, z));
        }

        return this;
    }

    public Polygon Append(params Func<float, float, float, float>[] defs)
    {
        var it = data.First;

        while (it != null)
        {
            float x = it.Value;
            it = it.Next;

            float y = it.Value;
            it = it.Next;

            float z = it.Value;
            it = it.Next;

            for (int n = 3; n < elementSize; n++)
                it = it.Next;
            
            foreach (var def in defs)
                data.AddBefore(it, def(x, y, z));
        }
        AppendLayout(defs.Length, "noname", "notype", defs);

        return this;
    }

    internal void AppendLayout(
        int size, string type, string name, 
        Func<float, float, float, float>[] defs = null
    )
    {
        this.layouts.Add(new(size, type, name, defs));
        this.elementSize += size;
    }
    
    internal string Header
    {
        get
        {
            StringBuilder sb = new StringBuilder();

            int location = 0;
            foreach (var layout in layouts)
                sb.AppendLine($"layout (location = {location}) in {layout.type} {layout.name};");

            return sb.ToString();
        }
    }

    internal float[] Data
        => data.ToArray();

    internal int ElementSize
        => this.elementSize;
    
    private record LayoutInfo(
        int size,
        string type,
        string name,
        Func<float, float, float, float>[] definition
    );
}