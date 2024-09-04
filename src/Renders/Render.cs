/* Author:  Leonardo Trevisan Silio
 * Date:    30/08/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Primitives;
using Exceptions;

using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;

/// <summary>
/// Represents a function that can used by GPU to draw in the screen.
/// </summary>
public class Render(
    Delegate function,
    params object[] curryingArguments
    ) : DynamicObject
{
    public override bool TryInvoke(
        InvokeBinder binder,
        object?[]? args,
        out object? result)
    {
        throw new NotImplementedException();
    }

    public Render Curry(params object[] args)
        => new(function, [ ..curryingArguments, ..args ]);

    public static implicit operator Action(Render render)
    {
        throw new NotImplementedException();
    }
}