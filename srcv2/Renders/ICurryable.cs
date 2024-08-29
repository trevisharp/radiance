/* Author:  Leonardo Trevisan Silio
 * Date:    23/01/2024
 */
namespace Radiance.Renders;

/// <summary>
/// Exposes the curry method for objects that can using Currying.
/// </summary>
public interface ICurryable
{
    /// <summary>
    /// Return a dynamic function appliyng currying.
    /// </summary>
    dynamic Curry(params object[] parameters);
}