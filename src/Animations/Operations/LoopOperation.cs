/* Author:  Leonardo Trevisan Silio
 * Date:    05/12/2024
 */
namespace Radiance.Animations.Operations;

/// <summary>
/// A operation that add a waiting time on animation.
/// </summary>
public class LoopOperation : AnimationOperation
{
    public override void OnAdd(AnimationData data)
    {
        data.TimeExpression = Utils.mod(data.TimeExpression, data.Duration);
    }
}