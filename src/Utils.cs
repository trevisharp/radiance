/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2024
 */
using System;
using System.Text;

namespace Radiance;

using Data;
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
}