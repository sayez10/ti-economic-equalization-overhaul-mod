// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the cohesion effect of a knowledge investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.knowledgePriorityCohesionChange), MethodType.Getter)]
    internal static class KnowledgeCohesionEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetKnowledgePriorityCohesionChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_COHESION_EFFECT = 0.1f;

            // Cohesion change is a centering effect, drawing it towards 5; additional logic is needed for that
            // Calculate the amount of change and prevent overshooting 5
            float cohesionChangeEffect = Math.Min(Math.Abs(__instance.cohesion - 5f), (Tools.EffectStrength(BASE_COHESION_EFFECT, __instance.population)));

            // Reduce cohesion instead if it's currently above 5
            if (__instance.cohesion > 5f)
            {
                cohesionChangeEffect *= -1f;
            }

            // Corruption reduces investment
            float corruptionMult = 1f - __instance.corruption;

            __result = cohesionChangeEffect * corruptionMult;


            return false; // Skip original method
        }
    }
}
