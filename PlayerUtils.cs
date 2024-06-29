using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowLib
{
    public class PlayerUtils
    {
        public static Dictionary<PlayerController, SteamId> steamIds = new();

        public static void OnAssignedPlayerController(PlayerController controller)
        {
            try
            {
                SteamId id = ((SteamConnection)controller.networkObject.GetOwner()).steamId;
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
    }
}
