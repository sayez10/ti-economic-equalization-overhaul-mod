// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the algorithm to calculate the miltech level of a nation after absorbing another nation
    /// Prefix patch calculates the miltech value the nation should have after the merger
    /// Postfix patch then overwrites the miltech value of the nation after the merger
    ///
    /// If two nations without any existing armies are merged, the higher miltech value is used, possibly increased by up to 0.5 if the difference is small enough
    /// The reasoning is that even two armies with the a similar technological level typically have some unique strengths which can complement each other
    /// Obviously that has limits (e.g. the US miltech level shouldn't increase by merging with some third world shthole)
    /// Any existing armies in either nation can lower the post-merger miltech value
    /// Number of regions of both nations is no longer included the formula (unlike vanilla)
    ///
    /// This formula is somewhat messy. The problem here is the abstract representation of military capabilities in the game.
    /// The miltech value includes available knowledge (e.g. technologies and institutional knowledge) which isn't really lost during a merger, but also hardware
    /// which might need to be replaced or upgraded, and personnel (including factors like doctrine, tactics and organization) which might need to be re-trained.
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.AbsorbNation))]
    public static class AbsorbNationMiltechCalculationPatch
    {
        [HarmonyPrefix]
        private static bool AbsorbNationPrefix(out float __state, TINationState __instance, TIFactionState actingFaction, TINationState joiningNationState)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) {
                __state = 0;
                return true;
            }

            const float MAX_BONUS_MILITARY_LEVEL = 0.5f;

            float maxMilitaryLevel = Math.Max(__instance.militaryTechLevel, joiningNationState.militaryTechLevel);
            float deltaMilitaryLevel = Math.Abs(__instance.militaryTechLevel - joiningNationState.militaryTechLevel);
            float bonusMilitaryLevel = Math.Max(MAX_BONUS_MILITARY_LEVEL, (0.5f - deltaMilitaryLevel));
            float orgMilitaryLevel = maxMilitaryLevel + bonusMilitaryLevel;

            // Higher = lower weighing of number of existing armies
            const float WEIGHT_NUM_FORCES_GROWTH_FACTOR = 5f;

            int numNationForces = __instance.numStandardArmies + __instance.numNavies;
            int numJoinerForces = joiningNationState.numStandardArmies + joiningNationState.numNavies;
            float weightNumNationForces = 0.5f * WEIGHT_NUM_FORCES_GROWTH_FACTOR / (WEIGHT_NUM_FORCES_GROWTH_FACTOR + numNationForces);
            float weightNumJoinerForces = 0.5f * WEIGHT_NUM_FORCES_GROWTH_FACTOR / (WEIGHT_NUM_FORCES_GROWTH_FACTOR + numJoinerForces);
            float weightOrgMilitaryLevel = 1f - weightNumNationForces - numJoinerForces;

            // Calculate the merged nation's military tech value, weigh the individual summands with their respective weights
            __state = ((orgMilitaryLevel * weightOrgMilitaryLevel) + (__instance.militaryTechLevel * weightNumNationForces) + (joiningNationState.militaryTechLevel * weightNumJoinerForces)) / (weightOrgMilitaryLevel + weightNumNationForces + weightNumJoinerForces);


            return true; // Continue with original method
        }



        [HarmonyPostfix]
        private static void AbsorbNationPostfix(float __state, TINationState __instance)
        {
            // If mod has been disabled, abort patch
            if (!Main.enabled) { return; }

            Traverse traverse = Traverse.Create(__instance);
            FileLog.Log(string.Format("[TIEconomyMod] Nation merger for {0}: Game set Miltech {1}, replaced by Mod with {2}", __instance.displayName, __instance.militaryTechLevel, __state));
            traverse.Property("militaryTechLevel", null).SetValue(__state);
            __instance.SetDataDirty();
        }
    }
}
