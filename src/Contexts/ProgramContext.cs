/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2024
 */
using System;
using static System.Console;

namespace Radiance.Contexts;

using Primitives;

/// <summary>
/// The manager for shaders and programs.
/// </summary>
public abstract class ProgramContext
{
    public abstract void FreeAllResources();
    public abstract int CreateProgram(
        string vertexSource,
        string fragmentSource,
        bool verbose = false
    );
    public abstract void Clear(Vec4 color);
    public abstract void UseProgram(int program);

    protected static void Error(string message, bool verbose, ref int tabIndex)
        => Verbose(message, ConsoleColor.White, ConsoleColor.Red, tabIndex, verbose);
    
    protected static void Information(string message, bool verbose, ref int tabIndex)
        => Verbose(message, ConsoleColor.Green, ConsoleColor.Black, tabIndex, verbose);
    
    protected static void Success(string message, bool verbose, ref int tabIndex)
        => Verbose(message + "\n", ConsoleColor.Blue, ConsoleColor.Black, --tabIndex, verbose);
    
    protected static void Code(string message, bool verbose, ref int tabIndex)
        => Verbose(message, ConsoleColor.DarkYellow, ConsoleColor.Black, tabIndex + 1, verbose);

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