using Il2CppInterop.Runtime;
using Il2CppTMPro;
using PrimitierMultiplayerMod.Networking.Common;
using PrimitierMultiplayerMod.Networking.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PrimitierMultiplayerMod.RemoteInLocal
{
    internal static class PMPRemotePlayerController
    {
        public static Dictionary<int, PMPRemotePlayerEntity> remotePlayers = new();

        public static void CreateNewRemotePlayer(NetworkPlayer netPlr)
        {
            GameObject remotePlayerMainObj = new($"MP_remotePlayer_{netPlr.PLID}_{netPlr.Nickname}");
            PMPRemotePlayerEntity remotePlayer = remotePlayerMainObj.AddComponent<PMPRemotePlayerEntity>();

            GameObject remotePlayerHead = GameObject.CreatePrimitive(PrimitiveType.Cube);
            remotePlayerHead.name = "Head";
            remotePlayerHead.transform.parent = remotePlayerMainObj.transform;
            remotePlayer.head = remotePlayerHead;
            remotePlayerHead.transform.localScale *= 0.1f;

            GameObject remotePlayerLeftHand = GameObject.CreatePrimitive(PrimitiveType.Cube);
            remotePlayerLeftHand.name = "LeftHand";
            remotePlayerLeftHand.transform.parent = remotePlayerMainObj.transform;
            remotePlayer.leftHand = remotePlayerLeftHand;
            remotePlayerLeftHand.transform.localScale *= 0.1f;

            GameObject remotePlayerRightHand = GameObject.CreatePrimitive(PrimitiveType.Cube);
            remotePlayerRightHand.name = "RightHand";
            remotePlayerRightHand.transform.parent = remotePlayerMainObj.transform;
            remotePlayer.rightHand = remotePlayerRightHand;
            remotePlayerRightHand.transform.localScale *= 0.1f;

            GameObject nameplateCanvasObj = new("MP_NameplateCanvas");
            nameplateCanvasObj.transform.parent = remotePlayerMainObj.transform;
            nameplateCanvasObj.transform.localScale *= 0.05f;
            nameplateCanvasObj.transform.localPosition = new Vector3(0f, 2.1f, 0f);

            TextMeshPro nameplateText = nameplateCanvasObj.AddComponent<TextMeshPro>();
            nameplateText.text = netPlr.Nickname;
            nameplateText.alignment = TextAlignmentOptions.Midline;

            remotePlayers.Add(netPlr.PLID, remotePlayer);
        }

        public static void UpdateRemotePlayer(int plid, PlayerState state)
        {
            remotePlayers[plid].UpdateRemotePlayer(state);
        }

        public static void RemoveRemotePlayer(int plid)
        {
            var remPlr = remotePlayers[plid];
            remPlr.RemoveRemotePlayer();
            remotePlayers.Remove(plid);
        }
    }
}
