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

            const float BASE_INEQUALITY = 0.05f;
            const float INEQUALITY_MULT_PER_RESOURCE_REGION = 0.25f;

            // For a full explanation of the logic backing this change, see WelfareInequalityEffectPatch.cs
            // I want an inequality gain rate of 0.05 a month for a 30k GDP per capita nation
            // Using the same method as with the welfare inequality, this gives me a single investment effect of 166667 / population democracy change
            float baseInequalityGain = Tools.EffectStrength(BASE_INEQUALITY, __instance.population);
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * INEQUALITY_MULT_PER_RESOURCE_REGION);

            __result = baseInequalityGain * resourceRegionsMult;


            return false; // Skip original method
        }
    }
}
