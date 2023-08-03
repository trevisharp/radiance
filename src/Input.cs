/* Author:  Leonardo Trevisan Silio
 * Date:    02/08/2023
 */
namespace DuckGL;

/// <summary>
/// Represents a input from hardware.
/// Equals to OpenTK.Windowing.GraphicsLibraryFramework.
/// </summary>
public enum Input
{
    //
    // Resumo:
    //     An unknown key.
    Unknown = -1,
    //
    // Resumo:
    //     The spacebar key.
    Space = 32,
    //
    // Resumo:
    //     The apostrophe key.
    Apostrophe = 39,
    //
    // Resumo:
    //     The comma key.
    Comma = 44,
    //
    // Resumo:
    //     The minus key.
    Minus = 45,
    //
    // Resumo:
    //     The period key.
    Period = 46,
    //
    // Resumo:
    //     The slash key.
    Slash = 47,
    //
    // Resumo:
    //     The 0 key.
    D0 = 48,
    //
    // Resumo:
    //     The 1 key.
    D1 = 49,
    //
    // Resumo:
    //     The 2 key.
    D2 = 50,
    //
    // Resumo:
    //     The 3 key.
    D3 = 51,
    //
    // Resumo:
    //     The 4 key.
    D4 = 52,
    //
    // Resumo:
    //     The 5 key.
    D5 = 53,
    //
    // Resumo:
    //     The 6 key.
    D6 = 54,
    //
    // Resumo:
    //     The 7 key.
    D7 = 55,
    //
    // Resumo:
    //     The 8 key.
    D8 = 56,
    //
    // Resumo:
    //     The 9 key.
    D9 = 57,
    //
    // Resumo:
    //     The semicolon key.
    Semicolon = 59,
    //
    // Resumo:
    //     The equal key.
    Equal = 61,
    //
    // Resumo:
    //     The A key.
    A = 65,
    //
    // Resumo:
    //     The B key.
    B = 66,
    //
    // Resumo:
    //     The C key.
    C = 67,
    //
    // Resumo:
    //     The D key.
    D = 68,
    //
    // Resumo:
    //     The E key.
    E = 69,
    //
    // Resumo:
    //     The F key.
    F = 70,
    //
    // Resumo:
    //     The G key.
    G = 71,
    //
    // Resumo:
    //     The H key.
    H = 72,
    //
    // Resumo:
    //     The I key.
    I = 73,
    //
    // Resumo:
    //     The J key.
    J = 74,
    //
    // Resumo:
    //     The K key.
    K = 75,
    //
    // Resumo:
    //     The L key.
    L = 76,
    //
    // Resumo:
    //     The M key.
    M = 77,
    //
    // Resumo:
    //     The N key.
    N = 78,
    //
    // Resumo:
    //     The O key.
    O = 79,
    //
    // Resumo:
    //     The P key.
    P = 80,
    //
    // Resumo:
    //     The Q key.
    Q = 81,
    //
    // Resumo:
    //     The R key.
    R = 82,
    //
    // Resumo:
    //     The S key.
    S = 83,
    //
    // Resumo:
    //     The T key.
    T = 84,
    //
    // Resumo:
    //     The U key.
    U = 85,
    //
    // Resumo:
    //     The V key.
    V = 86,
    //
    // Resumo:
    //     The W key.
    W = 87,
    //
    // Resumo:
    //     The X key.
    X = 88,
    //
    // Resumo:
    //     The Y key.
    Y = 89,
    //
    // Resumo:
    //     The Z key.
    Z = 90,
    //
    // Resumo:
    //     The left bracket(opening bracket) key.
    LeftBracket = 91,
    //
    // Resumo:
    //     The backslash.
    Backslash = 92,
    //
    // Resumo:
    //     The right bracket(closing bracket) key.
    RightBracket = 93,
    //
    // Resumo:
    //     The grave accent key.
    GraveAccent = 96,
    //
    // Resumo:
    //     The escape key.
    Escape = 256,
    //
    // Resumo:
    //     The enter key.
    Enter = 257,
    //
    // Resumo:
    //     The tab key.
    Tab = 258,
    //
    // Resumo:
    //     The backspace key.
    Backspace = 259,
    //
    // Resumo:
    //     The insert key.
    Insert = 260,
    //
    // Resumo:
    //     The delete key.
    Delete = 261,
    //
    // Resumo:
    //     The right arrow key.
    Right = 262,
    //
    // Resumo:
    //     The left arrow key.
    Left = 263,
    //
    // Resumo:
    //     The down arrow key.
    Down = 264,
    //
    // Resumo:
    //     The up arrow key.
    Up = 265,
    //
    // Resumo:
    //     The page up key.
    PageUp = 266,
    //
    // Resumo:
    //     The page down key.
    PageDown = 267,
    //
    // Resumo:
    //     The home key.
    Home = 268,
    //
    // Resumo:
    //     The end key.
    End = 269,
    //
    // Resumo:
    //     The caps lock key.
    CapsLock = 280,
    //
    // Resumo:
    //     The scroll lock key.
    ScrollLock = 281,
    //
    // Resumo:
    //     The num lock key.
    NumLock = 282,
    //
    // Resumo:
    //     The print screen key.
    PrintScreen = 283,
    //
    // Resumo:
    //     The pause key.
    Pause = 284,
    //
    // Resumo:
    //     The F1 key.
    F1 = 290,
    //
    // Resumo:
    //     The F2 key.
    F2 = 291,
    //
    // Resumo:
    //     The F3 key.
    F3 = 292,
    //
    // Resumo:
    //     The F4 key.
    F4 = 293,
    //
    // Resumo:
    //     The F5 key.
    F5 = 294,
    //
    // Resumo:
    //     The F6 key.
    F6 = 295,
    //
    // Resumo:
    //     The F7 key.
    F7 = 296,
    //
    // Resumo:
    //     The F8 key.
    F8 = 297,
    //
    // Resumo:
    //     The F9 key.
    F9 = 298,
    //
    // Resumo:
    //     The F10 key.
    F10 = 299,
    //
    // Resumo:
    //     The F11 key.
    F11 = 300,
    //
    // Resumo:
    //     The F12 key.
    F12 = 301,
    //
    // Resumo:
    //     The F13 key.
    F13 = 302,
    //
    // Resumo:
    //     The F14 key.
    F14 = 303,
    //
    // Resumo:
    //     The F15 key.
    F15 = 304,
    //
    // Resumo:
    //     The F16 key.
    F16 = 305,
    //
    // Resumo:
    //     The F17 key.
    F17 = 306,
    //
    // Resumo:
    //     The F18 key.
    F18 = 307,
    //
    // Resumo:
    //     The F19 key.
    F19 = 308,
    //
    // Resumo:
    //     The F20 key.
    F20 = 309,
    //
    // Resumo:
    //     The F21 key.
    F21 = 310,
    //
    // Resumo:
    //     The F22 key.
    F22 = 311,
    //
    // Resumo:
    //     The F23 key.
    F23 = 312,
    //
    // Resumo:
    //     The F24 key.
    F24 = 313,
    //
    // Resumo:
    //     The F25 key.
    F25 = 314,
    //
    // Resumo:
    //     The 0 key on the key pad.
    KeyPad0 = 320,
    //
    // Resumo:
    //     The 1 key on the key pad.
    KeyPad1 = 321,
    //
    // Resumo:
    //     The 2 key on the key pad.
    KeyPad2 = 322,
    //
    // Resumo:
    //     The 3 key on the key pad.
    KeyPad3 = 323,
    //
    // Resumo:
    //     The 4 key on the key pad.
    KeyPad4 = 324,
    //
    // Resumo:
    //     The 5 key on the key pad.
    KeyPad5 = 325,
    //
    // Resumo:
    //     The 6 key on the key pad.
    KeyPad6 = 326,
    //
    // Resumo:
    //     The 7 key on the key pad.
    KeyPad7 = 327,
    //
    // Resumo:
    //     The 8 key on the key pad.
    KeyPad8 = 328,
    //
    // Resumo:
    //     The 9 key on the key pad.
    KeyPad9 = 329,
    //
    // Resumo:
    //     The decimal key on the key pad.
    KeyPadDecimal = 330,
    //
    // Resumo:
    //     The divide key on the key pad.
    KeyPadDivide = 331,
    //
    // Resumo:
    //     The multiply key on the key pad.
    KeyPadMultiply = 332,
    //
    // Resumo:
    //     The subtract key on the key pad.
    KeyPadSubtract = 333,
    //
    // Resumo:
    //     The add key on the key pad.
    KeyPadAdd = 334,
    //
    // Resumo:
    //     The enter key on the key pad.
    KeyPadEnter = 335,
    //
    // Resumo:
    //     The equal key on the key pad.
    KeyPadEqual = 336,
    //
    // Resumo:
    //     The left shift key.
    LeftShift = 340,
    //
    // Resumo:
    //     The left control key.
    LeftControl = 341,
    //
    // Resumo:
    //     The left alt key.
    LeftAlt = 342,
    //
    // Resumo:
    //     The left super key.
    LeftSuper = 343,
    //
    // Resumo:
    //     The right shift key.
    RightShift = 344,
    //
    // Resumo:
    //     The right control key.
    RightControl = 345,
    //
    // Resumo:
    //     The right alt key.
    RightAlt = 346,
    //
    // Resumo:
    //     The right super key.
    RightSuper = 347,
    //
    // Resumo:
    //     The menu key.
    Menu = 348,
    //
    // Resumo:
    //     The last valid key in this enum.
    LastKey = 348
}