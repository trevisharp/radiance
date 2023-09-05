/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
 */
namespace Radiance.RenderFunctions.Renders;

/// <summary>
/// A single render function.
/// </summary>
public class SingleRender : IRender
{
    public RenderFunction RenderFunction { get; }

    public bool Visible { get; set; } = true;

    public SingleRender(RenderFunction render)
        => this.RenderFunction = render;
    
    public void Load()
        => this.RenderFunction.Load();

    public void Render()
    {
        if (!Visible)
            return;
        
        this.RenderFunction.Render();
    }

    public void Unload()
        => this.RenderFunction.Unload();

    public bool Has(IRender render)
        => render == this;
}