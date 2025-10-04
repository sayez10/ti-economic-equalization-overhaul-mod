// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch substantially reduces the increase to metal and noble metal price that is triggered by every economy investment completion
    /// This is necessary to avoid breaking the game due to the massively increased number of economy priority completions
    /// </summary>
    [HarmonyPatch(typeof(TIGlobalValuesState), nameof(TIGlobalValuesState.ModifyMarketValuesForEconomyPriority))]
    internal static class EconomyMarketEffectPatch
    {
        [HarmonyPrefix]
        private static bool ModifyMarketValuesForEconomyPriorityOverwrite(TIGlobalValuesState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float RESOURCE_MARKET_VALUE_MULT = 1.000_001f;

            __instance.resourceMarketValues[FactionResource.Metals] *= RESOURCE_MARKET_VALUE_MULT;
            __instance.resourceMarketValues[FactionResource.NobleMetals] *= RESOURCE_MARKET_VALUE_MULT;


            return false; // Skip original method
        }
    }
}
