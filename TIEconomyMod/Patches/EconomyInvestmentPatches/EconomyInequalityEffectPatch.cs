// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the inequality effect of an economy investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.economyPriorityInequalityChange), MethodType.Getter)]
    internal static class EconomyInequalityEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetEconomyPriorityInequalityChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_INEQUALITY = 0.0075f;
            const float INEQUALITY_MULT_PER_RESOURCE_REGION = 0.15f;

            // Effect is ~13.3 times weaker than welfare
            float baseInequalityGain = Tools.EffectStrength(BASE_INEQUALITY, __instance.population);
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * INEQUALITY_MULT_PER_RESOURCE_REGION);

            __result = baseInequalityGain * resourceRegionsMult;


            return false; // Skip original method
        }
    }
}
