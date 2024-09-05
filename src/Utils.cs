/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2024
 */
#pragma warning disable IDE1006

using System;
using System.Text;

namespace Radiance;

using Primitives;
using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;
using Renders;
using Exceptions;

/// <summary>
/// A facade with all utils to use Radiance features.
/// </summary>
public static class Utils
{
    internal readonly static WidthWindowDependence widthDep = new();
    internal readonly static HeightWindowDependence heightDep = new();

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action function)
    {
        ArgumentNullException.ThrowIfNull(function, nameof(function));
        
        var render = new Render(function);
        render.Load();

        return render;
    }
}