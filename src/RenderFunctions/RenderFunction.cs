/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2023
 */
using System;

namespace Radiance.RenderFunctions;

/// <summary>
/// Represents a Function with a draw interation.
/// </summary>
public class RenderFunction
{
    private bool loaded = false;
    private RenderOperations op;
    
    public Delegate Function => render;
    private Action<RenderOperations> render;

    public RenderFunction(Action<RenderOperations> render)
        => this.render = render;

    public void Render(params object[] parameters)
    {
        if (!this.loaded)
            throw new Exception("A Render request call be a Unloaded RenderFunction.");
        
        this.op.Render(parameters);
    }

    public void Load()
    {
        if (this.loaded)
            this.Unload();

        this.op = new RenderOperations(
            this.Function
        );

        var paramerters = Function.Method.GetParameters();
        object[] fakeInput = new object[paramerters.Length];
        fakeInput[0] = op;

        Function.DynamicInvoke(fakeInput);

        this.loaded = true;
    }

    public void Unload()
    {
        if (this.loaded)
            return;
        
        this.op.Unload();
        
        this.loaded = true;
    }

    public static implicit operator RenderFunction(
        Action<RenderOperations> render
    ) => new RenderFunction(render);

    public static implicit operator Action(
        RenderFunction func
    ) => () => func.Render();

    public static implicit operator Action<RenderOperations>(
        RenderFunction func
    ) => func.render;

    /// <summary>
    /// Return true if this render is rendering in the screen.
    /// </summary>
    public bool IsRendering
        => Window.IsRendering(render);
    
    /// <summary>
    /// Start render this in Window.
    /// </summary>
    public void StartRender()
    {
        if (IsRendering)
            return;
        
        Window.OnRender += render;
    }
    
    /// <summary>
    /// Stop render this in Window.
    /// </summary>
    public void StopRender()
    {
        if (!IsRendering)
            return;
        
        Window.OnRender -= render;
    }

    /// <summary>
    /// If is rendering, stop render.
    /// If is not rendering, start render.
    /// </summary>
    public void ToggleRender()
    {
        if (IsRendering)
            StopRender();
        else StartRender();
    }
}