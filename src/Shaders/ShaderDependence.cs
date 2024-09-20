/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System;
using System.Text;
using System.Collections.Generic;

namespace Radiance.Shaders;

using Managers;
using Dependencies;

/// <summary>
/// Represents a dependence for shaders creation and execution.
/// </summary>
public abstract class ShaderDependence
{
    public readonly static RandFunctionDependence RandDep = new();
    public readonly static NoiseFunctionDependence NoiseDep = new();
    public readonly static TimeDependence TimeDep = new();
    public readonly static PixelDependence PixelDep = new();
    public readonly static BufferDependence BufferDep = new();
    public readonly static WidthWindowDependence WidthDep = new();
    public readonly static HeightWindowDependence HeightDep = new();

    /// <summary>
    /// Add other dependences associated to this dependences.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<ShaderDependence> AddDependences() => [];

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
    /// Add extra code in the current shader of this dependence.
    /// </summary>
    public virtual void AddFunctions(StringBuilder sb) { }
    
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
    public virtual Action AddOperation(ShaderManager ctx) => null!;
    
    /// <summary>
    /// Add operation to be executed to load dependence data vertex shader.
    /// </summary>
    public virtual Action AddVertexOperation(ShaderManager ctx) => null!;
    
    /// <summary>
    /// Add operation to be executed to load dependence data fragment shader.
    /// </summary>
    public virtual Action AddFragmentOperation(ShaderManager ctx) => null!;

    /// <summary>
    /// Add code in the final of current shader of this dependence.
    /// </summary>
    public virtual void AddFinalCode(StringBuilder sb) { }

    /// <summary>
    /// Add code in the final of vertex shader.
    /// </summary>
    public virtual void AddVertexFinalCode(StringBuilder sb) { }
    
    /// <summary>
    /// Add code in the final of fragment shader.
    /// </summary>
    public virtual void AddFragmentFinalCode(StringBuilder sb) { }
    
    /// <summary>
    /// Update the data used by dependence in its operations.
    /// </summary>
    public virtual void UpdateData(object value) { }
}