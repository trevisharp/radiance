/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
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

    public void Render()
    {
        if (!this.loaded)
            throw new Exception("A Render request call be a Unloaded RenderFunction.");
        
        this.op.Render();
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
}