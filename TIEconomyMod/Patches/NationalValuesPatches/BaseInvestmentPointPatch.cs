// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the amount of investment points available to a nation to scale linearly with GDP
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.economyScore), MethodType.Getter)]
    internal static class BaseInvestmentPointPatch
    {
        [HarmonyPrefix]
        private static bool GetEconomyScoreOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Linear scaling: E.g. 500 billion GDP * 0.000_000_000_01 = 5 IP/month
            __result = (float)(__instance.GDP * Tools.IPPerGDP);


            return false; // Skip original method
        }
    }
}
