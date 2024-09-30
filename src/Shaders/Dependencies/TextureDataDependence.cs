/* Author:  Leonardo Trevisan Silio
 * Date:    30/09/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Contexts;
using Primitives;

/// <summary>
/// Represents a dependece of informations about a texture.
/// </summary>
public class TextureDataDependence(TextureDependence value) : ShaderDependence
{
    static int count = 0;

    public readonly string name = $"textureData{count++}";
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine($"uniform vec2 {name};");

    public override Action AddOperation(ShadeContext ctx)
        => () => ctx.SetVec(name, 
            value.Texture?.Width ?? 0,
            value.Texture?.Height?? 0
        );
}