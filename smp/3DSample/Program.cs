using Radiance;
using Radiance.Shaders.Objects;
using Radiance.Data;
using Radiance.Renders;
using static Radiance.Utils;

dynamic myRender = new Render((
    FloatShaderObject m00, FloatShaderObject m01, FloatShaderObject m02,
    FloatShaderObject m10, FloatShaderObject m11, FloatShaderObject m12,
    FloatShaderObject m20, FloatShaderObject m21, FloatShaderObject m22,
    FloatShaderObject bx, FloatShaderObject by, FloatShaderObject bz,
    FloatShaderObject r,
    FloatShaderObject g, 
    FloatShaderObject b,
    FloatShaderObject a
    ) =>
    {
        var xl = pos.x * m00 + pos.y * m01 + pos.z * m02 + bx;
        var yl = pos.x * m10 + pos.y * m11 + pos.z * m12 + by;
        var zl = pos.x * m20 + pos.y * m21 + pos.z * m22 + bz;

        pos = (
            (xl - zl) / MathF.Sqrt(2),
            (xl + 2 * yl + zl) / MathF.Sqrt(6),
            0
        );
        color = (r, g, b, a);
        fill();
    });


var myPoly = new MutablePolygon
{
    { -25, +25, -25 },
    { +25, +25, -25 },
    { +25, +25, +25 },
    { -25, +25, +25 }
};

var r1 = 
    myRender.Curry(myPoly)
    .Curry(
        1, 0, 0,
        0, 1, 0,
        0, 0, 1
    )
    .Curry(
        300, 300, 0
    )
    .Curry(red);

var myPoly2 = new MutablePolygon
{
    { -25, +25, -25 },
    { -25, +25, +25 },
    { -25, -25, +25 },
    { -25, -25, -25 }
};

var r2 = 
    myRender.Curry(myPoly2)
    .Curry(
        1, 0, 0,
        0, 1, 0,
        0, 0, 1
    )
    .Curry(
        300, 300, 0
    )
    .Curry(blue);

var myPoly3 = new MutablePolygon
{
    { -25, +25, -25 },
    { -25, -25, -25 },
    { +25, -25, -25 },
    { +25, +25, -25 }
};

var r3 = 
    myRender.Curry(myPoly3)
    .Curry(
        1, 0, 0,
        0, 1, 0,
        0, 0, 1
    )
    .Curry(
        300, 300, 0
    )
    .Curry(green);

Window.OnRender += () => 
{
    r1();
    r2();
    r3();
};

Window.CloseOn(Input.Escape);
Window.Open();