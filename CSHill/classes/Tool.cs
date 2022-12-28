using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Tool : EventEmitter
{
    public string Name;
    public int _SlotId;

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