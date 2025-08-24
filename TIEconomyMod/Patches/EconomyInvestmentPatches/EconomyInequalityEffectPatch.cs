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
    /// <summary>
    /// Patch changes the inequality effect of an economy investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "economyPriorityInequalityChange", MethodType.Getter)]
    public static class EconomyInequalityEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetEconomyPriorityInequalityChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_INEQUALITY = 0.0075f;
            const float INEQUALITY_MULT_PER_RESOURCE_REGION = 0.15f;

            // Effect is ~13.3 times weaker than welfare
            float baseInequalityGain = Tools.EffectStrength(BASE_INEQUALITY, __instance.population);
            float resourceRegionsMult = 1f + (__instance.currentResourceRegions * INEQUALITY_MULT_PER_RESOURCE_REGION);

            __result = baseInequalityGain * resourceRegionsMult;


            return false; // Skip original method
        }
    }
}
