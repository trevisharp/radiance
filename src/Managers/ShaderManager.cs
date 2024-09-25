/* Author:  Leonardo Trevisan Silio
 * Date:    24/09/2024
 */
using System;

namespace Radiance.Managers;

using Primitives;

/// <summary>
/// Represents the data and state of a shader program.
/// </summary>
public abstract class ShaderManager : IDisposable
{
    /// <summary>
    /// Associate a program id to this shader.
    /// </summary>
    public abstract void SetProgram(int program);

    /// <summary>
    /// Set a uniform with a name to a specific value.
    /// </summary>
    public abstract void SetFloat(string name, float value);

    /// <summary>
    /// Set a image uniform with a name to a specific value.
    /// </summary>
    public abstract void SetTextureData(string name, Texture texture);

    /// <summary>
    /// Add float values on layout of data buffers.
    /// </summary>
    public abstract void AddLayout(int size);

    /// <summary>
    /// Start to use a Polygon.
    /// </summary>
    public abstract void Use(Polygon poly);

    /// <summary>
    /// Draw Arrays in the selected buffer.
    /// </summary>
    public abstract void Draw(PrimitiveType primitiveType, Polygon poly);

    /// <summary>
    /// Releasing all resources associateds to this shader.
    /// </summary>
    public abstract void Dispose();
}