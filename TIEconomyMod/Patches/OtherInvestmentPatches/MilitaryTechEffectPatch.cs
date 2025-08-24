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
    /// Patch changes the military tech effect of a military investment to scale only with existing military units (armies and navies)
    /// Population size doesn't affect the result
    /// It also adds a catch-up boost to gain based on how far behind the global maximum tech level the country is
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "militaryPriorityTechLevelChange", MethodType.Getter)]
    public static class MilitaryTechEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetMilitaryPriorityTechLevelChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_MILITARY = 0.001f;
            const float MILITARY_MALUS_PER_ARMY_AND_NAVY = -0.00005f;
            const float MILITARY_MALUS_LIMIT = -0.0005f;
            const float MILITARY_PER_MILITARY_LEVEL_BEHIND = 0.5f;
            const float MILITARY_MULT_PER_EDUCATION_LEVEL = 0.1f;

            // FIXME: Formula is too simplistic. Let malus grow asymptotically to the base miltech effect?
            float armiesNumberEffect = Mathf.Max(MILITARY_MALUS_LIMIT, ((__instance.numStandardArmies + __instance.numNavies) * MILITARY_MALUS_PER_ARMY_AND_NAVY));

            // Add a catch-up multiplier dependent on how far behind the max miltech level the country is
            // A bonus 50% tech gain per full miltech level behind the global max
            // Max to 1 is to prevent weirdness if somehow current miltech is above max miltech
            float catchUpMult = Mathf.Max(1f, 1f + (MILITARY_PER_MILITARY_LEVEL_BEHIND * (__instance.maxMilitaryTechLevel - __instance.militaryTechLevel)));

            // Each full point of education gives +10% military score
            float educationMult = 1f + (__instance.education * MILITARY_MULT_PER_EDUCATION_LEVEL);

            __result = (BASE_MILITARY + armiesNumberEffect) * catchUpMult * educationMult;


            return false; // Skip original method
        }
    }
}
