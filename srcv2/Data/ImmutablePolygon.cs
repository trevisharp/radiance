/* Author:  Leonardo Trevisan Silio
 * Date:    26/02/2024
 */
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Data;

using Exceptions;

/// <summary>
/// Represents a data that can managed by GPU and drawed in screen.
/// </summary>
public class ImmutablePolygon(IEnumerable<float> data) : Polygon
{
    private readonly float[] data = data.ToArray();
    public override IEnumerable<float> Data => data;

    protected override void add(float x, float y, float z)
        => throw new ImmutablePolygonModifyException();

    public override Polygon Clone()
        => this;

    public override Polygon ToImmutable()
        => this;
}