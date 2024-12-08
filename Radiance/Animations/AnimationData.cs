/* Author:  Leonardo Trevisan Silio
 * Date:    05/12/2024
 */
namespace Radiance.Animations;

using Shaders.Objects;

/// <summary>
/// Represents all information about a animation. 
/// </summary>
public class AnimationData
{
    public float Duration { get; set; } = 0;
    public FloatShaderObject TimeExpression { get; set; } = Utils.t;
}