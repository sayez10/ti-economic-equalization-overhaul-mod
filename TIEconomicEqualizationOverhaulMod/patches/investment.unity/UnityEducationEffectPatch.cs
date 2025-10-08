// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the knowledge effect of a unity investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.unityPriorityEducationChange), MethodType.Getter)]
    internal static class UnityEducationEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetUnityPriorityEducationChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_EDUCATION_EFFECT = -0.01f;

            // 1/10 effect of Knowledge priority
            __result = EconomyScorePatch.EffectStrength(BASE_EDUCATION_EFFECT, __instance.population);


            return false; // Skip original method
        }
    }
}
