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
    [HarmonyPatch(typeof(TINationState), "economyPriorityInequalityChange", MethodType.Getter)]
    public static class EconomyInequalityEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetEconomyPriorityInequalityChangeOverwrite(ref float __result, TINationState __instance)
        {
            // Effect is ~13.3 times weaker than welfare.
            // Refer to EffectStrength() comments for explanation.
            float baseInequalityGain = Tools.EffectStrength(0.0075f, __instance.population);
            float resourceRegionsMult = 1f + ((float)__instance.currentResourceRegions * 0.15f);

            __result = baseInequalityGain * resourceRegionsMult;



            return false; //Skip original getter
        }
    }
}
