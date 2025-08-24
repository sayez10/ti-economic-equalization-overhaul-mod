// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT



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
    /// Patch changes the instant money effect of a spoils completion to be a flat value, not scaled by nation size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "spoilsPriorityMoney", MethodType.Getter)]
    public static class SpoilsMoneyEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetSpoilsPriorityMoneyOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_MONEY = 150;
            const float MONEY_MULT_PER_RESOURCE_REGION = 0.15f;
            const float MONEY_MULT_FROM_LOW_DEMOCRACY = 0.1f;

            // Add money per resource region
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * MONEY_MULT_PER_RESOURCE_REGION);

            // Up to 100% extra money at 0 democracy, 0% extra at 10 democracy
            float democracyMult = 1f + ((10f - __instance.democracy) * MONEY_MULT_FROM_LOW_DEMOCRACY);

            __result = BASE_MONEY * resourceRegionsMult * democracyMult;


            return false; // Skip original method
        }
    }
}
