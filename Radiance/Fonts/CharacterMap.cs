/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2026
 */
namespace Radiance.Fonts;

using static FontData;

/// <summary>
/// The table that turns a character into a glyph index. Fonts ship this on
/// several formats; the two that matter here are format 4, the segmented map
/// every Windows font carries, and format 12, used when the font goes past
/// the basic plane.
/// </summary>
public class CharacterMap
{
    byte[] data = [];
    int format;
    bool symbol;

    // Format 4.
    int segmentCount;
    int endCodePosition;
    int startCodePosition;
    int idDeltaPosition;
    int idRangeOffsetPosition;

    // Format 12.
    int groupCount;
    int groupPosition;

    /// <summary>
    /// Pick the best subtable of a cmap table and read its header.
    /// Returns null when no readable subtable exists.
    /// </summary>
    public static CharacterMap? Read(byte[]? cmap)
    {
        if (cmap is null || !Fits(cmap, 4, 0))
            return null;

        var count = U16(cmap, 2);
        var bestScore = -1;
        CharacterMap? best = null;

        for (int i = 0; i < count; i++)
        {
            var record = 4 + 8 * i;
            if (!Fits(cmap, record, 8))
                break;

            var platform = U16(cmap, record);
            var encoding = U16(cmap, record + 2);
            var offset = (int)U32(cmap, record + 4);

            if (!Fits(cmap, offset, 2))
                continue;

            var format = U16(cmap, offset);
            var score = Score(platform, encoding, format);
            if (score <= bestScore)
                continue;

            var map = Build(cmap, offset, format, platform, encoding);
            if (map is null)
                continue;

            bestScore = score;
            best = map;
        }

        return best;
    }

    /// <summary>
    /// Rank a subtable. Unicode wins over symbol, and the wider format wins
    /// when both are available.
    /// </summary>
    static int Score(int platform, int encoding, int format)
    {
        if (format is not 4 and not 12)
            return -1;

        var wide = format == 12 ? 1 : 0;

        // Windows Unicode full repertoire, Windows BMP, and plain Unicode.
        if (platform == 3 && encoding == 10) return 8 + wide;
        if (platform == 3 && encoding == 1) return 6 + wide;
        if (platform == 0) return 4 + wide;

        // Windows symbol: char codes live on the private use area.
        if (platform == 3 && encoding == 0) return 2 + wide;

        return 0 + wide;
    }

    static CharacterMap? Build(byte[] cmap, int offset, int format, int platform, int encoding)
    {
        var map = new CharacterMap
        {
            data = cmap,
            format = format,
            symbol = platform == 3 && encoding == 0
        };

        if (format == 4)
        {
            if (!Fits(cmap, offset + 14, 0))
                return null;

            map.segmentCount = U16(cmap, offset + 6) / 2;
            map.endCodePosition = offset + 14;
            map.startCodePosition = map.endCodePosition + 2 * map.segmentCount + 2;
            map.idDeltaPosition = map.startCodePosition + 2 * map.segmentCount;
            map.idRangeOffsetPosition = map.idDeltaPosition + 2 * map.segmentCount;

            return Fits(cmap, map.idRangeOffsetPosition, 2 * map.segmentCount) ? map : null;
        }

        if (!Fits(cmap, offset + 16, 0))
            return null;

        map.groupCount = (int)U32(cmap, offset + 12);
        map.groupPosition = offset + 16;

        return Fits(cmap, map.groupPosition, 12 * map.groupCount) ? map : null;
    }

    /// <summary>
    /// Get the glyph index of a character, or 0 when the font has none.
    /// </summary>
    public int Find(char character)
    {
        var index = FindCode(character);

        // Symbol fonts store their characters shifted into the private use
        // area, so a plain letter has to be looked up there too.
        if (index == 0 && symbol)
            index = FindCode(0xF000 | character);

        return index;
    }

    int FindCode(int code)
        => format == 4 ? FindOnSegments(code) : FindOnGroups(code);

    int FindOnSegments(int code)
    {
        for (int i = 0; i < segmentCount; i++)
        {
            // Segments are sorted, so the first one ending at or after the
            // character is the only one that can hold it.
            if (U16(data, endCodePosition + 2 * i) < code)
                continue;

            if (U16(data, startCodePosition + 2 * i) > code)
                return 0;

            var delta = S16(data, idDeltaPosition + 2 * i);
            var rangeOffset = U16(data, idRangeOffsetPosition + 2 * i);

            if (rangeOffset == 0)
                return (code + delta) & 0xFFFF;

            var start = U16(data, startCodePosition + 2 * i);
            var position = idRangeOffsetPosition + 2 * i + rangeOffset + 2 * (code - start);
            if (!Fits(data, position, 2))
                return 0;

            var glyph = U16(data, position);
            return glyph == 0 ? 0 : (glyph + delta) & 0xFFFF;
        }

        return 0;
    }

    int FindOnGroups(int code)
    {
        var low = 0;
        var high = groupCount - 1;

        while (low <= high)
        {
            var middle = (low + high) / 2;
            var group = groupPosition + 12 * middle;
            var start = U32(data, group);
            var end = U32(data, group + 4);

            if (code < start)
                high = middle - 1;
            else if (code > end)
                low = middle + 1;
            else
                return (int)(U32(data, group + 8) + (code - start));
        }

        return 0;
    }
}
