// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes the effects of the spoils investment
    /// This overwrite is necessary to change the propoganda effect, which would otherwise be far too powerful
    /// Otherwise, this method is almost as vanilla, barring referenced values that are changed in other patches
    ///
    /// nameof() can't be used here because OnSpoilsPriorityComplete is private in vanilla
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "OnSpoilsPriorityComplete")]
    internal static class SpoilsEffectsPatch
    {
        [HarmonyPrefix]
        private static bool OnSpoilsPriorityCompleteOverwrite(TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Same as vanilla
            __instance.AddToInequality(__instance.spoilsPriorityInequalityChange, TINationState.InequalityChangeReason.InqReason_SpoilsPriority);
            __instance.AddToDemocracy(__instance.spoilsPriorityDemocracyChange, TINationState.DemocracyChangeReason.DemReason_SpoilsPriority);
            TIFactionState controlPointTypeOwner = __instance.GetControlPointTypeOwner(ControlPointType.Aristocracy);
            TIFactionState controlPointTypeOwner2 = __instance.GetControlPointTypeOwner(ControlPointType.ExtractiveSector);

            foreach (TIControlPoint ticontrolPoint in __instance.controlPoints)
            {
                float num = __instance.spoilsPriorityMoneyPerControlPoint * ((controlPointTypeOwner == ticontrolPoint.faction) ? TemplateManager.global.aristoracySpoilsMult : 1f) + ((controlPointTypeOwner2 == ticontrolPoint.faction) ? (TemplateManager.global.extractiveSpoilsBonusPerResourceRegion * (float)__instance.currentResourceRegions) : 0f);
                num += TIEffectsState.SumEffectsModifiers(Context.SpoilsOutput, ticontrolPoint.faction, num, null);
                if (ticontrolPoint.faction != null && !ticontrolPoint.benefitsDisabled)
                {
                    ticontrolPoint.faction.AddToCurrentResource(num, FactionResource.Money, false, "Spoils");
                    ticontrolPoint.faction.thisWeeksCumulativeSpoils += num;
                    ticontrolPoint.faction.thisMonthsCumulativeSpoils += num;
                }
            }

            // Mod code!
            const float BASE_PROPAGANDA_MULT = 2f;
            const float PROPAGANDA_MULT_PER_DEMOCRACY_OR_EDUCATION_LEVEL = 1.5f;

            float basePropaganda = TemplateManager.global.spoilsPriorityPublicOpinionScaling * BASE_PROPAGANDA_MULT;
            float democracyEducationMult = (__instance.education + __instance.democracy) * PROPAGANDA_MULT_PER_DEMOCRACY_OR_EDUCATION_LEVEL;

            float propagandaEffect = Tools.EffectStrength(basePropaganda, __instance.population) * democracyEducationMult;

            foreach (TIFactionState iFaction in __instance.FactionsWithControlPoint)
            {
                __instance.PropagandaOnPop_PerOwnedCPFraction(iFaction.ideology, propagandaEffect);
            }

            // Same as Vanilla again, only replaced a now unused function argument (which contained a multiplication) with 0f as tiny micro-optimization
            __instance.AddToSustainability(__instance.spoilsSustainabilityChange);
            TIGlobalValuesState.GlobalValues.AddSpoilsPriorityEnvEffect(__instance, 0f);


            return false; // Skip original method
        }
    }
}
