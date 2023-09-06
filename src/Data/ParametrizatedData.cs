/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
 */
using System.Collections.Generic;

namespace Radiance.Data;

using Radiance.ShaderSupport;
using Radiance.ShaderSupport.Objects;
using ShaderSupport;
using ShaderSupport.Dependencies;
using ShaderSupport.Objects;

public class ParametrizatedData<D> : BaseData<D>
    where D : ShaderObject, new()
{
    public override Vec3ShaderObject VertexObject => throw new System.NotImplementedException();

    public override Vec4ShaderObject FragmentObject => throw new System.NotImplementedException();

    public override IEnumerable<ShaderOutput> Outputs => throw new System.NotImplementedException();

    public override int Size => throw new System.NotImplementedException();

    public override int Elements => throw new System.NotImplementedException();

    public override IEnumerable<int> Sizes => throw new System.NotImplementedException();

    public override void SetData(float[] arr, ref int indexoff)
    {
        throw new System.NotImplementedException();
    }
}