/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
using System.Drawing;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

#if DEBUG_OPENGL4
using System;
#endif

namespace Radiance.Implementations.OpenGL4;

using Windows;
using Primitives;

/// <summary>
/// A Implemetation of OpenGL4 window.
/// </summary>
public class OpenGL4Window : BaseWindow
{
    private GameWindow? win;
    private bool fullscreen = true;
    private ClearBufferMask clearMask = ClearBufferMask.ColorBufferBit;
    
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

    bool zBuffer = true;
    public override bool ZBufferEnable
    {
        get => zBuffer;
        set
        {
            RunOrSchedule(() =>
            {
                zBuffer = value;
                if (zBuffer)
                {
                    GL.Enable(EnableCap.DepthTest);
                    clearMask = ClearBufferMask.ColorBufferBit 
                        | ClearBufferMask.DepthBufferBit;

                    #if DEBUG_OPENGL4
                    Console.WriteLine("GL.Enable(EnableCap.DepthTest);");
                    #endif
                }
                else
                {
                    GL.Disable(EnableCap.DepthTest);
                    clearMask = ClearBufferMask.ColorBufferBit;

                    #if DEBUG_OPENGL4
                    Console.WriteLine("GL.Disable(EnableCap.DepthTest);");
                    #endif
                }
            });
        }
    }

    bool blendMode = false;
    public override bool BlendingMode
    {
        get => blendMode;
        set
        {
            RunOrSchedule(() =>
            {
                blendMode = value;
                if (blendMode)
                {
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(
                        BlendingFactor.SrcAlpha, 
                        BlendingFactor.OneMinusSrcAlpha
                    );
                    
                    #if DEBUG_OPENGL4
                    Console.WriteLine("GL.Enable(EnableCap.Blend);");
                    Console.WriteLine("GL.BlendFunc(");
                    Console.WriteLine("    BlendingFactor.SrcAlpha,");
                    Console.WriteLine("    BlendingFactor.OneMinusSrcAlpha");
                    Console.WriteLine(");");
                    #endif
                }
                else
                {
                    GL.Disable(EnableCap.Blend);
                    
                    #if DEBUG_OPENGL4
                    Console.WriteLine("GL.Disable(EnableCap.Blend);");
                    #endif
                }
            });
        }
    }

    bool lineSmooth = false;
    public override bool LineSmooth
    {
        get => lineSmooth;
        set
        {
            RunOrSchedule(() =>
            {
                lineSmooth = value;
                if (lineSmooth)
                {
                    GL.Enable(EnableCap.LineSmooth);
                    
                    #if DEBUG_OPENGL4
                    Console.WriteLine("GL.Enable(EnableCap.LineSmooth);");
                    #endif
                }
                else
                {
                    GL.Disable(EnableCap.LineSmooth);
                    
                    #if DEBUG_OPENGL4
                    Console.WriteLine("GL.Disable(EnableCap.LineSmooth);");
                    #endif
                }
            });
        }
    }

    Vec4 clearColor = (0f, 0f, 0f, 0f);
    public override Vec4 ClearColor
    {
        get => clearColor;
        set
        {
            RunOrSchedule(() =>
            {
                clearColor = value;
                GL.ClearColor(
                    value.X,
                    value.Y,
                    value.Z,
                    value.W
                );

                #if DEBUG_OPENGL4
                Console.WriteLine($"GL.ClearColor({value.X}, {value.Y}, {value.Z}, {value.W})");
                #endif
            });
        }
    }

    public override void Clear()
    {
        GL.Clear(clearMask);

        #if DEBUG_OPENGL4
        Console.WriteLine("GL.Clear(...)");
        #endif
    }

    public override void SwapBuffers()
    {
        if (win is null)
            return;

        win.SwapBuffers();
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
                    WindowState.Normal,
                TransparentFramebuffer = true
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
            UpdateSize(win);
            
            Load();
        };

        win.Unload += Unload;

        win.RenderFrame += e => RenderFrame();

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
        CanRender = size != Size.Empty;

        Width = size.Width;
        Height = size.Height;
        GL.Viewport(0, 0, Width, Height);
    }
}