// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using UnityEngine;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch changes AI evaluation of a control point's value/importance to account for the higher IP amount in large modded nations
    /// </summary>
    [HarmonyPatch(typeof(AIEvaluators), nameof(AIEvaluators.EvaluateControlPoint))]
    internal static class AIEvaluateControlPointPatch
    {
        // FIXME on vanilla update: This patch copies large parts of the overwritten vanilla function. Checking that vanilla function for changes
        // and updating this patch after every vanilla update is required.
        // I very briefly considered replacing it with a postfix patch which would have calculated the delta between the valuation of a nation's
        // economy in vanilla and this mod, then would have multiplied with the same terms as in vanilla, and finally would have added the result
        // to the function's return value. However, this of course would still require checks of the vanilla function after every vanilla update.
        // Updating the function wouldn't be a simple copy-and-past job anymore. And the result would be more brittle (at least the current patch
        // has a good chance to give reasonable results even if we miss some vanilla changes).
        [HarmonyPrefix]
        private static bool EvaluateControlPointOverwrite(ref float __result, in TIFactionState faction, in TIControlPoint controlPoint)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Same as vanilla
            float num = 0f;
            TINationState nation = controlPoint.nation;

            // Changed from GDP in billions ^1.05 in vanilla, to GDP in billions
            // Evaluation of nations' economy will be generally lower and the AI will no longer valuate richer nations disproportionally higher
            num += nation.economyScore * (float)Main.settings.GDPBillionsPerIP;

//            float vanillaEconomyScore = (float)Math.Pow(nation.GDP / 1_000_000_000d, (double)TIGlobalConfig.globalConfig.controlPointIPScaling);
//            float vanillaEconomyScoreAI = vanillaEconomyScore * vanillaEconomyScore * vanillaEconomyScore;
//            FileLog.Log(string.Format($"[TIEconomyMod::AIEvaluateControlPointPatch] {controlPoint.displayName}: GDP Evaluation in Vanilla: {vanillaEconomyScoreAI}, GDP Evaluation in Mod: {num}"));

            // Same as vanilla
            num += (nation.spaceFlightProgram ? (100f * faction.aiValues.wantSpaceFacilities * faction.aiValues.wantSpaceWarCapability) : 0f);
            num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Research, nation.GetMonthlyResearchFromControlPoint(faction));
            num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Money, (nation.spaceFunding_year + nation.spaceFundingIncome_year) / 2f);
            num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Boost, (nation.rawBoostPerMonth_dekatons + nation.boostIncome_month_dekatons) / 2f * TemplateManager.global.spaceResourceToTons);
            num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.MissionControl, (float)nation.GetMissionControlFromControlPoint(controlPoint.positionInNation));
            num += (nation.nuclearProgram ? (50f * faction.aiValues.wantEarthWarCapability) : 0f);
            num += (float)nation.GetNumArmiesAtControlPoint(controlPoint.positionInNation) * 25f * faction.aiValues.wantEarthWarCapability;
            num += nation.militaryTechLevel * 1.5f * faction.aiValues.wantEarthWarCapability;
            num += nation.spaceDefenseCoverage * 1000f;
            num += (nation.unrest + nation.unrestRestState) / 2f * -8f * faction.aiValues.riskAversion;
            num += (controlPoint.nation.CouncilControlPointFraction(faction, true, false) + 1f / (float)controlPoint.nation.numControlPoints) * 5f;
            num *= (controlPoint.executive ? 2f : 1f);
            num *= 1f + Mathf.Max(0f, (nation.GetPublicOpinionOfFaction(faction.ideology) - 0.2f) * 1.25f);
            if (faction.lostControlPoints.ContainsKey(controlPoint))
            {
                float num2 = (float)TITimeState.Now().DifferenceInDays(faction.lostControlPoints[controlPoint]) / 30.436874f;
                num *= Mathf.Clamp(6f - num2, 1f, 4f);
            }
            __result = num;


            return false; // Skip original method
        }
    }
}
