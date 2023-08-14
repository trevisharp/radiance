/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2023
 */
using System;

namespace Radiance.RenderFunctions;

/// <summary>
/// Represents a Function with a draw interation.
/// </summary>
public abstract class RenderFunction
{
    private bool loaded = false;
    private RenderOperations op;

    public abstract Delegate Function { get; }

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

        this.op = new RenderOperations();

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