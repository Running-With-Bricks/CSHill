using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Brick : EventEmitter
{
    public static uint _NetId = 0;
    public uint NetId;

    public string Name;
    public Vector3 Position;
    public Vector3 Scale;
    public int Rotation;
    public Color Color;

    public string Shape;
    public uint Model;
    public bool Collision = true;
    public double Visibility;
    
    public bool Destroyed;

    public bool LightEnabled = false;
    public int LightRange;
    public Color LightColor;

    public bool Clickable;
    public uint ClickDistance;

    public string Socket;

    public Brick(Vector3 position, Vector3 scale, Color color)
    {
        _NetId++;
        Position = position;
        Scale = scale;
        Color = color;
        NetId = _NetId;
    }

    public void SetPosition(Vector3 position)
    {
        Position = position;
        new PacketBuilder(9)
            .u32(NetId)
            .String("pos")
            .Float(position.x)
            .Float(position.y)
            .Float(position.z)
            .broadcast();
    }

    public void SetScale(Vector3 scale)
    {
        Scale = scale;
        new PacketBuilder(9)
            .u32(NetId)
            .String("scale")
            .Float(scale.x)
            .Float(scale.y)
            .Float(scale.z)
            .broadcast();
    }

    public void SetRotation(int rotation)
    {
        Rotation = rotation;
        new PacketBuilder(9)
            .u32(NetId)
            .String("rot")
            .u32((uint)rotation)
            .broadcast();
    }

    public void SetModel(uint model)
    {
        Model = model;
        new PacketBuilder(9)
            .u32(NetId)
            .String("model")
            .u32((uint)model)
            .broadcast();
    }

    public void SetColor(Color color)
    {
        Color = color;
        new PacketBuilder(9)
            .u32(NetId)
            .String("col")
            .u32(color.dec())
            .broadcast();
    }

    public void SetLightColor(Color color)
    {
        if (!LightEnabled) throw new Exception("Brick.LightEnabled must be enabled first!");
        LightColor = color;
        new PacketBuilder(9)
            .u32(NetId)
            .String("lightcol")
            .u32(color.dec())
            .broadcast();
    }

    public void SetLightRange(int range)
    {
        if (!LightEnabled) throw new Exception("Brick.LightEnabled must be enabled first!");
        LightRange = range;
        new PacketBuilder(9)
            .u32(NetId)
            .String("lightrange")
            .u32((uint)range)
            .broadcast();
    }

    public void SetVisibility(int visibility)
    {
        Visibility = visibility;
        new PacketBuilder(9)
            .u32(NetId)
            .String("alpha")
            .u32((uint)visibility)
            .broadcast();
    }

    public void SetCollision(bool collision)
    {
        Collision = collision;
        new PacketBuilder(9)
            .u32(NetId)
            .String("collide")
            .Bool(collision)
            .broadcast();
    }

    public void SetClickable(bool clickable, uint clickDistance = 50)
    {
        Clickable = clickable;
        ClickDistance = clickDistance;
        new PacketBuilder(9)
            .u32(NetId)
            .String("clickable")
            .Bool(clickable)
            .u32(clickDistance)
            .broadcast();
    }

    public Brick Clone()
    {
        return new Brick(Position, Scale, Color){
            Name = Name,
            Rotation = Rotation,
            Shape = Shape,
            Model = Model,
            Collision = Collision,
            Visibility = Visibility,
            LightEnabled = LightEnabled,
            LightRange = LightRange,
            LightColor = LightColor,
            Clickable = Clickable,
            ClickDistance = ClickDistance
        };
    }

    public void _Cleanup()
    {
        //we will see if we need this
        return;
    }

    public void Destroy()
    {
        if (Destroyed) throw new Exception("Brick has already been destroyed.");
        _Cleanup();
        new PacketBuilder(9)
            .u32(NetId)
            .String("destroy")
            .broadcast();
        Destroyed = true;
        Game.Bricks.Remove(this);
    }

    private void _HitDetection()
    {

    }

    public void PointTowards(Vector3 point)
    {

    }


}