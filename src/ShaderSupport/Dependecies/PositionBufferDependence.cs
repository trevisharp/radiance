/* Author:  Leonardo Trevisan Silio
 * Date:    23/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

using Data;
using Objects;

/// <summary>
/// Represents a dependece of a position buffer data.
/// </summary>
public class PositionBufferDependence : BufferDependence<Vec3ShaderObject>
{
    public PositionBufferDependence(IData data, int position = 0)
        : base("position", data, position) { }
}