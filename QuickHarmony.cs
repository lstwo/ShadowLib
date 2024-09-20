using HarmonyLib;
using System;
using UnityEngine;

namespace ShadowLib
{
    public class QuickHarmony
    {
        private string baseId;

        public QuickHarmony(string baseId)
        {
            this.baseId = baseId;
        }

        public Harmony Init(Type type, string id)
        {
            var h = new Harmony(baseId + "." + id);
            h.PatchAll(type);
            return h;
        }

        public Harmony Init(Type type)
        {
            return Init(type, type.GetType().Name);
        }
    }
}
