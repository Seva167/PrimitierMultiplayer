using PrimitierMultiplayerMod.Networking.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitierMultiplayerMod.Networking.Common.Packets
{
    public class PMPJoinAcceptS2CPacket
    {
        public int PLID { get; set; }
        public SaveHeader SaveHeader { get; set; }
    }

    public class PMPPlayerJoinedS2CPacket
    {
        public int PLID { get; set; }
        public string Nickname { get; set; }
    }

    public class PMPPlayerLeftS2CPacket
    {
        public int PLID { get; set; }
    }

    public class PMPPlayerUpdateStateS2CPacket
    {
        public int PLID { get; set; }
        public PlayerState State { get; set; }
    }
}
