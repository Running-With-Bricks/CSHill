using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Color
{
    public int r;
    public int g;
    public int b;
    public Color(int r, int g, int b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }
    public Color(double r, double g, double b)
    {
        this.r = (int)(r * 255);
        this.g = (int)(g * 255);
        this.b = (int)(b * 255);
    }

    public int getDec()
    {
        var rgb = r | (g << 8) | (b << 16);
        return rgb;
    }
}