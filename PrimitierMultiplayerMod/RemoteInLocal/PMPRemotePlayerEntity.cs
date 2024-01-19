using PrimitierMultiplayerMod.Networking.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.RemoteInLocal
{
    public class PMPRemotePlayerEntity : MonoBehaviour
    {
        public PMPRemotePlayerEntity(IntPtr ptr) : base(ptr) { }

        public int PLID;
        public GameObject head;
        public GameObject leftHand;
        public GameObject rightHand;

        public void UpdateRemotePlayer(PlayerState state)
        {
            transform.SetPositionAndRotation(state.originTransform.position, state.originTransform.rotation);
            head.transform.SetPositionAndRotation(state.headTransform.position, state.headTransform.rotation);
            leftHand.transform.SetPositionAndRotation(state.leftHandTransform.position, state.leftHandTransform.rotation);
            rightHand.transform.SetPositionAndRotation(state.rightHandTransform.position, state.rightHandTransform.rotation);
        }

        public void RemoveRemotePlayer()
        {
            Destroy(gameObject);
        }
    }
}
