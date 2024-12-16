/* Author:  Leonardo Trevisan Silio
 * Date:    16/12/2024
 */
namespace Radiance.Renders.ChainLinks;

using Contexts;
using Exceptions;

public class SubCallArgumentHandlerChainLink : ArgumentHandlerChainLink
{
    public override bool CanHandle(ArgumentHandlerArgs args)
        => RenderContext.GetContext() is not null;

    public override bool CanUpdate(ArgumentHandlerArgs args)
        => false;

    public override object? Handle(ArgumentHandlerArgs args)
    {
        var parameters = function.Method.GetParameters();
        var args = SplitShaderObjectsBySide(input);
        args = Display(arguments, args, expectedArguments);
        args = RemoveSkip(args);
        
        if (parameters.Length != args.Length)
            throw new SubRenderArgumentCountException(parameters.Length, args.Length);

        function.DynamicInvoke(args);
        return null;
    }
}