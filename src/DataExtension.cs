/* Author:  Leonardo Trevisan Silio
 * Date:    26/02/2024
 */
using System.Linq;

namespace Radiance;

using Data;
using Internal;

/// <summary>
/// Extension class of util operations with Data.
/// </summary>
public static class DataExtension
{
    public static MutablePolygon Triangules(this MutablePolygon vectors)
    {
        var triangularization = VectorsOperations
            .PlanarPolygonTriangulation(vectors.Data.ToArray());
        
        var result = new MutablePolygon();
        for (int i = 0; i < triangularization.Length; i += 3)
        {
            result.Add(
                triangularization[i + 0],
                triangularization[i + 1],
                triangularization[i + 2]
            );
        }

        return result;
    }
}