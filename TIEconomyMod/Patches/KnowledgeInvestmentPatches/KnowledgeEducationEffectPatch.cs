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
    [HarmonyPatch(typeof(TINationState), "knowledgePriorityEducationChange", MethodType.Getter)]
    public static class KnowledgeEducationEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetKnowledgePriorityEducationChangeOverwrite(ref float __result, TINationState __instance)
        {
            // Due to the moving of the Democracy effect from this to its own priority, a buff to this one may be warranted.

            // If mod has been disabled, abort patch and use original method.
            if (!Main.enabled) { return true; }

            const float BASE_EDUCATION = 0.1f;
            const float MAX_SCALE_FACTOR = 4f;
            const float DECAY_FACTOR = 0.87f;

            // Refer to EffectStrength() comments for explanation.
            float baseChange = Tools.EffectStrength(BASE_EDUCATION, __instance.population);


            // Additionally, scale the education change based on current education, using an exponential decay relationship
            // With a multiplier of 4, and a base of 0.87, we get:
            // 400% education gain at 0 education, 200% at 5 education, 100% at 10 education, 50% at 15 education, etc...
            // Basically, the effectiveness of the knowledge priority halves every 5 education.
            float decayMult = MAX_SCALE_FACTOR * (float)Mathd.Pow(DECAY_FACTOR, __instance.education);

            __result = baseChange * decayMult;


            return false; // Skip original method
        }
    }
}
