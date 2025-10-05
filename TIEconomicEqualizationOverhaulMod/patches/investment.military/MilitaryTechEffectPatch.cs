﻿// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the military tech effect of a military investment to scale only with existing military units (armies and navies)
    /// Population size doesn't affect the result
    /// It also adds a catch-up boost to gain based on how far behind the global maximum tech level the country is
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.militaryPriorityTechLevelChange), MethodType.Getter)]
    internal static class MilitaryTechEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetMilitaryPriorityTechLevelChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_MILITARY_EFFECT = 0.001f;
            const float MILITARY_PER_MILITARY_LEVEL_BEHIND = 0.5f;
            const float MILITARY_MULT_PER_EDUCATION_LEVEL = 0.1f;

            // Higher = faster miltech increase if nation has any armies
            const float FORCES_NUMBER_MULT_GROWTH_FACTOR = 5f;

            // Higher = faster miltech increase if nation has higher current miltech level
            const float MILITARY_LEVEL_MULT_GROWTH_FACTOR = 5f;

            // Each full point of education gives +10% military score
            float educationMult = 1f + (__instance.education * MILITARY_MULT_PER_EDUCATION_LEVEL);

            // Reduces the miltech increase per investment for each existing army or navy in nation
            // Modifier has no effect for 0 armies, then grows asymptotically to 1
            float armiesNumberMult = FORCES_NUMBER_MULT_GROWTH_FACTOR / (FORCES_NUMBER_MULT_GROWTH_FACTOR + __instance.numStandardArmies + __instance.numNavies);

            // Reduces the miltech increase per investment for each level of current military level
            // Modifier has no effect for current military level 0, then grows asymptotically to 1
            float militaryLevelMult = MILITARY_LEVEL_MULT_GROWTH_FACTOR / (MILITARY_LEVEL_MULT_GROWTH_FACTOR + __instance.militaryTechLevel);

            // Add a catch-up multiplier dependent on how far behind the max miltech level the country is
            // +50% miltech gain per full miltech level behind the global max
            // Max to 1 is to prevent weirdness if somehow current miltech is above max miltech
            float catchUpMult = Math.Max(1f, 1f + (MILITARY_PER_MILITARY_LEVEL_BEHIND * (__instance.maxMilitaryTechLevel - __instance.militaryTechLevel)));

            // Corruption reduces investment
            float corruptionMult = 1f - __instance.corruption;

            __result = BASE_MILITARY_EFFECT * educationMult * militaryLevelMult * armiesNumberMult * catchUpMult * corruptionMult;


            return false; // Skip original method
        }
    }
}
