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
    [HarmonyPatch(typeof(TIGlobalValuesState), "ModifyMarketValuesForEconomyPriority")]
    public static class EconomyMarketEffectPatch
    {
        [HarmonyPrefix]
        public static bool ModifyMarketValuesForEconomyPriorityOverwrite(TIGlobalValuesState __instance)
        {
            //Patches the small increase to metal and noble metal price that is triggered by every economy investment completion
            //Expect 1500 vs vanilla's 200-400 economy completions a month, so value is 20% of the vanilla value

            __instance.resourceMarketValues[FactionResource.Metals] *= 1f + UnityEngine.Random.Range(2E-06f, 4E-06f);
            __instance.resourceMarketValues[FactionResource.NobleMetals] *= 1f + UnityEngine.Random.Range(1E-06f, 2E-06f);


            return false; //Skip original method
        }
    }
}
