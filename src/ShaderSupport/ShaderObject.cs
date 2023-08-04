/* Author:  Leonardo Trevisan Silio
 * Date:    04/08/2023
 */
namespace DuckGL.ShaderSupport;

/// <summary>
/// Represents any data in a shader implementation.
/// </summary>
public class ShaderObject
{
    public ShaderObject(
        ShaderType type,
        string name = null,
        string exp = null
    )
    {
        this.Type = type;
        this.Name = name;
        this.Expression = exp;
    }

    public string Name { get; private set; }
    public ShaderType Type { get; private set; }
    public string Expression { get; private set; }
}