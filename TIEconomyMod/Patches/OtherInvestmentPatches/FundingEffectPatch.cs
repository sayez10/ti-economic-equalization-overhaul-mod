// SPDX-FileCopyrightText: Copyright © 2024 - 2025 explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the monthly funding gain from completing a funding investment
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.spaceFundingPriorityIncomeChange), MethodType.Getter)]
    public static class FundingEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetSpaceFundingPriorityIncomeChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float FUNDING_AMOUNT = 100f;

            // Spoils gives a instant funding of about 300-400 money, at the cost of a fair bit of greenhouse gas and half an investment of welfare in inequality
            // The funding gained from a funding investment is annual, so an annual income of 100 money is equal to a typical spoils completion (about 300-500 money) after ~4 years
            __result = FUNDING_AMOUNT;


            return false; // Skip original method
        }
    }
}
