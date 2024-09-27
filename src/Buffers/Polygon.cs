/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
using System;
using System.Collections.Generic;

namespace Radiance.Buffers;

using Primitives;
using Exceptions;

/// <summary>
/// A base type for all polygons and lines.
/// </summary>
public class Polygon(float[] data) : IBufferedData
{
    private Polygon triangulationPair = null!;

    /// <summary>
    /// Get the triangulation of this polygon.
    /// </summary>
    public Polygon Triangulation
    {
        get
        {
            if (triangulationPair is not null)
                return triangulationPair;
            
            var triangules = Operations
                .PlanarPolygonTriangulation([ ..Data ]);
            
            Polygon polygon = new Polygon(triangules);
            
            polygon.triangulationPair = polygon;
            triangulationPair = polygon;

            return triangulationPair;
        }
    }
    
    public float[] Data => data;

    /// <summary>
    /// Get the id of the buffer associated with the polygon data.
    /// </summary>
    public int? BufferId { get; set; } = null;
    
    public Buffer? Buffer { get; set; }
    
    public int Vertices => Data.Length / 3;

    
    public static implicit operator Polygon(float[] data) => new(data);

    /// <summary>
    /// Get a circle with radius 1 centralizated in (0, 0, 0)
    /// with 128 sides.
    /// </summary>
    public static readonly Polygon Circle = Ellipse(1, 1, 128);

    /// <summary>
    /// Get a square with side 1 centralizated in (0, 0, 0).
    /// </summary>
    public static readonly Polygon Square = Rect(1, 1);

    /// <summary>
    /// Create a rectangle with specific width and height
    /// centralized on (0, 0, 0) cordinate.
    /// </summary>
    public static Polygon Rect(float width, float height)
    {
        var halfWid = width / 2;
        var halfHei = height / 2;
        return new Polygon([
            -halfWid, -halfHei, 0f,
            -halfHei, halfWid, 0f,
            halfHei, halfWid, 0f,
            halfHei, -halfWid, 0f
        ]);
    }

    /// <summary>
    /// Create a rectangle with specific width and height
    /// centralized on (x, y, z) cordinate.
    /// </summary>
    public static Polygon Rect(
        float x, float y, float z,
        float width, float height)
    {
        var halfWid = width / 2;
        var halfHei = height / 2;
        return new Polygon([
            x - halfWid, y - halfHei, z,
            x - halfWid, y + halfHei, z,
            x + halfWid, y + halfHei, z,
            x + halfWid, y - halfHei, z
        ]);
    }

    /// <summary>
    /// Create a ellipse with specific a and b radius
    /// centralized on (x, y, z) cordinate with a specific
    /// quantity of sides.
    /// </summary>
    public static Polygon Ellipse(
        float x, float y, float z,
        float a, float b = float.NaN,
        int points = 63
    )
    {
        var result = new List<float>();

        float phi = MathF.Tau / points;
        if (float.IsNaN(b))
            b = a;

        for (int k = 0; k < points; k++)
        {
            result.Add(a * MathF.Cos(phi * k) + x);
            result.Add(b * MathF.Sin(-phi * k) + y);
            result.Add(z);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Create a ellipse with specific a and b radius
    /// centralized on (0, 0, 0) cordinate with a specific
    /// quantity of sides.
    /// </summary>
    public static Polygon Ellipse(
        float a, float b = float.NaN,
        int points = 63
    )
    {
        var result = new List<float>();

        float phi = MathF.Tau / points;
        if (float.IsNaN(b))
            b = a;

        for (int k = 0; k < points; k++)
        {
            result.Add(a * MathF.Cos(phi * k));
            result.Add(b * MathF.Sin(-phi * k));
            result.Add(0);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Create a polygon using a polar coordinates.
    /// The polarFunc is a function that recieves a angle (0 to 2pi)
    /// and returns the distânce of the center (x, y, z).
    /// </summary>
    public static Polygon Polar(
        Func<float, int, float> polarFunc,
        float x = 0, float y = 0, float z = 0,
        int points = 63
    )
    {
        var result = new List<float>();

        float phi = MathF.Tau / points;

        for (int k = 0; k < points; k++)
        {
            float angle = phi * k;
            float dist = polarFunc(angle, k);
            result.Add(x + dist * MathF.Cos(angle));
            result.Add(y + dist * MathF.Sin(-angle));
            result.Add(z);
        }

        return result.ToArray();
    }

    
    /// <summary>
    /// Create a polygon using a polar coordinates.
    /// The polarFunc is a function that recieves a angle (0 to 2pi)
    /// and returns the distânce of the center (x, y, z).
    /// </summary>
    public static Polygon Polar(
        Func<float, float> polarFunc,
        float x = 0, float y = 0, float z = 0,
        int points = 63
    ) => Polar((a, i) => polarFunc(a), x, y, z, points);

    /// <summary>
    /// Create a polygon based in recived data.
    /// </summary>
    public static Polygon FromData(params Vec3[] vectors)
    {
        var result = new List<float>();

        foreach (var v in vectors)
        {
            result.Add(v.X);
            result.Add(v.Y);
            result.Add(v.Z);
        }
        
        return result.ToArray();
    }
    
    /// <summary>
    /// Create a polygon based in recived data.
    /// </summary>
    public static Polygon FromData(params Vec2[] vectors)
    {
        var result = new List<float>();

        foreach (var v in vectors)
        {
            result.Add(v.X);
            result.Add(v.Y);
            result.Add(0);
        }
        
        return result.ToArray();
    }

    /// <summary>
    /// Get a rectangle with size of opened screen centralizated in center of screen.
    /// </summary>
    public static Polygon Screen => 
        Window.IsOpen ? Rect(
            Window.Width / 2, 
            Window.Height / 2, 0, 
            Window.Width, 
            Window.Height
        ) : throw new WindowClosedException();
}