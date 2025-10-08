// SPDX-FileCopyrightText: Copyright © 2024 - 2025 explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the unrest-reducing effect of an oppression investment to scale inversely with population size
    /// VERY powerful for totalitarian nations... especially if they're rich
    /// However, at full democracy, oppression does nothing to unrest
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.OppressionPriorityUnrestChange), MethodType.Getter)]
    internal static class OppressionUnrestEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetOppressionPriorityUnrestChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_UNREST_EFFECT = -2.5f;
            const float UNREST_PENALTY_MULT_PER_DEMOCRACY_LEVEL = 0.1f;
            const float UNREST_PENALTY_MULT_PER_MILITARY_LEVEL = 0.1f;
            const float FRAC_ARMIES_MULT = 2f;

            float baseUnrestLoss = EconomyScorePatch.EffectStrength(BASE_UNREST_EFFECT, __instance.population);

            // Effect is reduced by democracy
            float democracyMult = 1f - (__instance.democracy * UNREST_PENALTY_MULT_PER_DEMOCRACY_LEVEL);

            // Effect is increased by military level
            float militaryMult = 1f + (__instance.militaryTechLevel * UNREST_PENALTY_MULT_PER_MILITARY_LEVEL);

            // Effect is increased by fraction of regions with armies
            float fracArmiesMult = 1f + (FRAC_ARMIES_MULT * __instance.numStandardArmies / __instance.regions.Count);

            float unrestLoss = baseUnrestLoss * democracyMult * militaryMult * fracArmiesMult;

            // Vanilla code explicitly disallows a value that'd go under 0
            // So better play it safe
            __result = Math.Max(-__instance.unrest, unrestLoss);


            return false; // Skip original method
        }
    }
}
