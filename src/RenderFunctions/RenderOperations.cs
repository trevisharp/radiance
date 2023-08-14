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

// TODO: Refactor

/// <summary>
/// Provide render operations to draw data in screen.
/// </summary>
public class RenderOperations
{
    public bool Verbose { get; set; } = true;

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
        Func<Vec4ShaderObject> fragmentShader,
        params Vector[] data
    )
    {
        if (data.Length == 0)
            return;
        
        var gpuBuffer = createBuffer();

        var bufferData = data.GetBuffer();
        var buffer = new Vec3BufferDependence(bufferData);

        var position = new Vec3ShaderObject(data[0].GetName, buffer);

        var finalVertexObject = vertexShader(position);
        var vertexTuple = generateVertexShader(finalVertexObject, gpuBuffer);

        if (Verbose)
            Console.WriteLine(vertexTuple.source);

        var finalFragmentObject = fragmentShader();
        var fragmentTuple = generateFragmentShader(finalFragmentObject);

        if (Verbose)
            Console.WriteLine(fragmentTuple.source);
        
        effects += delegate
        {
            GL.BindBuffer(
                BufferTarget.ArrayBuffer, 
                gpuBuffer
            );

            int vertexObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexObject);

            GL.VertexAttribPointer(0, 3,
                VertexAttribPointerType.Float, 
                false, 
                3 * sizeof(float),
                3 * sizeof(float)
            );
            GL.EnableVertexAttribArray(0);

            createVertexShader(vertexTuple.source);
            createFragmentShader(fragmentTuple.source);

            createProgram();
            GL.UseProgram(program);

            if (vertexTuple.setup is not null)
                vertexTuple.setup();

            if (fragmentTuple.setup is not null)
                fragmentTuple.setup();

            GL.DrawArrays(PrimitiveType.TriangleFan, 0, bufferData.Length);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        };
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

        foreach (var buffer in bufferList)
            GL.DeleteBuffer(buffer);
        bufferList.Clear();
    }

    private event Action<object[]> effects;

    private int activatedProgram = -1;

    private List<int> bufferList = new();

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
        
        if (vertexShaderStack.Count == 0)
            throw new Exception("The program may contains a Vertex Shader");
        vertexShaderStack.Pop();
        
        if (fragmentShaderStack.Count == 0)
            throw new Exception("The program may contains a Fragment Shader");
        fragmentShaderStack.Pop();
    }
    
    private int createBuffer()
    {
        var bufferObject = GL.GenBuffer();
        bufferList.Add(bufferObject);
        return bufferObject;
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

    private (string source, Action setup) generateVertexShader(
        Vec3ShaderObject vertexObject,
        int buffer
    )
    {
        var sb = new StringBuilder();
        sb.AppendLine("#version 330 core");
        Action setup = null;

        var dependencens = vertexObject.Dependecies
            .Append(RadianceUtils._width)
            .Append(RadianceUtils._height)
            .Distinct();
        foreach (var dependence in dependencens)
        {
            switch (dependence.DependenceType)
            {
                case ShaderDependenceType.Uniform:
                    sb.AppendLine(dependence.GetHeader());

                    setup += delegate
                    {
                        setUniform(dependence.Name, dependence.Value, dependence.Type);
                    };
                    break;
                
                case ShaderDependenceType.CustomData:
                    sb.AppendLine(dependence.GetHeader());
                    float[] data = (float[])dependence.Value;

                    setup += delegate
                    {        
                        GL.BufferData(
                            BufferTarget.ArrayBuffer,
                            data.Length * sizeof(float), 
                            data, 
                            BufferUsageHint.DynamicDraw
                        );
                    };
                    break;
            }
        }
        
        var exp = vertexObject.Expression;

        sb.AppendLine();
        sb.AppendLine("void main()");
        sb.AppendLine("{");
        if (exp.Contains("position"))
        {
            sb.AppendLine($"\tposition.x = 2 * position.x / width - 1;");
            sb.AppendLine($"\tposition.y = 2 * position.y / height - 1;");
        }
        sb.AppendLine($"\tgl_Position = vec4({exp}, 1.0);");
        sb.AppendLine("}");

        return (sb.ToString(), setup);
    }

    
    private (string source, Action setup) generateFragmentShader(
        Vec4ShaderObject fragmentObject
    )
    {
        var sb = new StringBuilder();
        sb.AppendLine("#version 330 core");
        Action setup = null;

        var dependencens = fragmentObject.Dependecies
            .Distinct();
        foreach (var dependence in dependencens)
        {
            switch (dependence.DependenceType)
            {
                case ShaderDependenceType.Uniform:
                    sb.AppendLine(dependence.GetHeader());

                    setup += delegate
                    {
                        setUniform(dependence.Name, dependence.Value, dependence.Type);
                    };
                    break;
            }
        }
        
        var exp = fragmentObject.Expression;

        sb.AppendLine();
        sb.AppendLine("out vec4 outColor;");
        sb.AppendLine("void main()");
        sb.AppendLine("{");
        sb.AppendLine($"\toutColor = {exp};");
        sb.AppendLine("}");

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
}