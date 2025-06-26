/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2025
 */
using System.IO;

namespace Radiance.Fonts;

/// <summary>
/// A reader for True Type Font (.fft) files.
/// </summary>
public class TrueTypeFontFile
{
    public required int Version { get; init; }
    public required int NumberOfTables { get; init; }
    public required int SearchRange { get; init; }
    public required int EntrySelector { get; init; }
    public required int RangeShift { get; init; }
    public required TTFTable[] Tables { get; init; }

    public static TrueTypeFontFile Open(string file)
    {
        using var stream = File.OpenRead(file);

        var version = ReadBytes(stream, 4);
        var numberOfTables = ReadBytes(stream, 2);
        var searchRange = ReadBytes(stream, 2);
        var entrySelector = ReadBytes(stream, 2);
        var rangeShift = ReadBytes(stream, 2);

        var tables = new TTFTable[numberOfTables];

        for (int i = 0; i < numberOfTables; i++)
        {
            tables[i] = new TTFTable(
                ReadString(stream, 4), ReadBytes(stream, 4),
                ReadBytes(stream, 4), ReadBytes(stream, 4)
            );
        }

        return new TrueTypeFontFile {
            Version = version,
            NumberOfTables = numberOfTables,
            SearchRange = searchRange,
            EntrySelector = entrySelector,
            RangeShift = rangeShift,
            Tables = tables 
        };
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