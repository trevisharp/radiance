/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Buffers;
using Shaders;
using Windows;
using Shaders.Objects;
using Contexts;
using Primitives;
using Exceptions;
using Factories;

public abstract class Render(
    Delegate function, params object[] curryingArguments
    ) : DynamicObject
{
    public RenderContext? Context { get; protected set; }
    protected readonly Delegate function = function;
    protected readonly object[] curryingArguments = curryingArguments;
    protected ShaderDependence?[]? Dependences;

    /// <summary>
    /// Currying parameters to create a new render.
    /// </summary>
    public abstract Render Curry(params object?[] args);

    /// <summary>
    /// Generate a Shader object with dependencies based on ParameterInfo. Recive the parameterInfo,
    /// the index of the parameter and the array of curryied values without the curryied polygon.
    /// </summary>
    protected abstract ShaderObject GenerateDependence(ParameterInfo parameter, int index, object?[] curryiedValues);

    /// <summary>
    /// Fill or transform the data to perform the correct rendering operations.
    /// </summary>
    protected abstract IBufferedData FillData(IBufferedData buffer);
}