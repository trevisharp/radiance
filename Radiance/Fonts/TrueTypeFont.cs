/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2025
 */
using System.IO;
using System.Collections.Generic;

namespace Radiance.Fonts;

using Exceptions;
using Bufferings;
using System;
using System.Buffers.Binary;
using System.Linq;

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
    public bool IsLoaded => 
        GlyphOffsets is not null &&
        Glyphes is not null &&
        CharMap is not null;

    CMAPInfo? cmapInfo = null;
    int[]? GlyphOffsets = null;
    byte[]? Glyphes = null;
    Dictionary<char, EncodingRecord> CharMap = [];

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
        
        var tables = ReadTables(stream);

        var maxpTable = tables["maxp"];
        var maxp = ReadTable(stream, maxpTable);
        var numGlyphs = (maxp[4] << 8) | maxp[5];

        var headTable = tables["head"];
        var head = ReadTable(stream, headTable);
        var indexToLocFormatIsLong = ((head[50] << 8) | head[51]) == 1;

        var locaTable = tables["loca"];
        var loca = ReadTable(stream, locaTable);
        GlyphOffsets = indexToLocFormatIsLong ?
            ExtractAll32(loca, numGlyphs + 1) :
            ExtractAll16(loca, numGlyphs + 1);
        
        var glyfTable = tables["glyf"];
        Glyphes = ReadTable(stream, glyfTable);

        var cmapTable = tables["cmap"];
        var cmap = ReadTable(stream, cmapTable);
        LoadCmapData(cmap);
        
        return true;
    }

    void GetGlyph(char character)
    {
        if (GlyphOffsets is null || Glyphes is null)
            throw new InvalidOperationException("The font is not loaded.");

        var index = CharMap[character].Offset;
        var offset = GlyphOffsets[index];
        var length = GlyphOffsets[index + 1] - offset;
        if (length <= 0)
            return;
        
        var numberOfContours = Extract16(Glyphes, offset);
        var xMin = Extract16(Glyphes, offset + 2);
        var yMin = Extract16(Glyphes, offset + 4);
        var xMax = Extract16(Glyphes, offset + 6);
        var yMax = Extract16(Glyphes, offset + 8);
    }

    void LoadCmapData(byte[] cmap)
    {
        CharMap.Clear();

        var cmapOffset = FindOffSet(cmap, 3, 1, 4);
        var subformat = Extract16(cmap, cmapOffset);
        var length = Extract16(cmap, cmapOffset + 2);
        var segCount = Extract16(cmap, cmapOffset + 6) / 2;

        var endCountPos = cmapOffset + 14;
        var startCountPos = endCountPos + 2 * segCount + 2;
        var idDeltaPos = startCountPos + 2 * segCount;
        var idRangeOffsetPos = idDeltaPos + 2 * segCount;

        var endCode = new int[segCount];
        var startCode = new int[segCount];
        var idDelta = new int[segCount];
        var idRangeOffset = new int[segCount];

        for (int i = 0; i < segCount; i++)
        {
            endCode[i] = Extract16(cmap, endCountPos + i * 2);
            startCode[i] = Extract16(cmap, startCountPos + i * 2);
            idDelta[i] = Extract16(cmap, idDeltaPos + i * 2);
            idRangeOffset[i] = Extract16(cmap, idRangeOffsetPos + i * 2);
        }

        var info = new CMAPInfo(
            cmap,
            endCode, startCode, 
            idDelta, idRangeOffset,
            segCount, idRangeOffsetPos
        );
        
        cmapInfo = info;
    }

    Dictionary<string, TTFTable> ReadTables(FileStream stream)
    {
        Dictionary<string, TTFTable> tables = [];
        for (int i = 0; i < NumberOfTables; i++)
        {
            var table = new TTFTable(
                ReadString(stream, 4), ReadBytes(stream, 4),
                ReadBytes(stream, 4), ReadBytes(stream, 4)
            );
            tables[table.Tag] = table;
        }
        return tables;
    }

    int GetGlyphIndex(char character)
    {
        if (cmapInfo is null)
            throw new InvalidOperationException("The font is not loaded.");

        for (int i = 0; i < cmapInfo.SegmentCount; i++)
        {
            if (character < cmapInfo.StartCode[i] && cmapInfo.EndCode[i] < character)
                continue;
                
            if (cmapInfo.IdRangeOffset[i] == 0)
                return (character + cmapInfo.IdDelta[i]) & 0xFFFF;
            
            int offset = cmapInfo.IdRangeOffsetPosition + i * 2 + cmapInfo.IdRangeOffset[i];
            int glyphOffset = offset + 2 * (character - cmapInfo.StartCode[i]);
            if (glyphOffset >= cmapInfo.CmapBuffer.Length)
                return 0;
                
            var glyphIndex = Extract16(cmapInfo.CmapBuffer, glyphOffset);
            if (glyphIndex != 0)
                return (glyphIndex + cmapInfo.IdDelta[i]) & 0xFFFF;
            
            return 0;
        }

        return 0;
    }

    static int FindOffSet(byte[] cmap, int platformID, int encodingID, int format)
    {
        var cmapVer = Extract16(cmap, 0);
        var cmapTables = Extract16(cmap, 2);

        for (int i = 0; i < cmapTables; i++)
        {
            var recordOffset = 4 + i * 8;
            var tablePlatformID = Extract16(cmap, recordOffset);
            var tableEncodingID = Extract16(cmap, recordOffset + 2);
            var offset = Extract32(cmap, recordOffset + 4);
            var tableFormat = Extract16(cmap, offset);

            if (tableEncodingID != platformID)
                continue;
            
            if (tableEncodingID != encodingID)
                continue;
            
            if (tableFormat != format)
                continue;

            return offset;
        }

        throw new Exception($"Invalid plataform/encoding({platformID}/{encodingID})");
    }

    static int Extract16(byte[] bytes, int offset)
        => BinaryPrimitives.ReadInt16BigEndian(bytes.AsSpan(offset));
    
    static int Extract32(byte[] bytes, int offset)
        => BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(offset));
    
    static int[] ExtractAll16(byte[] bytes, int size)
    {
        var result = new int[size];
        for (int i = 0; i < result.Length; i++)
            result[i] = Extract16(bytes, 2 * i);
        return result;
    }

    static int[] ExtractAll32(byte[] bytes, int size)
    {
        var result = new int[size];
        for (int i = 0; i < result.Length; i++)
            result[i] = Extract32(bytes, 4 * i);
        return result;
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

    static byte[] ReadTable(FileStream stream, TTFTable table)
    {
        stream.Seek(table.Offset, SeekOrigin.Begin);
        var data = new byte[table.Length];
        stream.ReadExactly(data);
        return data;
    }

    public record TTFTable(
        string Tag, int Checksum,
        int Offset, int Length
    );

    public record EncodingRecord(
        int PlatformID,
        int EncodingID,
        int Offset
    );

    record CMAPInfo(
        byte[] CmapBuffer,
        int[] EndCode,
        int[] StartCode,
        int[] IdDelta,
        int[] IdRangeOffset,
        int SegmentCount,
        int IdRangeOffsetPosition
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