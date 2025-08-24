// SPDX-FileCopyrightText: Copyright © 2024 - 2025 explodoboy, sayez10
//
// SPDX-License-Identifier: MIT



using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the cohesion effect of an oppression investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "OppressionPriorityCohesionChange", MethodType.Getter)]
    public static class OppressionCohesionEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetOppressionPriorityCohesionChangeOverwrite(ref float __result, TINationState __instance)
        {

            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_COHESION = -1f;
            const float MIN_DEMOCRACY_FOR_COHESION_CHANGE = 5f;
            const float COHESION_RAMPUP_PER_DEMOCRACY_LEVEL = 0.2f;

            float baseEffect = Tools.EffectStrength(BASE_COHESION, __instance.population);

            // Effect ramps up the higher Democracy is: 0% at/under 5, 100% at 10
            float democracyMult = Mathf.Max(0f, COHESION_RAMPUP_PER_DEMOCRACY_LEVEL * (__instance.democracy - MIN_DEMOCRACY_FOR_COHESION_CHANGE));

            __result = baseEffect * democracyMult;


            return false; // Skip original method
        }
    }
}
