/* Author:  Leonardo Trevisan Silio
 * Date:    26/02/2024
 */
using System;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;

namespace Radiance;

using Data;
using Internal;
using Pipelines;

/// <summary>
/// Represents the main windows that applications run
/// </summary>
public static class Window
{
    private static TimeFrameController frameController = new TimeFrameController();
    private static GameWindow win;
    private static int width = -1;
    private static int height = -1;

    /// <summary>
    /// Return true if screen is Open.
    /// </summary>
    public static bool IsOpen { get; private set; } = false;

    /// <summary>
    /// The width of the screen.
    /// </summary>
    public static int Width => width;

    /// <summary>
    /// The height of the screen.
    /// </summary>
    public static int Height => height;

    /// <summary>
    /// Get and set if the cursor is visible.
    /// </summary>
    public static bool CursorVisible
    {
        get => win?.CursorState != CursorState.Hidden;
        set
        {
            if (win is null)
                return;
            
            win.CursorState = value ? CursorState.Normal : CursorState.Hidden;
        }
    }

    /// <summary>
    /// Open main application window.
    /// </summary>
    public static void Open(bool fullscreen = true)
    {
        win = new GameWindow(
            GameWindowSettings.Default,
            new NativeWindowSettings()
            {
                ClientSize = (800, 600),
                WindowState = 
                    fullscreen ?
                    WindowState.Fullscreen :
                    WindowState.Normal
            }
        );
        win.CursorState = CursorState.Normal;

        win.Resize += e =>
        {
            updateSize(win);
        };

        win.Load += () =>
        {
            IsOpen = true;
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.LineSmooth);
            GL.BlendFunc(
                BlendingFactor.SrcAlpha, 
                BlendingFactor.OneMinusSrcAlpha
            );

            updateSize(win);
            
            if (OnLoad is null)
                return;
            OnLoad();
        };

        win.Unload += () =>
        {
            if (OnUnload is null)
                return;
            
            OnUnload();
        };

        win.RenderFrame += e =>
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            if (OnRender is not null)
                OnRender.Render();

            win.SwapBuffers();
        };

        win.UpdateFrame += e =>
        {
            frameController.RegisterFrame();

            if (OnFrame is null)
                return;
            
            OnFrame();
        };

        win.KeyDown += e =>
        {
            if (OnKeyDown is null)
                return;

            Input input = (Input)e.Key;
            Modifier modifier = (Modifier)e.Modifiers;
            OnKeyDown(input, modifier);
        };

        win.KeyUp += e =>
        {
            if (OnKeyUp is null)
                return;

            Input input = (Input)e.Key;
            Modifier modifier = (Modifier)e.Modifiers;
            OnKeyUp(input, modifier);
        };

        win.MouseDown += e =>
        {
            if (OnMouseDown is null)
                return;
            
            MouseButton button = (MouseButton)e.Button;
            OnMouseDown(button);
        };

        win.MouseUp += e =>
        {
            if (OnMouseUp is null)
                return;
            
            MouseButton button = (MouseButton)e.Button;
            OnMouseUp(button);
        };

        win.MouseMove += e =>
        {
            if (OnMouseMove is null)
                return;
            
            OnMouseMove((e.X, Height - e.Y));
        };

        win.MouseWheel += e =>
        {
            if (OnMouseWhell is null)
                return;
            
            OnMouseWhell(e.OffsetY);
        };

        win.MouseEnter += () =>
        {
            if (OnMouseEnter is null)
                return;
            
            OnMouseEnter();
        };

        win.MouseLeave += () =>
        {
            if (OnMouseLeave is null)
                return;
            
            OnMouseLeave();
        };
        
        win.Run();
    }

    /// <summary>
    /// Run a function only if the window is open, else
    /// schedule execution.
    /// </summary>
    public static void RunOrSchedule(Action func)
    {
        if (IsOpen)
            func();
        else OnLoad += func;
    }

    /// <summary>
    /// Close main application window.
    /// </summary>
    public static void Close()
    {
        win.Close();
        win.Dispose();
        IsOpen = false;
    }

    /// <summary>
    /// Clear the background without use any render.
    /// </summary>
    public static void Clear(Vec4 color)
    {
        GL.ClearColor(
            color.X,
            color.Y,
            color.Z,
            color.W
        );
    }

    /// <summary>
    /// Set inputs to close the application.
    /// </summary>
    public static void CloseOn(Input input)
    {
        OnKeyDown += (e, m) =>
        {
            if (e == input && (m & Modifier.ActiveModifier) == 0)
                Close();
        };
    }
    
    public static float DeltaTime => frameController.DeltaTime;
    public static float Fps => frameController.Fps;

    private static PipelineCollection renderPipeline = [];
    public static PipelineCollection OnRender => renderPipeline;

    public static event Action OnLoad;
    public static event Action OnUnload;
    public static event Action OnFrame;

    public static event Action<Input, Modifier> OnKeyDown;
    public static event Action<Input, Modifier> OnKeyUp;

    public static event Action<(float x, float y)> OnMouseMove;
    public static event Action<MouseButton> OnMouseDown;
    public static event Action<MouseButton> OnMouseUp;
    public static event Action<float> OnMouseWhell;
    public static event Action OnMouseEnter;
    public static event Action OnMouseLeave;

    private static void updateSize(GameWindow win)
    {
        if (win is null)
            return;
        
        var size = (System.Drawing.Size)win.Size;

        width = size.Width;
        height = size.Height;
        GL.Viewport(0, 0, width, height);
    }
}