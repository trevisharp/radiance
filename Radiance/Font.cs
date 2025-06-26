/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2025
 */
using System;
using System.IO;
using System.Collections.Generic;

namespace Radiance;

using Fonts;
using Exceptions;

/// <summary>
/// A utilities type for working with fonts.
/// </summary>
public static class Font
{
    /// <summary>
    /// List of valid IFont types.
    /// </summary>
    public readonly static List<Type> Types = [ typeof(TrueTypeFont) ];

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
}