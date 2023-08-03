using System;
using System.Linq.Expressions;

namespace DuckGL;

public class GraphicsBuilder
{
    private string vertexShader = string.Empty;
    private string fragmentShader = string.Empty;
    private int width = -1;
    private int height = -1;

    public GraphicsBuilder SetWidth(int value)
    {
        this.width = value;
        return this;
    }

    public GraphicsBuilder SetHeight(int value)
    {
        this.height = value;
        return this;
    }

    public GraphicsBuilder SetVertexShader(string value)
    {
        this.vertexShader = value;
        return this;
    }

    public GraphicsBuilder SetFragmentShader(string value)
    {
        this.fragmentShader = value;
        return this;
    }

    public GraphicsBuilder SetVertexShader(Action<ShaderContext> exp)
    {
        throw new NotImplementedException();
    }

    public GraphicsBuilder SetFragmentShader(Action<ShaderContext> exp)
    {
        throw new NotImplementedException();
    }
}