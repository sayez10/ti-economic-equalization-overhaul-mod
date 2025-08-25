// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT



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
    /// Patch changes the economy investment's GDP effect from increasing GDPPC by a flat(ish) amount, to increasing GDP by a flat(ish) amount and distributing that across the population as GDPPC
    /// The most significant change is that GDP growth is dependent on an exponential decay function off of per capita GDP
    /// This makes developing poor countries much more effective than developing rich ones, accounting for all factors
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "economyPriorityPerCapitaIncomeChange", MethodType.Getter)]
    public static class EconomyGDPEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetEconomyPriorityPerCapitaIncomeChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Base GDP change in billions, written this way because it's easier to modify
            const float BASE_GDP_CHANGE_BILLIONS = 0.25f;
            const float BASE_GDP_CHANGE = BASE_GDP_CHANGE_BILLIONS * 1000000000f;

            const float GROWTH_MULT_PER_SPECIAL_REGION = 0.1f;
            const float GROWTH_MULT_PER_DEMOCRACY_LEVEL = 0.05f;
            const float GROWTH_MULT_PER_EDUCATION_LEVEL = 0.15f;

            // Maximum GDP growth bonus from low per-capita GDP, as multiplier
            const float MAX_SCALING_MULT = 2f;

            // Base GDP growth diminishment rate, as multiplier
            const float DECAY_FACTOR = 0.98f;

            // Per-capita GDP increments for applying diminishment
            const float DECAY_INCREMENT_PER_CAPITA_GDP = 1500f;

            float numSpecialRegions = __instance.currentResourceRegions + __instance.numCoreEconomicRegions_dailyCache;
            float specialRegionMult = 1f + (numSpecialRegions * GROWTH_MULT_PER_SPECIAL_REGION);
            float democracyMult = 1f + (__instance.democracy * GROWTH_MULT_PER_DEMOCRACY_LEVEL);
            float educationMult = 1f + (__instance.education * GROWTH_MULT_PER_EDUCATION_LEVEL);
            float bonusPCGDPMult = TIEffectsState.SumEffectsModifiers(Context.Economy_BasePCGDPIncrease, __instance, 1f);

            /*
             * Exponential decay function that gives low-PCGDP countries a considerable boost to growth. Heavily modified from the original mod author's vision.
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
             * 200% growth rate at 0 PCGDP
             * 150% growth rate at 21k PCGDP
             * 125% growth rate at 35k PCGDP
             * 100% growth rate at 51k PCGDP
             *  75% growth rate at 73k PCGDP
             *  50% growth rate at 102k PCGDP
             *
             * The main takeaway from this is that poor nations get a strong bonus, which drops off relatively quickly. Rich ones get diminishing - but (hopefully) manageable - returns.
             */
            float scalingMult = MAX_SCALING_MULT * Mathf.Pow(DECAY_FACTOR, __instance.perCapitaGDP / DECAY_INCREMENT_PER_CAPITA_GDP);

            float modifiedGDPChange = BASE_GDP_CHANGE * specialRegionMult * democracyMult * educationMult * bonusPCGDPMult + scalingMult;

            __result = modifiedGDPChange / __instance.population;


            return false; // Skip original method
        }
    }
}
