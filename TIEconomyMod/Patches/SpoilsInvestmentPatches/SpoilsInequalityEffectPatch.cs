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
    [HarmonyPatch(typeof(TINationState), "spoilsPriorityInequalityChange", MethodType.Getter)]
    public static class SpoilsInequalityEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetSpoilsPriorityInequalityChangeOverwrite(ref float __result, TINationState __instance)
        {
            //Patch changes the inequality effect of a spoils investment to scale inversely with population size
            //This keeps the inequality gain rate of nations of different populations but identical demographic stats otherwise the same

            //For a full explanation of the logic backing this change, see WelfareInequalityEffectPatch
            //I want an inequality gain rate of 0.05 a month for a 30k GDP per capita nation
            //Using the same method as with the welfare inequality, this gives me a single investment effect of 166667 / population democracy change

            // Refer to EffectStrength() comments for explanation.
            float inequalityGain = Tools.EffectStrength(0.05f, __instance.population);
            float resourceRegionsMult = 1f * (__instance.currentResourceRegions * 0.25f);

            __result = inequalityGain * resourceRegionsMult;

            return false; //Skip original getter
        }
    }
}
