// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the democracy effect of a spoils completion to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.spoilsPriorityDemocracyChange), MethodType.Getter)]
    internal static class SpoilsDemocracyEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetSpoilsPriorityDemocracyChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_DEMOCRACY_EFFECT = -0.1f;

            __result = Tools.EffectStrength(BASE_DEMOCRACY_EFFECT, __instance.population);


            return false; // Skip original method
        }
    }
}
