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

            // Effect is 1/2 that of Environment.
            // Refer to EffectStrength() comments for explanation.
            float baseEffect = Tools.EffectStrength(0.05f, __instance.population);

            // Scaling is more aggressive than in Environment. We'll see if this ends up being a good idea.
            float regionMult = 1f + (__instance.currentResourceRegions * 0.25f);

            __result = baseEffect * regionMult;



            return false; //Skip the original method
        }
    }
}
