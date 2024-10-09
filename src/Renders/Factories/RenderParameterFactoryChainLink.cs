/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2024
 */
namespace Radiance.Renders.Factories;

public abstract class RenderParameterFactoryChainLink
{
    /// <summary>
    /// Returns true if the value can be used to create a factory.
    /// </summary>
    public abstract bool Is(object value);

    /// <summary>
    /// Create a factory using a object, throw a error if the
    /// type cannot be created.
    /// </summary>
    public abstract RenderParameterFactory Create(object value);

    /// <summary>
    /// Gets or Sets the next link in the chain.
    /// </summary>
    public RenderParameterFactoryChainLink? Next { get; set; }
}