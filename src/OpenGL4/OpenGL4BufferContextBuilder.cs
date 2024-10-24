/* Author:  Leonardo Trevisan Silio
 * Date:    24/10/2024
 */
namespace Radiance.OpenGL4;

using Contexts;

public class OpenGL4BufferContextBuilder : IBufferContextBuilder
{
    public IBufferContext Build()
        => new OpenGL4BufferContext();
}