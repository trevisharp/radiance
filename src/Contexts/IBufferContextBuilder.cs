/* Author:  Leonardo Trevisan Silio
 * Date:    24/10/2024
 */
namespace Radiance.Contexts;

/// <summary>
/// A Builder for a BufferContext.
/// </summary>
public interface IBufferContextBuilder
{
    /// <summary>
    /// Build the buffer context.
    /// </summary>
    IBufferContext Build();
}