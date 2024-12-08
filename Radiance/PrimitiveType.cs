/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2023
 */
namespace Radiance;

/// <summary>
/// Represents a input from hardware.
/// Equals to OpenTK.Graphics.OpenGL4.PrimitiveType.
/// </summary>
public enum PrimitiveType
{
    //
    // Summary:
    //     [requires: v1.0] Original was GL_POINTS = 0x0000
    Points = 0,
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LINES = 0x0001
    Lines = 1,
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LINE_LOOP = 0x0002
    LineLoop = 2,
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LINE_STRIP = 0x0003
    LineStrip = 3,
    //
    // Summary:
    //     [requires: v1.0 or ARB_tessellation_shader] Original was GL_TRIANGLES = 0x0004
    Triangles = 4,
    //
    // Summary:
    //     [requires: v1.0] Original was GL_TRIANGLE_STRIP = 0x0005
    TriangleStrip = 5,
    //
    // Summary:
    //     [requires: v1.0] Original was GL_TRIANGLE_FAN = 0x0006
    TriangleFan = 6,
    //
    // Summary:
    //     [requires: v4.0] Original was GL_QUADS = 0x0007
    Quads = 7,
    //
    // Summary:
    //     Original was GL_QUADS_EXT = 0x0007
    QuadsExt = 7,
    //
    // Summary:
    //     [requires: v3.2] Original was GL_LINES_ADJACENCY = 0x000A
    LinesAdjacency = 10,
    //
    // Summary:
    //     [requires: ARB_geometry_shader4] Original was GL_LINES_ADJACENCY_ARB = 0x000A
    LinesAdjacencyArb = 10,
    //
    // Summary:
    //     Original was GL_LINES_ADJACENCY_EXT = 0x000A
    LinesAdjacencyExt = 10,
    //
    // Summary:
    //     [requires: v3.2] Original was GL_LINE_STRIP_ADJACENCY = 0x000B
    LineStripAdjacency = 11,
    //
    // Summary:
    //     [requires: ARB_geometry_shader4] Original was GL_LINE_STRIP_ADJACENCY_ARB = 0x000B
    LineStripAdjacencyArb = 11,
    //
    // Summary:
    //     Original was GL_LINE_STRIP_ADJACENCY_EXT = 0x000B
    LineStripAdjacencyExt = 11,
    //
    // Summary:
    //     [requires: v3.2] Original was GL_TRIANGLES_ADJACENCY = 0x000C
    TrianglesAdjacency = 12,
    //
    // Summary:
    //     [requires: ARB_geometry_shader4] Original was GL_TRIANGLES_ADJACENCY_ARB = 0x000C
    TrianglesAdjacencyArb = 12,
    //
    // Summary:
    //     Original was GL_TRIANGLES_ADJACENCY_EXT = 0x000C
    TrianglesAdjacencyExt = 12,
    //
    // Summary:
    //     [requires: v3.2] Original was GL_TRIANGLE_STRIP_ADJACENCY = 0x000D
    TriangleStripAdjacency = 13,
    //
    // Summary:
    //     [requires: ARB_geometry_shader4] Original was GL_TRIANGLE_STRIP_ADJACENCY_ARB
    //     = 0x000D
    TriangleStripAdjacencyArb = 13,
    //
    // Summary:
    //     Original was GL_TRIANGLE_STRIP_ADJACENCY_EXT = 0x000D
    TriangleStripAdjacencyExt = 13,
    //
    // Summary:
    //     [requires: v4.0 or ARB_tessellation_shader, NV_gpu_shader5] Original was GL_PATCHES
    //     = 0x000E
    Patches = 14,
    //
    // Summary:
    //     Original was GL_PATCHES_EXT = 0x000E
    PatchesExt = 14
}