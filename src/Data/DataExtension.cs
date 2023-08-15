/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
namespace Radiance.Data;

using System;
using ShaderSupport.Objects;

/// <summary>
/// Extension class of util operations with Data.
/// </summary>
public static class DataExtension
{
    public static float[] GetBuffer(this Data data)
    {
        float[] buffer = new float[data.Size];

        data.SetData(buffer, 0);
        
        return buffer;
    }

    // public static Vectors transform(
    //     this Vectors data, 
    //     Func<Vec3ShaderObject, Vec3ShaderObject> transformation
    // )
    // {

    // }
}