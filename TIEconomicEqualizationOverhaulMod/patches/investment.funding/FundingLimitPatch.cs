// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the amount of investment points available to a nation to scale linearly with GDP
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.maxFunding_year), MethodType.Getter)]
    internal static class FundingLimitPatch
    {
        [HarmonyPrefix]
        private static bool MaxFundingYearOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const double ANNUAL_FUNDING_PER_GDP = 0.000_000_05d;

            __result = (float)(ANNUAL_FUNDING_PER_GDP * __instance.GDP);


            return false; // Skip original method
        }
    }
}
