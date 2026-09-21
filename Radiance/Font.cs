/* Author:  Leonardo Trevisan Silio
 * Date:    21/09/2026
 */
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance;

using Fonts;
using Exceptions;

/// <summary>
/// A utilities type for working with fonts.
/// </summary>
public static class Font
{
    const string builtInResource = "Radiance.Roboto-Regular.ttf";

    /// <summary>
    /// List of valid IFont types.
    /// </summary>
    public readonly static List<Type> Types = [ typeof(TrueTypeFont) ];

    static IFont? defaultFont = null;

    /// <summary>
    /// Get or set the font used when none is given, by Polygon.FromText and by
    /// the conversion from string to Polygon. Starts as the built in Roboto.
    /// </summary>
    public static IFont Default
    {
        get => defaultFont ??= OpenBuiltIn();
        set => defaultFont = value ?? throw new CallingNullArgumentException(null, 0);
    }

    /// <summary>
    /// Open a font.
    /// </summary>
    public static IFont Open(string file)
    {
        if (!File.Exists(file))
            throw new InvalidFontFileException(file);

        foreach (var type in Types)
        {
            if (Activator.CreateInstance(type) is not IFont font)
                continue;

            if (font.LoadFromFile(file))
                return font;
        }

        throw new InvalidFontFileException(file);
    }

    /// <summary>
    /// Open a font from bytes already in memory.
    /// </summary>
    public static IFont Open(byte[] data, string name)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        foreach (var type in Types)
        {
            if (Activator.CreateInstance(type) is not IFont font)
                continue;

            if (font.LoadFromMemory(data, name))
                return font;
        }

        throw new InvalidFontFileException(name);
    }

    /// <summary>
    /// Open the font that travels inside Radiance, a subset of Roboto covering
    /// latin text. See THIRD-PARTY-NOTICES.md for its license.
    /// </summary>
    public static IFont OpenBuiltIn()
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(builtInResource)
            ?? throw new InvalidFontFileException(builtInResource);

        using var memory = new MemoryStream();
        stream.CopyTo(memory);

        return Open(memory.ToArray(), "Roboto");
    }
}
