/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2024
 */
namespace Radiance.Contexts.OpenGL4;

public class OpenGL4ProgramContextBuilder : ProgramContextBuilder
{
    public override ProgramContext Build()
        => new OpenGL4ProgramContext();
}