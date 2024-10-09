/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2024
 */
namespace Radiance.Renders.Factories;

/// <summary>
/// Represents a structure of a factory to generate parameters for a render.
/// </summary>
public abstract class RenderParameterFactory
{
    /// <summary>
    /// The main chain used to create factories on renders.
    /// </summary>
    public readonly static RenderParameterFactoryChain Chain = [];

    /// <summary>
    /// Fill data in args with the generated data by the factory.
    /// </summary>
    public abstract void GenerateData(int i, float[] buffer, int offset);

    /// <summary>
    /// Get if new data genrated by the factory is the same that last generation.
    /// </summary>
    public abstract bool NeedRegenerate { get; }
}