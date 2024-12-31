/* Author:  Leonardo Trevisan Silio
 * Date:    31/12/2024
 */
namespace Radiance.Exceptions;

public class ColorException(string colorSystem, string channel, float minValue, float maxValue, float actually) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        In the atempt of create a color of type '{colorSystem}' the channel
        '{channel}' recive the value {actually} but only accepts the range {minValue}
        to {maxValue}.
        """;
}