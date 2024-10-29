/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2024
 */
using System;

namespace Radiance.Contexts;

using Primitives;
using Radiance.Buffers;

/// <summary>
/// Represents the data and state of a shader program.
/// </summary>
public interface IShaderConfiguration : IDisposable
{
    /// <summary>
    /// Set a uniform with a name to a specific value.
    /// </summary>
    void SetFloat(string name, float value);
    
    /// <summary>
    /// Set a uniform with a name to a specific value.
    /// </summary>
    void SetVec(string name, float x, float y);
    
    /// <summary>
    /// Set a uniform with a name to a specific value.
    /// </summary>
    void SetVec(string name, float x, float y, float z);
    
    /// <summary>
    /// Set a uniform with a name to a specific value.
    /// </summary>
    void SetVec(string name, float x, float y, float z, float w);

    /// <summary>
    /// Set a image uniform with a name to a specific value.
    /// </summary>
    void SetTextureData(string name, Texture texture);

    /// <summary>
    /// Add float values on layout of data buffers.
    /// </summary>
    void AddLayout(int size, DataType dataType);
}