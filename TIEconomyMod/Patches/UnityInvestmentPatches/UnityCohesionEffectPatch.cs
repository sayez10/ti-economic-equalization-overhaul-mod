// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;




namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the cohesion effect of a unity investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.unityPriorityCohesionChange), MethodType.Getter)]
    public static class UnityCohesionEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetUnityPriorityCohesionChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_COHESION = 1f;
            const float MALUS_LIMIT = 0.5f;
            const float COHESION_PENALTY_MULT_PER_EDUCATION_AND_DEMOCRACY_LEVEL = 0.025f;

            float baseEffect = Tools.EffectStrength(BASE_COHESION, __instance.population);

            // Democracy and Education incurs a 2.5% malus per point, up to -50%
            // A combined score of 20 causes the max effect
            float penaltyMult = Math.Min(MALUS_LIMIT, 1f - ((__instance.education + __instance.democracy) * COHESION_PENALTY_MULT_PER_EDUCATION_AND_DEMOCRACY_LEVEL));

            __result = baseEffect * penaltyMult;


            return false; // Skip original method
        }
    }
}
