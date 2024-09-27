/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
namespace Radiance.OpenGL4;

using Contexts;

public class OpenGL4BufferContextBuilder : BufferContextBuilder
{
    public override BufferContext Build()
        => new OpenGL4BufferContext();
}