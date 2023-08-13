/* Author:  Leonardo Trevisan Silio
 * Date:    13/08/2023
 */
using System;

using OpenTK.Graphics.OpenGL4;

namespace Radiance.RenderFunctions;

/// <summary>
/// Represents a Function with a draw interation.
/// </summary>
public abstract class RenderFunction
{
    private int program = -1;

    public abstract Delegate Function { get; }

    public void Render(params object[] parameters)
    {
        if (this.program == -1)
            throw new Exception("A Render request call be a Unloaded RenderFunction.");
    }

    public void Load()
    {
        if (this.program != -1)
            this.Unload();

        this.program = GL.CreateProgram();

        RenderOperations op = new RenderOperations();

        var paramerters = Function.Method.GetParameters();
        object[] fakeInput = new object[paramerters.Length];
        fakeInput[0] = op;

        Function.DynamicInvoke(fakeInput);
        
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, op.VertexShader);
        GL.CompileShader(vertexShader);

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, op.FragmentShader);
        GL.CompileShader(fragmentShader);
        
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        
        GL.LinkProgram(program);

        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);

        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }

    public void Unload()
    {
        if (this.program == -1)
            return;
        
        GL.UseProgram(0);
        GL.DeleteProgram(program);
        this.program = -1;
    }
}