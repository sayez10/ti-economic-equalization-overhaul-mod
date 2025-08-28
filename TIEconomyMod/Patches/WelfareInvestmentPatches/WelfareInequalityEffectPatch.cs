// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the inequality effect of a welfare investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.welfarePriorityInequalityChange), MethodType.Getter)]
    internal static class WelfareInequalityEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetWelfarePriorityInequalityChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_INEQUALITY = -0.1f;

            __result = Tools.EffectStrength(BASE_INEQUALITY, __instance.population);


            return false; // Skip original method
        }
    }
}
