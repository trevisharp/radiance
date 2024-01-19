/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
using System.Threading;
using System.Collections.Generic;

namespace Radiance.Internal;

using Data;
using RenderFunctions;
using ShaderSupport.Objects;

/// <summary>
/// A global thread-safe context to shader construction.
/// </summary>
internal class RenderContext
{
    private static Dictionary<int, RenderContext> threadMap = new();
    internal static RenderContext CreateContext()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        if (threadMap.ContainsKey(id))
            threadMap.Remove(id);
    
        var ctx = new RenderContext();
        threadMap.Add(id, ctx);
        return ctx;
    }
    internal static RenderContext GetContext()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        if (threadMap.ContainsKey(id))
            return threadMap[id];
        
        return null;
    }
    
    internal Polygon Data { get; set; }
    internal RenderOperations Operation { get; set; }
    internal Vec3ShaderObject Position { get; set; }
    internal Vec4ShaderObject Color { get; set; }
}