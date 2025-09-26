// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the economy investment's GDP effect from increasing GDPPC by a flat(ish) amount, to increasing GDP by a flat(ish) amount and distributing that across the population as GDPPC
    /// The most significant change is that GDP growth is dependent on an exponential decay function off of per capita GDP
    /// This makes developing poor countries much more effective than developing rich ones, accounting for all factors
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.economyPriorityPerCapitaIncomeChange), MethodType.Getter)]
    internal static class EconomyGDPEffectPatch
    {
        [HarmonyPrefix]
        private static bool GetEconomyPriorityPerCapitaIncomeChangeOverwrite(ref float __result, in TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_GDP_EFFECT = 250_000_000f;

            const float GROWTH_MULT_PER_SPECIAL_REGION = 0.1f;
            const float GROWTH_MULT_PER_DEMOCRACY_LEVEL = 0.05f;
            const float GROWTH_MULT_PER_EDUCATION_LEVEL = 0.15f;

            // Maximum GDP growth bonus from low per-capita GDP, as multiplier
            const float MAX_SCALING_MULT = 2f;

            // Base GDP growth diminishment rate, as multiplier
            const float DECAY_BASE = 0.98f;

            // Per-capita GDP increments for applying diminishment
            const float DECAY_INCREMENT_PER_CAPITA_GDP = 1_500f;

            float numSpecialRegions = __instance.currentResourceRegions + __instance.numCoreEconomicRegions_dailyCache;
            float specialRegionMult = 1f + (numSpecialRegions * GROWTH_MULT_PER_SPECIAL_REGION);
            float democracyMult = 1f + (__instance.democracy * GROWTH_MULT_PER_DEMOCRACY_LEVEL);
            float educationMult = 1f + (__instance.education * GROWTH_MULT_PER_EDUCATION_LEVEL);
            float bonusGDPPCMult = 1f + TIEffectsState.SumEffectsModifiers(Context.Economy_BasePCGDPIncrease, __instance, 1f);

            /*
             * Exponential decay function that gives low-GDPPC countries a considerable boost to growth. Heavily modified from the original mod author's vision.
             *
             * This was done because growth simply didn't make sense in the original mod. Poor countries grew WAY too fast, and rich countries were practically stagnant.
             * Take China and the US (the main countries I balanced this mod around) for example. In the real world, China's GDP 2022-2023 growth percentage was about 66% quicker than in the US.
             * In the original run of this mod, the US had about 15% the growth rate of China.
             * With this new function, China should have a mostly-accurate natural growth rate compared to the US.
             * More importantly, China still has a lot of potential but isn't so powerful that choosing any other major power is a bad idea.
             * Note that during testing, China invested 24% of its IP into Economy, and the US invested 21%. This is what the factionless AI does, so that is what I kept it at.
             * Democracy and education have a strong effect on GDP growth, so the US' raw multiplier (at game start) is about half of China's, but after other factors it's ~60%.
             *
             * Anyways, the big numbers, at default values:
             * 200% growth rate at 0 GDPPC
             * 150% growth rate at 21k GDPPC
             * 125% growth rate at 35k GDPPC
             * 100% growth rate at 51k GDPPC
             *  75% growth rate at 73k GDPPC
             *  50% growth rate at 102k GDPPC
             *
             * The main takeaway from this is that poor nations get a strong bonus, which drops off relatively quickly. Rich ones get diminishing - but (hopefully) manageable - returns.
             */
            float scalingMult = MAX_SCALING_MULT * (float)Math.Pow(DECAY_BASE, __instance.perCapitaGDP / DECAY_INCREMENT_PER_CAPITA_GDP);

            float gdpGain = BASE_GDP_EFFECT * specialRegionMult * democracyMult * educationMult * bonusGDPPCMult * scalingMult;

            __result = gdpGain / __instance.population;

            // FIXME: Verify that the changed formula actually works as intended. Might require a full-length game.
//            FileLog.Log(string.Format($"[TIEconomyMod::EconomyGDPEffectPatch] {__instance.displayName}: specialRegionMult: {specialRegionMult}, democracyMult: {democracyMult}, educationMult: {educationMult}, bonusGDPPCMult: {bonusGDPPCMult}, scalingMult: {scalingMult}"));


            return false; // Skip original method
        }
    }
}
