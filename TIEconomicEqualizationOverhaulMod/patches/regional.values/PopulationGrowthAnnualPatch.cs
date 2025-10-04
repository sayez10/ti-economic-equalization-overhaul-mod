// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using UnityEngine;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the calculation of a region's annual population growth rate
    /// The initial national population change modifier now also expires after a fixed number of years, just like the regional population change modifier
    /// Add a cap on the education level malus on population growth (at level 8)
    /// Remove the cap of (currently) 180k on the GPDPC bonus on population growth
    /// Reduced the minimum global temperature anomaly required for population growth to decline from 8 K to 2 K
    /// Slightly increase base population growth and the GPDPC bonus
    /// Add a modifier to in increase population growth with lifespan extension technologies
    /// </summary>
    [HarmonyPatch(typeof(TIRegionState), nameof(TIRegionState.annualPopulationGrowth), MethodType.Getter)]
    internal static class PopulationGrowthAnnualPatch
    {
        // Lookup table for the strength of the global warming malus for different environment types
        private static readonly float[] _environmentTypeEffects = new float[4] {
            0f,   // None
            0.5f, // Beneficiary
            1f,   // Standard
            2f    // Vulnerable
        };

        [HarmonyPrefix]
        private static bool GetAnnualPopulationGrowthOverwrite(ref double __result, in TIRegionState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Those are vanilla numbers, some of them rounded
            const double BASE_POPULATION_CHANGE = 4.5d;
            // PavonisInteractive.TerraInvicta.TIRegionState.YEARS_UNTIL_POPULATION_REGRESSION is private and thus can't be accessed directly
            const float INITIAL_POPULATION_CHANGE_DURATION_YEARS = 25f;
            // Multiplication with reciprocal instead of division
            const float INITIAL_POPULATION_CHANGE_DURATION_YEARS_RECIPROCAL = 1f / INITIAL_POPULATION_CHANGE_DURATION_YEARS;

            const double EDUCATION_LEVEL_MALUS_CAP = 8d;

            const double POPULATION_CHANGE_PER_EDUCATION_LEVEL =         -0.418_191d;
            const double POPULATION_CHANGE_PER_COHESION_LEVEL =          -0.062_48d;
            const double POPULATION_CHANGE_PER_GDPPC =                    0.000_01d;
            const double POPULATION_CHANGE_PER_LATITUDE =                -0.115_74d;
            const double POPULATION_CHANGE_PER_XENOFORMING_LEVEL =       -0.005d;
            const int    POPULATION_CHANGE_PER_NUKE_DETONATED =          -4;
            const double POPULATION_CHANGE_PER_LIFEFIME_EXTENSION_YEAR = -0.014d;

            // The initial regional and national population growth modifiers expire over the course of 25 years after the start of a campaign
            // Calculate what's left of that timespan
            double remainingInitialPopulationChangeFraction = Math.Max(0d, (INITIAL_POPULATION_CHANGE_DURATION_YEARS - TITimeState.CampaignDuration_years_Exact()) * INITIAL_POPULATION_CHANGE_DURATION_YEARS_RECIPROCAL);

            // Effect of the remaining fraction of the initial regional and national population growth modifiers on a region's population growth rate
            double initialPopulationChangeAddend = (__instance.annualPopGrowthModifier + __instance.nation.template.popGrowthModifier) * remainingInitialPopulationChangeFraction;

            // Effect of the nation's education level on a region's population growth rate
            double educationAddend = Math.Min(EDUCATION_LEVEL_MALUS_CAP, __instance.nation.education) * POPULATION_CHANGE_PER_EDUCATION_LEVEL;

            // Effect of the nation's cohesion level on a region's population growth rate
            double cohesionAddend = __instance.nation.cohesion * POPULATION_CHANGE_PER_COHESION_LEVEL;

            // Effect of the nation's GDP per capita on a region's population growth rate
            double gdppcAddend = __instance.nation.perCapitaGDP * POPULATION_CHANGE_PER_GDPPC;

            // Effect of a region's latitude on its population growth rate
            double latitudeAddend = Math.Sqrt(Math.Abs(__instance.latitude)) * POPULATION_CHANGE_PER_LATITUDE;

            // Effect of a region's xenoforming level on its population growth rate
            double xenoformingAddend = __instance.xenoforming.xenoformingLevel * POPULATION_CHANGE_PER_XENOFORMING_LEVEL;

            // Effect of the number of past nuclear detonations in a region on its population growth rate
            double detonatedNukesAddend = __instance.nuclearDetonations * POPULATION_CHANGE_PER_NUKE_DETONATED;

            // Effect of lifespan extension technologies of the faction owning the executive CP of a nation on its regions' population growth rate
            double lifespanExtensionAddend = TIEffectsState.SumEffectsModifiers(Context.HumanLifespan, __instance, 1f, null) * POPULATION_CHANGE_PER_LIFEFIME_EXTENSION_YEAR;

            double populationGrowthBeforeGlobalWarmingEffects = BASE_POPULATION_CHANGE + initialPopulationChangeAddend + educationAddend + cohesionAddend + gdppcAddend + latitudeAddend + xenoformingAddend + detonatedNukesAddend + lifespanExtensionAddend;

            // Precaution to avoid unexpected results under extreme circumstances
            populationGrowthBeforeGlobalWarmingEffects = Mathd.Clamp(populationGrowthBeforeGlobalWarmingEffects, -10d, 10d);

            // The minimum global temperature anomaly required for population growth to decline
            const double MIN_GLOBAL_WARMING_FOR_POPULATION_DECLINE = 2d;

            // Effect of Global Warming on a region's population growth rate
            double globalWarmingAddend = Math.Max(0d, (Math.Abs(GameStateManager.GlobalValues().temperatureAnomaly_C) - MIN_GLOBAL_WARMING_FOR_POPULATION_DECLINE)) * _environmentTypeEffects[(int)__instance.template.environment];

            __result = Math.Max((populationGrowthBeforeGlobalWarmingEffects + globalWarmingAddend), -100d) * 0.01d;

//            double vanillaResult = Mathd.Max(Mathd.Clamp(4.49788037409348d + Mathd.Max(-4.49788037409348d, -0.418190741 * (double)__instance.nation.education) + -0.0624798523403752 * (double)__instance.nation.cohesion + 9.80843732089162E-06 * (double)Mathf.Min(180000f, __instance.nation.perCapitaGDP) + -0.115739931206548 * (double)Mathf.Sqrt(Mathf.Abs(__instance.latitude)) + (double)(__instance.annualPopGrowthModifier * Mathf.Max(0f, (25f - TITimeState.CampaignDuration_years_Exact()) / 25f)) + (double)__instance.nation.template.popGrowthModifier - (double)(__instance.xenoforming.xenoformingLevel / 200f) - (double)(__instance.nuclearDetonations * 4), -10.0, 10.0) - (double)(Math.Max(0f, Mathf.Abs(GameStateManager.GlobalValues().temperatureAnomaly_C) - 8f) * ((__instance.template.environment == EnvironmentType.Beneficiary) ? 0.5f : ((__instance.template.environment == EnvironmentType.Vulnerable) ? 2f : 1f))), -100.0) * 0.01;
//            FileLog.Log(string.Format($"[TIEconomyMod::PopulationGrowthPatch] Nation {__instance.nation.displayName}, Region {__instance.displayName}: Vanilla Pop Change: {vanillaResult}, Mod Pop Change: {__result}, lifespanExtensionAddend {lifespanExtensionAddend}"));


            return false; // Skip original method
        }
    }
}
