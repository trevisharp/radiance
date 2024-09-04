/* Author:  Leonardo Trevisan Silio
 * Date:    30/08/2024
 */
using System;

namespace Radiance;

using Primitives;
using Windows;
using Windows.OpenGL;

/// <summary>
/// Global singleton reference to window in use. 
/// </summary>
public static class Window
{
    private static WindowFactory factory = new OpenGLWindowFactory();
    public static WindowFactory Factory
    {
        get => factory;
        set => factory = value ?? 
            throw new ArgumentNullException(nameof(Factory));
    }

    public static BaseWindow New(bool fullscreen = true)
        => Factory.New(fullscreen);
    
    public static void Reset(bool fullscreen)
        => current = New(fullscreen);

    private static BaseWindow current = New();
    public static BaseWindow Current => current;

    /// <summary>
    /// Return true if screen is Open.
    /// </summary>
    public static bool IsOpen => Current.IsOpen;

    /// <summary>
    /// The width of the screen.
    /// </summary>
    public static int Width => Current.Width;

    /// <summary>
    /// The height of the screen.
    /// </summary>
    public static int Height => Current.Height;

    /// <summary>
    /// Get and set if the cursor is visible.
    /// </summary>
    public static bool CursorVisible
    {
        get => Current.CursorVisible;
        set => Current.CursorVisible = value;
    }

    /// <summary>
    /// Open main application window.
    /// </summary>
    public static void Open(bool fullscreen = true)
        => Current.Open(fullscreen);

    /// <summary>
    /// Run a function only if the window is open, else
    /// schedule execution.
    /// </summary>
    public static void RunOrSchedule(Action func)
        => Current.RunOrSchedule(func);

    /// <summary>
    /// Close main application window.
    /// </summary>
    public static void Close()
        => Current.Close();

    /// <summary>
    /// Clear the background without use any render.
    /// </summary>
    public static void Clear(Vec4 color)
        => Current.Clear(color);

    /// <summary>
    /// Set inputs to close the application.
    /// </summary>
    public static void CloseOn(Input input)
        => Current.CloseOn(input);
    
    /// <summary>
    /// The time between the current and the last frame.
    /// </summary>
    public static float DeltaTime => Current.DeltaTime;

    /// <summary>
    /// Current Frames Per Second for this application.
    /// </summary>
    public static float Fps => Current.Fps;
    
    public static event Action OnRender
    {
        add => Current.OnRender += value;
        remove => Current.OnRender -= value;
    }

    public static event Action OnLoad
    {
        add => Current.OnLoad += value;
        remove => Current.OnLoad -= value;
    }

    public static event Action OnUnload
    {
        add => Current.OnUnload += value;
        remove => Current.OnUnload -= value;
    }

    public static event Action OnFrame
    {
        add => Current.OnFrame += value;
        remove => Current.OnFrame -= value;
    }

    public static event Action<Input, Modifier> OnKeyDown
    {
        add => Current.OnKeyDown += value;
        remove => Current.OnKeyDown -= value;
    }

    public static event Action<Input, Modifier> OnKeyUp
    {
        add => Current.OnKeyUp += value;
        remove => Current.OnKeyUp -= value;
    }
    

    public static event Action<(float x, float y)> OnMouseMove
    {
        add => Current.OnMouseMove += value;
        remove => Current.OnMouseMove -= value;
    }
    
    public static event Action<MouseButton> OnMouseDown
    {
        add => Current.OnMouseDown += value;
        remove => Current.OnMouseDown -= value;
    }
    
    public static event Action<MouseButton> OnMouseUp
    {
        add => Current.OnMouseUp += value;
        remove => Current.OnMouseUp -= value;
    }
    
    public static event Action<float> OnMouseWhell
    {
        add => Current.OnMouseWhell += value;
        remove => Current.OnMouseWhell -= value;
    }
    
    public static event Action OnMouseEnter
    {
        add => Current.OnMouseEnter += value;
        remove => Current.OnMouseEnter -= value;
    }
    
    public static event Action OnMouseLeave
    {
        add => Current.OnMouseLeave += value;
        remove => Current.OnMouseLeave -= value;
    }
}