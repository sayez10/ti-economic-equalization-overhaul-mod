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
    public static class NavyInvestmentUpkeepPatch
    {
        [HarmonyPrefix]
        public static bool GetInvestmentNavyFactorOverwrite(ref float __result, TIArmyState __instance)
        {
            // Multiply maintenance cost by miltech level
            __result = TemplateManager.global.nationalInvestmentNavyFactor * __instance.homeNation.militaryTechLevel;

            return false; // Skip original method
        }
    }
}
