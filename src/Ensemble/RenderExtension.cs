/* Author:  Leonardo Trevisan Silio
 * Date:    08/10/2024
 */
using System;
using System.Linq.Expressions;

namespace Radiance.Ensemble;

using Buffers;
using Shaders;

/// <summary>
/// Provide a set of funtions to handle shaders as colletions of shaders.
/// </summary>
public static class RenderExtension
{
    public class Wrapper<T>
    {
        
    }

    public static Wrapper<T> Select<T>(
        this Polygon poly, 
        Expression<Func<Wrapper<Polygon>, T>> expression)
    {
        return new();
    }

    public static Wrapper<R> Select<T, R>(
        this Wrapper<T> wrapper,
        Expression<Func<Wrapper<T>, R>> expression)
    {
        return new();
    }

    // public static RenderCollection<T> Select<T>(this RenderCollection<T> collection, Func<float, string> f)
    //     where T : ShaderObject
    // {
    //     System.Console.WriteLine("oi");
    //     return collection;
    // }

    // public static RenderCollection<T> Where<T>(this RenderCollection<T> collection, Func<float, string> f)
    //     where T : ShaderObject
    // {
    //     System.Console.WriteLine("oi");
    //     return collection;
    // }
}