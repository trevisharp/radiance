/* Author:  Leonardo Trevisan Silio
 * Date:    24/12/2024
 */
#pragma warning disable CS8981
#pragma warning disable IDE1006
#pragma warning disable IDE0130

using System.Collections.Generic;

namespace Radiance;

using Shaders;

/// <summary>
/// Represent a Bool data in shader implementation.
/// </summary>
public class boolean : ShaderObject
{
    public boolean(
        string value, ShaderOrigin origin,
        IEnumerable<ShaderDependence> deps
        ) : base(ShaderType.Bool, value, origin, deps) { }

    public static boolean operator &(boolean a, boolean b)
        => Union<boolean>($"({a} & {b})", a, b);

    public static boolean operator |(boolean a, boolean b)
        => Union<boolean>($"({a} || {b})", a, b);
    
    public static boolean operator !(boolean a)
        => Transform<boolean, boolean>($"(!{a})", a);

    public static implicit operator boolean(bool value)
        => new (value.ToString(), ShaderOrigin.Global, []);
}