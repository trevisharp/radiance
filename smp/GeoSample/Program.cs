using Radiance;
using static Radiance.Utils;

var poly = Empty
    .Add(700, 200, 0)
    .Add(710, 210, 0)
    .Add(720, 230, 0)
    .Add(730, 300, 0)
    .Add(740, 220, 0)
    .Add(750, 230, 0)
    .Add(760, 270, 0)
    .Add(770, 200, 0)
    .Add(780, 300, 0)
    .Add(790, 220, 0)
    .Add(800, 250, 0)
    .Add(810, 260, 0)
    .Add(820, 230, 0)
    .Add(830, 280, 0)
    .Add(840, 350, 0)
    .Add(850, 280, 0)
    .Add(860, 300, 0)
    .Add(870, 290, 0)
    .Add(880, 200, 0)
    .Add(890, 400, 0)
    .Add(900, 150, 0);

Window.OnRender += () =>
    Kit.Fill(poly, red);

Window.CloseOn(Input.Escape);
Window.Open();