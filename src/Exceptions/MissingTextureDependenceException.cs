/* Author:  Leonardo Trevisan Silio
 * Date:    30/09/2024
 */
namespace Radiance.Exceptions;

using Shaders.Objects;
using Shaders.Dependencies;

public class MissingTextureDependenceException : RadianceException
{
    public override string ErrorMessage =>
        $"""
        A {nameof(Sampler2DShaderObject)} cannot be create wihtout a
        {nameof(TextureDependence)} has a dependence.
        """;
}