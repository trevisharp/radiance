/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
namespace Radiance.Contexts;

/// <summary>
/// A Builder for a BufferContext.
/// </summary>
public abstract class BufferContextBuilder
{
    public abstract BufferContext Build();
}