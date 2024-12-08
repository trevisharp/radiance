/* Author:  Leonardo Trevisan Silio
 * Date:    05/12/2024
 */
namespace Radiance.Animations;

/// <summary>
/// A base class for all operations executed on a animation.
/// </summary>
public abstract class AnimationOperation
{
    /// <summary>
    /// Method executed when operations is added.
    /// </summary>
    public virtual void OnAdd(AnimationData data) { }

    /// <summary>
    /// Method executed when operation is built.
    /// </summary>
    public virtual void OnBuild(AnimationData data) { }
}