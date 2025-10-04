// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Called every time after TINationState.ModifyGDP() completes
    /// That vanilla method is called every time a nation's GDP changes
    /// Overwrites the nation's economyScore member (= monthly IP), which had been set during ModifyGDP()
    /// Instead of GDPInBillions^0.33 like in Vanilla, we use linear scaling
    /// That can be further modified by a user-defined factor
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.ModifyGDP))]
    internal static class EconomyScorePatch
    {
        [HarmonyPostfix]
        private static void ModifyGDPPostfix(TINationState __instance)
        {
            // If mod has been disabled, abort patch
            if (!Main.enabled) { return; }

            // Linear scaling: E.g. 500 billion GDP * 0.000_000_000_01d = 5 IP/month
            float economyScore = (float)(__instance.GDP * Tools.IPPerGDP);

            Traverse traverse = Traverse.Create(__instance);
            traverse.Property("economyScore", null).SetValue(economyScore);
            __instance.SetDataDirty();
        }
    }
}
