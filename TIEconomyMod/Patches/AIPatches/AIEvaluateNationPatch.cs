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
        private static bool EvaluateNationOverwrite(ref float __result, in TIFactionState faction, in TINationState nation)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Changed from economyScore^3, which was vanilla GDP in billions, to this, which is modded GDP in billions
            // Vanilla and modded nations will be evaluated the same given a certain GDP
            float num = nation.economyScore * 100f;

            // Same as vanilla
            float num2 = 100f * ((faction != null) ? faction.aiValues.wantSpaceFacilities : 1f) * ((faction != null) ? faction.aiValues.wantSpaceWarCapability : 1f);
            num += (nation.spaceFlightProgram ? num2 : 0f);
            float num3 = (90f - nation.BestBoostLatitude) / 3f;
            num += num3;
            if (faction != null)
            {
                num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Money, (nation.spaceFunding_year + nation.spaceFundingIncome_year) / 2f);
                num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Boost, (nation.rawBoostPerMonth_dekatons + nation.boostIncome_month_dekatons) / 2f * TemplateManager.global.spaceResourceToTons);
                num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Research, nation.research_month);
                num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.MissionControl, (float)nation.missionControl);
            }
            num += (nation.nuclearProgram ? (50f * ((faction != null) ? faction.aiValues.wantEarthWarCapability : 1f)) : 0f);
            num += (float)nation.armies.Count * 25f * ((faction != null) ? faction.aiValues.wantEarthWarCapability : 1f);
            num += nation.militaryTechLevel * 1.5f * ((faction != null) ? faction.aiValues.wantEarthWarCapability : 1f);
            __result = num + nation.spaceDefenseCoverage * 1000f;


            return false; // Skip original method
        }
    }
}
