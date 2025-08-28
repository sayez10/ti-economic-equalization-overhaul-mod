// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomyMod.AIPatches
{
    /// <summary>
    /// Patch changes AI evaluation of a nation's value/importance to account for the higher IP amount in large modded nations
    /// </summary>
    [HarmonyPatch(typeof(AIEvaluators), nameof(AIEvaluators.EvaluateNation))]
    internal static class AIEvaluateNationPatch
    {
        [HarmonyPrefix]
        private static bool EvaluateNationOverwrite(TIFactionState faction, TINationState nation, ref float __result)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Changed from economyScore^3, which was vanilla GDP in billions, to this, which is modded GDP in billions
            // Vanilla and modded nations will be evaluated the same given a certain GDP
            float num = nation.economyScore * 100f;

            // As vanilla
            float num2 = 100f * faction.aiValues.wantSpaceFacilities * faction.aiValues.wantSpaceWarCapability;
            float num3 = num + (nation.spaceFlightProgram ? num2 : 0f);
            float num4 = (90f - nation.BestBoostLatitude) / 3f;
            __result = num3 + num4 + AIEvaluators. EvaluateMonthlyResourceIncome(faction, FactionResource.Money, nation.spaceFundingIncome_month) + AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Boost, nation.boostIncome_month_dekatons * TemplateManager.global.spaceResourceToTons) + AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Research, nation.research_month) + AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.MissionControl, nation.missionControl) + (nation.nuclearProgram ? (50f * faction.aiValues.wantEarthWarCapability) : 0f) + (float)nation.armies.Count * 25f * faction.aiValues.wantEarthWarCapability + nation.militaryTechLevel * 1.5f * faction.aiValues.wantEarthWarCapability + nation.spaceDefenseCoverage * 1000f;


            return false; // Skip original method
        }
    }
}
