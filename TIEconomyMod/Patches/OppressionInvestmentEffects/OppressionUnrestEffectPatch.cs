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
    /// Patch changes the unrest-reducing effect of an oppression investment to scale inversely with population size
    /// VERY powerful for totalitarian nations... especially if they're rich
    /// However, at full democracy, oppression does nothing to unrest
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.OppressionPriorityUnrestChange), MethodType.Getter)]
    public static class OppressionUnrestEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetOppressionPriorityUnrestChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_UNREST = -2.5f;
            const float UNREST_PENALTY_MULT_PER_DEMOCRACY_LEVEL = 0.1f;

            float baseEffect = Tools.EffectStrength(BASE_UNREST, __instance.population);

            // Effect is reduced by democracy, -10% per full point
            float adjustedEffect = baseEffect * (1f - (__instance.democracy * UNREST_PENALTY_MULT_PER_DEMOCRACY_LEVEL));

            // Vanilla code explicitly disallows a value that'd go under 0
            // So better play it safe
            __result = Mathf.Max(-__instance.unrest, adjustedEffect);


            return false; // Skip original method
        }
    }
}
