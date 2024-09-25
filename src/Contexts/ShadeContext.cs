/* Author:  Leonardo Trevisan Silio
 * Date:    25/09/2024
 */
using System;
using static System.Console;

namespace Radiance.Contexts;

using Shaders;
using Primitives;

/// <summary>
/// Represents the data and state of a shader program.
/// </summary>
public abstract class ShadeContext : IDisposable
{
    /// <summary>
    /// Set a uniform with a name to a specific value.
    /// </summary>
    public abstract void SetFloat(string name, float value);

    /// <summary>
    /// Set a image uniform with a name to a specific value.
    /// </summary>
    public abstract void SetTextureData(string name, Texture texture);

    /// <summary>
    /// Add float values on layout of data buffers.
    /// </summary>
    public abstract void AddLayout(int size);

    /// <summary>
    /// Start to use a Polygon.
    /// </summary>
    public abstract void Use(Polygon poly);

    /// <summary>
    /// Draw Arrays in the selected buffer.
    /// </summary>
    public abstract void Draw(PrimitiveType primitiveType, Polygon poly);
    
    /// <summary>
    /// Create and associeate the context to a program.
    /// </summary>
    public abstract void CreateProgram(ShaderPair pair, bool verbose = false);

    /// <summary>
    /// Use the associated specific Program.
    /// </summary>
    public abstract void UseProgram();

    /// <summary>
    /// Releasing all resources associateds to this shader.
    /// </summary>
    public abstract void Dispose();

    /// <summary>
    /// Show a Error message if verbose is true.
    /// </summary>
    protected static void Error(string message, bool verbose, ref int tabIndex)
        => Verbose(message, ConsoleColor.White, ConsoleColor.Red, tabIndex, verbose);
    
    /// <summary>
    /// Show a Information message if verbose is true.
    /// </summary>
    protected static void Information(string message, bool verbose, ref int tabIndex)
        => Verbose(message, ConsoleColor.Green, ConsoleColor.Black, tabIndex, verbose);
    
    /// <summary>
    /// Show a Success message if verbose is true. Close the block reducing the tabIndex.
    /// </summary>
    protected static void Success(string message, bool verbose, ref int tabIndex)
        => Verbose(message + "\n", ConsoleColor.Blue, ConsoleColor.Black, --tabIndex, verbose);
    
    /// <summary>
    /// Show a Code if verbose is true.
    /// </summary>
    protected static void Code(string message, bool verbose, ref int tabIndex)
        => Verbose(message, ConsoleColor.DarkYellow, ConsoleColor.Black, tabIndex + 1, verbose);

    /// <summary>
    /// Show a Start message if verbose is true. Open a block increasing the tabIndex.
    /// </summary>
    protected static void Start(string message, bool verbose, ref int tabIndex)
        => Verbose("Start: " + message, ConsoleColor.Magenta, ConsoleColor.Black, tabIndex++, verbose);

    static void Verbose(
        string text, 
        ConsoleColor fore = ConsoleColor.White,
        ConsoleColor back = ConsoleColor.Black,
        int tabIndex = 0,
        bool verbose = false,
        bool newline = true
    )
    {
        if (!verbose)
            return;
        
        var fullTab = "";
        for (int i = 0; i < tabIndex; i++)
            fullTab += "\t";

        text = fullTab + text.Replace("\n", "\n" + fullTab);
        
        ForegroundColor = fore;
        BackgroundColor = back;
        Write(text);
        
        if (newline)
            WriteLine();
    }
}