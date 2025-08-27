// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using static PavonisInteractive.TerraInvicta.TINationState;



namespace TIEconomyMod
{
    /// <summary>
    /// This patch changes the effects of the unity investment
    /// This overwrite is necessary to fix the propoganda effect, which would otherwise be far too powerful
    /// Otherwise, this method is almost as vanilla, barring referenced values that are changed in other patches
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.OnUnityPriorityComplete))]
    public static class UnityEffectsPatch
    {
        [HarmonyPrefix]
        public static bool OnUnityPriorityCompleteOverwrite(TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            /*
             * Propaganda Effect:
             *
             * As with the Spoils propoganda effect, it's unfortunately beyond me to understand this well enough to get things where I want them
             * For now, the best way for me to handle this issue is to simply disable the system entirely, pending additional research and math
             * For now, I'm tacking on a x0.2 multiplier until I can understand what's going on there.
             *
             * FIXME: Change this once I understand PropagandaOnPop
             */

            // Strength is multiplied by 0.2, to account for much higher IP
            const float BASE_PROPAGANDA = 0.2f;
            float propagandaEffect = TemplateManager.global.unityPublicOpinionBaseStrength * __instance.priorityEffectPopScaling * BASE_PROPAGANDA;
            TIFactionState controlPointOfTypeFaction = __instance.GetControlPointOfTypeFaction(ControlPointType.Religion);

            // As vanilla
            foreach (TIFactionState item in __instance.FactionsWithControlPoint)
            {
                __instance.PropagandaOnPop_PerOwnedCP(item.ideology, propagandaEffect, (controlPointOfTypeFaction == item) ? TemplateManager.global.religionUnityPublicOpinionBonusStrength : 0);
            }

            // As vanilla
            __instance.AddToCohesion(__instance.unityPriorityCohesionChange, CohesionChangeReason.CohesionReason_UnityPriority);
            __instance.AddToEducation(__instance.unityPriorityEducationChange, EducationChangeReason.EducationReason_UnityPriority);


            return false; // Skip original method
        }
    }
}
