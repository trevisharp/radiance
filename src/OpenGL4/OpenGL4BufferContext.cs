/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
using OpenTK.Graphics.OpenGL4;

namespace Radiance.OpenGL4;

using Contexts;

public class OpenGL4BufferContext : BufferContext
{
    public override void Bind(int id)
        => GL.BindBuffer(BufferTarget.ArrayBuffer, id);

    public override int Create()
        => GL.GenBuffer();

    public override void Delete(int id)
        => GL.DeleteBuffer(id);

    public override void Store(float[] data, bool dynamicData)
        => GL.BufferData(
            BufferTarget.ArrayBuffer, data.Length * sizeof(float), data,
            dynamicData ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw
        );
}