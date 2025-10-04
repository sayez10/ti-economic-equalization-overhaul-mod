// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod
{
    /// <summary>
    /// This patch changes the effects of the unity investment
    /// This overwrite is necessary to fix the propoganda effect, which would otherwise be far too powerful
    /// Otherwise, this method is almost as vanilla, barring referenced values that are changed in other patches
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.OnUnityPriorityComplete))]
    internal static class UnityEffectsPatch
    {
        [HarmonyPrefix]
        private static bool OnUnityPriorityCompleteOverwrite(TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_PROPAGANDA_MULT = 0.1f;
            const float PROPAGANDA_PENALTY_MULT_PER_EDUCATION_AND_DEMOCRACY_LEVEL = 0.02f;
            const float EDUCATION_AND_DEMOCRACY_PENALTY_MAX = 0.9f;

            float basePropaganda = TIGlobalConfig.globalConfig.unityPublicOpinionBaseStrength * BASE_PROPAGANDA_MULT;

            // Democracy and Education incurs a 2% penalty per point, up to -90%
            // A combined score of 45 causes the max effect
            float educationDemocracyPenaltyMult = Math.Min(EDUCATION_AND_DEMOCRACY_PENALTY_MAX, 1f - ((__instance.education + __instance.democracy) * PROPAGANDA_PENALTY_MULT_PER_EDUCATION_AND_DEMOCRACY_LEVEL));

            float propagandaEffect = Tools.EffectStrength(basePropaganda, __instance.population) * educationDemocracyPenaltyMult;

            TIFactionState religionCPOwner = __instance.GetControlPointOfTypeFaction(ControlPointType.Religion);

            foreach (TIFactionState iFaction in __instance.FactionsWithControlPoint)
            {
                __instance.PropagandaOnPop_PerOwnedCP(iFaction.ideology, propagandaEffect, (religionCPOwner == iFaction) ? TemplateManager.global.religionUnityPublicOpinionBonusStrength : 0);
            }

            // As in vanilla
            __instance.AddToCohesion(__instance.unityPriorityCohesionChange, TINationState.CohesionChangeReason.CohesionReason_UnityPriority);
            __instance.AddToEducation(__instance.unityPriorityEducationChange, TINationState.EducationChangeReason.EducationReason_UnityPriority);

            if (__instance.canAccumulateLegitimizeClaimTriggers)
            {
                Traverse traverse = Traverse.Create(__instance);
                Traverse accumulatedTriggersTraverse = traverse.Property("accumulatedLegitimizeClaimTriggers", null);

                float accumulatedTriggers = __instance.accumulatedLegitimizeClaimTriggers;
                accumulatedTriggersTraverse.SetValue(accumulatedTriggers + 1f);

                const float LEGITIMIZE_THRESHOLD = (float)(Tools.VANILLA_LEGITIMIZE_THRESHOLD * Tools.REGION_UPGRADE_THRESHOLD_MULT);

                if (accumulatedTriggers >= LEGITIMIZE_THRESHOLD && __instance.CandidateLegitimizeClaimRegions().Count > 0)
                {
                    __instance.OnLegitimizeClaimPriorityComplete();
                    accumulatedTriggersTraverse.SetValue(0f);
                }
            }


            return false; // Skip original method
        }
    }
}
