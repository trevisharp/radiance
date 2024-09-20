/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2024
 */
namespace Radiance.Renders;

using System.Runtime.InteropServices;
using Radiance.Shaders.Objects;
using static Radiance.Utils;

/// <summary>
/// A Kit whit some Render default implementations.
/// </summary>
public class RenderKit
{
    public readonly static RenderKit Shared = new();

    private dynamic? _fill;
    /// <summary>
    /// Receiving rgba color, fill a polygon.
    /// </summary>
    public void Fill(dynamic r, dynamic g, dynamic b, dynamic a)
    {
        _fill ??= render((r, g, b, a) =>
        {
            color = (r, g, b, a);
            fill();
        });

        _fill(r, g, b, a);
    }

    private dynamic? _draw;
    /// <summary>
    /// Receiving rgba color, draw a polygon.
    /// </summary>
    public void Draw(dynamic r, dynamic g, dynamic b, dynamic a)
    {
        _draw ??= render((r, g, b, a) =>
        {
            color = (r, g, b, a);
            draw();
        });

        _draw(r, g, b, a);
    }

    private dynamic? centralize;
    /// <summary>
    /// Centralize a polygon on the center of the screen.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public void Centralize()
    {
        centralize ??= render(() => {
            pos += (width / 2, height / 2, 0);
        });

        centralize();
    }

    private dynamic? zoom;
    /// <summary>
    /// Receiving x, y and a factor, performa a zoom on polygon on point (x, y) with the factor scale.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public void Zoom(dynamic x, dynamic y, dynamic factor)
    {
        zoom ??= render((cx, cy, factor) => {
            var nx = factor * (pos.x - cx) + cx;
            var ny = factor * (pos.y - cy) + cy;
            pos = (nx, ny, pos.z);
        });

        zoom(x, y, factor);
    }
}