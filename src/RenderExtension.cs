/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
 */
namespace Radiance;

using RenderFunctions;

/// <summary>
/// Extension class of util operations with Renders.
/// </summary>
public static class RenderExtesion
{
    /// <summary>
    /// Rerturn true if the render is in the screen.
    /// </summary>
    public static bool IsRendering(this IRender render)
        => Window.OnRender.Has(render);

    /// <summary>
    /// Return true if the render is in the screen and is visible.
    /// </summary>
    public static bool IsVisible(this IRender render)
        => render.IsRendering() && render.Visible;

    /// <summary>
    /// Show the render.
    /// </summary>
    public static IRender Show(this IRender render)
    {   
        render.Visible = true;

        if (render.IsRendering())
            return render;
        
        Window.OnRender += render;
        return render;
    }

    /// <summary>
    /// Set visibility to false.
    /// </summary>
    public static IRender SoftHide(this IRender render)
    {
        render.Visible = false;
        return render;
    }

    /// <summary>
    /// Remove the render of the screen.
    /// </summary>
    public static IRender HardHide(this IRender render)
    {
        Window.OnRender -= render;
        return render;
    }
    
    /// <summary>
    /// Toggle visibility of render.
    /// </summary>
    public static IRender Toggle(this IRender render)
    {
        if (render.IsVisible())
            return render.SoftHide();
        
        return render.Show();
    }
}