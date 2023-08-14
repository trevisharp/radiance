/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2023
 */
namespace Radiance.Data;

/// <summary>
/// A base class to all data layouts.
/// </summary>
public abstract class Data
{
    public abstract int SetData(float[] arr, int indexoff);
    public abstract int Size { get; }
    public abstract string GetLayout { get; }
    public abstract string GetName { get; }
}