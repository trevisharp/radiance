/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
 */
using System;
using System.Collections.Generic;

namespace Radiance.RenderFunctions.Renders;

/// <summary>
/// A render that is a collection of renders.
/// </summary>
public class BlockRender : IRender
{
    private bool canLoad = false;
    private List<IRender> list = new();

    public bool Visible { get; set; } = true;

    public void Load()
    {
        canLoad = true;
        foreach (var render in list)
            render.Load();
    }

    public void Add(IRender render)
    {
        if (list.Contains(render))
            return;

        if (canLoad)
            render.Load();
        
        list.Add(render);
    }

    public void Remove(IRender render)
    {
        list.Remove(render);
        render.Unload();
    }

    public void Render()
    {
        if (!Visible)
            return;

        foreach (var render in list)
            render.Render();
    }

    public void Unload()
    {
        foreach (var render in list)
            render.Unload();
    }

    public bool Has(IRender render)
    {
        if (render == this)
            return true;
        
        foreach (var innerRender in list)
        {
            if (innerRender.Has(render))
                return true;
        }

        return false;
    }

    public static BlockRender operator +(BlockRender queue, IRender render)
    {
        queue.Add(render);
        return queue;
    }

    public static BlockRender operator -(BlockRender queue, IRender render)
    {
        queue.Remove(render);
        return queue;
    }

    public static BlockRender operator +(BlockRender queue, Action<RenderOperations> func)
    {
        var renderFunction = new RenderFunction(func);
        var render = new SingleRender(renderFunction);

        queue.Add(render);
        return queue;
    }
}