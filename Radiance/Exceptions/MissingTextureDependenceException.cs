/* Author:  Leonardo Trevisan Silio
 * Date:    30/09/2024
 */
namespace Radiance.Exceptions;

using Shaders.Dependencies;

public class MissingTextureDependenceException : RadianceException
{
    public override string ErrorMessage =>
        $"""
        A {nameof(img)} cannot be create wihtout a
        {nameof(TextureDependence)} has a dependence.
        """;
}