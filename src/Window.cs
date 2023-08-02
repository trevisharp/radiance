using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Duck;

public static class Window
{
    private static GameWindow win;

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
            if (OnKeyDown is null)
                return;

            Input input = (Input)e.Key;
            OnKeyUp(input);
        };

        win.Run();
    }

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
}