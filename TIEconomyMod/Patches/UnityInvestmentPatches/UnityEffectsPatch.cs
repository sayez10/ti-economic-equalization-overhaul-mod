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

            // FIXME: Propaganda effect might need to be re-balanced. Scaling with population?
            // Strength is multiplied by 0.2 for now, to account for much higher IP and more priority completions
            const float BASE_PROPAGANDA_EFFECT = 0.2f;
            float propagandaEffect = TemplateManager.global.unityPublicOpinionBaseStrength * __instance.priorityEffectPopScaling * BASE_PROPAGANDA_EFFECT;
            TIFactionState religionCPOwner = __instance.GetControlPointOfTypeFaction(ControlPointType.Religion);

            foreach (TIFactionState iFaction in __instance.FactionsWithControlPoint)
            {
                __instance.PropagandaOnPop_PerOwnedCP(iFaction.ideology, propagandaEffect, (religionCPOwner == iFaction) ? TemplateManager.global.religionUnityPublicOpinionBonusStrength : 0);
            }

            // As vanilla
            __instance.AddToCohesion(__instance.unityPriorityCohesionChange, TINationState.CohesionChangeReason.CohesionReason_UnityPriority);
            __instance.AddToEducation(__instance.unityPriorityEducationChange, TINationState.EducationChangeReason.EducationReason_UnityPriority);


            return false; // Skip original method
        }
    }
}
