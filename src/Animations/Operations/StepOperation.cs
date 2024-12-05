/* Author:  Leonardo Trevisan Silio
 * Date:    05/12/2024
 */
using System;

namespace Radiance.Animations.Operations;

using Shaders.Objects;

public class StepOperation(Action<FloatShaderObject> setpFunc) : AnimationOperation
{
    readonly Action<FloatShaderObject> StepFunc = setpFunc;

    public override void OnBuild(AnimationData data)
    {
        
    }
}