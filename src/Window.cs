/* Author:  Leonardo Trevisan Silio
 * Date:    02/08/2023
 */
using System;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace DuckGL;

/// <summary>
/// Represents the main windows that applications run
/// </summary>
public static class Window
{
    private static GameWindow win;
    private static int width = 0;
    private static int height = 0;

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
            if (OnLoad is null)
                return;
            
            updateSize(win);
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
            if (OnRender is not null)
                OnRender();

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
    
    public static event Action OnRender;
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

    private static void test()
    {
        GraphicsBuilder gb = new GraphicsBuilder();
    }
}