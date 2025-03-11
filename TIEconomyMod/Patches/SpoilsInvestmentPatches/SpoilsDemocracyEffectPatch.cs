using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TIEconomyMod
{
    [HarmonyPatch(typeof(TINationState), "spoilsPriorityDemocracyChange", MethodType.Getter)]
    public static class SpoilsDemocracyEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetSpoilsPriorityDemocracyChangeOverwrite(ref float __result, TINationState __instance)
        {
            //Patch changes the democracy effect of a spoils investment to scale inversely with population size

            // Refer to EffectStrength() comments for explanation.
            __result = Tools.EffectStrength(0.02f, __instance.population);

            return false; //Skip original getter
        }
    }
}
