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

            // Refer to EffectStrength() comments for explanation.
            float baseEffect = Tools.EffectStrength(-0.1f, __instance.population);

            // Each full point of sustainability give a +10% bonus, up to +100%.
            float sustainabilityMult = 1f + (__instance.sustainability * 0.1f);

            // Each nuked region causes a Sustainability generation malus of -5%, up to -50%.
            float nukedCounter = __instance.regions.Sum((TIRegionState x) => x.nuclearDetonations);
            float nukedMult = Mathf.Max(0.50f, 1f - (nukedCounter * 0.05f));

            // Base effect is first hit by the 'nuked' malus, then given the 'sustainability' bonus.
            __result = (baseEffect * nukedMult) * sustainabilityMult;



            return false; //Skip the original method
        }
    }
}
