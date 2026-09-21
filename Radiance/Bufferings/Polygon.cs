/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2026
 */
using System;

namespace Radiance.Bufferings;

using Internal;
using Exceptions;

/// <summary>
/// A buffer with a Polygon points. A polygon has a contour and, optionally,
/// holes inside it. The buffered data holds the contour followed by every hole.
/// </summary>
public class Polygon : IPolygon
{
    readonly float[][] rings;
    readonly float[] data;
    readonly FillRule rule;

    Buffer? buffer = null;
    BufferData? pointsPair = null;
    BufferData? boundPair = null;
    BufferData? triangulationPair = null;

    public Polygon(float[] contour) : this([ contour ]) { }

    public Polygon(float[] contour, float[][] holes) : this([ contour, ..holes ]) { }

    public Polygon(float[][] rings) : this(rings, FillRule.Nesting) { }

    public Polygon(float[][] rings, FillRule rule)
    {
        this.rings = rings;
        this.rule = rule;
        data = Join(rings);
    }

    /// <summary>
    /// Get the number of closed contours of this polygon.
    /// </summary>
    public int Contours => rings.Length;

    public int Rows => data.Length / 3;

    public int Columns => 3;

    public int Instances => 1;

    public int InstanceLength => Rows;

    public bool IsGeometry => true;

    public Buffer Buffer => buffer ??= Buffer.From(this);

    public Changes Changes { get; set; } = [];

    public IBufferedData Triangules
        => triangulationPair ??= FindTriangules();

    public IBufferedData Lines
        => boundPair ??= FindBounds();

    public IBufferedData Points
        => pointsPair ??= FindPoints();

    public float this[int index]
    {
        get => data[index];
        set => throw new ImutablePolygonException();
    }

    public float[] GetBufferData()
        => data[..];

    /// <summary>
    /// Get a copy of a closed contour of this polygon.
    /// </summary>
    public float[] GetContour(int index)
        => rings[index][..];

    BufferData FindPoints()
        => CreateBuffer(data[..]);

    BufferData FindBounds()
        => CreateBuffer(Bounds.GetBounds(rings));

    BufferData FindTriangules()
        => CreateBuffer(Triangulations.Triangulate(rings, rule));

    static BufferData CreateBuffer(float[] points)
    {
        var bufferData = new BufferData(3, points.Length / 3, true);
        bufferData.AddRange(points);

        return bufferData;
    }

    /// <summary>
    /// Put every ring on a single array.
    /// </summary>
    static float[] Join(float[][] rings)
    {
        if (rings.Length == 1)
            return rings[0];

        var size = 0;
        foreach (var ring in rings)
            size += ring.Length;

        var data = new float[size];
        var offset = 0;

        foreach (var ring in rings)
        {
            Array.Copy(ring, 0, data, offset, ring.Length);
            offset += ring.Length;
        }

        return data;
    }

    public static implicit operator Polygon(float[] data) => new(data);

    public static VirtualPolygons operator *(Polygon polygon, int times)
        => new (polygon, times);

    public static VirtualPolygons operator *(int times, Polygon polygon)
        => new (polygon, times);

    public override string ToString()
        => $$"""
        Polygon {
            Rows: {{Rows}},
            Columns: {{Columns}},
            Contours: {{Contours}},
            Instances: {{Instances}},
            InstanceLength: {{InstanceLength}},
            IsGeometry: {{IsGeometry}}
        }
        """;
}
