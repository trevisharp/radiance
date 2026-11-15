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
    {
        if (a.IsConstant(true) || b.IsConstant(false))
            return b;

        if (b.IsConstant(true) || a.IsConstant(false))
            return a;

        return Union<boolean>($"({a} && {b})", a, b);
    }

    public static boolean operator |(boolean a, boolean b)
    {
        if (a.IsConstant(false) || b.IsConstant(true))
            return b;

        if (b.IsConstant(false) || a.IsConstant(true))
            return a;

        return Union<boolean>($"({a} || {b})", a, b);
    }

    public static boolean operator !(boolean a)
    {
        if (a.IsConstant(true))
            return false;

        if (a.IsConstant(false))
            return true;

        return Transform<boolean, boolean>($"(!{a})", a);
    }

    /// <summary>
    /// Get if this boolean is the constant true or false.
    /// </summary>
    public bool IsConstant(bool value)
        => Expression == (value ? "true" : "false");

    public static implicit operator boolean(bool value)
        => new (value ? "true" : "false", ShaderOrigin.Global, []);
}