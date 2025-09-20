// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod.InvestmentPointPatches
{
    /// <summary>
    /// Patch changes the IP upkeep of navies to be dependent on miltech level of the owning nation
    /// </summary>
    [HarmonyPatch(typeof(TIArmyState), nameof(TIArmyState.investmentNavyFactor), MethodType.Getter)]
    internal static class NavyInvestmentUpkeepPatch
    {
        [HarmonyPrefix]
        private static bool GetInvestmentNavyFactorOverwrite(ref float __result, in TIArmyState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Multiply maintenance cost by miltech level
            __result = TemplateManager.global.nationalInvestmentNavyFactor * __instance.homeNation.militaryTechLevel;

            return false; // Skip original method
        }
    }
}
