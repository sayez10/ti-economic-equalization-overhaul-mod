// SPDX-FileCopyrightText: Copyright © 2024 - 2025 explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the democracy effect of a government investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.governmentPriorityDemocracyChange), MethodType.Getter)]
    internal static class GovernmentDemocracyEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetGovernmentPriorityDemocracyChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_DEMOCRACY_EFFECT = 0.05f;
            const float DEMOCRACY_MULT_PER_EDUCATION_LEVEL = 0.1f;

            float baseDemocracyGain = Tools.EffectStrength(BASE_DEMOCRACY_EFFECT, __instance.population);

            // Each full point of Education gives +10% Democracy score
            float educationMult = 1f + (__instance.education * DEMOCRACY_MULT_PER_EDUCATION_LEVEL);

            __result = baseDemocracyGain * educationMult;


            return false; // Skip original method
        }
    }
}
