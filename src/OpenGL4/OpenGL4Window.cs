/* Author:  Leonardo Trevisan Silio
 * Date:    30/08/2024
 */
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Radiance.OpenGL4;

using Windows;
using Primitives;

/// <summary>
/// A Implemetation of OpenGL window
/// </summary>
public class OpenGLWindow(bool fullscreen) : BaseWindow
{
    private GameWindow? win;
    private bool canRender = true;
    
    public override int Width { get; protected set; }
    public override int Height { get; protected set; }

    CursorState cursorVisisble = CursorState.Normal;
    public override bool CursorVisible
    {
        get => (win?.CursorState ?? cursorVisisble) != CursorState.Hidden;
        set
        {
            var state = value ? CursorState.Normal : CursorState.Hidden;
            if (win is null)
            {
                cursorVisisble = state;
                return;
            }
            
            win.CursorState = value ? CursorState.Normal : CursorState.Hidden;
        }
    }

    public override void Clear(Vec4 color)
    {
        GL.ClearColor(
            color.X,
            color.Y,
            color.Z,
            color.W
        );
    }

    public override void Open()
    {
        win = new(
            GameWindowSettings.Default,
            new NativeWindowSettings()
            {
                ClientSize = (800, 600),
                WindowState =
                    fullscreen ?
                    WindowState.Fullscreen :
                    WindowState.Normal
            }
        )
        {
            CursorState = cursorVisisble
        };

        win.FramebufferResize += e =>
        {
            UpdateSize(win);
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

            UpdateSize(win);
            
            Load();
        };

        win.Unload += Unload;

        win.RenderFrame += e =>
        {
            if (!canRender)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            Render();

            win.SwapBuffers();
        };

        win.UpdateFrame += e =>
        {
            frameController.RegisterFrame();

            Frame();
        };

        win.KeyDown += e => KeyDown((Input)e.Key, (Modifier)e.Modifiers);

        win.KeyUp += e => KeyUp((Input)e.Key, (Modifier)e.Modifiers);

        win.MouseDown += e => MouseDown((MouseButton)e.Button);

        win.MouseUp += e => MouseUp((MouseButton)e.Button);

        win.MouseMove += e => MouseMove(e.X, Height - e.Y);

        win.MouseWheel += e => MouseWheel(e.OffsetY);

        win.MouseEnter += MouseEnter;

        win.MouseLeave += MouseLeave;
        
        win.Run();
    }

    protected override void CloseWindow()
    {
        win?.Close();
        win?.Dispose();
    }

    private void UpdateSize(GameWindow win)
    {
        if (win is null)
            return;
        
        var size = (Size)win.Size;
        canRender = size != Size.Empty;

        Width = size.Width;
        Height = size.Height;
        GL.Viewport(0, 0, Width, Height);
    }
}