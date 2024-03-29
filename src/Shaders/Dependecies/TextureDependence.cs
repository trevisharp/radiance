/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2023
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Data;

/// <summary>
/// Represents a dependece of a Sampler2D used by textures in OpenGL.
/// </summary>
public class TextureDependence(string textureName) : ShaderDependence
{
    private Texture texture = null;
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine($"uniform sampler2D {textureName};");

    public override Action AddOperation(ShaderContext ctx)
        => () => ctx.SetTextureData(texture, textureName);

    public override void UpdateData(object value)
        => this.texture = value as Texture;
}