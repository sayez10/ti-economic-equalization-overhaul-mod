using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/* Pending deletion; Knowledge no longer has a Democracy modifier. */

namespace TIEconomyMod
{
    [HarmonyPatch(typeof(TINationState), "OppressionPriorityCohesionChange", MethodType.Getter)] /* Was 'knowledgePriorityDemocracyChange', but it looks like the method was renamed to 'governmentPriorityDemocracyChange'. Seems to work. */
    public static class OppressionCohesionEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetOppressionPriorityCohesionChangeOverwrite(ref float __result, TINationState __instance)
        {
            // Refer to EffectStrength() comments for explanation.
            float baseEffect = Tools.EffectStrength(-1.0f, __instance.population);

            // Effect ramps up the higher Democracy is. 0% at/under 5, 100% at 10.
            float democracyMult = Mathf.Max(0f, 0.2f * (__instance.democracy - 5));

            __result = baseEffect * democracyMult;



            return false; //Skip original getter
        }
    }
}
