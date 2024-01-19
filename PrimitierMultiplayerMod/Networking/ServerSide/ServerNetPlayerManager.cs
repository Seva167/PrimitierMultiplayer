using LiteNetLib;
using PrimitierMultiplayerMod.Networking.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitierMultiplayerMod.Networking.ServerSide
{
    internal static class ServerNetPlayerManager
    {
        public static Dictionary<int, ServerSidePlayer> netPlayers = new();
        public static ServerSidePlayer thisPlayer = new();
    }
}
