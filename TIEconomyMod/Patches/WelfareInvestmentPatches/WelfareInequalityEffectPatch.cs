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
    /// Patch changes the inequality effect of a welfare investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "welfarePriorityInequalityChange", MethodType.Getter)]
    public static class WelfareInequalityEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetWelfarePriorityInequalityChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_INEQUALITY = -0.1f;

            __result = Tools.EffectStrength(BASE_INEQUALITY, __instance.population);


            return false; // Skip original method
        }
    }
}
