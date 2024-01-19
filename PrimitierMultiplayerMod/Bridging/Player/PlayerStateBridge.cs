using PrimitierMultiplayerMod.Networking.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Bridging.Player
{
    internal static class PlayerStateBridge
    {
        static UnityEngine.Transform origin;
        static UnityEngine.Transform head;
        static UnityEngine.Transform leftHand;
        static UnityEngine.Transform rightHand;

        public static void Init()
        {
            origin = GameObject.Find("XR Origin").transform;
            head = GameObject.Find("Main Camera").transform;
            leftHand = GameObject.Find("LeftHand").transform;
            rightHand = GameObject.Find("RightHand").transform;
        }

        public static PlayerState GetPlayerState()
        {
            var state = new PlayerState
            {
                originTransform = new Networking.Common.Models.Transform { position = origin.position, rotation = origin.rotation },
                headTransform = new Networking.Common.Models.Transform { position = head.position, rotation = head.rotation },
                leftHandTransform = new Networking.Common.Models.Transform { position= leftHand.position, rotation = leftHand.rotation },
                rightHandTransform = new Networking.Common.Models.Transform { position = rightHand.position, rotation= rightHand.rotation }
            };

            return state;
        }
    }
}
