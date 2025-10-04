// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the CO2 removal effect of an environment investment at 10 sustainment to not scale with population anymore
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.EnvPriorityCO2Removed))]
    internal static class EnvironmentCO2RemovalPatch
    {
        [HarmonyPrefix]
        private static bool EnvPriorityCO2RemovedOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float CO2_REMOVAL_MULTIPLIER = 1f;

            __result = (TemplateManager.global.WelCO2_ppm + TIEffectsState.SumEffectsModifiers(Context.Welfare_CO2_ppm, __instance, TemplateManager.global.WelCO2_ppm)) * CO2_REMOVAL_MULTIPLIER;


            return false; // Skip original method
        }
    }
}
