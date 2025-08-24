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

            const float BASE_MILTECH = 0.0025f;
            const float MILTECH_PER_MILTECH_LEVEL_BEHIND = 0.5f;
            const float MILTECH_MALUS_PER_ARMY = -0.001f;
            const float MILTECH_MALUS_PER_NAVY = -0.0005f;
            const float MALUS_LIMIT = -0.00125f;

            // FIXME: Formula is too simplistic. Let malus grow asymptotically to the base miltech effect? Add multiplicative knowledge bonus? Miltech growth probably too fast, too.
            float armiesNumberEffect = Mathf.Max(MALUS_LIMIT, (__instance.numStandardArmies * MILTECH_MALUS_PER_ARMY) + (__instance.numNavies * MILTECH_MALUS_PER_NAVY));

            // Additionally, add a catch-up multiplier dependent on how far behind the max tech level the country is
            // A bonus 50% tech gain per full tech level behind the global max
            // Max to 1 is to prevent weirdness if somehow mil tech is above max mil tech
            float catchUpMult = Mathf.Max(1f, 1f + (MILTECH_PER_MILTECH_LEVEL_BEHIND * (__instance.maxMilitaryTechLevel - __instance.militaryTechLevel)));

            __result = (BASE_MILTECH + armiesNumberEffect) * catchUpMult;


            return false; // Skip original method
        }
    }
}
