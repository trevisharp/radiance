/* Author:  Leonardo Trevisan Silio
 * Date:    21/10/2024
 */
using System;
using System.Collections.Generic;

namespace Radiance.Renders;

using Shaders;
using Contexts;

public record CallMatch(
    List<Type> Types,
    List<ShaderDependence> Dependences,
    RenderContext Context
);