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

            // Now, it would be nice to go into a really precise and comprehensive calculation here to figure out how much research per million pop I should give
            // But the reality is that this is way too complicated a relationship between population, GDP, GDP per capita, and the various multipliers here
            // So I'm gonna go with old faithful: Guess and check some values until it's where I want it to be.

            // Settings values cached for readability.
            float researchMult = Main.settings.researchMult;

            const float BASE_RESEARCH = 0.0000002f;

            // Linear scaling with population
            float popMult = __instance.population_Millions;

            // Vanilla power 2
            float educationEffect = __instance.education * __instance.education;

            // Linear scaling with GDP
            float gdpPerCapEffect = __instance.perCapitaGDP;

            // Get 50% bonus at 10 democracy, 50% penalty at 0
            float democracyEffect = 0.5f + (__instance.democracy * 0.1f);;

            // Vanilla, get 25% research bonus at 5 cohesion, 25% penalty at 0 or 10 cohesion
            float cohesionEffect = 1.25f - (Mathf.Abs(__instance.cohesion - 5f) * 0.1f);

            // Vanilla, get 100% research at 0 unrest, increasing quadratically to 0% at 10 unrest
            float unrestEffect = 1f - (__instance.unrest * __instance.unrest * 0.01f);

            // Vanilla, up to 25% bonus at 25 councilor science score
            float advisorBonus = 1f + __instance.adviserScienceBonus;

            __result = BASE_RESEARCH * popMult * educationEffect * gdpPerCapEffect * democracyEffect * cohesionEffect * unrestEffect * advisorBonus * researchMult;


            return false; // Skip original method
        }
    }
}
