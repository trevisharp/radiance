/* Author:  Leonardo Trevisan Silio
 * Date:    04/08/2023
 */
using System.Text;
using System.Collections.Generic;

namespace DuckGL.ShaderSupport;

/// <summary>
/// Define a context to store shader code informations and declarations.
/// </summary>
public class ShaderContext
{
    public string Version { get; set; } = "330 core";

    public List<(int pos, ShaderType type)> Layout { get; private set; } = new();

    public List<ShaderObject> InVariables { get; private set; } = new();

    public List<ShaderObject> Unifroms { get; private set; } = new();

    public List<(ShaderObject, ShaderObject)> OutVariables { get; private set; } = new();
    
    public ShaderObject Position { get; set; } = null;

    public ShaderObject FragColor { get; set; } = null;
}