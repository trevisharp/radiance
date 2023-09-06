using System;

using Radiance;
using Radiance.Types;
using static Radiance.Window;
using static Radiance.RadianceUtils;

Player player = new Player();

Cursor cursor = new Cursor();

Ligth ligth1 = new Ligth();
Ligth ligth2 = new Ligth();
Ligth ligth3 = new Ligth();
Ligth ligth4 = new Ligth();
Ligth ligth5 = new Ligth();
Ligth ligth6 = new Ligth();

OnFrame += delegate
{
    player.TryCapture(
        ligth1, ligth2, ligth3, 
        ligth4, ligth5, ligth6
    );
    Console.WriteLine(Fps);
};

CloseOn(Input.Escape);

CursorVisible = false;

Open();

public class Ligth
{
    public float X => x.Value;
    public float Y => y.Value;

    gfloat x = -1;
    gfloat y = -1;
    gfloat captured = 1; 

    float nextx = -1;
    float nexty = -1;

    Player player = null;

    public Ligth()
    {
        render(r =>
        {
            r.FillTriangles(
                data(n, i, i + j, n, j, i + j)
                .transform(v => (50 * v.x + x, 50 * v.y + y, v.z))
                .colorize(v => 
                {
                    var d = distance(v, (0.5, 0.5, 0));
                    var s = 0.001 / (d * d);
                    return (s / captured, s, s / captured, s);
                })
            );
        }).Show();

        OnFrame += delegate
        {
            Move();
        };
    }

    public void Move()
    {
        if (x.Value == -1)
        {
            x = Random.Shared.NextSingle() * Window.Width;
            y = Random.Shared.NextSingle() * Window.Height;
        }

        if (nextx == -1 || nexty == -1)
        {
            nextx = player is null ?
                Random.Shared.NextSingle() * Window.Width :
                player.X + Random.Shared.NextSingle() * 50 - 25;
            nexty = player is null ?
                Random.Shared.NextSingle() * Window.Height :
                player.Y + Random.Shared.NextSingle() * 50 - 25;
        }

        var dx = nextx - x.Value;
        var dy = nexty - y.Value;
        var d = MathF.Sqrt(dx * dx + dy * dy);

        if (d < (player is not null ? 1 : 10))
        {
            nextx = nexty = -1;
            return;
        }

        var vx = dx / d + 4 * (Random.Shared.NextSingle() - .5f);
        var vy = dy / d + 4 * (Random.Shared.NextSingle() - .5f);

        x += 50 * vx * dt * (player is not null ? 6 : 1);
        y += 50 * vy * dt * (player is not null ? 6 : 1);
    }

    public void Capture(Player player)
    {
        nextx = nexty = -1;
        this.player = player;
        captured = 10;
    }
}

public class Cursor
{
    gfloat x = -1;
    gfloat y = -1;

    public Cursor()
    {
        render(r =>
        {
            r.FillTriangles(
                data(n, i, i + j, n, j, i + j)
                .transform(v => (50 * v.x + x, 50 * v.y + y, v.z))
                .colorize(v => 
                {
                    var d = distance(v, (0.5, 0.5, 0));
                    var s = 0.001 / (d * d);
                    return (s, 0, 0, s);
                })
            );
        }).Show();

        OnMouseMove += p => (x, y) = p;
    }
}

public class Player
{
    public float X => x.Value;
    public float Y => y.Value;

    gfloat x = 0;
    gfloat y = 0;

    float ox;
    float oy;

    public Player()
    {
        render(r => {
            r.Draw(
                rect(0, 0, 50, 50)
                .transform(v => (v.x + x, v.y + y, 0))
            );
        }).Show();

        OnMouseMove += p => (ox, oy) = p;

        OnFrame += delegate
        {
            var dx = ox - x.Value;
            var dy = oy - y.Value;
            var d = MathF.Sqrt(dx * dx + dy * dy);

            if (d < 2)
                return;

            var vx = dx / d;
            var vy = dy / d;

            x += 150 * vx * dt;
            y += 150 * vy * dt;
        };
    }

    public void TryCapture(params Ligth[] ligths)
    {
        foreach (var ligth in ligths)
            TryCapture(ligth);
    }

    public void TryCapture(Ligth ligth)
    {
        if (ligth.X > x.Value + 25 || ligth.Y > y.Value + 25 ||
            ligth.X < x.Value - 25 || ligth.Y < y.Value - 25)
            return;
        
        ligth.Capture(this);
    }

}