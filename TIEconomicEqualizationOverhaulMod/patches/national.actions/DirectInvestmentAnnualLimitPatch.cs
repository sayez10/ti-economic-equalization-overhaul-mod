// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Changes the formula to calculate the maximum annual direct investments in nations
    /// The number has been increased significantly
    /// The limit scales linearly with a nation's monthly IP, multiplied with a user-definable factor
    /// Every nation also gets a flat amount of 200 to allow substantial direct investments in small and poor nations
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.MaxAnnualDirectInvestIPs), MethodType.Getter)]
    internal static class DirectInvestmentAnnualLimitPatch
    {
        [HarmonyPrefix]
        private static bool GetMaxAnnualDirectInvestIPsOverwrite(ref int __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_DIRECT_INVEST_IP = 200f;

            __result = (int)(BASE_DIRECT_INVEST_IP + (__instance.economyScore * TIGlobalConfig.globalConfig.nationalDirectInvestmentCapGlobalMultiplier));


            return false; // Skip original method
        }
    }
}
