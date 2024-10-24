/* Author:  Leonardo Trevisan Silio
 * Date:    24/10/2024
 */
using OpenTK.Graphics.OpenGL4;

namespace Radiance.OpenGL4;

using Contexts;

public class OpenGL4BufferContext : IBufferContext
{
    public void Bind(int id)
        => GL.BindBuffer(BufferTarget.ArrayBuffer, id);

    public int Create()
        => GL.GenBuffer();

    public void Delete(int id)
        => GL.DeleteBuffer(id);

    public void Store(float[] data, bool dynamicData)
        => GL.BufferData(
            BufferTarget.ArrayBuffer, data.Length * sizeof(float), data,
            dynamicData ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw
        );
}