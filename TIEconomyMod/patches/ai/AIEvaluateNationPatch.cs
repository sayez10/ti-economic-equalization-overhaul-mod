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
        // FIXME on vanilla update: This patch copies large parts of the overwritten vanilla function. Checking that vanilla function for changes
        // and updating this patch after every vanilla update is required.
        // I very briefly considered replacing it with a postfix patch which would have calculated the delta between the valuation of a nation's
        // economy in vanilla and this mod and then would have added the result to the function's return value. While much more practical than
        // similar changes to AIEvaluateControlPointPatch, it would have the same fundamental drawbacks. It would still require checks of the
        // vanilla function after every vanilla update. Updating the function wouldn't be a simple copy-and-past job anymore. And the result
        // would be more brittle (at least the current patch has a good chance to give reasonable results even if we miss some vanilla changes).
        // It would also break the current symmetry between of the implementation of this mod's AI patches, which, although of far less importance,
        // would still be undesirable.
        [HarmonyPrefix]
        private static bool EvaluateNationOverwrite(ref float __result, in TIFactionState faction, in TINationState nation)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Changed from GDP in billions ^1.05 in vanilla, to GDP in billions
            // Evaluation of nations' economy will be generally lower and the AI will no longer valuate richer nations disproportionally higher
            float num = nation.economyScore * (float)Main.settings.GDPBillionsPerIP;

//            float vanillaEconomyScore = (float)Math.Pow(nation.GDP / 1_000_000_000d, (double)TIGlobalConfig.globalConfig.controlPointIPScaling);
//            float vanillaEconomyScoreAI = vanillaEconomyScore * vanillaEconomyScore * vanillaEconomyScore;
//            FileLog.Log(string.Format($"[TIEconomyMod::AIEvaluateNationPatch] {nation.displayName}: GDP Evaluation in Vanilla: {vanillaEconomyScoreAI}, GDP Evaluation in Mod: {num}"));

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
