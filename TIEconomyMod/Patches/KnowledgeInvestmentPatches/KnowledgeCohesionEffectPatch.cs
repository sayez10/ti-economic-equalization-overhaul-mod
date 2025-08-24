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
    /// Patch changes the cohesion effect of a knowledge investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "knowledgePriorityCohesionChange", MethodType.Getter)]
    public static class KnowledgeCohesionEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetKnowledgePriorityCohesionChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_COHESION = 0.1f;

            // Cohesion change is a centering effect, drawing it towards 5; additional logic is needed for that
            // Calculate the amount of change and prevent overshooting 5
            float cohesionChangeAmount = Math.Min(Mathf.Abs(__instance.cohesion - 5f), (Tools.EffectStrength(BASE_COHESION, __instance.population)));
            if (__instance.cohesion > 5f)
            {
                // Reduce cohesion instead if it's currently above 5
                cohesionChangeAmount *= -1f;
            }

            __result = cohesionChangeAmount;


            return false; // Skip original method
        }
    }
}
