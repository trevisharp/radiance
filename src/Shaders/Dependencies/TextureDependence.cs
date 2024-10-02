/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2023
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Contexts;
using Primitives;

/// <summary>
/// Represents a dependece of a Sampler2D used by textures in OpenGL.
/// </summary>
public class TextureDependence(string textureName) : ShaderDependence
{
    public Texture? Texture { get; private set; }
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine($"uniform sampler2D {textureName};");

    public override Action AddOperation(ShaderContext ctx)
        => () => 
        {
            if (Texture is null)
                return;
            
            ctx.SetTextureData(textureName, Texture);
        };

    public override void UpdateData(object value)
        => Texture = value as Texture;
}