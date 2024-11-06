/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
namespace Radiance.Implementations;

using OpenGL4;
using Exceptions;

/// <summary>
/// The implementation configuration to get or set the main implementation of Radiance.
/// </summary>
public static class ImplementationConfig
{
    private static IImplementationFactory implementation = new OpenGL4Factory();
    public static IImplementationFactory Implementation
    {
        get => implementation;
        set
        {
            if (value is null)
                throw new InvalidImplementationException(value);
            
            implementation = value;
        }
    }
}