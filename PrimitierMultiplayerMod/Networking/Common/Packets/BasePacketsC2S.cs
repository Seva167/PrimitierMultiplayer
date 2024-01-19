using PrimitierMultiplayerMod.Networking.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitierMultiplayerMod.Networking.Common.Packets
{
    public class PMPJoinRequestC2SPacket
    {
        public string Nickname { get; set; }
    }

    public class PMPPlayerUpdateStateC2SPacket
    {
        public PlayerState State { get; set; }
    }
}
