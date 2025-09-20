// SPDX-FileCopyrightText: Copyright © 2024 - 2025 explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the cohesion effect of an oppression investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.OppressionPriorityCohesionChange), MethodType.Getter)]
    internal static class OppressionCohesionEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetOppressionPriorityCohesionChangeOverwrite(ref float __result, in TINationState __instance)
        {

            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_COHESION_EFFECT = -1f;
            const float MIN_DEMOCRACY_FOR_COHESION_CHANGE = 5f;
            const float COHESION_MULT_PER_DEMOCRACY_LEVEL = 0.2f;

            float baseCohesionLoss = Tools.EffectStrength(BASE_COHESION_EFFECT, __instance.population);

            // Effect ramps up the higher Democracy is: 0% at/under 5, 100% at 10
            float democracyMult = Math.Max(0f, COHESION_MULT_PER_DEMOCRACY_LEVEL * (__instance.democracy - MIN_DEMOCRACY_FOR_COHESION_CHANGE));

            __result = baseCohesionLoss * democracyMult;


            return false; // Skip original method
        }
    }
}
