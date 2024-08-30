/* Author:  Leonardo Trevisan Silio
 * Date:    30/08/2024
 */
using System;

namespace Radiance.Windows;

using Data;

/// <summary>
/// A Base class for all window implementations.
/// </summary>
public abstract class BaseWindow
{
    internal protected class TimeFrameController
    {
        DateTime newer = DateTime.UtcNow;
        DateTime older = DateTime.UtcNow;

        public void RegisterFrame()
        {
            older = newer;
            newer = DateTime.UtcNow;
        }

        public float DeltaTime
        {
            get
            {
                var delta = newer - older;
                var time = delta.TotalSeconds;
                return (float)time;
            }
        }

        public float Fps => 1.0f / DeltaTime;
    }

    protected readonly TimeFrameController frameController = new();

    /// <summary>
    /// The time between the current and the last frame.
    /// </summary>
    public float DeltaTime => frameController.DeltaTime;

    /// <summary>
    /// Current Frames Per Second for this application.
    /// </summary>
    public float Fps => frameController.Fps;
    
    /// <summary>
    /// Return true if screen is Open.
    /// </summary>
    public bool IsOpen { get; protected set; } = false;

    /// <summary>
    /// The width of the screen.
    /// </summary>
    public abstract int Width { get; protected set; }

    /// <summary>
    /// The height of the screen.
    /// </summary>
    public abstract int Height { get; protected set; }

    /// <summary>
    /// Get and set if the cursor is visible.
    /// </summary>
    public abstract bool CursorVisible { get; set; }

    /// <summary>
    /// Open main application window.
    /// </summary>
    public abstract void Open(bool fullscreen = true);

    /// <summary>
    /// Run a function only if the window is open, else
    /// schedule execution.
    /// </summary>
    public void RunOrSchedule(Action func)
    {
        if (IsOpen)
            func();
        else OnLoad += func;
    }

    /// <summary>
    /// Close main application window.
    /// </summary>
    public void Close()
    {
        CloseWindow();
        IsOpen = false;
    }
    protected abstract void CloseWindow();

    /// <summary>
    /// Clear the background without use any render.
    /// </summary>
    public abstract void Clear(Vec4 color);

    /// <summary>
    /// Set inputs to close the application.
    /// </summary>
    public void CloseOn(Input input)
    {
        OnKeyDown += (e, m) =>
        {
            if (e == input && (m & Modifier.ActiveModifier) == 0)
                Close();
        };
    }
    
    // TODO: Pipeline generation process
    event Action? RenderActions;
    public event Action OnRender
    {
        add
        {
            RenderActions += value;
        }
        remove
        {
            RenderActions -= value;
        }
    }

    public event Action? OnLoad;
    public event Action? OnUnload;
    public event Action? OnFrame;

    public event Action<Input, Modifier>? OnKeyDown;
    public event Action<Input, Modifier>? OnKeyUp;

    public event Action<(float x, float y)>? OnMouseMove;
    public event Action<MouseButton>? OnMouseDown;
    public event Action<MouseButton>? OnMouseUp;
    public event Action<float>? OnMouseWhell;
    public event Action? OnMouseEnter;
    public event Action? OnMouseLeave;

    protected void MouseLeave()
    {
        if (OnMouseLeave is null)
            return;
        
        OnMouseLeave();
    }

    protected void MouseEnter()
    {
        if (OnMouseEnter is null)
            return;
        
        OnMouseEnter();
    }

    protected void MouseWheel(float offset)
    {
        if (OnMouseWhell is null)
            return;
        
        OnMouseWhell(offset);
    }

    protected void MouseMove(float x, float y)
    {
        if (OnMouseMove is null)
            return;
        
        OnMouseMove((x, y));
    }

    protected void MouseUp(MouseButton buttons)
    {
        if (OnMouseUp is null)
            return;
        
        OnMouseUp(buttons);
    }

    protected void MouseDown(MouseButton buttons)
    {
        if (OnMouseDown is null)
            return;
        
        OnMouseDown(buttons);
    }

    protected void KeyDown(Input input, Modifier modifier)
    {
        if (OnKeyDown is null)
            return;
        
        OnKeyDown(input, modifier);
    }
    
    protected void KeyUp(Input input, Modifier modifier)
    {
        if (OnKeyUp is null)
            return;
        
        OnKeyUp(input, modifier);
    }

    protected void Frame()
    {
        if (OnFrame is null)
            return;
        
        OnFrame();
    }

    protected void Render()
    {
        if (RenderActions is null)
            return;
        
        RenderActions();
    }

    protected void Load()
    {
        if (OnLoad is null)
            return;
        
        OnLoad();
    }

    protected void Unload()
    {
        if (OnUnload is null)
            return;
        
        OnUnload();
    }
}