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
    [HarmonyPatch(typeof(TINationState), "spoilsSustainabilityChange", MethodType.Getter)]
    public static class SpoilsSustainabilityEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetSpoilsSustainabilityChangeOverwrite(ref float __result, TINationState __instance)
        {
            // Changes spoils priority to scale inversly by population.
            // Overall, the scaling should have lesser extremes. Might need tweaking.

            // If mod has been disabled, abort patch and use original method.
            if (!Main.enabled) { return true; }

            // BASE_SUSTAINABILITY is inverted, because for whatever reason sustainability increases with negative change, and decreases with positive change.
            const float BASE_SUSTAINABILITY = 0.05f;
            const float SUSTAINABILITY_MULT_PER_RESOURCE_REGION = 0.25f;

            // Effect is 1/2 that of Environment.
            // Refer to EffectStrength() comments for explanation.
            float baseEffect = Tools.EffectStrength(BASE_SUSTAINABILITY, __instance.population);

            // Scaling is more aggressive than in Environment. We'll see if this ends up being a good idea.
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * SUSTAINABILITY_MULT_PER_RESOURCE_REGION);

            __result = baseEffect * resourceRegionsMult;


            return false; // Skip original method
        }
    }
}
