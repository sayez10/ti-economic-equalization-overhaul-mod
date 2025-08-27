// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the amount of research points available to a nation
    /// Removes the flat research amount every country gets based on its education score
    /// Output now scales linearly with GDP
    /// Quadratic scaling with knowledge is no longer capped at knowledge == 12
    /// Added a research malus for nations with democracy below 5 and simplified formula
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.research_month), MethodType.Getter)]
    public static class ResearchPointsPatch
    {
        [HarmonyPrefix]
        public static bool GetResearchMonthOverwrite(ref float __result, TINationState __instance)
        {

            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Settings values cached for readability
            float researchMult = Main.settings.researchMult;

            const float BASE_RESEARCH = 0.0000002f;

            // Linear scaling with population
            float popMult = __instance.population_Millions;

            // As vanilla, but power 2 not capped at education == 12
            float educationEffect = __instance.education * __instance.education;

            // Linear scaling with GDP
            float gdpPerCapEffect = __instance.perCapitaGDP;

            // Get 50% bonus at 10 democracy, 50% penalty at 0
            float democracyEffect = 0.5f + (__instance.democracy * 0.1f);;

            // As vanilla, get 25% research bonus at 5 cohesion, 25% penalty at 0 or 10 cohesion
            float cohesionEffect = 1.25f - (Math.Abs(__instance.cohesion - 5f) * 0.1f);

            // As vanilla, get 100% research at 0 unrest, increasing quadratically to 0% at 10 unrest
            float unrestEffect = 1f - (__instance.unrest * __instance.unrest * 0.01f);

            // As vanilla, up to 25% bonus at 25 councilor science score
            float advisorBonus = 1f + __instance.adviserScienceBonus;

            __result = BASE_RESEARCH * researchMult * popMult * educationEffect * gdpPerCapEffect * democracyEffect * cohesionEffect * unrestEffect * advisorBonus;


            return false; // Skip original method
        }
    }
}
