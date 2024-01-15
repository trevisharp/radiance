/* Author:  Leonardo Trevisan Silio
 * Date:    15/01/2024
 */
using System;
using System.Text;
using System.Collections.Generic;

namespace Radiance.Data;

/// <summary>
/// Represents a data that can be sended to a shader and drawed.
/// </summary>
public abstract class NewData
{
    private List<LayoutInfo> layouts = new();
    internal void AppendLayout(int size, string type, string name)
        => this.layouts.Add(new(size, type, name));
    internal string GetHeader()
    {
        StringBuilder sb = new StringBuilder();

        int location = 0;
        foreach (var layout in layouts)
            sb.AppendLine($"layout (location = {location}) in {layout.type} {layout.name};");

        return sb.ToString();
    }

    private record LayoutInfo(
        int size,
        string type,
        string name
    );
}