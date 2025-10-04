// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the N2O removal effect of an environment investment at 10 sustainment to not scale with population anymore
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.EnvPriorityN2ORemoved))]
    internal static class EnvironmentN2ORemovalPatch
    {
        [HarmonyPrefix]
        private static bool EnvPriorityN2ORemovedOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float N2O_REMOVAL_MULTIPLIER = 1f;

            __result = (TemplateManager.global.WelN2O_ppm + TIEffectsState.SumEffectsModifiers(Context.Welfare_N2O_ppm, __instance, TemplateManager.global.WelN2O_ppm)) * N2O_REMOVAL_MULTIPLIER;


            return false; // Skip original method
        }
    }
}
