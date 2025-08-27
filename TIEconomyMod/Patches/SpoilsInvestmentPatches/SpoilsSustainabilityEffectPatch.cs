// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT



using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the sustainability effect of a spoils completion to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.spoilsSustainabilityChange), MethodType.Getter)]
    public static class SpoilsSustainabilityEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetSpoilsSustainabilityChangeOverwrite(ref float __result, TINationState __instance)
        {
            // FIXME: Overall, the scaling should have lesser extremes. Might need tweaking.

            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // BASE_SUSTAINABILITY is inverted, because for whatever reason sustainability increases with negative change, and decreases with positive change
            const float BASE_SUSTAINABILITY = 0.2f;
            const float SUSTAINABILITY_MULT_PER_RESOURCE_REGION = 1f;

            float baseEffect = Tools.EffectStrength(BASE_SUSTAINABILITY, __instance.population);

            // Scaling is more aggressive than in Environment
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * SUSTAINABILITY_MULT_PER_RESOURCE_REGION);

            __result = baseEffect * resourceRegionsMult;


            return false; // Skip original method
        }
    }
}
