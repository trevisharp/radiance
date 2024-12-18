/* Author:  Leonardo Trevisan Silio
 * Date:    17/12/2024
 */
namespace Radiance.Renders.ChainLinks;

public class DisplayArgumentHandlerChainLink : ArgumentHandlerChainLink
{
    public override bool CanHandle(ArgumentHandlerArgs args)
        => false;

    public override bool CanUpdate(ArgumentHandlerArgs args)
        => true;

    public override ArgumentHandlerArgs Update(ArgumentHandlerArgs args)
        => new ArgumentHandlerArgs {
            Args = RenderUtils.DisplayArguments(args.Args, args.NewArgs),
            NewArgs = [],
            Render = args.Render
        };
}