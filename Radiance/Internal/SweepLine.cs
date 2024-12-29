/* Author:  Leonardo Trevisan Silio
 * Date:    29/12/2024
 */
using System;

namespace Radiance.Internal;

/// <summary>
/// Represents a SweepLine algorithm.
/// </summary>
public readonly ref struct SweepLine(Span<PlanarVertex> points, Span<int> map)
{
    readonly Span<PlanarVertex> points = points;
    readonly Span<int> map = map;

    public ref PlanarVertex this[int index] => ref points[map[index]];

    public static SweepLine Create(Span<PlanarVertex> points, Span<int> map)
    {
        Sort(points, map);
        return new SweepLine(points, map);
    }

    const int sortTreshold = 8;
    
    /// <summary>
    /// Sort elements usings values has data[map[i] + offsetA] to order
    /// and data[map[i] + offsetB] on ties. Return a array of positions.
    /// </summary>
    static void Sort(Span<PlanarVertex> data, Span<int> map)
    {
        for (int i = 0; i < map.Length; i++)
            map[i] = i;

        QuickSort(data, map, 0, map.Length);
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
        var pivo = data[pivoIndex].Y;

        map[goodPivoIndex] = map[end - 1];
        map[end - 1] = pivoIndex;

        int i = start, j = end - 2;
        while (i < j)
        {
            float iv = data[map[i]].Y;
            while(iv < pivo && i < j)
                iv = data[map[++i]].Y;
            
            float jv = data[map[j]].Y;
            while (jv > pivo && i < j)
                jv = data[map[--j]].Y;

            if (i >= j)
                break;

            (map[j], map[i]) = (map[i], map[j]);
        }

        if (data[map[j]].Y < pivo)
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
        bool sorted = false;
        while (!sorted)
        {
            sorted = true;
            for (int i = start; i < end - 1; i++)
            {
                int j = map[i],
                    k = map[i + 1];
                var v1 = data[j].Y;
                var v2 = data[k].Y;
                if (v1 < v2)
                    continue;

                if (v1 == v2)
                {
                    v1 = data[j].X;
                    v2 = data[k].X;
                    if (v1 <= v2)
                        continue;
                }
                
                map[i] = k;
                map[i + 1] = j;
                sorted = false;
            }
        }
    }
}