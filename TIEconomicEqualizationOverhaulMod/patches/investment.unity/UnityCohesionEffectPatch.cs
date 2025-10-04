// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;




namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the cohesion effect of a unity investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.unityPriorityCohesionChange), MethodType.Getter)]
    internal static class UnityCohesionEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetUnityPriorityCohesionChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_COHESION_EFFECT = 1f;
            const float COHESION_PENALTY_MULT_PER_EDUCATION_AND_DEMOCRACY_LEVEL = 0.025f;
            const float EDUCATION_AND_DEMOCRACY_PENALTY_MAX = 0.5f;

            float baseCohesionGain = Tools.EffectStrength(BASE_COHESION_EFFECT, __instance.population);

            // Democracy and Education incurs a 2.5% penalty per point, up to -50%
            // A combined score of 20 causes the max effect
            float educationDemocracyPenaltyMult = Math.Min(EDUCATION_AND_DEMOCRACY_PENALTY_MAX, 1f - ((__instance.education + __instance.democracy) * COHESION_PENALTY_MULT_PER_EDUCATION_AND_DEMOCRACY_LEVEL));

            // Corruption reduces investment
            float corruptionMult = 1f - __instance.corruption;

            __result = baseCohesionGain * educationDemocracyPenaltyMult * corruptionMult;


            return false; // Skip original method
        }
    }
}
