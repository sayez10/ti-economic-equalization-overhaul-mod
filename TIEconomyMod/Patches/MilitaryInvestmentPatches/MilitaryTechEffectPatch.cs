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
    [HarmonyPatch(typeof(TINationState), "militaryPriorityTechLevelChange", MethodType.Getter)]
    public static class MilitaryTechEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetMilitaryPriorityTechLevelChangeOverwrite(ref float __result, TINationState __instance)
        {
            //Patch changes the military tech effect of a military investment to scale inversely with population size
            //It also adds a catch-up boost to gain based on how far behind the global maximum tech level the country is
            //This keeps the military tech improvement rate of nations of different populations but identical demographic stats otherwise the same

            // Refer to EffectStrength() comments for explanation.
            float baseEffect = Tools.EffectStrength(0.025f, __instance.population);


            //Additionally, add a catch-up multiplier dependent on how far behind the max tech level the country is
            //A bonus 50% tech gain per full tech level behind the global max
            float catchUpMult = Mathf.Max(1.0f + (0.5f * (__instance.maxMilitaryTechLevel - __instance.militaryTechLevel)), 1.0f); //Max to 1 is to prevent weirdness if somehow mil tech is above max mil tech

            __result = baseEffect * catchUpMult;



            return false; //Skip original getter
        }
    }
}
