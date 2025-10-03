// SPDX-FileCopyrightText: Copyright © 2024 - 2025 explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using System.Linq;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the sustainability effect of an environment investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.environmentPrioritySustainabilityChange), MethodType.Getter)]
    internal static class EnvironmentSustainabilityEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetEnvironmentPrioritySustainabilityChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // FIXME: Overall, the scaling should have lesser extremes. Might need tweaking.

            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // For whatever reason, sustainability is increased with negative values
            const float BASE_SUSTAINABILITY_EFFECT = -0.1f;
            const float SUSTAINABILITY_MULT_PER_SUSTAINABILITY_LEVEL = 0.1f;
            const float PENALTY_PER_NUKED_REGION = 0.05f;
            const float MAX_PENALTY_FROM_NUKED_REGIONS = 0.5f;

            float baseSustainabilityGain = Tools.EffectStrength(BASE_SUSTAINABILITY_EFFECT, __instance.population);

            // Each full point of sustainability gives a +10% bonus, up to +100%
            float sustainabilityMult = 1f + (__instance.sustainability * SUSTAINABILITY_MULT_PER_SUSTAINABILITY_LEVEL);

            // FIXME: Uncomment if this modifier is ever used in Vanilla and append it as factor to calculation for result
//            float environmentSustainabilityChangeMult = 1f + TIEffectsState.SumEffectsModifiers(Context.Environment_SustainabilityChange, __instance, 1f);

            // Each nuked region causes a sustainability generation penalty of -5%, up to -50%
            float nukedCounter = __instance.regions.Sum((TIRegionState x) => x.nuclearDetonations);
            float nukedMult = Math.Max(MAX_PENALTY_FROM_NUKED_REGIONS, 1f - (nukedCounter * PENALTY_PER_NUKED_REGION));

            // Corruption reduces investment
            float corruptionMult = 1f - __instance.corruption;

            __result = baseSustainabilityGain * sustainabilityMult * nukedMult * corruptionMult;


            return false; // Skip original method
        }
    }
}
