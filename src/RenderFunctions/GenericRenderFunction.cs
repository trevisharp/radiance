/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2023
 */
using System;

namespace Radiance.RenderFunctions;

/// <summary>
/// Represents a Function with a draw interation.
/// </summary>
public class GenericRenderFunction : RenderFunction
{
    public override Delegate Function => render;
    private Action<RenderOperations> render;

    public GenericRenderFunction(Action<RenderOperations> render)
        => this.render = render;
    
    public static implicit operator GenericRenderFunction(
        Action<RenderOperations> render
    ) => new GenericRenderFunction(render);

    public static implicit operator Action(
        GenericRenderFunction func
    ) => () => func.Render();
}

/// <summary>
/// Represents a Function with a draw interation.
/// </summary>
public class GenericRenderFunction<T> : RenderFunction
{
    public override Delegate Function => render;
    private Action<RenderOperations, T> render;

    public GenericRenderFunction(Action<RenderOperations, T> render)
        => this.render = render;
    
    public static implicit operator GenericRenderFunction<T>(
        Action<RenderOperations, T> render
    ) => new GenericRenderFunction<T>(render);

    public static implicit operator Action<T>(
        GenericRenderFunction<T> func
    ) => p1 => func.Render(p1);
}

/// <summary>
/// Represents a Function with a draw interation.
/// </summary>
public class GenericRenderFunction<T1, T2> : RenderFunction
{
    public override Delegate Function => render;
    private Action<RenderOperations, T1, T2> render;

    public GenericRenderFunction(Action<RenderOperations, T1, T2> render)
        => this.render = render;
    
    public static implicit operator GenericRenderFunction<T1, T2>(
        Action<RenderOperations, T1, T2> render
    ) => new GenericRenderFunction<T1, T2>(render);

    public static implicit operator Action<T1, T2>(
        GenericRenderFunction<T1, T2> func
    ) => (p1, p2) => func.Render(p1, p2);
}

/// <summary>
/// Represents a Function with a draw interation.
/// </summary>
public class GenericRenderFunction<T1, T2, T3> : RenderFunction
{
    public override Delegate Function => render;
    private Action<RenderOperations, T1, T2, T3> render;

    public GenericRenderFunction(Action<RenderOperations, T1, T2, T3> render)
        => this.render = render;
    
    public static implicit operator GenericRenderFunction<T1, T2, T3>(
        Action<RenderOperations, T1, T2, T3> render
    ) => new GenericRenderFunction<T1, T2, T3>(render);

    public static implicit operator Action<T1, T2, T3>(
        GenericRenderFunction<T1, T2, T3> func
    ) => (p1, p2, p3) => func.Render(p1, p2, p3);
}

/// <summary>
/// Represents a Function with a draw interation.
/// </summary>
public class GenericRenderFunction<T1, T2, T3, T4> : RenderFunction
{
    public override Delegate Function => render;
    private Action<RenderOperations, T1, T2, T3, T4> render;

    public GenericRenderFunction(Action<RenderOperations, T1, T2, T3, T4> render)
        => this.render = render;
    
    public static implicit operator GenericRenderFunction<T1, T2, T3, T4>(
        Action<RenderOperations, T1, T2, T3, T4> render
    ) => new GenericRenderFunction<T1, T2, T3, T4>(render);

    public static implicit operator Action<T1, T2, T3, T4>(
        GenericRenderFunction<T1, T2, T3, T4> func
    ) => (p1, p2, p3, p4) => func.Render(p1, p2, p3, p4);
}

/// <summary>
/// Represents a Function with a draw interation.
/// </summary>
public class GenericRenderFunction<T1, T2, T3, T4, T5> : RenderFunction
{
    public override Delegate Function => render;
    private Action<RenderOperations, T1, T2, T3, T4, T5> render;

    public GenericRenderFunction(Action<RenderOperations, T1, T2, T3, T4, T5> render)
        => this.render = render;
    
    public static implicit operator GenericRenderFunction<T1, T2, T3, T4, T5>(
        Action<RenderOperations, T1, T2, T3, T4, T5> render
    ) => new GenericRenderFunction<T1, T2, T3, T4, T5>(render);

    public static implicit operator Action<T1, T2, T3, T4, T5>(
        GenericRenderFunction<T1, T2, T3, T4, T5> func
    ) => (p1, p2, p3, p4, p5) => func.Render(p1, p2, p3, p4, p5);
}