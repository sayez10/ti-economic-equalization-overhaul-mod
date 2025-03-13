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
    [HarmonyPatch(typeof(TINationState), "economyPriorityPerCapitaIncomeChange", MethodType.Getter)]
    public static class EconomyGDPEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetEconomyPriorityPerCapitaIncomeChangeOverwrite(ref float __result, TINationState __instance)
        {
            //This patch changes the economy investment's GDP effect from increasing gdp per capita by a flat(ish) amount, to increasing gdp by a flat(ish) amount and distributing that across the pop as gdp per capita
            //The most significant change is that GDP growth is dependent on an exponential decay function off of per capita GDP. This makes developing poor countries much more effective than developing rich ones, accounting for all factors

            // Written this way because it's easier for me to modify.
            // 0.25 * 1 billion; 0.25 billion
            float baseGDPChange = 0.25f * 1000000000f;

            float regionMult = 1f + (__instance.currentResourceRegions * 0.1f) + (__instance.numCoreEconomicRegions_dailyCache * 0.1f);
            float democracyMult = 1f + (__instance.democracy * 0.05f);
            float educationMult = 1f + (__instance.education * 0.15f);



            //Per capita GDP multiplier
            //This is an exponential decay function that gives countries with very low GDP per capita a large (up to 6 times!) bonus to growth
            //This is 6 for a country with 0 gdp per capita, about 3.25 for a country with 15k, 1.76 for a country with 30k, 1 for a country with 44k, and 0.34 for a country with 70k
            //In other words, countries with low gdp per capita will grow their absolute gdp per capita much faster than a country with higher, with things really speeding up for the first 15-20k, and really slowing down after 50k gdp per capita
            //float scalingMult = 6f * Mathf.Pow(0.96f, __instance.perCapitaGDP / 1000f);



            // Exponential decay function that gives low-PCGDP countries a considerable boost to growth. Heavily modified from the original mod author's vision.
            /* This was done because growth simply didn't make sense in the original mod. Poor countries grew WAY too fast, and rich countries were practically stagnant.
             * Take China and the US (the main countries I balanced this mod around) for example. In the real world, China's GDP 2022-2023 growth percentage was about 66% quicker than in the US. In the original run of this mod, the US had about 15% the growth rate of China.
             * With this new function, China should have a mostly-accurate natural growth rate compared to the US. More importantly, China still has a lot of potential but isn't so powerful that choosing any other major power is a bad idea.
             * Note that during testing, China invested 24% of its IP into Economy, and the US invested 21%. This is what the factionless AI does, so that is what I kept it at. Democracy and education have a strong effect on GDP growth, so the US' raw multiplier (at game start) is about half of China's, but after other factors it's ~60%.
             * I shudder at the thought of what a Full Democracy, 20 Education China would be capable of. China truly #1 LOLOLOL
             * 
             * Anyways, the big numbers:
             * 200% growth rate at 0 PCGDP
             * 150% growth rate at 21k PCGDP
             * 125% growth rate at 35k PCGDP
             * 100% growth rate at 51k PCGDP
             *  75% growth rate at 73k PCGDP
             *  50% growth rate at 102k PCGDP
             * 
             * The main takeaway from this is that poor nations get a strong bonus, which drops off relatively quickly. Rich ones get diminishing - but (hopefully) manageable - returns.
             * 
             */
            float scalingMult = 2f * Mathf.Pow(0.98f, __instance.perCapitaGDP / 1500f);

            float modifiedGDPChange = baseGDPChange * regionMult * democracyMult * educationMult * scalingMult;

            __result = modifiedGDPChange / __instance.population;



            return false; //Skip original getter
        }
    }
}
