﻿using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ShadowLib
{
    public class PlayerUtils
    {
        public static Dictionary<PlayerController, SteamId> steamIds = new();

        public static void OnAssignedPlayerController(PlayerController controller)
        {
            try
            {
                var id = ((SteamConnection)controller.networkObject.GetOwner()).steamId;
                steamIds.Add(controller, id);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.StackTrace);
            }
        }

        public static void OnUnassignedPlayerController(PlayerController controller)
        {
            try
            {
                steamIds.Remove(controller);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.StackTrace);
            }
        }

        public static PlayerController GetMyPlayer()
        {
            if(GameInstance.Instance.GetGamemode() == null)
            {
                Debug.LogError("Can't get Player Controller! Not currently in a Game!");
                return null;
            }

            foreach(PlayerController player in GameInstance.Instance.GetPlayerControllers())
            {
                if(player.networkObject.IsOwner())
                {
                    return player;
                }
            }

            return null;
        }

        public static PlayerController GetHostPlayer()
        {
            if (GameInstance.Instance.GetGamemode() == null)
            {
                Debug.LogError("Can't get Player Controller! Not currently in a Game!");
                return null;
            }

            foreach (PlayerController player in GameInstance.Instance.GetPlayerControllers())
            {
                if (player.networkObject.IsServer())
                {
                    return player;
                }
            }

            return null;
        }

        public static PlayerController GetRandomPlayer()
        {
            if (GameInstance.Instance.GetGamemode() == null)
            {
                Debug.LogError("Can't get Player Controller! Not currently in a Game!");
                return null;
            }

            var num = UnityEngine.Random.RandomRangeInt(0, GameInstance.Instance.GetPlayerControllers().Count);

            return GameInstance.Instance.GetPlayerControllers()[num];
        }
    }
}
