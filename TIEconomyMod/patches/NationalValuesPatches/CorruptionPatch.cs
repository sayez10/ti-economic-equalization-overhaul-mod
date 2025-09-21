// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using UnityEngine;



namespace TIEconomyMod.InvestmentPointPatches
{
    /// <summary>
    /// Patch changes the IP upkeep of armies to be dependent on mil tech level of the owning nation
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.corruption), MethodType.Getter)]
    internal static class CorruptionPatch
    {
        [HarmonyPrefix]
        private static bool GetCorruptionOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            if (__instance.alienNation)
            {
                __result = 0f;
            }

            // Those are vanilla numbers, some of them rounded down
            const float BASE_CORRUPTION_EFFECT = 0.5f;
            const float CORRUPTION_LOSS_PER_DEMOCRACY_LEVEL = -0.026_551_5f;
            const float CORRUPTION_LOSS_PER_COHESION_LEVEL =  -0.005_734_2f;
            const float CORRUPTION_LOSS_PER_GDPPC =           -0.000_002_75f;

            float democracyAddend = __instance.democracy * CORRUPTION_LOSS_PER_DEMOCRACY_LEVEL;
            float cohesionAddend = __instance.cohesion * CORRUPTION_LOSS_PER_COHESION_LEVEL;
            float gdppcAddend = __instance.perCapitaGDP * CORRUPTION_LOSS_PER_GDPPC;

            float corruption = BASE_CORRUPTION_EFFECT + democracyAddend + cohesionAddend + gdppcAddend;
            corruption += TIEffectsState.SumEffectsModifiers(Context.Corruption, __instance.executiveFaction, corruption);

            const float BASE_MAX_CORRUPTION = 0.95f;
            const float BASE_MIN_CORRUPTION = 0.05f;
            // Needs to be slightly smaller than -0.005 to avoid rounding up at some point during the calculations, which would result in a minimum corruption value above 0 even at 10 democracy
            // Unfortunately there's no Math.NextAfter() in .NET
            const float MIN_CORRUPTION_REDUCTION_PER_DEMOCRACY_LEVEL = -0.005_000_001f;

            float minCorruption = Math.Max(0, (BASE_MIN_CORRUPTION + (__instance.democracy * MIN_CORRUPTION_REDUCTION_PER_DEMOCRACY_LEVEL)));

            __result = Mathf.Clamp(corruption, minCorruption, BASE_MAX_CORRUPTION);


            return false; // Skip original method
        }
    }
}
