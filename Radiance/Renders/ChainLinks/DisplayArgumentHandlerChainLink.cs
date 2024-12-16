/* Author:  Leonardo Trevisan Silio
 * Date:    16/12/2024
 */
namespace Radiance.Renders.ChainLinks;

public class DisplayArgumentHandlerChainLink : ArgumentHandlerChainLink
{
    public override bool CanHandle(ArgumentHandlerArgs args)
        => false;

    public override bool CanUpdate(ArgumentHandlerArgs args)
        => true;

    public override ArgumentHandlerArgs Update(ArgumentHandlerArgs args)
    {
        return new ArgumentHandlerArgs {
            Args = BaseRender.DisplayArguments(args.Args, args.NewArgs),
            NewArgs = [],
            Render = args.Render
        };
    }
}