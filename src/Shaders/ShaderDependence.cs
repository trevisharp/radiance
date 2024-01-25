/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders;

using Objects;

/// <summary>
/// Represents a dependence .
/// </summary>
public abstract class ShaderDependence
{
    /// <summary>
    /// Add code in the current shader of this dependence.
    /// </summary>
    public virtual void AddCode(StringBuilder sb) { }

    /// <summary>
    /// Add code in the vertex shader.
    /// </summary>
    public virtual void AddVertexCode(StringBuilder sb) { }
    
    /// <summary>
    /// Add code in the fragment shader.
    /// </summary>
    public virtual void AddFragmentCode(StringBuilder sb) { }

    /// <summary>
    /// Add header in the current shader of this dependence.
    /// </summary>
    public virtual void AddHeader(StringBuilder sb) { }
    
    /// <summary>
    /// Add header in the vertex shader.
    /// </summary>
    public virtual void AddVertexHeader(StringBuilder sb) { }

    /// <summary>
    /// Add header in the fragment shader.
    /// </summary>
    public virtual void AddFragmentHeader(StringBuilder sb) { }

    /// <summary>
    /// Add operation to be executed to load dependence data in the current
    /// shader of the dependence.
    /// </summary>
    public virtual Action AddOperation(ShaderContext ctx) => null;
    
    /// <summary>
    /// Add operation to be executed to load dependence data vertex shader.
    /// </summary>
    public virtual Action AddVertexOperation(ShaderContext ctx) => null;
    
    /// <summary>
    /// Add operation to be executed to load dependence data fragment shader.
    /// </summary>
    public virtual Action AddFragmentOperation(ShaderContext ctx) => null;

    /// <summary>
    /// Update the data used by dependence in its operations.
    /// </summary>
    public virtual void UpdateData(object value) { }
}