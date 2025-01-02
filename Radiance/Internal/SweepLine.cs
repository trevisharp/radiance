/* Author:  Leonardo Trevisan Silio
 * Date:    02/01/2025
 */
using System;
using System.Linq;

namespace Radiance.Internal;

/// <summary>
/// Represents a SweepLine algorithm.
/// </summary>
public readonly ref struct SweepLine(Span<PlanarVertex> points, Span<int> map)
{
    readonly Span<PlanarVertex> source = points;

    public readonly Span<int> MapBuffer = map;

    public int Length => MapBuffer.Length;
    
    public ref PlanarVertex this[int index] => ref source[MapBuffer[index]];

    public SweepLine ApplyFilter(int[] points)
    {
        Span<int> modifiedMap = new int[points.Length];
        
        for (int i = 0, j = 0; i < MapBuffer.Length; i++)
        {
            if (points.Contains(MapBuffer[i]))
                modifiedMap[j++] = MapBuffer[i];
        }

        return new SweepLine(this.source, modifiedMap);
    }

    public static SweepLine Create(Span<PlanarVertex> points, Span<int> map)
    {
        Sort(points, map);
        return new SweepLine(points, map);
    }
    
    const int sortTreshold = 16;
    
    /// <summary>
    /// Sort elements usings values has data[map[i] + offsetA] to order
    /// and data[map[i] + offsetB] on ties. Return a array of positions.
    /// </summary>
    static void Sort(Span<PlanarVertex> data, Span<int> map)
    {
        for (int i = 0; i < data.Length; i++)
            map[i] = i;

        QuickSort(data, map, 0, data.Length);
    }

    /// <summary>
    /// Considering a map of positions and a data, sort elements between start and end - 1
    /// values using data[map[i] + offsetA] to order and data[map[i] + offsetB] on ties.
    /// </summary>
    static void QuickSort(Span<PlanarVertex> data, Span<int> map, int start, int end)
    {
        int len = end - start;
        if (len < sortTreshold)
        {
            SlowSort(data, map, start, end);
            return;
        }

        var goodPivoIndex = start + len / 4;
        var pivoIndex = map[goodPivoIndex];
        var pivo = data[pivoIndex].Yp;
        var pivo2 = data[pivoIndex].Xp;

        map[goodPivoIndex] = map[end - 1];
        map[end - 1] = pivoIndex;

        int i = start, j = end - 2;
        while (i < j)
        {
            float iv = data[map[i]].Yp;
            float iv2 = data[map[i]].Xp;
            while((iv > pivo || (iv == pivo && iv2 > pivo2)) && i < j)
            {
                iv = data[map[++i]].Yp;
                iv2 = data[map[i]].Xp;
            }
            
            float jv = data[map[j]].Yp;
            float jv2 = data[map[j]].Xp;
            while ((jv < pivo || (jv == pivo && jv2 < pivo2)) && i < j)
            {
                jv = data[map[--j]].Yp;
                jv2 = data[map[j]].Xp;
            }

            if (i >= j)
                break;

            (map[j], map[i]) = (map[i], map[j]);
        }

        float lv = data[map[j]].Yp;
        float lv2 = data[map[j]].Xp;
        if (lv > pivo || (lv == pivo && lv2 > pivo2))
            j++;

        (map[end - 1], map[j]) = (map[j], map[end - 1]);
        QuickSort(data, map, start, j);
        QuickSort(data, map, j, end);
    }

    /// <summary>
    /// Considering a map of positions and a data, sort elements between start and end - 1
    /// values using data[map[i] + offsetA] to order and data[map[i] + offsetB] on ties.
    /// Fast for tiny vectors.
    /// </summary>
    static void SlowSort(Span<PlanarVertex> data, Span<int> map, int start, int end)
    {
        for (int i = start + 1; i < end; i++)
        {
            var index = map[i];
            var value = data[index].Yp;
            var value2 = data[index].Xp;

            var cmpPos = i - 1;
            var point = data[map[cmpPos]];

            while (point.Yp < value || (point.Yp == value && point.Xp < value2))
            {
                map[cmpPos + 1] = map[cmpPos];
                cmpPos--;
                if (cmpPos < start)
                    break;
                point = data[map[cmpPos]];
            }
            
            map[cmpPos + 1] = index;
        }
    }
}