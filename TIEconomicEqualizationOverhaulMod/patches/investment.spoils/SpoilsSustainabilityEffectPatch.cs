﻿// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the sustainability effect of a spoils completion to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.spoilsSustainabilityChange), MethodType.Getter)]
    internal static class SpoilsSustainabilityEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetSpoilsSustainabilityChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // BASE_SUSTAINABILITY is inverted, because for whatever reason sustainability increases with negative change, and decreases with positive change
            const float BASE_SUSTAINABILITY_EFFECT = 0.2f;
            const float SUSTAINABILITY_MULT_PER_RESOURCE_REGION = 0.2f;

            float baseSustainabilityLoss = EconomyScorePatch.EffectStrength(BASE_SUSTAINABILITY_EFFECT, __instance.population);

            // Scaling is more aggressive than in Environment
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * SUSTAINABILITY_MULT_PER_RESOURCE_REGION);

            __result = baseSustainabilityLoss * resourceRegionsMult;


            return false; // Skip original method
        }
    }
}
