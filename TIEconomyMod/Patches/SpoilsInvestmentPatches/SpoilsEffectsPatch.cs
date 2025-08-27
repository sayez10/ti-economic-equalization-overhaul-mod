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
    /// Patch changes the effects of the spoils investment
    /// This overwrite is necessary to fix the propoganda effect, which would otherwise be far too powerful
    /// Otherwise, this method is almost as vanilla, barring referenced values that are changed in other patches
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "OnSpoilsPriorityComplete")]
    public static class SpoilsEffectsPatch
    {
        [HarmonyPrefix]
        public static bool OnSpoilsPriorityCompleteOverwrite(TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // As vanilla
            __instance.AddToInequality(__instance.spoilsPriorityInequalityChange, InequalityChangeReason.InqReason_SpoilsPriority);
            __instance.AddToDemocracy(__instance.spoilsPriorityDemocracyChange, DemocracyChangeReason.DemReason_SpoilsPriority);
            TIFactionState controlPointTypeOwner = __instance.GetControlPointTypeOwner(ControlPointType.Aristocracy);
            TIFactionState controlPointTypeOwner2 = __instance.GetControlPointTypeOwner(ControlPointType.ExtractiveSector);

            foreach (TIControlPoint controlPoint in __instance.controlPoints)
            {
                float num = __instance.spoilsPriorityMoneyPerControlPoint * ((controlPointTypeOwner == controlPoint.faction) ? TemplateManager.global.aristoracySpoilsMult : 1f) + ((controlPointTypeOwner2 == controlPoint.faction) ? (TemplateManager.global.extractiveSpoilsBonusPerResourceRegion * (float)__instance.currentResourceRegions) : 0f);
                num += TIEffectsState.SumEffectsModifiers(Context.SpoilsOutput, controlPoint.faction, num);
                if (controlPoint.faction != null && !controlPoint.benefitsDisabled)
                {
                    controlPoint.faction.AddToCurrentResource(num, FactionResource.Money, suppressFactionResourcesUpdatedEvent: false, "Spoils");
                    controlPoint.faction.thisWeeksCumulativeSpoils += num;
                    controlPoint.faction.thisMonthsCumulativeSpoils += num;
                }
            }

            /*
             * Propaganda Effect:
             *
             * Neither I nor the original author yet understand what's going on with propaganda enough to understand how it scales with population.
             * The original author disabled it, but I didn't like that.
             */
            const float BASE_PROPAGANDA = -0.1f;
            float propagandaEffect = (__instance.education + __instance.democracy) * BASE_PROPAGANDA;

            foreach (TIFactionState item in __instance.FactionsWithControlPoint)
            {
                __instance.PropagandaOnPop_PerOwnedCPFraction(item.ideology, propagandaEffect);
            }

            // As vanilla
            __instance.AddToSustainability(__instance.spoilsSustainabilityChange);
            TIGlobalValuesState.GlobalValues.AddSpoilsPriorityEnvEffect(__instance, __instance.priorityEffectPopScaling * __instance.sustainability);


            return false; // Skip original method
        }
    }
}
