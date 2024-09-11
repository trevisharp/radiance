/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2024
 */
namespace Radiance.OpenGL4;

using Managers;

public class OpenGL4ProgramManagerBuilder : ProgramManagerBuilder
{
    public override ProgramManager Build()
        => new OpenGL4ProgramManager();
}