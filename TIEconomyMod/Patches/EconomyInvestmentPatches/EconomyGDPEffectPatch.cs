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

            float baseGDPChange = 330000000f; //One investment gives a base amount of 0.33 Bn GDP
            //float basePerCapGDPChange = baseGDPChange / __instance.population;

            float regionMult = 1f + (__instance.currentResourceRegions * 0.1f) + (__instance.numCoreEconomicRegions_dailyCache * 0.1f);
            float democracyMult = 1f + (__instance.democracy * 0.05f);
            float educationMult = 1f + (__instance.education * 0.15f);

            //Per capita GDP multiplier
            //This is an exponential decay function that gives countries with very low GDP per capita a large (up to 6 times!) bonus to growth
            //This is 6 for a country with 0 gdp per capita, about 3.25 for a country with 15k, 1.76 for a country with 30k, 1 for a country with 44k, and 0.34 for a country with 70k
            //In other words, countries with low gdp per capita will grow their absolute gdp per capita much faster than a country with higher, with things really speeding up for the first 15-20k, and really slowing down after 50k gdp per capita
            float scalingMult = 6f * Mathf.Pow(0.96f, __instance.perCapitaGDP / 1000f);

            float modifiedGDPChange = baseGDPChange * regionMult * democracyMult * educationMult * scalingMult;

            __result = modifiedGDPChange / __instance.population;



            return false; //Skip original getter
        }
    }
}
