/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2025
 */
namespace Radiance.Fonts;

using Bufferings;

/// <summary>
/// A interface for all fonts
/// </summary>
public interface IFont
{
    /// <summary>
    /// Return true if the font already is loaded.
    /// </summary>
    bool IsLoaded { get; }

    /// <summary>
    /// Load internal data from file. Returns false if the
    /// type cannot handle the file.
    /// </summary>
    bool LoadFromFile(string filePath);

    /// <summary>
    /// Get a polygon generated from a string.
    /// </summary>
    IPolygon GetPolygon(string text);
}