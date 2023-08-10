/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2023
 */
namespace Radiance.DataLayouts;

using Core;

/// <summary>
/// /// Represents a tridimensional point with axes x, y and z and a color.
/// </summary>
// public record ColoredVertex : IDataLayout
// {
//     public Color Color { get; set; }

//     public ColoredVertex(float x, float y, float z, Color color)
//         : base(x, y, z) => this.Color = color;
    
//     public static implicit operator ColoredVertex((float x, float y, float z, byte r, byte g, byte b) tuple)
//         => new (tuple.x, tuple.y, tuple.z, new (255, tuple.r, tuple.b, tuple.b));
    
//     public static implicit operator ColoredVertex((float x, float y, float z, byte a, byte r, byte g, byte b) tuple)
//         => new (tuple.x, tuple.y, tuple.z, new (tuple.a, tuple.r, tuple.b, tuple.b));
    
//     public static implicit operator ColoredVertex((float x, float y, float z, Color c) tuple)
//         => new (tuple.x, tuple.y, tuple.z, tuple.c);
        
//     public static implicit operator ColoredVertex((int x, int y, int z, byte r, byte g, byte b) tuple)
//         => new (tuple.x, tuple.y, tuple.z, new (255, tuple.r, tuple.b, tuple.b));
    
//     public static implicit operator ColoredVertex((int x, int y, int z, byte a, byte r, byte g, byte b) tuple)
//         => new (tuple.x, tuple.y, tuple.z, new (tuple.a, tuple.r, tuple.b, tuple.b));
    
//     public static implicit operator ColoredVertex((int x, int y, int z, Color c) tuple)
//         => new (tuple.x, tuple.y, tuple.z, tuple.c);
// }