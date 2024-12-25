/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2024
 */
using System;
using static System.Console;

namespace Radiance.Contexts;

using BufferData;
using Primitives;
using CodeGeneration;

public abstract class ShaderContext : IShaderInvoker, IShaderConfiguration
{
    public abstract void SetFloat(string name, float value);
    public abstract void SetVec(string name, float x, float y);
    public abstract void SetVec(string name, float x, float y, float z);
    public abstract void SetVec(string name, float x, float y, float z, float w);
    public abstract void SetTextureData(string name, Texture texture);
    public abstract void Dispose();
    public abstract void CreateProgram(ShaderPair pair, bool verbose = false);
    public abstract void Draw(PrimitiveType primitiveType, IBufferedData data);
    public abstract void InitArgs(object[] args);
    public abstract void UseArgs(object[] args);
    public abstract void UseProgram();
    public virtual void FirstConfiguration() {}
    public abstract void SetLineWidth(float width);
    public abstract void SetPointSize(float size);

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