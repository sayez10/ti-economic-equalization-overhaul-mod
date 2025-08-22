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
    [HarmonyPatch(typeof(TINationState), "research_month", MethodType.Getter)]
    public static class ResearchPointsPatch
    {
        [HarmonyPrefix]
        public static bool GetResearchMonthOverwrite(ref float __result, TINationState __instance)
        {
            // Patches the amount of research points available to a nation
            // This removes both the flat research amount every country gets based on its education score, and the ^1.1 scaling on population size

            // If mod has been disabled, abort patch and use original method.
            if (!Main.enabled) { return true; }

            // Settings values cached for readability.
            float baseResearchAtPCGDP = Main.settings.researchProduction.baseResearchAtPCGDP;
            float worstMalus = Main.settings.researchProduction.worstMalus;
            float researchOffset = Main.settings.researchProduction.researchOffset;


            // Now, it would be nice to go into a really precise and comprehensive calculation here to figure out how much research per million pop I should give
            // But the reality is that this is way too complicated a relationship between population, GDP, GDP per capita, and the various multipliers here
            // So I'm gonna go with old faithful: Guess and check some values until it's where I want it to be.

            // Increased vanilla 0.00225 multiplier to 0.00370
            float baseResearch = 0.00370f;

            // Removed vanilla better-than-linear scaling.
            float popMultiplier = __instance.population_Millions;

            // Vanilla power 2
            float educationEffect = Mathf.Pow(__instance.education, 2.0f);

            // Floored at 60% research at 12000 gdp per capita, 200% at 40k, and indefinitely upward
            float gdpPerCapEffect = Mathf.Max(__instance.perCapitaGDP / baseResearchAtPCGDP, worstMalus);

            // Vanilla, gives about 60% boost at 10 democracy, 40% penalty at 0
            float democracyEffect = Mathf.Pow(Mathf.Max(__instance.democracy, 0.1f), 0.2f);

            // Vanilla, get 25% research boost at 5 cohesion, 25% penalty at 0 or 10 cohesion
            float cohesionEffect = (1.25f - Mathf.Abs(__instance.cohesion - 5f) / 10f);

            // Vanilla, get 100% research at <=2 unrest, up to 80% penalty at 10 unrest
            float unrestEffect = (1f - Mathf.Max(__instance.unrest - 2f, 0f) / 10f);

            // Vanilla, up to 25% boost at 25 councilor science score
            float advisorBonus = (1f + __instance.adviserScienceBonus);

            __result = baseResearch * popMultiplier * educationEffect * gdpPerCapEffect * democracyEffect * cohesionEffect * unrestEffect * advisorBonus * researchOffset;


            return false; // Skip original method
        }
    }
}
