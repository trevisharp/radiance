/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2026
 */
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Radiance.Fonts;

using Internal;
using Bufferings;
using Exceptions;

using static FontData;

/// <summary>
/// A reader for True Type Font (.ttf) files.
/// </summary>
public partial class TrueTypeFont : IFont
{
    const uint trueTypeVersion = 0x00010000;
    const uint trueTypeMagic = 0x74727565;

    readonly Dictionary<string, FontTable> tables = [];
    readonly Dictionary<int, Glyph> glyphCache = [];
    readonly Dictionary<int, float> kerning = [];
    readonly Dictionary<char, int> characterCache = [];

    byte[]? glyf = null;
    byte[]? hmtx = null;
    int[]? glyphOffsets = null;
    CharacterMap? characterMap = null;

    int unitsPerEm = 1000;
    int numberOfHMetrics = 0;
    int ascender = 0;
    int descender = 0;
    int lineGap = 0;

    /// <summary>
    /// Get or set how many segments a curve spanning the whole em is broken
    /// into. Higher values make rounder letters and more triangules.
    /// </summary>
    public int Quality { get; set; } = 32;

    public bool IsLoaded => glyf is not null && glyphOffsets is not null && characterMap is not null;

    public string Name { get; private set; } = string.Empty;

    public float LineHeight => (ascender - descender + lineGap) / (float)unitsPerEm;

    public float Ascender => ascender / (float)unitsPerEm;

    public float Descender => descender / (float)unitsPerEm;

    public bool LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (extension is not ".ttf" and not ".otf")
            return false;

        var data = File.ReadAllBytes(filePath);
        if (data.Length < 12)
            return false;

        var version = U32(data, 0);
        if (version != trueTypeVersion && version != trueTypeMagic)
            return false;

        ReadTableDirectory(data);

        // OpenType files with PostScript outlines carry a CFF table instead of
        // glyf. They are a different outline format and are not handled here.
        if (!tables.ContainsKey("glyf") || !tables.ContainsKey("loca"))
            return false;

        ReadHead(data, filePath);
        ReadHorizontalHeader(data);
        ReadGlyphOffsets(data, filePath);

        glyf = ReadTable(data, tables["glyf"]);
        hmtx = tables.TryGetValue("hmtx", out var hmtxTable) ? ReadTable(data, hmtxTable) : null;

        characterMap = CharacterMap.Read(ReadTableOrNull(data, "cmap"));
        if (characterMap is null)
            throw new InvalidFontException(filePath, "no usable character map was found.");

        ReadKerning(ReadTableOrNull(data, "kern"));
        Name = ReadName(ReadTableOrNull(data, "name")) ?? Path.GetFileNameWithoutExtension(filePath);

        return true;
    }

    public bool HasCharacter(char character)
        => GetGlyphIndex(character) > 0;

    public float MeasureWidth(string text, float size = 1f)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));

        var scale = size / unitsPerEm;
        var width = 0f;
        var previous = -1;

        foreach (var character in text)
        {
            var index = GetGlyphIndex(character);
            width += GetKerning(previous, index) + GetGlyph(index).AdvanceWidth;
            previous = index;
        }

        return width * scale;
    }

    public IPolygon GetPolygon(string text)
        => GetPolygon(text, 1f);

    public IPolygon GetPolygon(string text, float size, float maxWidth = float.PositiveInfinity)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));

        if (!IsLoaded)
            throw new InvalidOperationException("The font is not loaded.");

        var scale = size / unitsPerEm;
        var lineHeight = LineHeight * size;
        var rings = new List<float[]>();

        var line = 0;
        foreach (var lineText in BreakLines(text, size, maxWidth))
        {
            WriteLine(rings, lineText, scale, -line * lineHeight);
            line++;
        }

        // Glyph contours declare filled and hollow by the way they run, and
        // some of them overlap instead of nesting, so nesting cannot classify.
        return new Polygon([ ..rings ], FillRule.Winding);
    }

    /// <summary>
    /// Put every contour of every character of a line on the ring list,
    /// advancing the pen and applying the kerning between each pair.
    /// </summary>
    void WriteLine(List<float[]> rings, string text, float scale, float baseline)
    {
        var pen = 0f;
        var previous = -1;

        foreach (var character in text)
        {
            var index = GetGlyphIndex(character);
            var glyph = GetGlyph(index);

            pen += GetKerning(previous, index);
            previous = index;

            foreach (var contour in glyph.Contours)
                rings.Add(Place(contour, pen, scale, baseline));

            pen += glyph.AdvanceWidth;
        }
    }

    /// <summary>
    /// Copy a contour to its place on the text, converting font units to the
    /// final size.
    /// </summary>
    static float[] Place(float[] contour, float pen, float scale, float baseline)
    {
        var placed = new float[contour.Length];

        for (int i = 0; i < contour.Length; i += 3)
        {
            placed[i + 0] = (contour[i + 0] + pen) * scale;
            placed[i + 1] = contour[i + 1] * scale + baseline;
            placed[i + 2] = contour[i + 2];
        }

        return placed;
    }

    /// <summary>
    /// Split a text on the explicit line breaks and, when a maximum width is
    /// given, on the spaces that would make a line too wide. A single word
    /// wider than the limit is kept whole on its own line.
    /// </summary>
    IEnumerable<string> BreakLines(string text, float size, float maxWidth)
    {
        foreach (var paragraph in text.Split('\n'))
        {
            if (float.IsInfinity(maxWidth) || MeasureWidth(paragraph, size) <= maxWidth)
            {
                yield return paragraph.TrimEnd('\r');
                continue;
            }

            var line = new StringBuilder();
            foreach (var word in paragraph.TrimEnd('\r').Split(' '))
            {
                if (line.Length == 0)
                {
                    line.Append(word);
                    continue;
                }

                var candidate = $"{line} {word}";
                if (MeasureWidth(candidate, size) <= maxWidth)
                {
                    line.Clear();
                    line.Append(candidate);
                    continue;
                }

                yield return line.ToString();
                line.Clear();
                line.Append(word);
            }

            yield return line.ToString();
        }
    }

    /// <summary>
    /// Get the glyph index of a character, or 0 when the font has no drawing
    /// for it. Glyph 0 is the notdef box, which every font carries.
    /// </summary>
    int GetGlyphIndex(char character)
    {
        if (characterCache.TryGetValue(character, out var cached))
            return cached;

        var index = characterMap?.Find(character) ?? 0;
        characterCache[character] = index;
        return index;
    }

    /// <summary>
    /// Get the space the font wants between two glyphs, in font units.
    /// </summary>
    float GetKerning(int left, int right)
    {
        if (left < 0 || kerning.Count == 0)
            return 0f;

        return kerning.TryGetValue((left << 16) | right, out var value) ? value : 0f;
    }

    /// <summary>
    /// Get the advance width of a glyph in font units. Fonts store one metric
    /// per glyph only until the last one that differs; every glyph after that
    /// repeats the last width.
    /// </summary>
    float GetAdvanceWidth(int index)
    {
        if (hmtx is null || numberOfHMetrics == 0)
            return unitsPerEm / 2f;

        var metric = int.Min(index, numberOfHMetrics - 1);
        return Fits(hmtx, 4 * metric, 2) ? U16(hmtx, 4 * metric) : unitsPerEm / 2f;
    }

    void ReadTableDirectory(byte[] data)
    {
        var count = U16(data, 4);

        for (int i = 0; i < count; i++)
        {
            var record = 12 + 16 * i;
            if (!Fits(data, record, 16))
                break;

            var tag = Encoding.ASCII.GetString(data, record, 4);
            tables[tag] = new FontTable(
                tag, U32(data, record + 4),
                (int)U32(data, record + 8), (int)U32(data, record + 12)
            );
        }
    }

    void ReadHead(byte[] data, string filePath)
    {
        var head = ReadTableOrNull(data, "head")
            ?? throw new InvalidFontException(filePath, "the head table is missing.");

        if (!Fits(head, 50, 2))
            throw new InvalidFontException(filePath, "the head table is truncated.");

        unitsPerEm = U16(head, 18);
        if (unitsPerEm == 0)
            unitsPerEm = 1000;
    }

    void ReadHorizontalHeader(byte[] data)
    {
        var hhea = ReadTableOrNull(data, "hhea");
        if (hhea is null || !Fits(hhea, 34, 2))
            return;

        ascender = S16(hhea, 4);
        descender = S16(hhea, 6);
        lineGap = S16(hhea, 8);
        numberOfHMetrics = U16(hhea, 34);
    }

    void ReadGlyphOffsets(byte[] data, string filePath)
    {
        var head = ReadTableOrNull(data, "head")!;
        var maxp = ReadTableOrNull(data, "maxp")
            ?? throw new InvalidFontException(filePath, "the maxp table is missing.");

        var numGlyphs = U16(maxp, 4);
        var longFormat = S16(head, 50) == 1;
        var loca = ReadTableOrNull(data, "loca")!;

        var offsets = new int[numGlyphs + 1];
        for (int i = 0; i <= numGlyphs; i++)
        {
            // The short format stores the offset divided by two, so it must be
            // doubled back. Reading it as signed would break past 32767.
            if (longFormat)
                offsets[i] = Fits(loca, 4 * i, 4) ? (int)U32(loca, 4 * i) : 0;
            else
                offsets[i] = Fits(loca, 2 * i, 2) ? 2 * U16(loca, 2 * i) : 0;
        }

        glyphOffsets = offsets;
    }

    /// <summary>
    /// Read the horizontal kerning pairs. Only the classic kern table with
    /// format 0 is handled; fonts that keep kerning on GPOS get none.
    /// </summary>
    void ReadKerning(byte[]? kern)
    {
        if (kern is null || !Fits(kern, 4, 0))
            return;

        var subtables = U16(kern, 2);
        var offset = 4;

        for (int i = 0; i < subtables; i++)
        {
            if (!Fits(kern, offset, 6))
                return;

            var length = U16(kern, offset + 2);
            var coverage = U16(kern, offset + 4);
            var horizontal = (coverage & 0x0001) != 0;
            var format = coverage >> 8;

            if (horizontal && format == 0)
                ReadKerningPairs(kern, offset + 6);

            if (length <= 0)
                return;

            offset += length;
        }
    }

    void ReadKerningPairs(byte[] kern, int offset)
    {
        if (!Fits(kern, offset, 8))
            return;

        var pairs = U16(kern, offset);
        var start = offset + 8;

        for (int i = 0; i < pairs; i++)
        {
            var record = start + 6 * i;
            if (!Fits(kern, record, 6))
                return;

            var left = U16(kern, record);
            var right = U16(kern, record + 2);
            kerning[(left << 16) | right] = S16(kern, record + 4);
        }
    }

    /// <summary>
    /// Read the full font name from the name table, when it is there.
    /// </summary>
    static string? ReadName(byte[]? name)
    {
        if (name is null || !Fits(name, 6, 0))
            return null;

        var count = U16(name, 2);
        var storage = U16(name, 4);

        for (int i = 0; i < count; i++)
        {
            var record = 6 + 12 * i;
            if (!Fits(name, record, 12))
                return null;

            if (U16(name, record + 6) != 4)
                continue;

            var length = U16(name, record + 8);
            var offset = storage + U16(name, record + 10);
            if (!Fits(name, offset, length))
                continue;

            var platform = U16(name, record);
            var text = platform == 1
                ? Encoding.ASCII.GetString(name, offset, length)
                : Encoding.BigEndianUnicode.GetString(name, offset, length);

            if (!string.IsNullOrWhiteSpace(text))
                return text;
        }

        return null;
    }

    byte[]? ReadTableOrNull(byte[] data, string tag)
        => tables.TryGetValue(tag, out var table) ? ReadTable(data, table) : null;

    static byte[] ReadTable(byte[] data, FontTable table)
    {
        if (!Fits(data, table.Offset, table.Length))
            return [];

        var result = new byte[table.Length];
        Array.Copy(data, table.Offset, result, 0, table.Length);
        return result;
    }

    public override string ToString()
        => $$"""
        TrueTypeFont {
            Name: {{Name}},
            UnitsPerEm: {{unitsPerEm}},
            Glyphs: {{(glyphOffsets?.Length ?? 1) - 1}},
            KerningPairs: {{kerning.Count}},
            LineHeight: {{LineHeight}}
        }
        """;
}
