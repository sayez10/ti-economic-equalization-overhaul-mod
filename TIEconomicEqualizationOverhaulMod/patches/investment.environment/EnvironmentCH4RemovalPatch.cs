// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the CH4 removal effect of an environment investment at 10 sustainment to not scale with population anymore
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.EnvPriorityCH4Removed))]
    internal static class EnvironmentCH4RemovalPatch
    {
        [HarmonyPrefix]
        private static bool EnvPriorityCH4RemovedOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float CH4_REMOVAL_MULTIPLIER = 1f;

            __result = (TemplateManager.global.WelCH4_ppm + TIEffectsState.SumEffectsModifiers(Context.Welfare_CH4_ppm, __instance, TemplateManager.global.WelCH4_ppm)) * CH4_REMOVAL_MULTIPLIER;


            return false; // Skip original method
        }
    }
}
