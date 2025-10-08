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
            const float INEQUALITY_MULT_PER_RESOURCE_REGION = 0.2f;

            float baseDemocracyLoss = EconomyScorePatch.EffectStrength(BASE_DEMOCRACY_EFFECT, __instance.population);
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * INEQUALITY_MULT_PER_RESOURCE_REGION);

            __result = baseDemocracyLoss * resourceRegionsMult;


            return false; // Skip original method
        }
    }
}
