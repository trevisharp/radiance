/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2025
 */
using System.IO;
using System.Collections.Generic;

namespace Radiance.Fonts;

using Exceptions;
using Bufferings;
using System;

/// <summary>
/// A reader for True Type Font (.fft) files.
/// </summary>
public class TrueTypeFont : IFont
{
    public int Version { get; private set; }
    public int NumberOfTables { get; private set; }
    public int SearchRange { get; private set; }
    public int EntrySelector { get; private set; }
    public int RangeShift { get; private set; }
    public Dictionary<string, TTFTable> Tables { get; private set; } = [];

    bool isLoaded = false;
    public bool IsLoaded => isLoaded; 

    public IPolygon GetPolygon(string text)
    {
        throw new System.NotImplementedException();
    }

    public bool LoadFromFile(string filePath)
    {
        if (Path.GetExtension(filePath) != ".ttf")
            throw new InvalidFontException(
                filePath, "missing file."
            );

        using var stream = File.OpenRead(filePath);

        Version = ReadBytes(stream, 4);
        if (Version != 65536)
            throw new InvalidFontException(
                filePath, "The file is not a .ttf file."
            );

        NumberOfTables = ReadBytes(stream, 2);
        SearchRange = ReadBytes(stream, 2);
        EntrySelector = ReadBytes(stream, 2);
        RangeShift = ReadBytes(stream, 2);

        if (SearchRange != 16 * Math.Pow(2, EntrySelector))
            throw new InvalidFontException(filePath, 
                $"Invalid SearchRange({SearchRange}) should be 16 * 2^EntrySelector({EntrySelector}) header."
            );

        if (RangeShift != NumberOfTables * 16 - SearchRange)
            throw new InvalidFontException(
                filePath, $"Invalid RangeShift({RangeShift}) should be NumberOfTables({NumberOfTables}) * 16 - SearchRange({SearchRange}) header."
            );

        for (int i = 0; i < NumberOfTables; i++)
        {
            var table = new TTFTable(
                ReadString(stream, 4), ReadBytes(stream, 4),
                ReadBytes(stream, 4), ReadBytes(stream, 4)
            );
            Tables[table.Tag] = table;
        }

        isLoaded = true;
        return true;
    }

    static int ReadBytes(FileStream stream, int bytes)
    {
        var value = 0;
        var shift = 8 * (bytes - 1);
        for (int i = 0; i < bytes; i++, shift -= 8)
            value |= stream.ReadByte() << shift;
        return value;
    }

    static string ReadString(FileStream stream, int chars)
    {
        var characters = new char[chars];
        for (int i = 0; i < chars; i++)
            characters[i] = (char)stream.ReadByte();
        return new string(characters);
    }

    public record TTFTable(
        string Tag, int Checksum,
        int Offset, int Length
    );

    public override string ToString() => 
        $$"""
        TrueTypeFontFile {
            Version = {{Version}},
            NumberOfTables = {{NumberOfTables}},
            SearchRange = {{SearchRange}},
            EntrySelector = {{EntrySelector}},
            RangeShift = {{RangeShift}}
        }
        """;
}