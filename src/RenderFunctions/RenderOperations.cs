/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
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
    public bool Verbose { get; set; } = false;

    public RenderOperations()
    {
        effects += delegate
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        };
    }

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
        };
    }

    public void Fill(
        Func<Vec3ShaderObject, Vec3ShaderObject> vertexShader,
        Func<Vec4ShaderObject> fragmentShader,
        Data data
    )
    {
        var gpuBuffer = createBuffer();

        var buffer = data.ToDependence;
        
        int vertexObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexObject);

        GL.VertexAttribPointer(0, 3,
            VertexAttribPointerType.Float, 
            false, 
            3 * sizeof(float),
            0
        );
        GL.EnableVertexAttribArray(0);

        int program = createProgram();

        var position = new Vec3ShaderObject(data.GetName, buffer);

        var finalVertexObject = vertexShader(position);
        var vertexTuple = generateVertexShader(finalVertexObject, gpuBuffer, program);

        if (Verbose)
            Console.WriteLine(vertexTuple.source);

        var finalFragmentObject = fragmentShader();
        var fragmentTuple = generateFragmentShader(finalFragmentObject, program);

        if (Verbose)
            Console.WriteLine(fragmentTuple.source);
        
        createVertexShader(vertexTuple.source);
        createFragmentShader(fragmentTuple.source);

        addShaders(program);
        
        effects += delegate
        {
            GL.UseProgram(program);
            GL.BindVertexArray(vertexObject);

            GL.BindBuffer(
                BufferTarget.ArrayBuffer, 
                gpuBuffer
            );

            if (vertexTuple.setup is not null)
                vertexTuple.setup();

            if (fragmentTuple.setup is not null)
                fragmentTuple.setup();

            GL.DrawArrays(
                PrimitiveType.Triangles,
                0, data.Elements
            );
        };
    }

    public void Draw(
        Func<Vec3ShaderObject, Vec3ShaderObject> vertexShader,
        Func<Vec4ShaderObject> fragmentShader,
        Data data
    )
    {
        var gpuBuffer = createBuffer();

        var buffer = data.ToDependence;
        
        int vertexObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexObject);

        GL.VertexAttribPointer(0, 3,
            VertexAttribPointerType.Float, 
            false, 
            3 * sizeof(float),
            0
        );
        GL.EnableVertexAttribArray(0);

        int program = createProgram();

        var position = new Vec3ShaderObject(data.GetName, buffer);

        var finalVertexObject = vertexShader(position);
        var vertexTuple = generateVertexShader(finalVertexObject, gpuBuffer, program);

        if (Verbose)
            Console.WriteLine(vertexTuple.source);

        var finalFragmentObject = fragmentShader();
        var fragmentTuple = generateFragmentShader(finalFragmentObject, program);

        if (Verbose)
            Console.WriteLine(fragmentTuple.source);
        
        createVertexShader(vertexTuple.source);
        createFragmentShader(fragmentTuple.source);

        addShaders(program);
        
        effects += delegate
        {
            GL.UseProgram(program);
            GL.BindVertexArray(vertexObject);

            GL.BindBuffer(
                BufferTarget.ArrayBuffer, 
                gpuBuffer
            );

            if (vertexTuple.setup is not null)
                vertexTuple.setup();

            if (fragmentTuple.setup is not null)
                fragmentTuple.setup();

            GL.DrawArrays(
                PrimitiveType.LineLoop,
                0, data.Elements
            );
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

    private List<int> bufferList = new();

    private List<int> programList = new();
    
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

        if (!Verbose)
            return;

        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetShaderInfoLog(vertexShader);
            throw new Exception($"Error occurred in Shader({vertexShader}) compilation: {infoLog}");
        }
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
        
        if (!Verbose)
            return;

        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetShaderInfoLog(vertexShader);
            throw new Exception($"Error occurred in Shader({vertexShader}) compilation: {infoLog}");
        }
    }

    private int createProgram()
    {
        var program = GL.CreateProgram();
        this.programList.Add(program);
        return program;
    }

    private void addShaders(int program)
    {
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

        if (!Verbose)
            return;

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            throw new Exception($"Error occurred Program({program}) linking.");
        }
    }
    
    private int createBuffer()
    {
        var bufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, bufferObject);
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
        int buffer,
        int program
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
                        setUniform(program, dependence.Name, dependence.Value, dependence.Type);
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
                            BufferUsageHint.StaticDraw
                        );
                    };
                    break;
            }
        }
        
        var exp = vertexObject.Expression;

        sb.AppendLine();
        sb.AppendLine("void main()");
        sb.AppendLine("{");
        sb.AppendLine($"vec3 finalPosition = {exp};");
        sb.AppendLine($"\tvec3 tposition = vec3(2 * finalPosition.x / width - 1, 2 * finalPosition.y / height - 1, finalPosition.z);");
        sb.AppendLine($"\tgl_Position = vec4(tposition, 1.0);");
        sb.AppendLine("}");

        return (sb.ToString(), setup);
    }

    
    private (string source, Action setup) generateFragmentShader(
        Vec4ShaderObject fragmentObject,
        int program
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
                        setUniform(program, dependence.Name, dependence.Value, dependence.Type);
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
    
    private void setUniform(int program, string name, object value, ShaderType type)
    {
        switch (type)
        {
            case ShaderType.Float:
                setUniformFloat(program, name, (float)value);
                break;
        }
    }

    private void setUniformFloat(int program, string name, float value)
    {
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform1(code, value);
    }
}