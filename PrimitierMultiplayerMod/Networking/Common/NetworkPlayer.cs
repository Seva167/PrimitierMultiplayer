using PrimitierMultiplayerMod.Networking.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitierMultiplayerMod.Networking.Common
{
    public abstract class NetworkPlayer
    {
        public int PLID {  get; set; }
        public string Nickname { get; set; }
        public PlayerState State
        {
            get => currState;
            set
            {
                PrevState = currState;
                currState = value;
            }
        }
        PlayerState currState;
        public PlayerState PrevState { get; private set; }
    }
}
