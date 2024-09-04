/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace Radiance.Contexts;

/// <summary>
/// A Thread-Safe context data object.
/// </summary>
public abstract class Context
{
    static readonly Dictionary<int, List<(Type, Context)>> threadMap = [];

    static int GetCurrentThreadId()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        return id;
    }

    static List<(Type, Context)> GetContextBuffer()
    {
        var id = GetCurrentThreadId();
        if (threadMap.TryGetValue(id, out var list))
            return list;
        
        List<(Type, Context)> buffer = [];
        threadMap.Add(id, buffer);
        return buffer;
    }

    public static void OpenContext<T>()
        where T : Context, new()
    {
        CloseContext<T>();
        var buffer = GetContextBuffer();
        buffer.Add((typeof(T), new T()));
    }

    public static void CloseContext<T>()
        where T : Context, new()
    {
        var buffer = GetContextBuffer();
        var type = typeof(T);
        buffer.RemoveAll(t => t.Item1 == type);
    }

    public static T? GetContext<T>()
        where T : Context, new()
    {
        var buffer = GetContextBuffer();
        
        var type = typeof(T);
        var ctx = buffer
            .FirstOrDefault(t => t.Item1 == type);
        
        return ctx.Item2 as T;
    }
}