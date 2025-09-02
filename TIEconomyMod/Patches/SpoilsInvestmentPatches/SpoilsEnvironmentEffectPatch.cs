// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch overwrites the vanilla greenhouse gas emissions values for completing a spoils investment
    /// Removes the scaling to emissions added in the vanilla function that does not work with this mod
    /// </summary>
    [HarmonyPatch(typeof(TIGlobalValuesState), nameof(TIGlobalValuesState.AddSpoilsPriorityEnvEffect))]
    internal static class SpoilsEnvironmentEffectPatch
    {
        [HarmonyPrefix]
        private static bool AddSpoilsPriorityEnvEffectOverwrite(in TINationState nation, TIGlobalValuesState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float ENVIRONMENT_MULT_PER_RESOURCE_REGION = 0.5f;
            float resourceRegionsMult = 1f + (nation.currentResourceRegions * ENVIRONMENT_MULT_PER_RESOURCE_REGION);

            // CO2
            float effectCO2 = TemplateManager.global.SpoCO2_ppm * resourceRegionsMult;
            __instance.AddCO2_ppm(effectCO2, GHGSources.SpoilsPriority);

            // CH4
            float effectCH4 = TemplateManager.global.SpoCH4_ppm * resourceRegionsMult;
            __instance.AddCH4_ppm(effectCH4, GHGSources.SpoilsPriority);

            // N2O
            float effectN2O = TemplateManager.global.SpoN2O_ppm * resourceRegionsMult;
            __instance.AddN2O_ppm(effectN2O, GHGSources.SpoilsPriority);


            return false; // Skip original method
        }
    }
}
