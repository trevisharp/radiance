﻿using Radiance;
using static Radiance.RadianceUtils;

var myRender = render(r =>
{
    var img1 = open("faustao1.png");
    var img2 = open("faustao2.png");

    /**
        clear(white);
        x = (300 * x + width / 2);
        y = (300 * y + height / 2);
        var point = (300 * x / width + 0.5f, 300 * y / height + 0.5f);
        color = mix(texture(img1, point), texture(img2, point), (sin(t) + 1) / 2);
        fill();
     **/

    r.Clear(white);
    r.FillTriangles(circle
        .triangules()
        .transform(v => (300 * v.x + width / 2, 300 * v.y + height / 2, v.z))
        .colorize(v => 
            mix(
                texture(img1, (300 * v.x / width + 0.5f, 300 * v.y / height + 0.5f)),
                texture(img2, (300 * v.x / width + 0.5f, 300 * v.y / height + 0.5f)),
                (sin(t) + 1) / 2
            )
        )
    );
});

Window.OnRender += myRender;

Window.CloseOn(Input.Escape);

Window.Open();