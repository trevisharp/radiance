/* Author:  Leonardo Trevisan Silio
 * Date:    23/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

using Data;
using Objects;

/// <summary>
/// Represents a dependece of a position buffer data.
/// </summary>
public class ColorBufferDependence : BufferDependence<Vec4ShaderObject>
{
    public ColorBufferDependence(IData data, int position = 1)
        : base("color", data, position) { }
}