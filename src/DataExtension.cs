/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
namespace Radiance;

using Data;
using Internal;

/// <summary>
/// Extension class of util operations with Data.
/// </summary>
public static class DataExtension
{
    public static Polygon Triangules(this Polygon vectors)
    {
        var triangularization = VectorsOperations
            .PlanarPolygonTriangulation(vectors.Data);
        
        var result = new Polygon();
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