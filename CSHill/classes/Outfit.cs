using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Outfit
{
    public Colors Colors = new();
    public Assets Assets = new();

    public Outfit hat1(uint id)
    {
        Assets.hat1 = id;
        return this;
    }
    public Outfit hat2(uint id)
    {
        Assets.hat2 = id;
        return this;
    }
    public Outfit hat3(uint id)
    {
        Assets.hat3 = id;
        return this;
    }

    public Outfit face(uint id)
    {
        Assets.face = id;
        return this;
    }

    public Outfit shirt(uint id)
    {
        Assets.shirt = id;
        return this;
    }

    public Outfit pants(uint id)
    {
        Assets.pants = id;
        return this;
    }

    public Outfit tshirt(uint id)
    {
        Assets.tshirt = id;
        return this;
    }

    public Outfit body(Color color)
    {
        Colors.Head = color;
        Colors.Torso = color;
        Colors.RightArm = color;
        Colors.LeftArm = color;
        Colors.RightLeg = color;
        Colors.LeftLeg = color;
        return this;
    }

    public Outfit head(Color color)
    {
        Colors.Head = color;
        return this;
    }

    public Outfit torso(Color color)
    {
        Colors.Torso = color;
        return this;
    }

    public Outfit rightArm(Color color)
    {
        Colors.RightArm = color;
        return this;
    }

    public Outfit leftArm(Color color)
    {
        Colors.LeftArm = color;
        return this;
    }

    public Outfit rightLeg(Color color)
    {
        Colors.RightLeg = color;
        return this;
    }

    public Outfit leftLeg(Color color)
    {
        Colors.LeftLeg = color;
        return this;
    }
}

public class Assets
{
    public uint face;
    public uint hat1;
    public uint hat2;
    public uint hat3;
    public uint shirt;
    public uint tshirt;
    public uint pants;
}
