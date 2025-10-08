// SPDX-FileCopyrightText: Copyright © 2024 - 2025 explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the democracy effect of an oppression investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.OppressionPriorityDemocracyChange), MethodType.Getter)]
    internal static class OppressionDemocracyEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetOppressionPriorityDemocracyChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_DEMOCRACY_EFFECT = -0.0175f;

            __result = EconomyScorePatch.EffectStrength(BASE_DEMOCRACY_EFFECT, __instance.population);


            return false; // Skip original method
        }
    }
}
