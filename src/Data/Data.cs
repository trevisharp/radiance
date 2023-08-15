/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
using System;
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// A base class to all data layouts.
/// </summary>
public abstract class Data
{
    public abstract int SetData(float[] arr, int indexoff);
    public abstract int Size { get; }
    public abstract int Elements { get; }
    public abstract string GetLayoutDeclaration { get; }
    public abstract string GetName { get; }
    public abstract ShaderDependence ToDependence { get; }
}