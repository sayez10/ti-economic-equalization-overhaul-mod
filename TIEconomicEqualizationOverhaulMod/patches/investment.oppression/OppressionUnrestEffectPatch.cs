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

            float baseUnrestLoss = Tools.EffectStrength(BASE_UNREST_EFFECT, __instance.population);

            // Effect is reduced by democracy, -10% per full point
            float democracyMult = 1f - (__instance.democracy * UNREST_PENALTY_MULT_PER_DEMOCRACY_LEVEL);

            // Vanilla code explicitly disallows a value that'd go under 0
            // So better play it safe
            __result = Math.Max(-__instance.unrest, (baseUnrestLoss * democracyMult));


            return false; // Skip original method
        }
    }
}
