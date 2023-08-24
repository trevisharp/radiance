/* Author:  Leonardo Trevisan Silio
 * Date:    23/08/2023
 */
using System;
using System.Collections.Generic;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;

namespace Radiance;

using Internal;
using RenderFunctions;

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
    /// The width of the screen
    /// </summary>
    public static int Width => width;

    /// <summary>
    /// The height of the screen
    /// </summary>
    public static int Height => height;

    /// <summary>
    /// Open main application window
    /// </summary>
    public static void Open(bool fullscreen = true)
    {
        win = new GameWindow(
            GameWindowSettings.Default,
            new NativeWindowSettings()
            {
                Size = (800, 600),
                WindowState = 
                    fullscreen ?
                    WindowState.Fullscreen :
                    WindowState.Normal
            }
        );

        win.Resize += e =>
        {
            updateSize(win);
        };

        win.Load += () =>
        {
            updateSize(win);
            initializated = true;
            foreach (var render in renders)
                mapRender(render);
            renders.Clear();
            
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
            if (onRender is not null)
                onRender();

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
            
            OnMouseMove((e.X, e.Y));
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
    /// Close main application window
    /// </summary>
    public static void Close()
    {
        win.Close();
        win.Dispose();
    }

    /// <summary>
    /// Set inputs to close the application
    /// </summary>
    public static void CloseOn(Input input)
    {
        OnKeyDown += (e, m) =>
        {
            if (e == input && (m & Modifier.ActiveModifier) == 0)
                Close();
        };
    }

    private static event Action onRender;
    private static Dictionary<Action<RenderOperations>, Action> renderMap = new();

    private static bool initializated = false;
    private static List<Action<RenderOperations>> renders = new();
    public static event Action<RenderOperations> OnRender
    {
        add
        {
            if (value is null)
                return;

            if (initializated)
                mapRender(value);
            else renders.Add(value);
        }
        remove
        {
            if (!renderMap.ContainsKey(value))
                return;
            
            var mappedAction = renderMap[value];
            onRender -= mappedAction;

            renderMap.Remove(value);
        }
    }
    private static void mapRender(Action<RenderOperations> value)
    {
        GenericRenderFunction renderFunction = value;
        renderFunction.Load();

        Action mappedAction = renderFunction;
        renderMap.Add(value, mappedAction);

        onRender += mappedAction;
    }
    
    public static float DeltaTime => frameController.DeltaTime;
    public static float Fps => frameController.Fps;

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