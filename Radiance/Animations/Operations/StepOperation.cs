/* Author:  Leonardo Trevisan Silio
 * Date:    05/12/2024
 */
using System;

namespace Radiance.Animations.Operations;

/// <summary>
/// A operation that execute a function between a start and a end point.
/// </summary>
public class StepOperation(float duration, Action<val> setpFunc) : AnimationOperation
{
    float start;
    float end;

    public override void OnAdd(AnimationData data)
    {
        start = data.Duration;
        data.Duration += duration;
        end = data.Duration;
    }

    public override void OnBuild(AnimationData data)
    {
        var step = Utils.smoothstep(start, end, data.TimeExpression);
        setpFunc(step);
    }
}