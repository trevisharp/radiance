/* Author:  Leonardo Trevisan Silio
 * Date:    05/12/2024
 */
using System;
using System.Collections.Generic;

namespace Radiance.Animations;

using Operations;
using Shaders.Objects;

/// <summary>
/// A Builder for any animation.
/// </summary>
public class AnimationBuilder
{
    public List<AnimationOperation> Operations { get; private set; } = [];
    public AnimationData Data { get; private set; } = new();

    public void Add(AnimationOperation operation)
    {
        operation.OnAdd(Data);
        Operations.Add(operation);
    }

    /// <summary>
    /// Add a step on animation.
    /// </summary>
    public void Step(float duration, Action<FloatShaderObject> action)
        => Add(new StepOperation(duration, action));
    
    /// <summary>
    /// Add a wait on animation.
    /// </summary>
    public void Wait(float duration)
        => Add(new WaitOperation(duration));
    
    /// <summary>
    /// Make this operation occurs on a loop.
    /// </summary>
    public void Loop()
        => Add(new LoopOperation());

    public void Build()
    {
        foreach (var operation in Operations)
            operation.OnBuild(Data);
    }
}