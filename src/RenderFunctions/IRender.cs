/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
 */
namespace Radiance.RenderFunctions;

/// <summary>
/// A base render element to build a chain of renders.
/// </summary>
public interface IRender
{
    bool Visible { get; set; }

    void Load();
    void Render();
    void Unload();

    bool Has(IRender render);
}