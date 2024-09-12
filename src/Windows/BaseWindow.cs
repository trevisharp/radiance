/* Author:  Leonardo Trevisan Silio
 * Date:    12/09/2024
 */
using System;

namespace Radiance.Windows;

using Primitives;

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
    /// Get the phase of render pipeline from this window.
    /// </summary>
    public WindowPhase Phase { get; protected set; } = WindowPhase.None;

    /// <summary>
    /// Get or Set if the window is active and will call events.
    /// </summary>
    public bool Active { get; set; } = true;

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
    public abstract void Open();

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
    
    public event Action? OnRender;
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
        
        if (!Active)
            return;
        
        OnMouseLeave();
    }

    protected void MouseEnter()
    {
        if (OnMouseEnter is null)
            return;
        
        if (!Active)
            return;
        
        OnMouseEnter();
    }

    protected void MouseWheel(float offset)
    {
        if (OnMouseWhell is null)
            return;
        
        if (!Active)
            return;
        
        OnMouseWhell(offset);
    }

    protected void MouseMove(float x, float y)
    {
        if (OnMouseMove is null)
            return;
        
        if (!Active)
            return;
        
        OnMouseMove((x, y));
    }

    protected void MouseUp(MouseButton buttons)
    {
        if (OnMouseUp is null)
            return;
        
        if (!Active)
            return;
        
        OnMouseUp(buttons);
    }

    protected void MouseDown(MouseButton buttons)
    {
        if (OnMouseDown is null)
            return;
        
        if (!Active)
            return;
        
        OnMouseDown(buttons);
    }

    protected void KeyDown(Input input, Modifier modifier)
    {
        if (OnKeyDown is null)
            return;
        
        if (!Active)
            return;
        
        OnKeyDown(input, modifier);
    }
    
    protected void KeyUp(Input input, Modifier modifier)
    {
        if (OnKeyUp is null)
            return;
        
        if (!Active)
            return;
        
        OnKeyUp(input, modifier);
    }

    protected void Frame()
    {
        if (OnFrame is null)
            return;
        
        if (!Active)
            return;
        
        Phase = WindowPhase.OnFrame;
        OnFrame();
        Phase = WindowPhase.None;
    }

    protected void Render()
    {
        if (OnRender is null)
            return;
        
        if (!Active)
            return;
        
        Phase = WindowPhase.OnRender;
        OnRender();
        Phase = WindowPhase.None;
    }

    protected void Load()
    {
        if (OnLoad is null)
            return;
        
        Phase = WindowPhase.OnLoad;
        OnLoad();
        Phase = WindowPhase.None;
    }

    protected void Unload()
    {
        if (OnUnload is null)
            return;
        
        Phase = WindowPhase.OnUnload;
        OnUnload();
        Phase = WindowPhase.None;
    }
}