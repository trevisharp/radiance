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

/// <summary>
/// Provide render operations to draw data in screen.
/// </summary>
public class RenderOperations
{
    public bool Verbose { get; set; } = false;
    public string VersionText { get; set; } = "330 core";

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

    public void FillTriangles<D>(
        Data<D, Vec3ShaderObject> data
    ) 
        where D : ShaderDependence<Vec3ShaderObject>
        => baseDraw(PrimitiveType.Triangles, data);
        
    public void Draw<D>(
        Data<D, Vec3ShaderObject> data
    ) 
        where D : ShaderDependence<Vec3ShaderObject>
        => baseDraw(PrimitiveType.LineLoop, data);

    private void baseDraw<D>(
        PrimitiveType type,
        Data<D, Vec3ShaderObject> data
    ) 
        where D : ShaderDependence<Vec3ShaderObject>
    {
        var gpuBuffer = createBuffer();
        int vertexArray = createVertexArray();
        int program = createProgram();

        var vertexTuple = generateVertexShader(data.ToObject, program);
        if (Verbose)
            Console.WriteLine(vertexTuple.source);

        var fragmentTuple = generateFragmentShader(data.DataColor, program);
        if (Verbose)
            Console.WriteLine(fragmentTuple.source);

        addShaders(
            program,
            createVertexShader(vertexTuple.source),
            createFragmentShader(fragmentTuple.source)
        );
        
        effects += delegate
        {
            GL.UseProgram(program);
            GL.BindVertexArray(vertexArray);

            GL.BindBuffer(
                BufferTarget.ArrayBuffer, 
                gpuBuffer
            );

            if (vertexTuple.setup is not null)
                vertexTuple.setup();

            if (fragmentTuple.setup is not null)
                fragmentTuple.setup();

            GL.DrawArrays(
                type,
                0, data.Elements
            );
        };

        unloadEffects += delegate
        {
            GL.DeleteVertexArray(vertexArray);
        };
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

        foreach (var shaderKey in shaderMap)
            GL.DeleteShader(shaderKey.Value);
        shaderMap.Clear();

        unloadEffects();
    }

    private event Action<object[]> effects;
    private event Action unloadEffects;

    private List<int> bufferList = new();

    private List<int> programList = new();

    private Dictionary<int, int> shaderMap = new();

    private int createVertexShader(string source)
    {
        return createShader(
            OpenTK.Graphics.OpenGL4.ShaderType.VertexShader,
            source
        );
    }
    
    private int createFragmentShader(string source)
    {
        return createShader(
            OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader,
            source
        );
    }

    private int createShader(OpenTK.Graphics.OpenGL4.ShaderType type, string source)
    {
        var hash = getHash(source);
        if (shaderMap.ContainsKey(hash))
            return shaderMap[hash];

        var shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        shaderMap.Add(hash, shader);

        if (!Verbose)
            return shader;

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error occurred in Shader({shader}) compilation: {infoLog}");
        }

        return shader;
    }

    private int createProgram()
    {
        var program = GL.CreateProgram();
        this.programList.Add(program);
        return program;
    }

    private int getHash(string str)
    {
        int hash = 0;
        var bytes =  Encoding.UTF8.GetBytes(str);

        for (int i = 0; i < bytes.Length - 1; i++)
            hash += bytes[i] * bytes[i + 1];

        return hash;
    }

    private void addShaders(int program, int vertexShader, int fragmentShader)
    {        
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        
        GL.LinkProgram(program);

        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);

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
        Vec3ShaderObject vertexObject, int program)
    {
        var sb = getCodeBuilder();
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
                        setUniform(program, dependence);
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
        sb.AppendLine($"\tvec3 finalPosition = {exp};");
        sb.AppendLine($"\tvec3 tposition = vec3(2 * finalPosition.x / width - 1, 2 * finalPosition.y / height - 1, finalPosition.z);");
        sb.AppendLine($"\tgl_Position = vec4(tposition, 1.0);");
        sb.AppendLine("}");

        return (sb.ToString(), setup);
    }

    
    private (string source, Action setup) generateFragmentShader(
        Vec4ShaderObject fragmentObject, int program)
    {
        var sb = getCodeBuilder();
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
                        setUniform(program, dependence);
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

    private StringBuilder getCodeBuilder()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"#version {VersionText}");
        return sb;
    }
    
    private void setUniform(int program, ShaderDependence dependence)
    {
        switch (dependence)
        {
            case ShaderDependence<FloatShaderObject>:
                setUniformFloat(program, dependence.Name, (float)dependence.Value);
                break;
        }
    }

    private void setUniformFloat(int program, string name, float value)
    {
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform1(code, value);
    }

    private int createVertexArray()
    {
        int vertexObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexObject);

        GL.VertexAttribPointer(0, 3,
            VertexAttribPointerType.Float, 
            false, 
            3 * sizeof(float),
            0
        );
        GL.EnableVertexAttribArray(0);

        return vertexObject;
    }
}