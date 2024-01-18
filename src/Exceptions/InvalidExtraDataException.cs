using System;

namespace Radiance.Exceptions;

public class InvalidExtraDataException : Exception
{
    public override string Message =>
    """
        An add function call with extra data parameter should be concise to layout appended:
        
        float x = 9, y = 8, z = 7;
        var poly = new Polygon();
        poly.Append(1);
        // ...
        poly.Add(x, y, z, 1, 2, 3); // InvalidExtraDataException

        Try update layout:
        
        // ...
        poly.Append(2);
        poly.Add(x, y, z, 1, 2, 3); // Ok

        Or reduce data in update:
        
        // ...
        poly.Add(x, y, z, 1); // Ok
    """;
}