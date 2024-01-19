using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.ServerSide
{
    public class ServerGroupNetworkBehavior : MonoBehaviour
    {
        internal static ulong idCtr = 1;

        public ServerGroupNetworkBehavior(IntPtr ptr) : base(ptr)
        {
            id = idCtr++;
        }

        public ulong id;
        public int ownerPLID;
    }
}
