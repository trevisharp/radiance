using System;
using System.Text;
using System.Linq.Expressions;

namespace Duck.Internal;

internal class ShaderConverter
{
    internal string ToVertex(Expression<Action> exp)
    {
        var sb = new StringBuilder();

        sb.AppendLine("#version 330 core");

        

        return sb.ToString();
    }    
}