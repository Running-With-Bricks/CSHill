using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
public class Color
{
    public uint r;
    public uint g;
    public uint b;

    public static string formatHex(string input)
    {
        if (input == null) return "";
        Regex COLOR_REGEX = new("\\[#[a-fA-F0-9]{6}\\]");
        var match = COLOR_REGEX.Match(input);
        //Console.WriteLine(match);
        //if (!match)
        //    return input;
        //match.forEach((colorCode) => {
        //let hexCol = colorCode.replace(/[[#\]]/g, "").toUpperCase();
        //hexCol = colorModule_1.default.rgbToBgr(hexCol);
        //input = input.replace(colorCode, `< color:${ hexCol}>`);

        return input;
    }
    public Color(uint r = 0, uint g = 0, uint b = 0)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }
    public Color(double r, double g, double b)
    {
        this.r = (uint)(r * 255);
        this.g = (uint)(g * 255);
        this.b = (uint)(b * 255);
    }
    public Color(string hex)
    {
        if (hex[0] == '#')
        {
            hex = hex.Remove(0, 1);
        }
        uint bigint = Convert.ToUInt32(hex, 16);
        r = (bigint >> 16) & 255;
        g = (bigint >> 8) & 255;
        b = bigint & 255;
    }

    public uint dec()
    {
        uint rgb = r | (g << 8) | (b << 16);
        return rgb;
    }
    public string hex()
    {
        return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }
    public string gmlhex()
    {
        return "<color:" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + ">";
    }
    public (uint, uint, uint) rgb()
    {
        return (r, g, b);
    }
}

public class Colors
{
    public Color Head = new();
    public Color Torso = new();
    public Color LeftArm = new();
    public Color RightArm = new();
    public Color LeftLeg = new();
    public Color RightLeg = new();
}