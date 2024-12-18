/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
using System;
using System.Linq;

namespace Radiance.Renders;

using ChainLinks;

public class DelegateRender(Delegate function) : BaseRender(
    RenderUtils.GenerateArrayByTypes(from p in function.Method.GetParameters() select p.GetType()),
    ArgumentHandlerChain.Create("delegate-render", chain => chain
        .Add<SubCallArgumentHandlerChainLink>()
        .Add<DisplayArgumentHandlerChainLink>()
        // Make a subcall if is possible
        // Display arguments
        // Stop if has no polygon parameter
        // Error on missing polygon
        // Error on excess of parameters
        // Error on Curry on execution phase
        // Execute
        // Curry
    )
);