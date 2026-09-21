/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2026
 */
namespace Radiance.Fonts;

/// <summary>
/// Binary helpers to read the big endian data of a font file.
/// </summary>
public static class FontData
{
    /// <summary>
    /// Read a unsigned 16 bits value. Used by counts, offsets and char codes,
    /// where 0xFFFF must read as 65535 and not as -1.
    /// </summary>
    public static int U16(byte[] data, int offset)
        => (data[offset] << 8) | data[offset + 1];

    /// <summary>
    /// Read a signed 16 bits value. Used by coordinates and deltas.
    /// </summary>
    public static int S16(byte[] data, int offset)
        => (short)((data[offset] << 8) | data[offset + 1]);

    /// <summary>
    /// Read a unsigned 32 bits value.
    /// </summary>
    public static uint U32(byte[] data, int offset)
        => ((uint)data[offset] << 24) | ((uint)data[offset + 1] << 16)
         | ((uint)data[offset + 2] << 8) | data[offset + 3];

    /// <summary>
    /// Read a F2Dot14 value: a fixed point number with 2 integer bits and
    /// 14 fraction bits, used by the transforms of composite glyphs.
    /// </summary>
    public static float F2Dot14(byte[] data, int offset)
        => S16(data, offset) / 16384f;

    /// <summary>
    /// Get if a offset and a size fit inside the data.
    /// </summary>
    public static bool Fits(byte[] data, int offset, int size)
        => offset >= 0 && size >= 0 && offset + size <= data.Length;
}

/// <summary>
/// An entry of the table directory of a font file.
/// </summary>
public record FontTable(string Tag, uint Checksum, int Offset, int Length);

/// <summary>
/// A glyph outline already flattened into closed rings of (x, y) points,
/// on font units, plus the space it occupies on a line.
/// </summary>
public record Glyph(float[][] Contours, float AdvanceWidth);
