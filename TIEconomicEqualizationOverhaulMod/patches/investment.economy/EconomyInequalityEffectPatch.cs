// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
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

            const float BASE_INEQUALITY_EFFECT = 0.0075f;
            const float INEQUALITY_MULT_PER_RESOURCE_REGION = 0.15f;
            const float INEQUALITY_MULT_PER_INEQUALITY_LEVEL = 0.25f;

            // Effect is ~13.3 times weaker than welfare
            float baseInequalityGain = Tools.EffectStrength(BASE_INEQUALITY_EFFECT, __instance.population);
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * INEQUALITY_MULT_PER_RESOURCE_REGION);
            float inequalityMult = 1f + ((__instance.inequality - 5f) * INEQUALITY_MULT_PER_INEQUALITY_LEVEL);

            __result = baseInequalityGain * resourceRegionsMult * inequalityMult;


            return false; // Skip original method
        }
    }
}
