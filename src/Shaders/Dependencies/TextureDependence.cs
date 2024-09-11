/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2023
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Managers;
using Primitives;

/// <summary>
/// Represents a dependece of a Sampler2D used by textures in OpenGL.
/// </summary>
public class TextureDependence(string textureName) : ShaderDependence
{
    private Texture texture = null!;
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine($"uniform sampler2D {textureName};");

    public override Action AddOperation(ShaderManager ctx)
        => () => ctx.SetTextureData(textureName, texture);

    public override void UpdateData(object value)
        => texture = (value as Texture)!;
}