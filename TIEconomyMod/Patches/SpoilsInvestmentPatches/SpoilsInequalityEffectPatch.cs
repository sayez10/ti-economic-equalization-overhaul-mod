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
    /// <summary>
    /// Patch changes the inequality effect of a spoils completion to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "spoilsPriorityInequalityChange", MethodType.Getter)]
    public static class SpoilsInequalityEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetSpoilsPriorityInequalityChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_INEQUALITY = 0.2f;
            const float INEQUALITY_MULT_PER_RESOURCE_REGION = 1f;

            float baseInequalityGain = Tools.EffectStrength(BASE_INEQUALITY, __instance.population);
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * INEQUALITY_MULT_PER_RESOURCE_REGION);

            __result = baseInequalityGain * resourceRegionsMult;


            return false; // Skip original method
        }
    }
}
