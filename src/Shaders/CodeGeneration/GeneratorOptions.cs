/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2024
 */
namespace Radiance.Shaders.CodeGeneration;

/// <summary>
/// Configuration used on Code Generator.
/// </summary>
public class GeneratorOptions
{
    /// <summary>
    /// Use condinates pixel based.
    /// </summary>
    public bool PixelBased { get; set; }

    /// <summary>
    /// Use Z-Index between 0 to 1000.
    /// </summary>
    public bool LargeZIndex { get; set; }

    /// <summary>
    /// The default configuration used.
    /// </summary>
    public static GeneratorOptions Default { get; set; } = new()
    {
        LargeZIndex = true,
        PixelBased = true
    };
}