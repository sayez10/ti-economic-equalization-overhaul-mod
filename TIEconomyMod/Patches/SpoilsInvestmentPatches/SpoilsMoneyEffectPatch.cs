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
    [HarmonyPatch(typeof(TINationState), "spoilsPriorityMoney", MethodType.Getter)]
    public static class SpoilsMoneyEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetSpoilsPriorityMoneyOverwrite(ref float __result, TINationState __instance)
        {
            // Patch changes the instant money effect of a spoils investment to be a flat value, not scaled by nation size

            // If mod has been disabled, abort patch and use original method.
            if (!Main.enabled) { return true; }

            const float BASE_MONEY = 60f;
            const float MONEY_MULT_PER_RESOURCE_REGION = 0.15f;
            const float MAX_MULT_FROM_LOW_DEMOCRACY = 0.3f;

            // Add money per resource region.
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * MONEY_MULT_PER_RESOURCE_REGION);

            // Up to 30% extra money at 0 democracy, 0% extra at 10 democracy
            float democracyMult = 1f + MAX_MULT_FROM_LOW_DEMOCRACY - (__instance.democracy * MAX_MULT_FROM_LOW_DEMOCRACY * 0.1f);

            __result = BASE_MONEY * resourceRegionsMult * democracyMult;


            return false; // Skip original method
        }
    }
}
