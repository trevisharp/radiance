/* Author:  Leonardo Trevisan Silio
 * Date:    12/09/2024
 */
using System;

namespace Radiance;

using Windows;
using Primitives;
using Implementations.OpenGL4;

/// <summary>
/// Global singleton reference to main window in use. 
/// </summary>
public static class Window
{
    private static WindowBuilder factory = new OpenGL4WindowBuilder();
    public static WindowBuilder Factory
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
    /// Get the phase of render pipeline from this window.
    /// </summary>
    public static WindowPhase Phase => Current.Phase;

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
    /// Get and set if the Z-Buffer (Depth buffer) is enable.
    /// This put the distance objects behind the nearby objects.
    /// </summary>
    public static bool ZBufferEnable
    {
        get => Current.ZBufferEnable;
        set => Current.ZBufferEnable = value;
    }

    /// <summary>
    /// Get and set if the blending is enable.
    /// This allow transparency on window.
    /// </summary>
    public static bool BlendingMode
    {
        get => Current.BlendingMode;
        set => Current.BlendingMode = value;
    }

    /// <summary>
    /// Get or Set the color used on Clear operation.
    /// </summary
    public static Vec4 ClearColor
    {
        get => Current.ClearColor;
        set => Current.ClearColor = value;
    }

    /// <summary>
    /// Get and set if the line smoothing is enable.
    /// This enable the antialaising.
    /// </summary>
    public static bool LineSmooth
    {
        get => Current.LineSmooth;
        set => Current.LineSmooth = value;
    }

    /// <summary>
    /// Open main application window.
    /// </summary>
    public static void Open()
        => Current.Open();
    
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
    
    /// <summary>
    /// Occurs when the render pipeline is activated.
    /// </summary>
    public static event Action OnRender
    {
        add => Current.OnRender += value;
        remove => Current.OnRender -= value;
    }

    /// <summary>
    /// Occurs when the window is loading to open.
    /// </summary>
    public static event Action OnLoad
    {
        add => Current.OnLoad += value;
        remove => Current.OnLoad -= value;
    }

    /// <summary>
    /// Occurs when the window is loading to close.
    /// </summary>
    public static event Action OnUnload
    {
        add => Current.OnUnload += value;
        remove => Current.OnUnload -= value;
    }

    /// <summary>
    /// Occurs before the render event. This event not draw new information on screen.
    /// </summary>
    public static event Action OnFrame
    {
        add => Current.OnFrame += value;
        remove => Current.OnFrame -= value;
    }

    /// <summary>
    /// Occurs when the user press a key.
    /// </summary>
    public static event Action<Input, Modifier> OnKeyDown
    {
        add => Current.OnKeyDown += value;
        remove => Current.OnKeyDown -= value;
    }

    /// <summary>
    /// Occurs when the user release a key.
    /// </summary>
    public static event Action<Input, Modifier> OnKeyUp
    {
        add => Current.OnKeyUp += value;
        remove => Current.OnKeyUp -= value;
    }

    /// <summary>
    /// Occures when the user move the cursor.
    /// </summary>
    public static event Action<(float x, float y)> OnMouseMove
    {
        add => Current.OnMouseMove += value;
        remove => Current.OnMouseMove -= value;
    }
    
    /// <summary>
    /// Occures when the user press the cursor button.
    /// </summary>
    public static event Action<MouseButton> OnMouseDown
    {
        add => Current.OnMouseDown += value;
        remove => Current.OnMouseDown -= value;
    }
    
    /// <summary>
    /// Occures when the user release the cursor button.
    /// </summary>
    public static event Action<MouseButton> OnMouseUp
    {
        add => Current.OnMouseUp += value;
        remove => Current.OnMouseUp -= value;
    }
    
    /// <summary>
    /// Occurs when the user move the mouse wheel.
    /// </summary>
    public static event Action<float> OnMouseWhell
    {
        add => Current.OnMouseWhell += value;
        remove => Current.OnMouseWhell -= value;
    }
    
    /// <summary>
    /// Occurs when the cursor enters the screen.
    /// </summary>
    public static event Action OnMouseEnter
    {
        add => Current.OnMouseEnter += value;
        remove => Current.OnMouseEnter -= value;
    }
    
    /// <summary>
    /// Occurs when the cursor leaves the screen.
    /// </summary>
    public static event Action OnMouseLeave
    {
        add => Current.OnMouseLeave += value;
        remove => Current.OnMouseLeave -= value;
    }
}