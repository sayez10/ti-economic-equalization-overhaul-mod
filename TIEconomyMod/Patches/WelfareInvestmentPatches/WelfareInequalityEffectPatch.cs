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
    [HarmonyPatch(typeof(TINationState), "welfarePriorityInequalityChange", MethodType.Getter)]
    public static class WelfareInequalityEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetWelfarePriorityInequalityChangeOverwrite(ref float __result, TINationState __instance)
        {
            // Refer to EffectStrength() comments for explanation.
            __result = Tools.EffectStrength(-0.1f, __instance.population);



            return false; //Skip original getter
        }
    }
}
