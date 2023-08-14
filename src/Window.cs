/* Author:  Leonardo Trevisan Silio
 * Date:    13/08/2023
 */
using System;
using System.Collections.Generic;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Radiance;

using RenderFunctions;

/// <summary>
/// Represents the main windows that applications run
/// </summary>
public static class Window
{
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
    public static void Open()
    {
        win = new GameWindow(
            GameWindowSettings.Default,
            new NativeWindowSettings()
            {
                Size = (800, 600),
                WindowState = WindowState.Fullscreen
            }
        );

        win.Load += delegate
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

        win.Unload += delegate
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
            if (OnFrame is null)
                return;
            
            OnFrame();
        };

        win.KeyDown += e =>
        {
            if (OnKeyDown is null)
                return;

            Input input = (Input)e.Key;
            OnKeyDown(input);
        };

        win.KeyUp += e =>
        {
            if (OnKeyUp is null)
                return;

            Input input = (Input)e.Key;
            OnKeyUp(input);
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
        OnKeyDown += e =>
        {
            if (e == input)
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
    
    public static event Action OnLoad;
    public static event Action OnUnload;
    public static event Action OnFrame;
    public static event Action<Input> OnKeyDown;
    public static event Action<Input> OnKeyUp;

    private static void updateSize(GameWindow win)
    {
        if (win is null)
            return;
        
        var size = (System.Drawing.Size)win.Size;

        width = size.Width;
        height = size.Height;
    }
}