// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patches the getter for control cost of a single control point for a nation
    /// Formula now uses a power function again
    /// The effect of five global technologies to reduce the control point cost of CPs by up to 75% was removed
    /// The CP cost of nations with low GDP has been increased relative to vanilla, that of rich nations has been decreased
    /// Unifying nations is now required to reduce the CP cost
    /// The global techs, which had reduced CP cost in previous versions of this mod, aren't critical anymore
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.ControlPointMaintenanceCost), MethodType.Getter)]
    internal static class ControlPointCostPatch
    {
        [HarmonyPrefix]
        private static bool GetControlPointMaintenanceCostOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            if (!__instance.alienNation)
            {
                // Re-using the nation's missionDifficultyEconomyScore to save one division and one Math.Pow() call
//              float baseControlPointCost = (float)Math.Pow(__instance.GDP / 1_000_000_000d, 0.333_333_34d);
                float baseControlPointCost = __instance.missionDifficultyEconomyScore;

                // Settings values cached for readability
                float settingsMult = Main.settings.controlPointCostMult;

                // Total cost is split across the control points
                __result = (baseControlPointCost * settingsMult) / __instance.numControlPoints;

//                float vanillaResult = (float)(Math.Pow(__instance.GDP / 1_000_000_000d, (double)TIGlobalConfig.globalConfig.controlPointCostScaling) / (double)(TemplateManager.global.controlPointMaintenanceDivisor * (float)__instance.numControlPoints));
//                FileLog.Log(string.Format($"[TIEconomyMod::ControlPointCostPatch] {__instance.displayName}: CP Cost in Vanilla: {vanillaResult}, CP Cost in Mod: {__result}"));
            }
            else
            {
                // No control point cost for the Alien Administration
                __result = 0f;
            }

            return false; // Skip original method
        }
    }
}
