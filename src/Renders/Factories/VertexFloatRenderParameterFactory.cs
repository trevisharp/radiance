/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2024
 */
using System;

namespace Radiance.Renders.Factories;

public class VertexFloatRenderParameterFactory(Func<int, float> factory) : RenderParameterFactory
{
    public override bool NeedRegenerate => false;

    public override void GenerateData(int i, float[] buffer, int offset)
        => buffer[offset] = factory(i);
    
}