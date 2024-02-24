/* Author:  Leonardo Trevisan Silio
 * Date:    30/01/2024
 */
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Data;

using Internal;
using Exceptions;

/// <summary>
/// Represents a data that can managed by GPU and drawed in screen.
/// </summary>
public class Polygon
{
    private bool isImmutable = false;
    private int elementSize = 0;
    private List<LayoutInfo> layouts = new();
    private LinkedList<float> data = new();
    private Polygon triangulationPair = null;
    
    /// <summary>
    /// Get the triangulation of this polygon.
    /// </summary>
    public Polygon Triangulation
    {
        get
        {
            if (triangulationPair is not null)
                return triangulationPair;
            
            var triangules = VectorsOperations
                .PlanarPolygonTriangulation(Data);
            
            Polygon polygon = new Polygon();
            for (int i = 0; i < triangules.Length; i += 3)
                polygon.Add(
                    triangules[i + 0],
                    triangules[i + 1],
                    triangules[i + 2]
                );
            
            polygon.triangulationPair = polygon;
            triangulationPair = polygon;

            return triangulationPair;
        }
    }

    public Polygon()
        => AppendLayout(3, "vec3", "pos");
    
    /// <summary>
    /// Add a point (x, y, z) with extra data fields.
    /// </summary>
    public Polygon Add(float x, float y, float z, params float[] extra)
    {
        if (isImmutable)
            throw new ImmutablePolygonModifyException();

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

        change(true, false);
        return this;
    }

    /// <summary>
    /// Add a n fields in every single data point with value 0. 
    /// </summary>
    public Polygon Append(int fields, params float[] initialValue)
    {
        if (isImmutable)
            throw new ImmutablePolygonModifyException();
        
        var it = data.First;

        while (it != null)
        {
            for (int n = 0; n < elementSize; n++)
                it = it.Next;
            
            for (int i = 0; i < fields; i++)
            {
                var value = 
                    i < initialValue.Length ?
                    initialValue[i] : 0;
                data.AddBefore(it, value);
            }
        }
        AppendLayout(fields, "noname", "notype", null);

        change(true, true);
        return this;
    }

    /// <summary>
    /// Add a n fields in every single data point with value computed
    /// over (x, y, z) point.
    /// </summary>
    public Polygon Append(params Func<float, float, float, float>[] defs)
    {
        if (isImmutable)
            throw new ImmutablePolygonModifyException();
        
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

        change(true, true);
        return this;
    }

    /// <summary>
    /// Make this polygon immutable avoid data modification.
    /// </summary>
    public Polygon MakeImmutable()
    {
        isImmutable = true;
        return this;
    }
    
    /// <summary>
    /// Copy the polygon data ignoring the mutability state and the
    /// shader data associated to the polygon.
    /// </summary>
    public Polygon Clone()
        => new Polygon
        {
            elementSize = this.elementSize,
            layouts = new(this.layouts),
            data = new(this.data)
        };

    public event Action<bool, bool> OnChange;
    private void change(bool bufferBreak, bool layoutBreak)
    {
        if (OnChange is not null)
            OnChange(bufferBreak, layoutBreak);
    }

    internal void AppendLayout(
        int size, string type, string name, 
        Func<float, float, float, float>[] defs = null
    )
    {
        this.layouts.Add(new(size, type, name, defs));
        this.elementSize += size;
    }
    
    internal IEnumerable<LayoutInfo> Layouts => layouts;

    internal int Buffer { get; set; } = -1;
    
    internal int VertexObjectArray { get; set; } = -1;

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