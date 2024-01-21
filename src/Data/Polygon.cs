/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Data;

using Exceptions;

/// <summary>
/// Represents a data that can be sended to a shader and drawed.
/// </summary>
public class Polygon
{
    private int elementSize = 0;
    private List<LayoutInfo> layouts = new();
    private LinkedList<float> data = new();

    internal IEnumerable<LayoutInfo> Layouts => layouts;

    public Polygon()
        => AppendLayout(3, "vec3", "pos");

    public Polygon Add(float x, float y, float z, params float[] extra)
    {
        data.AddLast(x);
        data.AddLast(y);
        data.AddLast(z);

        for (int i = 1; i < layouts.Count; i++)
        {
            var layout = layouts[i];
            if (layout.definition is not null)
            {
                foreach (var def in layout.definition)
                    data.AddLast(def(x, y, z));
                continue;
            }

            if (extra.Length == 0)
            {
                for (int k = 0; k < layout.size; k++)
                    data.AddLast(0);
                continue;
            }
            
            if (extra.Length != layout.size)
                throw new InvalidExtraDataException();
            
            for (int k = 0; k < layout.size; k++)
                data.AddLast(extra[k]);
        }

        change();
        return this;
    }

    public Polygon Append(int fields)
    {
        var it = data.First;

        while (it != null)
        {
            for (int n = 0; n < elementSize; n++)
                it = it.Next;
            
            for (int i = 0; i < fields; i++)
                data.AddBefore(it, 0);
        }
        AppendLayout(fields, "noname", "notype", null);

        change();
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

        change();
        return this;
    }

    public event Action OnChange;
    private void change()
    {
        if (OnChange is not null)
            OnChange();
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
    
    internal int Elements
        => this.data.Count / this.elementSize;
    
    internal record LayoutInfo(
        int size,
        string type,
        string name,
        Func<float, float, float, float>[] definition
    );
}