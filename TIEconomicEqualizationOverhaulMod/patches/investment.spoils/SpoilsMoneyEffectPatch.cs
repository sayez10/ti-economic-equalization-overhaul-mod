// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the instant money effect of a spoils completion to be a flat value, not scaled by nation size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.spoilsPriorityMoney), MethodType.Getter)]
    internal static class SpoilsMoneyEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetSpoilsPriorityMoneyOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_MONEY_EFFECT = 150f;
            const float MONEY_MULT_PER_RESOURCE_REGION = 0.2f;
            const float MONEY_MULT_FROM_LOW_DEMOCRACY = 0.1f;

            // Add money per resource region
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * MONEY_MULT_PER_RESOURCE_REGION);

            // Up to 100% extra money at 0 democracy, 0% extra at 10 democracy
            float democracyMult = 1f + ((10f - __instance.democracy) * MONEY_MULT_FROM_LOW_DEMOCRACY);

            // Corruption reduces income
            float corruptionMult = 1f - __instance.corruption;

            __result = BASE_MONEY_EFFECT * resourceRegionsMult * democracyMult * corruptionMult;


            return false; // Skip original method
        }
    }
}
