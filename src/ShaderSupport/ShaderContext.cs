/* Author:  Leonardo Trevisan Silio
 * Date:    03/08/2023
 */
using System.Collections.Generic;

namespace DuckGL.ShaderSupport;

public class ShaderContext
{
    public string Version { get; set; } = "330 core";

    public List<(int pos, ShaderType type)> Layout { get; set; } = new();
}