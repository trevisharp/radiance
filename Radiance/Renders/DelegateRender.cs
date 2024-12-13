/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
using System;
using System.Linq;

namespace Radiance.Renders;

using ChainLinks;

public class DelegateRender(Delegate function) : BaseRender(
    GenerateArrayByTypes(from p in function.Method.GetParameters() select p.GetType()),
    ArgumentHandlerChain.Create("delegate-render", chain => chain
        .Add<SubCallArgumentHandlerChainLink>()
    )
);