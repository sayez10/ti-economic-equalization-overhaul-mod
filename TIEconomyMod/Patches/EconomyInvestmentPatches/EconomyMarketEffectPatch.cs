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
    [HarmonyPatch(typeof(TIGlobalValuesState), "ModifyMarketValuesForEconomyPriority")]
    public static class EconomyMarketEffectPatch
    {
        [HarmonyPrefix]
        public static bool ModifyMarketValuesForEconomyPriorityOverwrite(TIGlobalValuesState __instance)
        {
            // If mod has been disabled, abort patch and use original method.
            if (!Main.enabled) { return true; }

            const float RESOURCE_MARKET_VALUE_MULT = 1f + 0.000001f;

            // Patches the small increase to metal and noble metal price that is triggered by every economy investment completion
            // Substantially reduce the multiplier (in particular for non-noble metals) to avoid breaking the game due to the massively increased number of economy priority completions
            __instance.resourceMarketValues[FactionResource.Metals] *= RESOURCE_MARKET_VALUE_MULT;
            __instance.resourceMarketValues[FactionResource.NobleMetals] *= RESOURCE_MARKET_VALUE_MULT;


            return false; // Skip original method
        }
    }
}
