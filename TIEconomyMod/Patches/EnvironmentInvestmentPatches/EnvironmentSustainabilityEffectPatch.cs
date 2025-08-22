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
    [HarmonyPatch(typeof(TINationState), "environmentPrioritySustainabilityChange", MethodType.Getter)]
    public static class EnvironmentSustainabilityEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetEnvironmentPrioritySustainabilityChangeOverwrite(ref float __result, TINationState __instance)
        {
            // Changes environment priority to scale inversly by population, while still being affected by other modifiers.
            // Overall, the scaling should have lesser extremes. Might need tweaking.

            // If mod has been disabled, abort patch and use original method.
            if (!Main.enabled) { return true; }

            // For whatever reason, sustainability is increased with negative values.
            const float BASE_SUSTAINABILITY = -0.1f;
            const float SUSTAINABILITY_MULT_PER_SUSTAINABILITY_LEVEL = 0.1f;
            const float PENALTY_PER_NUKED_REGION = 0.05f;
            const float MAX_PENALTY_FROM_NUKED_REGIONS = 0.5f;

            // Refer to EffectStrength() comments for explanation.
            float baseEffect = Tools.EffectStrength(BASE_SUSTAINABILITY, __instance.population);

            // Each full point of sustainability give a +10% bonus, up to +100%.
            float sustainabilityMult = 1f + (__instance.sustainability * SUSTAINABILITY_MULT_PER_SUSTAINABILITY_LEVEL);

            // Each nuked region causes a Sustainability generation malus of -5%, up to -50%.
            float nukedCounter = __instance.regions.Sum((TIRegionState x) => x.nuclearDetonations);
            float nukedMult = Mathf.Max(MAX_PENALTY_FROM_NUKED_REGIONS, 1f - (nukedCounter * PENALTY_PER_NUKED_REGION));

            __result = baseEffect * nukedMult * sustainabilityMult;


            return false; // Skip original method
        }
    }
}
