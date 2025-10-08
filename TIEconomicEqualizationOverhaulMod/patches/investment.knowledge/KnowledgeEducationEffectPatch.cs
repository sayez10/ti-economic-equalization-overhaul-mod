// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the education effect of a knowledge investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.knowledgePriorityEducationChange), MethodType.Getter)]
    internal static class KnowledgeEducationEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetKnowledgePriorityEducationChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_EDUCATION_EFFECT = 0.1f;
            const float MAX_SCALE_FACTOR = 4f;
            const float DECAY_FACTOR = 0.87f;

            float baseEducationGain = EconomyScorePatch.EffectStrength(BASE_EDUCATION_EFFECT, __instance.population);

            // Additionally, scale the education change based on current education, using an exponential decay relationship
            // With a multiplier of 4, and a base of 0.87, we get:
            // 400% education gain at 0 education, 200% at 5 education, 100% at 10 education, 50% at 15 education, etc...
            // Basically, the effectiveness of the knowledge priority halves every 5 education
            float decayMult = MAX_SCALE_FACTOR * (float)Math.Pow(DECAY_FACTOR, __instance.education);

            // Corruption reduces investment
            float corruptionMult = 1f - __instance.corruption;

            __result = baseEducationGain * decayMult * corruptionMult;


            return false; // Skip original method
        }
    }
}
