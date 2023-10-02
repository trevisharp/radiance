/* Author:  Leonardo Trevisan Silio
 * Date:    01/10/2023
 */
using System.Collections.Generic;

namespace Radiance.Internal;

internal class PolygonEdgeCollection
{
    int last;
    List<int>[] list;
    
    internal PolygonEdgeCollection(int verticesCount)
    {
        this.last = verticesCount - 1;
        list = new List<int>[verticesCount];
    }

    internal List<int> GetConnections(int i)
    {
        init(i);
        return list[i];
    }

    internal List<int[]> GetPolygons()
    {
        List<int[]> result = new List<int[]>();

        bool[] visited = new bool[list.Length];
        Stack<int> stack = new Stack<int>();


        for (int i = 0; i < list.Length; i++)
        {
            if (visited[i])
                continue;

            List<int> poly = new List<int>();

            stack.Clear();
            stack.Push(i);
            while (stack.Count > 0)
            {
                var index = stack.Pop();
                if (visited[index])
                    continue;
                
                visited[index] = true;
                poly.Add(index);

                foreach (var conn in list[index])
                    stack.Push(conn);
            }
            result.Add(poly.ToArray());
        }

        return result;
    }

    internal bool IsConnected(int i, int j)
    {
        if (i == j)
            return false;

        if (list[i] is null &&
            (i + 1 == j) ||
            (i - 1 == j) ||
            (i == 0 && j == last) ||
            (i == last && i == 0) 
        ) return true;

        return list[i].Contains(j);
    }

    internal void Connect(int i, int j)
    {
        init(i);
        init(j);
        list[i].Add(j);
        list[j].Add(i);
    }

    private void init(int index)
    {
        if (list[index] is not null)
            return;
        
        list[index] = new List<int>();
        list[index].Add(index == last ? 0 : index + 1);
        list[index].Add(index == 0 ? last : index - 1);

    }
}