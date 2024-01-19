using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitierMultiplayerMod.Networking.ClientSide
{
    internal static class ClientNetPlayerManager
    {
        public static ClientSidePlayer localPlayer = new();
        public static Dictionary<int, ClientSidePlayer> remotePlayers = new();
    }
}
