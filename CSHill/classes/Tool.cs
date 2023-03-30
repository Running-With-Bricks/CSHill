using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Tool : EventEmitter
{
    public static uint _NetId = 0;

    public string Name;
    public bool Enabled = true;
    public uint Model = 0;
    public uint _SlotId;

    public Tool(string name)
    {
        _NetId++;
        Name = name;
        _SlotId = _NetId;
    }

    public void Emit()
    {

    }

    public void Destroy()
    {
        foreach (var player in Game.Players)
        {
            if (player.ToolEquipped == this)
            {
                player.ToolEquipped = null;
                //Emit("unequipped", player);
            }

            player.DestroyTool(this);

            if (!Game.Tools.Contains(this)) return;
            Game.Tools.Remove(this);

            new PacketBuilder(11)
                .Bool(false)
                .u32((uint)_SlotId)
                .String(Name)
                .broadcast();
        }
    }
}