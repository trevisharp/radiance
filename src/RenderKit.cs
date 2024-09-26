/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2024
 */
namespace Radiance;

using System.Runtime.InteropServices;
using Radiance.Shaders.Objects;
using static Radiance.Utils;

/// <summary>
/// A Kit whit some Render default implementations.
/// </summary>
public class RenderKit
{
    public readonly static RenderKit Shared = new();

    private dynamic? fillRender;
    /// <summary>
    /// Receiving rgba color, fill a polygon.
    /// </summary>
    public void Fill(dynamic r, dynamic g, dynamic b, dynamic a)
    {
        fillRender ??= render((r, g, b, a) =>
        {
            color = (r, g, b, a);
            fill();
        });

        fillRender(r, g, b, a);
    }

    private dynamic? drawRender;
    /// <summary>
    /// Receiving rgba color, draw a polygon.
    /// </summary>
    public void Draw(dynamic r, dynamic g, dynamic b, dynamic a)
    {
        drawRender ??= render((r, g, b, a) =>
        {
            color = (r, g, b, a);
            draw();
        });

        drawRender(r, g, b, a);
    }

    private dynamic? centralizeRender;
    /// <summary>
    /// Centralize a polygon on the center of the screen.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public void Centralize()
    {
        centralizeRender ??= render(() => {
            pos += (width / 2, height / 2, 0);
        });

        centralizeRender();
    }

    private dynamic? zoomRender;
    /// <summary>
    /// Receiving x, y and a factor, performa a zoom on polygon on point (x, y) with the factor scale.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public void Zoom(dynamic x, dynamic y, dynamic factor)
    {
        zoomRender ??= render((cx, cy, factor) => {
            var nx = factor * (pos.x - cx) + cx;
            var ny = factor * (pos.y - cy) + cy;
            pos = (nx, ny, pos.z);
        });

        zoomRender(x, y, factor);
    }

    private dynamic? rotateRender;
    public void Rotate(dynamic speed)
    {
        rotateRender ??= render(speed => {
            pos = (
                pos.x * cos(speed * t) - pos.y * sin(speed * t),
                pos.y * cos(speed * t) + pos.x * sin(speed * t),
                pos.z);
        });

        rotateRender(speed);
    }
}