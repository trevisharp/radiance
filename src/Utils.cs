/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
using System;
using System.Text;
using System.Linq;

namespace Radiance;

using Data;

using Renders;

using Internal;

using RenderFunctions;
using RenderFunctions.Renders;

using ShaderSupport;
using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

/// <summary>
/// A facade with all utils to use Radiance features.
/// </summary>
public static class utils
{

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static Render render(Action function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static Render render(Action<FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }
    
    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static Render render(Action<
        FloatShaderObject, FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }
    
    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static Render render(Action<
        FloatShaderObject, FloatShaderObject,
        FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static Render render(Action<
        FloatShaderObject, FloatShaderObject, 
        FloatShaderObject, FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

}