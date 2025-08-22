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
            // Patch changes the democracy effect of a spoils investment to scale inversely with population size

            // If mod has been disabled, abort patch and use original method.
            if (!Main.enabled) { return true; }

            const float BASE_DEMOCRACY = -0.02f;

            // Refer to EffectStrength() comments for explanation.
            __result = Tools.EffectStrength(BASE_DEMOCRACY, __instance.population);


            return false; // Skip original method
        }
    }
}
