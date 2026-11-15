/* Author:  Leonardo Trevisan Silio
 * Date:    15/11/2026
 */
namespace Radiance.Exceptions;

public class FragmentConditionOnPositionException : RadianceException
{
    public override string ErrorMessage =>
        """
        A rif with a condition that depends on pixel data (like x, y or textures) can not change pos.
        The position is computed on the vertex shader, before the pixels exist.
        """;
}
