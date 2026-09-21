/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2026
 */
using System;
using System.Collections.Generic;

namespace Radiance.Fonts;

using static FontData;

/// <summary>
/// The part of the True Type reader that turns the glyf table into closed
/// contours of straight segments.
/// </summary>
public partial class TrueTypeFont
{
    const int onCurvePoint = 0x01;
    const int xShortVector = 0x02;
    const int yShortVector = 0x04;
    const int repeatFlag = 0x08;
    const int xIsSameOrPositive = 0x10;
    const int yIsSameOrPositive = 0x20;

    const int argsAreWords = 0x0001;
    const int argsAreXYValues = 0x0002;
    const int haveScale = 0x0008;
    const int moreComponents = 0x0020;
    const int haveXAndYScale = 0x0040;
    const int haveTwoByTwo = 0x0080;

    const int maxCompositeDepth = 5;

    readonly record struct OutlinePoint(float X, float Y, bool OnCurve);

    /// <summary>
    /// Get the outline of a glyph, already flattened, on font units.
    /// </summary>
    Glyph GetGlyph(int index, int depth = 0)
    {
        if (glyphCache.TryGetValue(index, out var cached))
            return cached;

        var glyph = ReadGlyph(index, depth);

        if (depth == 0)
            glyphCache[index] = glyph;

        return glyph;
    }

    Glyph ReadGlyph(int index, int depth)
    {
        var advance = GetAdvanceWidth(index);

        if (glyf is null || glyphOffsets is null)
            return new Glyph([], advance);

        if (index < 0 || index + 1 >= glyphOffsets.Length)
            return new Glyph([], advance);

        var offset = glyphOffsets[index];
        var length = glyphOffsets[index + 1] - offset;

        // A glyph with no outline, like the space, is stored as an empty slot.
        if (length <= 0 || !Fits(glyf, offset, 10))
            return new Glyph([], advance);

        var numberOfContours = S16(glyf, offset);

        var contours = numberOfContours >= 0
            ? ReadSimpleGlyph(offset, numberOfContours)
            : ReadCompositeGlyph(offset, depth);

        return new Glyph(contours, advance);
    }

    /// <summary>
    /// Read a glyph made of its own points.
    /// </summary>
    float[][] ReadSimpleGlyph(int offset, int numberOfContours)
    {
        if (numberOfContours == 0)
            return [];

        var position = offset + 10;
        var endPoints = new int[numberOfContours];

        for (int i = 0; i < numberOfContours; i++, position += 2)
        {
            if (!Fits(glyf!, position, 2))
                return [];

            endPoints[i] = U16(glyf!, position);
        }

        var pointCount = endPoints[^1] + 1;
        if (pointCount <= 0)
            return [];

        // The hinting instructions steer the rasterizer on small sizes and
        // have nothing to say about the outline itself.
        if (!Fits(glyf!, position, 2))
            return [];

        position += 2 + U16(glyf!, position);

        var flags = ReadFlags(pointCount, ref position);
        if (flags is null)
            return [];

        var x = ReadCoordinates(flags, pointCount, xShortVector, xIsSameOrPositive, ref position);
        var y = ReadCoordinates(flags, pointCount, yShortVector, yIsSameOrPositive, ref position);
        if (x is null || y is null)
            return [];

        var contours = new List<float[]>(numberOfContours);
        var start = 0;

        foreach (var end in endPoints)
        {
            if (end < start || end >= pointCount)
                break;

            var points = new OutlinePoint[end - start + 1];
            for (int i = start; i <= end; i++)
                points[i - start] = new(x[i], y[i], (flags[i] & onCurvePoint) != 0);

            var contour = Flatten(points);
            if (contour.Length >= 9)
                contours.Add(contour);

            start = end + 1;
        }

        return [ ..contours ];
    }

    /// <summary>
    /// Read a glyph built from other glyphs. Accented letters are stored this
    /// way: the letter and the accent are separate glyphs placed together.
    /// </summary>
    float[][] ReadCompositeGlyph(int offset, int depth)
    {
        if (depth >= maxCompositeDepth)
            return [];

        var position = offset + 10;
        var contours = new List<float[]>();
        int flags;

        do
        {
            if (!Fits(glyf!, position, 4))
                break;

            flags = U16(glyf!, position);
            var glyphIndex = U16(glyf!, position + 2);
            position += 4;

            int arg1, arg2;
            if ((flags & argsAreWords) != 0)
            {
                if (!Fits(glyf!, position, 4))
                    break;

                arg1 = S16(glyf!, position);
                arg2 = S16(glyf!, position + 2);
                position += 4;
            }
            else
            {
                if (!Fits(glyf!, position, 2))
                    break;

                arg1 = (sbyte)glyf![position];
                arg2 = (sbyte)glyf![position + 1];
                position += 2;
            }

            float a = 1f, b = 0f, c = 0f, d = 1f;
            if ((flags & haveScale) != 0 && Fits(glyf!, position, 2))
            {
                a = d = F2Dot14(glyf!, position);
                position += 2;
            }
            else if ((flags & haveXAndYScale) != 0 && Fits(glyf!, position, 4))
            {
                a = F2Dot14(glyf!, position);
                d = F2Dot14(glyf!, position + 2);
                position += 4;
            }
            else if ((flags & haveTwoByTwo) != 0 && Fits(glyf!, position, 8))
            {
                a = F2Dot14(glyf!, position);
                b = F2Dot14(glyf!, position + 2);
                c = F2Dot14(glyf!, position + 4);
                d = F2Dot14(glyf!, position + 6);
                position += 8;
            }

            // Placing by matching point numbers is a rare legacy mode and is
            // treated as no displacement instead of being guessed.
            var dx = (flags & argsAreXYValues) != 0 ? arg1 : 0;
            var dy = (flags & argsAreXYValues) != 0 ? arg2 : 0;

            foreach (var contour in GetGlyph(glyphIndex, depth + 1).Contours)
                contours.Add(Transform(contour, a, b, c, d, dx, dy));
        }
        while ((flags & moreComponents) != 0);

        return [ ..contours ];
    }

    static float[] Transform(float[] contour, float a, float b, float c, float d, float dx, float dy)
    {
        var result = new float[contour.Length];

        for (int i = 0; i < contour.Length; i += 3)
        {
            var x = contour[i + 0];
            var y = contour[i + 1];
            result[i + 0] = a * x + c * y + dx;
            result[i + 1] = b * x + d * y + dy;
            result[i + 2] = contour[i + 2];
        }

        return result;
    }

    /// <summary>
    /// Read one flag per point. A flag can declare that it repeats, which is
    /// how long straight runs are compressed.
    /// </summary>
    byte[]? ReadFlags(int pointCount, ref int position)
    {
        var flags = new byte[pointCount];
        var read = 0;

        while (read < pointCount)
        {
            if (!Fits(glyf!, position, 1))
                return null;

            var flag = glyf![position++];
            flags[read++] = flag;

            if ((flag & repeatFlag) == 0)
                continue;

            if (!Fits(glyf!, position, 1))
                return null;

            var repeats = glyf![position++];
            for (int i = 0; i < repeats && read < pointCount; i++)
                flags[read++] = flag;
        }

        return flags;
    }

    /// <summary>
    /// Read one axis of the points. Coordinates are stored as deltas from the
    /// previous point, on one or two bytes, and a flag can say the value did
    /// not change at all.
    /// </summary>
    int[]? ReadCoordinates(byte[] flags, int pointCount, int shortBit, int sameOrPositiveBit, ref int position)
    {
        var values = new int[pointCount];
        var value = 0;

        for (int i = 0; i < pointCount; i++)
        {
            var flag = flags[i];
            var isShort = (flag & shortBit) != 0;
            var sameOrPositive = (flag & sameOrPositiveBit) != 0;

            if (isShort)
            {
                if (!Fits(glyf!, position, 1))
                    return null;

                var delta = glyf![position++];
                value += sameOrPositive ? delta : -delta;
            }
            else if (!sameOrPositive)
            {
                if (!Fits(glyf!, position, 2))
                    return null;

                value += S16(glyf!, position);
                position += 2;
            }

            values[i] = value;
        }

        return values;
    }

    /// <summary>
    /// Turn a contour of on and off curve points into a closed ring of straight
    /// segments. True Type draws with quadratic curves where the off curve
    /// points are the controls, and two off curve points in a row imply an on
    /// curve point exactly between them.
    /// </summary>
    float[] Flatten(OutlinePoint[] points)
    {
        if (points.Length < 2)
            return [];

        var expanded = InsertImpliedPoints(points);

        var first = -1;
        for (int i = 0; i < expanded.Count && first < 0; i++)
        {
            if (expanded[i].OnCurve)
                first = i;
        }

        if (first < 0)
            return [];

        var ring = new List<float>(3 * expanded.Count);
        var current = expanded[first];
        Add(ring, current);

        var position = 1;
        while (position <= expanded.Count)
        {
            var point = expanded[(first + position) % expanded.Count];

            if (point.OnCurve)
            {
                Add(ring, point);
                current = point;
                position++;
                continue;
            }

            var end = expanded[(first + position + 1) % expanded.Count];
            AddQuadratic(ring, current, point, end);
            current = end;
            position += 2;
        }

        return [ ..ring ];
    }

    /// <summary>
    /// Put back the on curve points the font leaves implied between every pair
    /// of consecutive control points.
    /// </summary>
    static List<OutlinePoint> InsertImpliedPoints(OutlinePoint[] points)
    {
        var expanded = new List<OutlinePoint>(2 * points.Length);

        for (int i = 0; i < points.Length; i++)
        {
            var current = points[i];
            var next = points[(i + 1) % points.Length];

            expanded.Add(current);

            if (current.OnCurve || next.OnCurve)
                continue;

            expanded.Add(new(
                (current.X + next.X) / 2,
                (current.Y + next.Y) / 2,
                true
            ));
        }

        return expanded;
    }

    /// <summary>
    /// Break a quadratic curve into straight segments. Longer curves get more
    /// segments, measured against the em so that every font behaves the same.
    /// </summary>
    void AddQuadratic(List<float> ring, OutlinePoint start, OutlinePoint control, OutlinePoint end)
    {
        var length = Distance(start, control) + Distance(control, end);
        var segments = Math.Clamp(2 + (int)(Quality * length / unitsPerEm), 2, 64);

        for (int i = 1; i <= segments; i++)
        {
            var t = (float)i / segments;
            var inverse = 1 - t;
            var a = inverse * inverse;
            var b = 2 * inverse * t;
            var c = t * t;

            ring.Add(a * start.X + b * control.X + c * end.X);
            ring.Add(a * start.Y + b * control.Y + c * end.Y);
            ring.Add(0f);
        }
    }

    static float Distance(OutlinePoint a, OutlinePoint b)
        => MathF.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

    static void Add(List<float> ring, OutlinePoint point)
    {
        ring.Add(point.X);
        ring.Add(point.Y);
        ring.Add(0f);
    }
}
