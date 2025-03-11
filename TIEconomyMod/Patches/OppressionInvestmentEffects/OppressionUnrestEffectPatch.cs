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
    [HarmonyPatch(typeof(TINationState), "OppressionPriorityUnrestChange", MethodType.Getter)] /* Was 'knowledgePriorityDemocracyChange', but it looks like the method was renamed to 'governmentPriorityDemocracyChange'. Seems to work. */
    public static class OppressionUnrestEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetOppressionPriorityUnrestChangeOverwrite(ref float __result, TINationState __instance)
        {
            // Changes Oppression so its effect scales inversely with population.

            // VERY powerful for totalitarian nations...especially if they're rich. In practice, no country will get this close without heavy player intervention.
            // Refer to EffectStrength() comments for explanation.
            float baseEffect = Tools.EffectStrength(-5.0f, __instance.population);

            // Effect is reduced by Democracy, -10% per full point. At full Democracy, Oppression does nothing to Unrest.
            float adjustedEffect = baseEffect * (1f * (__instance.democracy * 0.1f));

            // For whatever reason, vanilla code explicitly disallows a value that'd go under 0. I'm playing it safe by doing that too.
            __result = Mathf.Min(-__instance.unrest, baseEffect);



            return false; //Skip original getter
        }
    }
}
