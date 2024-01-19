using PrimitierMultiplayerMod.Networking.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.ClientSide
{
    public class ClientGroupNetworkBehavior : MonoBehaviour
    {
        public ClientGroupNetworkBehavior(IntPtr ptr) : base(ptr)
        {
            id = ClientNetChunkManager.idSet;
        }

        public ulong id;
        public bool isOwnerLocal;
    }
}
