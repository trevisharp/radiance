using System;
using System.Linq;

using Radiance;
using Radiance.Types;
using Radiance.RenderFunctions;
using static Radiance.RadianceUtils;

var start = ZeroTime;
float cx = 0, cy = 0;
var data = ellip(0, 0, 1);

IRender getWave(float x, float y)
{
    gfloat initialTime = (float)(DateTime.Now - start).TotalSeconds;
    gfloat gx = x;
    gfloat gy = y;

    return render(r => {
        var size = 50 * (t - initialTime);
        r.Draw(data
            .transform(v => (size * v.x + gx, size * v.y + gy, v.z))
            .colorize(white)
        );
    });
}

Window.OnMouseDown += b =>
{
    if (b != MouseButton.Left)
        return;
    
    Window.OnRender += getWave(cx, cy);
};

Window.OnMouseMove += e => (cx, cy) = e;

Window.CloseOn(Input.Escape);

Window.Open();