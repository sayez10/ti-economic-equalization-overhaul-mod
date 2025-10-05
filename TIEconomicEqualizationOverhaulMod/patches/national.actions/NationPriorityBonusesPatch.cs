// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// All priorities related to the military now benefit from mining regions in a nation
    /// All other priorities don't benefit from them anymore
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.NationalPriorityBonuses))]
    internal static class NationPriorityBonusesPatch
    {
        [HarmonyPrefix]
        private static bool NationalPriorityBonusesOverwrite(ref float __result, in TINationState __instance, PriorityType priority)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            if (priority == PriorityType.Economy)
            {
                __result = __instance.restofFederationECOBonus_dailyCache * TemplateManager.global.federationGDPEconomyBonus;
            }
            else if (priority >= PriorityType.Military_FoundMilitary)
            {
                __result = (float)__instance.numMiningRegions_dailyCache * TemplateManager.global.coreMineralBuildMilitaryModifier;
            }
            else
            {
                __result = 0f;
            }


            return false; // Skip original method
        }
    }
}
