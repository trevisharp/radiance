/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
using System;
using System.Reflection;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Radiance.Implementations.OpenGL4;

using Windows;
using Primitives;

/// <summary>
/// A Implemetation of OpenGL4 window.
/// </summary>
public class OpenGL4Window : BaseWindow
{
    private GameWindow? win;
    private ClearBufferMask clearMask = ClearBufferMask.ColorBufferBit;
    
    public override int Width { get; protected set; }
    public override int Height { get; protected set; }

    string title = Assembly.GetEntryAssembly()?.GetName().Name ?? "Radiance";
    public override string Title
    {
        get => title;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            title = value;
            if (win is null)
                return;

            win.Title = value;
        }
    }

    (int width, int height) size = (800, 600);
    public override (int width, int height) Size
    {
        get => win is null ? size : (Width, Height);
        set
        {
            if (value.width <= 0 || value.height <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "The size must be positive.");

            size = value;
            if (win is null)
                return;

            win.ClientSize = new Vector2i(value.width, value.height);
        }
    }

    (int width, int height) minimumSize = (0, 0);
    public override (int width, int height) MinimumSize
    {
        get => minimumSize;
        set
        {
            if (value.width < 0 || value.height < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "The minimum size cannot be negative.");

            minimumSize = value;
            if (win is null)
                return;

            win.MinimumSize = GetMinimumClientSize();
        }
    }

    WindowMode mode = WindowMode.Fullscreen;
    public override WindowMode Mode
    {
        get => mode;
        set
        {
            mode = value;
            if (win is null)
                return;

            win.WindowBorder = GetBorder();
            win.WindowState = GetState();
        }
    }

    bool resizable = true;
    public override bool Resizable
    {
        get => resizable;
        set
        {
            resizable = value;
            if (win is null)
                return;

            win.WindowBorder = GetBorder();
        }
    }

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

    VSync vSyncMode = VSync.Off;
    public override VSync VSync
    {
        get => vSyncMode;
        set
        {
            vSyncMode = value;
            if (win is null)
                return;
            
            win.VSync = (VSyncMode)value;
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
                    GL.DepthFunc(DepthFunction.Lequal);
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

    bool scissorTest = false;
    public override bool ScissorTest
    {
        get => scissorTest;
        set
        {
            RunOrSchedule(() =>
            {
                scissorTest = value;
                if (value) GL.Enable(EnableCap.ScissorTest);
                else GL.Disable(EnableCap.ScissorTest);

                #if DEBUG_OPENGL4
                if (value)
                    Console.WriteLine($"GL.Enable(EnableCap.ScissorTest)");
                else Console.WriteLine($"GL.Disable(EnableCap.ScissorTest)");
                #endif
            });
        }
    }

    int scissorX = 0;
    int scissorY = 0;
    int scissorWidth = 0;
    int scissorHeight = 0;
    public override (int x, int y) ScissorPoint
    {
        get => (scissorX, scissorY);
        set
        {
            RunOrSchedule(() =>
            {
                (scissorX, scissorY) = value;
                GL.Scissor(scissorX, scissorY, scissorWidth, scissorHeight);

                #if DEBUG_OPENGL4
                Console.WriteLine($"GL.Scissor({scissorX}, {scissorY}, {scissorWidth}, {scissorHeight});");
                #endif
            });
        }
    }
    
    public override (int width, int height) ScissorSize
    {
        get => (scissorWidth, scissorHeight);
        set
        {
            RunOrSchedule(() =>
            {
                (scissorWidth, scissorHeight) = value;
                GL.Scissor(scissorX, scissorY, scissorWidth, scissorHeight);

                #if DEBUG_OPENGL4
                Console.WriteLine($"GL.Scissor({scissorX}, {scissorY}, {scissorWidth}, {scissorHeight});");
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
                Title = title,
                ClientSize = new Vector2i(size.width, size.height),
                MinimumClientSize = GetMinimumClientSize(),
                WindowBorder = GetBorder(),
                WindowState = GetState(),
                TransparentFramebuffer = true,
                Flags = ContextFlags.Default,
                Vsync = (VSyncMode)vSyncMode
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

        win.Unload += () =>
        {
            Unload();
            OpenGL4ShaderContext.FreeAllResources();
        };

        win.RenderFrame += e => RenderFrame();

        win.UpdateFrame += e => Frame();

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

        var clientSize = win.ClientSize;
        CanRender = clientSize.X > 0 && clientSize.Y > 0;

        var changed = clientSize.X != Width || clientSize.Y != Height;
        Width = clientSize.X;
        Height = clientSize.Y;

        var framebuffer = win.FramebufferSize;
        GL.Viewport(0, 0, framebuffer.X, framebuffer.Y);

        if (changed && CanRender && IsOpen)
            Resize(Width, Height);
    }

    Vector2i? GetMinimumClientSize()
        => minimumSize == (0, 0) ? null : new Vector2i(minimumSize.width, minimumSize.height);

    WindowBorder GetBorder() => (mode, resizable) switch
    {
        (WindowMode.Borderless, _) => WindowBorder.Hidden,
        (_, true) => WindowBorder.Resizable,
        _ => WindowBorder.Fixed
    };

    WindowState GetState() => mode switch
    {
        WindowMode.Fullscreen => WindowState.Fullscreen,
        WindowMode.Maximized => WindowState.Maximized,
        _ => WindowState.Normal
    };
}