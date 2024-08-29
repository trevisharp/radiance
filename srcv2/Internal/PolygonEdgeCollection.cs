/* Author:  Leonardo Trevisan Silio
 * Date:    12/01/2024
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

    internal bool IsConnected(int i, int j)
    {
        if (i == j)
            return false;
        
        init(i);
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