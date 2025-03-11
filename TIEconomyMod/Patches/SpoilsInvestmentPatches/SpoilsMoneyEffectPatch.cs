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
    [HarmonyPatch(typeof(TINationState), "spoilsPriorityMoney", MethodType.Getter)]
    public static class SpoilsMoneyEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetSpoilsPriorityMoneyOverwrite(ref float __result, TINationState __instance)
        {
            //Patch changes the instant money effect of a spoils investment to be a flat value, not scaled by nation size

            float baseMoneyGained = 240f; //Base 240 money

            //Add money per resource region.
            float resourceRegionBonusMoney = __instance.currentResourceRegions * 40f;

            //Up to 30% extra money at 0 democracy, 0% extra at 10 democracy
            float democracyMult = 1.3f - (__instance.democracy * (0.3f / 10f));

            __result = (baseMoneyGained + resourceRegionBonusMoney) * democracyMult;


            return false; //Skip original getter
        }
    }
}
