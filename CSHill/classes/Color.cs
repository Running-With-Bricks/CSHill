using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
public class Color
{
    public int r;
    public int g;
    public int b;

    public static string formatHex(string input)
    {
        Regex COLOR_REGEX = new("/(\\[#[0-9a-fA-F]{6}\\])/g");
        var match = COLOR_REGEX.Match(input);
        Console.WriteLine(match);
        //if (!match)
        //    return input;
        //match.forEach((colorCode) => {
        //let hexCol = colorCode.replace(/[[#\]]/g, "").toUpperCase();
        //hexCol = colorModule_1.default.rgbToBgr(hexCol);
        //input = input.replace(colorCode, `< color:${ hexCol}>`);
    
        return input;
    }
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
    public Color(string hex)
    {
        hex = hex.Remove(0, 1);
        var bigint = Convert.ToInt32(hex, 16);
        r = (bigint >> 16) & 255;
        g = (bigint >> 8) & 255;
        b = bigint & 255;
    }

    public int dec()
    {
        var rgb = r | (g << 8) | (b << 16);
        return rgb;
    }
    public string hex()
    {
        return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }
    public (int,int,int) rgb()
    {
        return (r,g,b);
    }
}