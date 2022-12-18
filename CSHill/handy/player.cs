using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scripts.player
{
    public class SendEnv
    {
        public SendEnv(string IpPort)
        {
            new PacketBuilder(7)
                .String("Ambient")
                .u32((uint)Game.Environment.ambient.dec())
                .send(IpPort);

            new PacketBuilder(7)
                .String("Sky")
                .u32((uint)Game.Environment.skyColor.dec())
                .send(IpPort);

            new PacketBuilder(7)
                 .String("BaseCol")
                 .u32((uint)Game.Environment.baseColor.dec())
                 .send(IpPort);

            new PacketBuilder(7)
                 .String("BaseSize")
                 .u32((uint)Game.Environment.baseSize)
                 .send(IpPort);

            new PacketBuilder(7)
                 .String("Sun")
                 .u32((uint)Game.Environment.sunIntensity)
                 .send(IpPort);

        }
    }
}
