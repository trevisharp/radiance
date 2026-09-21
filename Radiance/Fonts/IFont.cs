/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2026
 */
namespace Radiance.Fonts;

using Bufferings;

/// <summary>
/// A interface for all fonts.
/// </summary>
public interface IFont
{
    /// <summary>
    /// Return true if the font already is loaded.
    /// </summary>
    bool IsLoaded { get; }

    /// <summary>
    /// The name the font declares for itself, or the file name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The distance between the baselines of two lines, for a text of size 1.
    /// </summary>
    float LineHeight { get; }

    /// <summary>
    /// How far the tallest letters go over the baseline, for a text of size 1.
    /// </summary>
    float Ascender { get; }

    /// <summary>
    /// How far the lowest letters go under the baseline, for a text of size 1.
    /// Negative.
    /// </summary>
    float Descender { get; }

    /// <summary>
    /// Load internal data from file. Returns false if the
    /// type cannot handle the file.
    /// </summary>
    bool LoadFromFile(string filePath);

    /// <summary>
    /// Get if the font has a drawing for a character.
    /// </summary>
    bool HasCharacter(char character);

    /// <summary>
    /// Get the width a text occupies on a single line.
    /// </summary>
    float MeasureWidth(string text, float size = 1f);

    /// <summary>
    /// Get a polygon generated from a string. The baseline of the first line
    /// sits on y = 0 and the text grows to the right and downwards.
    /// </summary>
    IPolygon GetPolygon(string text);

    /// <summary>
    /// Get a polygon generated from a string with a specific size, breaking
    /// lines so that none is wider than maxWidth.
    /// </summary>
    IPolygon GetPolygon(string text, float size, float maxWidth = float.PositiveInfinity);
}
