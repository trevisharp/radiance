/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2023
 */
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

namespace Radiance.RenderFunctions;

using Data;

using ShaderSupport;
using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

/// <summary>
/// Provide render operations to draw data in screen.
/// </summary>
public class RenderOperations
{
    public void Clear(Color color)
    {
        effects += delegate
        {
            GL.ClearColor(
                color.R / 255f,
                color.G / 255f,
                color.B / 255f,
                color.A / 255f
            );
            GL.Clear(ClearBufferMask.ColorBufferBit);
        };
    }

    public void Fill(
        Func<Vec3ShaderObject, Vec3ShaderObject> vertexShader,
        Func<Vec4ShaderObject> color,
        params Vector[] data
    )
    {
        if (data.Length == 0)
            return;

        var buffer = new Vec3BufferDependence(
            data.GetBuffer()
        );
        var position = new Vec3ShaderObject(data[0].GetName, buffer);

        var vertexObject = vertexShader(position);
        var vertexTuple = generateVertexShader(vertexObject);

    }

    internal void FinishSetup()
    {

    }

    internal void Render(params object[] parameters)
    {
        if (effects is null)
            return;
        
        effects(parameters);
    }

    internal void Unload()
    {
        GL.UseProgram(0);
        foreach (var program in programList)
            GL.DeleteProgram(program);
        programList.Clear();
    }

    private event Action<object[]> effects;

    private int activatedProgram = -1;

    private List<int> programList = new();
    private int program => 
        programList.Count == 0 ? -1 :
        programList[programList.Count - 1];
    
    private Stack<int> vertexShaderStack = new();
    private int vertexShader =>
        vertexShaderStack.Count == 0 ? -1 :
        vertexShaderStack.Peek();
    
    private Stack<int> fragmentShaderStack = new();
    private int fragmentShader =>
        fragmentShaderStack.Count == 0 ? -1 :
        fragmentShaderStack.Peek();

    private void createVertexShader(string source)
    {
        vertexShaderStack.Push(
            GL.CreateShader(
                OpenTK.Graphics.OpenGL4.ShaderType.VertexShader
            )
        );
        GL.ShaderSource(vertexShader, source);
        GL.CompileShader(vertexShader);
    }
    
    private void createFragmentShader(string source)
    {
        fragmentShaderStack.Push(
            GL.CreateShader(
                OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader
            )
        );
        GL.ShaderSource(fragmentShader, source);
        GL.CompileShader(fragmentShader);
    }

    private void createProgram()
    {
        this.programList.Add(
            GL.CreateProgram()
        );

        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        
        GL.LinkProgram(program);

        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);

        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);

        vertexShaderStack.Pop();
        fragmentShaderStack.Pop();
    }
    
    private List<ShaderDependence> getDependences(IEnumerable<ShaderObject> objs)
    {
        var dependences = new List<ShaderDependence>();
        foreach (var obj in objs)
        {
            foreach (var dependence in obj.Dependecies)
            {
                if (dependences.Contains(dependence))
                    continue;
                
                dependences.Add(dependence);
            }
        }
        return dependences;
    }

    // TODO: Refactor
    private (string, Action) generateVertexShader(Vec3ShaderObject vertexObject)
    {
        var sb = new StringBuilder();
        sb.AppendLine("#version 330 core");
        Action setup = null;

        var dependencens = vertexObject.Dependecies.Distinct();
        foreach (var dependence in dependencens)
        {
            switch (dependence.DependenceType)
            {
                case ShaderDependenceType.Uniform:
                    sb.AppendLine(
                        $"uniform {getShaderType(dependence.Type)} {dependence.Name};"
                    );

                    setup += delegate
                    {
                        setUniform(dependence.Name, dependence.Value, dependence.Type);
                    };
                    break;
                
                case ShaderDependenceType.CustomData:
                    sb.AppendLine(
                        
                    );

                    setup += delegate
                    {
                        setUniform(dependence.Name, dependence.Value, dependence.Type);
                    };
                    break;
            }
        }

        return (sb.ToString(), setup);
    }

    private string getShaderType(ShaderType type)
        => type switch
        {
            ShaderType.Float => "float",
            ShaderType.Vec4 => "vec4",
            ShaderType.Vec3 => "vec3",
            ShaderType.Vec2 => "vec2",
            _ => "unknow"
        };
    
    private void setUniform(string name, object value, ShaderType type)
    {
        switch (type)
        {
            case ShaderType.Float:
                setUniformFloat(name, (float)value);
                break;
        }
    }

    private void setUniformFloat(string name, float value)
    {
        var colorCode = GL.GetUniformLocation(activatedProgram, name);
        GL.Uniform1(colorCode, value);
    }

    // private static int bufferObject = int.MinValue;
    
    // private int program = 0;
    // private int vertexObject = 0;
    // private bool disposed = false;


    // private int[] layoutInfo;

    // static RenderOperations()
    // { 
    //     bufferObject = GL.GenBuffer();
    //     GL.BindBuffer(
    //         BufferTarget.ArrayBuffer, 
    //         bufferObject
    //     );
    // }


    // private void load()
    // {
    //     vertexObject = GL.GenVertexArray();
    //     GL.BindVertexArray(vertexObject);

    //     int stride = layoutInfo.Sum();
    //     int offset = 0;
    //     for (int i = 0; i < layoutInfo.Length; i++)
    //     {
    //         GL.VertexAttribPointer(i,
    //             layoutInfo[i],
    //             VertexAttribPointerType.Float, 
    //             false, 
    //             stride * sizeof(float),
    //             offset * sizeof(float)
    //         );
    //         GL.EnableVertexAttribArray(i);

    //         offset += layoutInfo[i];
    //     }
    // }

    // private void unload()
    // { 
    //     GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    //     GL.BindVertexArray(0);

    //     GL.DeleteBuffer(bufferObject);
    //     GL.DeleteVertexArray(vertexObject);
    // }

    // public void FillPolygon(double value, params Vertex[] pts)
    // {
    //     SetUniform(0, (float)value);
    //     FillPolygon(pts);
    // }
    
    // public void DrawPolygon(double value, params Vertex[] pts)
    // {
    //     SetUniform(0, (float)value);
    //     DrawPolygon(pts);
    // }

    // public void FillPolygon(float value, params Vertex[] pts)
    // {
    //     SetUniform(0, value);
    //     FillPolygon(pts);
    // }
    
    // public void DrawPolygon(float value, params Vertex[] pts)
    // {
    //     SetUniform(0, value);
    //     DrawPolygon(pts);
    // }
    
    // public void FillPolygon(Color color, params Vertex[] pts)
    // {   
    //     SetUniform(0, color);
    //     FillPolygon(pts);
    // }
    
    // public void DrawPolygon(Color color, params Vertex[] pts)
    // {
    //     SetUniform(0, color); 
    //     DrawPolygon(pts);
    // }

    // public void FillPolygon(params Vertex[] pts)
    // {
    //     GL.UseProgram(program);

    //     float[] vertices = toArr(pts, true);
        
    //     GL.BufferData(
    //         BufferTarget.ArrayBuffer,
    //         vertices.Length * sizeof(float), 
    //         vertices, 
    //         BufferUsageHint.DynamicDraw
    //     );

    //     GL.BindVertexArray(vertexObject);
    //     GL.DrawArrays(PrimitiveType.TriangleStrip, 0, pts.Length + 1);
    // }
    
    // public void DrawPolygon(params Vertex[] pts)
    // {
    //     GL.UseProgram(program);

    //     float[] vertices = toArr(pts, false);
        
    //     GL.BufferData(
    //         BufferTarget.ArrayBuffer,
    //         vertices.Length * sizeof(float), 
    //         vertices, 
    //         BufferUsageHint.DynamicDraw
    //     );

    //     GL.BindVertexArray(vertexObject);
    //     GL.DrawArrays(PrimitiveType.LineLoop, 0, pts.Length);
    // }

    // public void FillPolygon(params ColoredVertex[] pts)
    // {
    //     GL.UseProgram(program);

    //     float[] vertices = toArr(pts, true);
        
    //     GL.BufferData(
    //         BufferTarget.ArrayBuffer,
    //         vertices.Length * sizeof(float), 
    //         vertices, 
    //         BufferUsageHint.DynamicDraw
    //     );

    //     GL.BindVertexArray(vertexObject);
    //     GL.DrawArrays(PrimitiveType.TriangleFan, 0, pts.Length + 1);
    // }
    
    // public void DrawPolygon(params ColoredVertex[] pts)
    // {
    //     GL.UseProgram(program);

    //     float[] vertices = toArr(pts, false);
        
    //     GL.BufferData(
    //         BufferTarget.ArrayBuffer,
    //         vertices.Length * sizeof(float), 
    //         vertices, 
    //         BufferUsageHint.DynamicDraw
    //     );

    //     GL.BindVertexArray(vertexObject);
    //     GL.DrawArrays(PrimitiveType.LineLoop, 0, pts.Length);
    // }

    // private float[] toArr(ColoredVertex[] pts, bool loop)
    // {
    //     if (pts is null)
    //         return new float[0];

    //     int size = 7 * pts.Length + (loop ? 7 : 0);
    //     float[] vertices = new float[size];

    //     int offset = 0;
    //     for (int i = 0; i < pts.Length; i++, offset += 7)
    //     {
    //         var pt = pts[i];
    //         transformBasedOnWindowSize(pt, vertices, offset);

    //         var color = pt.Color;
    //         transformColor(color, vertices, offset + 3);
    //     }
        
    //     if (!loop)
    //         return vertices;
        
    //     var first = pts[0];
    //     transformBasedOnWindowSize(first, vertices, offset);

    //     var firstColor = first.Color;
    //     transformColor(firstColor, vertices, offset + 3);

    //     return vertices;
    // }

    // private float[] toArr(Vertex[] pts, bool loop)
    // {
    //     if (pts is null)
    //         return new float[0];

    //     int size = 3 * pts.Length + (loop ? 3 : 0);
    //     float[] vertices = new float[size];

    //     int offset = 0;
    //     for (int i = 0; i < pts.Length; i++, offset += 3)
    //     {
    //         var pt = pts[i];
    //         transformBasedOnWindowSize(pt, vertices, offset);
    //     }
        
    //     if (!loop)
    //         return vertices;

        
    //     var first = pts[0];
    //     transformBasedOnWindowSize(first, vertices, offset);

    //     return vertices;
    // }

    // private void transformColor(Color color, float[] data, int offset)
    // {
    //     data[offset + 0] = color.R / 255f;
    //     data[offset + 1] = color.G / 255f;
    //     data[offset + 2] = color.B / 255f;
    //     data[offset + 3] = color.A / 255f;
    // }

    // private void transformBasedOnWindowSize(Vertex pt, float[] data, int offset)
    // {
    //     data[offset + 0] = 2 * (pt.x / width) - 1;
    //     data[offset + 1] = 1 - 2 * (pt.y / height);
    //     data[offset + 2] = pt.z;
    // }
}