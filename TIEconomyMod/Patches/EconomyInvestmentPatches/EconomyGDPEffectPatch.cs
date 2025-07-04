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

            // If mod has been disabled, abort patch and use original method.
            if (!Main.enabled) { return true; }

            // Settings values are cached for readability.
            float baseChangeBillions = Main.settings.econonyInvestmentGDP.baseChangeBillions;
            float growthMultPerSpecialRegion = Main.settings.econonyInvestmentGDP.growthMultPerSpecialRegion;
            float growthMultPerDemocracyLevel = Main.settings.econonyInvestmentGDP.growthMultPerDemocracyLevel;
            float growthMultPerEducationLevel = Main.settings.econonyInvestmentGDP.growthMultPerEducationLevel;
            float maxScaleFactor = Main.settings.econonyInvestmentGDP.maxScaleFactor;
            float decayFactor = Main.settings.econonyInvestmentGDP.decayFactor;
            float decayIncrementPerCapitaGDP = Main.settings.econonyInvestmentGDP.decayIncrementPerCapitaGDP;

            // Written this way because it's easier for me to modify.
            float baseGDPChange = baseChangeBillions * 1000000000f;

            float regionMult = 1f + (__instance.currentResourceRegions * growthMultPerSpecialRegion) + (__instance.numCoreEconomicRegions_dailyCache * growthMultPerSpecialRegion);
            float democracyMult = 1f + (__instance.democracy * growthMultPerDemocracyLevel);
            float educationMult = 1f + (__instance.education * growthMultPerEducationLevel);

            // Exponential decay function that gives low-PCGDP countries a considerable boost to growth. Heavily modified from the original mod author's vision.
            /* This was done because growth simply didn't make sense in the original mod. Poor countries grew WAY too fast, and rich countries were practically stagnant.
             * Take China and the US (the main countries I balanced this mod around) for example. In the real world, China's GDP 2022-2023 growth percentage was about 66% quicker than in the US. In the original run of this mod, the US had about 15% the growth rate of China.
             * With this new function, China should have a mostly-accurate natural growth rate compared to the US. More importantly, China still has a lot of potential but isn't so powerful that choosing any other major power is a bad idea.
             * Note that during testing, China invested 24% of its IP into Economy, and the US invested 21%. This is what the factionless AI does, so that is what I kept it at. Democracy and education have a strong effect on GDP growth, so the US' raw multiplier (at game start) is about half of China's, but after other factors it's ~60%.
             * I shudder at the thought of what a Full Democracy, 20 Education China would be capable of. China truly #1 LOLOLOL
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
            float scalingMult = maxScaleFactor * Mathf.Pow(decayFactor, __instance.perCapitaGDP / decayIncrementPerCapitaGDP);

            float modifiedGDPChange = baseGDPChange * regionMult * democracyMult * educationMult * scalingMult;

            __result = modifiedGDPChange / __instance.population;




            // Below is an experiment in making this function change GDP as a percentage rather than 
            //float baseGDPGrowthPercentage = 3.0f / 100f; //current calculations say: 8.5f
            //float GTemp = baseGDPGrowthPercentage + (0.77f / 100f); //TEMP, climate change crutch
            //float GTemp2 = GTemp / 0.23f; //TEMP, should get AI-driven US 3% growth/year

            // Distributes GDP growth across every IP 
            //float growthPerDay = GTemp / 365;
            //float growthPerIP = growthPerDay / __instance.economyScore;

            //__result = ((float)__instance.GDP / growthPerIP) / __instance.population;



            return false; //Skip original getter
        }
    }
}
