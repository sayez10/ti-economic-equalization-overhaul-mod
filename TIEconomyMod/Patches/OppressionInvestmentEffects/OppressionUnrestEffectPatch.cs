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
    [HarmonyPatch(typeof(TINationState), "OppressionPriorityUnrestChange", MethodType.Getter)]
    public static class OppressionUnrestEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetOppressionPriorityUnrestChangeOverwrite(ref float __result, TINationState __instance)
        {
            // Changes Oppression so its effect scales inversely with population.

            // If mod has been disabled, abort patch and use original method.
            if (!Main.enabled) { return true; }

            // Settings values are cached for readability.
            const float BASE_UNREST = -2.5f;
            const float UNREST_PENALTY_MULT_PER_DEMOCRACY_LEVEL = 0.1f;

            // VERY powerful for totalitarian nations... especially if they're rich. In practice, no country will get this close unless controlled by the player.
            // Refer to EffectStrength() comments for explanation
            float baseEffect = Tools.EffectStrength(BASE_UNREST, __instance.population);

            // Effect is reduced by Democracy, -10% per full point. At full Democracy, Oppression does nothing to Unrest.
            float adjustedEffect = baseEffect * (1f - (__instance.democracy * UNREST_PENALTY_MULT_PER_DEMOCRACY_LEVEL));

            // For whatever reason, vanilla code explicitly disallows a value that'd go under 0. I'm playing it safe by doing that too.
            __result = 0f - Mathf.Min(__instance.unrest, baseEffect);


            return false; // Skip original method
        }
    }
}
