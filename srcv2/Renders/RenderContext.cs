/* Author:  Leonardo Trevisan Silio
 * Date:    27/08/2024
 */
using System;
using System.Text;
using System.Linq;

using OpenTK.Graphics.OpenGL4;

namespace Radiance.Renders;

using Data;
using Shaders;
using Shaders.Objects;

/// <summary>
/// A context used to shader construction..
/// </summary>
public class RenderContext
{
    private event Action<Polygon, object[]> DrawOperations;

    public bool IsVerbose { get; set; } = false;
    public string VersionText { get; set; } = "330 core";
    public Vec3ShaderObject Position { get; set; }
    public Vec4ShaderObject Color { get; set; }
    
    public void Render(Polygon polygon, object[] parameters)
    {
        if (DrawOperations is null)
            return;
        
        DrawOperations(polygon, parameters);
    }

    public void AddClear(Vec4 color)
    {
        DrawOperations += delegate
        {
            GL.ClearColor(
                color.X,
                color.Y,
                color.Z,
                color.W
            );
        };
    }

    public void AddPoints() 
        => AddDrawOperation(PrimitiveType.Points);

    public void AddLines() 
        => AddDrawOperation(PrimitiveType.Lines);
    
    public void AddDraw() 
        => AddDrawOperation(PrimitiveType.LineLoop);
    
    public void AddFill()
        => AddDrawOperation(PrimitiveType.Triangles, true);
    
    public void AddTriangules() 
        => AddDrawOperation(PrimitiveType.Triangles);
    
    public void AddStrip() 
        => AddDrawOperation(PrimitiveType.TriangleStrip);
    
    public void AddFan() 
        => AddDrawOperation(PrimitiveType.TriangleFan);

    private void AddDrawOperation(
        PrimitiveType primitive, 
        bool needTriangularization = false
    )
    {
        var shaderCtx = new ShaderContext();
        var (vertSource, vertSetup, fragSoruce, fragSetup) = 
            GenerateShaders(Position, Color, shaderCtx);
        
        var program = RenderProgram.CreateProgram(
            vertSource, fragSoruce, IsVerbose
        );
        shaderCtx.Program = program;
        
        DrawOperations += (poly, data) =>
        {
            if (needTriangularization)
                poly = poly.Triangulation;

            shaderCtx.CreateResources(poly);
            GL.UseProgram(program);

            shaderCtx.Use(poly);

            if (vertSetup is not null)
                vertSetup();

            if (fragSetup is not null)
                fragSetup();

            GL.DrawArrays(primitive, 0, poly.Data.Count() / 3);
        };
    }
}