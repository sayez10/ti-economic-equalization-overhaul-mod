using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Pending deletion; Knowledge no longer has a Democracy modifier. */

namespace TIEconomyMod
{
    [HarmonyPatch(typeof(TINationState), "OppressionPriorityDemocracyChange", MethodType.Getter)] /* Was 'knowledgePriorityDemocracyChange', but it looks like the method was renamed to 'governmentPriorityDemocracyChange'. Seems to work. */
    public static class OppressionDemocracyEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetOppressionPriorityDemocracyChangeOverwrite(ref float __result, TINationState __instance)
        {

            // About 35% of (base) Government effect.
            // Refer to EffectStrength() comments for explanation.
            __result = Tools.EffectStrength(0.0175f, __instance.population);



            return false; //Skip original getter
        }
    }
}
