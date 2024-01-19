/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
using System;
using System.Linq;
using System.Dynamic;

namespace Radiance.Renders;

public class Render : DynamicObject
{
    private Delegate function;

    private readonly int parameterCount;
    public int ParameterCount => parameterCount;

    public Render(Action function)
    {
        this.parameterCount = 1;
        this.function = function;
    }

    public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
    {
        this.function.DynamicInvoke(args[1..]);

        result = true;
        return true;
    }
}