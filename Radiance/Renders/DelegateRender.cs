/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
using System;

namespace Radiance.Renders;

public class DelegateRender(Delegate function)
    : BaseRender(
        InitArguments(function),
        ArgumentHandlerChain.Create("delegate-render", 
            chain => chain.Add(null)
        )
    )
{    
    private static object[] InitArguments(Delegate function)
    {
        int expectedArguments = 0;
        foreach (var arg in function.Method.GetParameters())
        {
            
        }

        return null;
    }
}