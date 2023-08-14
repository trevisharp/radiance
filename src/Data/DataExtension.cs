/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2023
 */
namespace Radiance.Data;

public static class DataExtension
{
    public static float[] GetBuffer(this Data[] data)
    {
        if (data.Length == 0)
            return new float[0];
        
        float[] buffer = new float[data[0].Size * data.Length];

        int indexoff = 0;
        foreach (var value in data)
            indexoff = value.SetData(buffer, indexoff);
        
        return buffer;
    }
}