/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
namespace Radiance.Renders;

/// <summary>
/// A chain that can handle arguments called from a render.
/// </summary>
public abstract class ArgumentHandlerChainLink
{
    /// <summary>
    /// Return true if the chain link can handle and return a result.
    /// </summary>
    public abstract bool CanHandle(ArgumentHandlerArgs args);

    /// <summary>
    /// Return true if the chain link can get args and update their values.
    /// </summary>
    public abstract bool CanUpdate(ArgumentHandlerArgs args);

    /// <summary>
    /// Apply update operation.
    /// </summary>
    
    public virtual ArgumentHandlerArgs Update(ArgumentHandlerArgs args) => args;

    /// <summary>
    /// Apply Handle Operation.
    /// </summary>
    public virtual object? Handle(ArgumentHandlerArgs args) => null;
}