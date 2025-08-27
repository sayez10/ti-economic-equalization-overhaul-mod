// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using System.Linq;
using System.Text;



namespace TIEconomyMod
{
    [HarmonyPatch(typeof(NationInfoController), "BuildInvestmentTooltip")]
    public static class IPTooltipPatch
    {
        [HarmonyPrefix]
        public static bool BuildInvestmentTooltipOverwrite(ref string __result, TINationState nation)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Improved breakdown of IP bonuses and penalties
            StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Nation.InvestmentPoints")).AppendLine();
            bool penaltyMilitary = false;
            bool penaltyUnrest = false;

            float economyScore = nation.economyScore;
            float finalEconomyScore = nation.BaseInvestmentPoints_month();
            stringBuilder.Append(Loc.T("UI.Nation.BaseIPs", economyScore.ToString("N2")));
            if (finalEconomyScore != economyScore)
            {
                stringBuilder.Append(Loc.T("UI.Nation.CurrentIPs", finalEconomyScore.ToString("N2")));
            }
            stringBuilder.AppendLine();

            float adviserAdministrationBonus = nation.adviserAdministrationBonus;
            if (adviserAdministrationBonus > 0f)
            {
                stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.AdviserBonus", adviserAdministrationBonus.ToPercent("P0"), TIUtilities.FormatSmallNumber(economyScore * adviserAdministrationBonus)));
            }

            float investmentPoints_unrestPenalty_frac = nation.investmentPoints_unrestPenalty_frac;
            float unrestPenalty = economyScore * investmentPoints_unrestPenalty_frac;
            if (investmentPoints_unrestPenalty_frac > 0f)
            {
                penaltyUnrest = true;
                stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.IPUnrestPenalty", investmentPoints_unrestPenalty_frac.ToPercent("P0"), TIUtilities.FormatSmallNumber(unrestPenalty)));
            }

            // Changed to reflect impact from mil tech
            int armiesAtHome = nation.armies.Count((TIArmyState x) => x.investmentArmyFactor > 0f && x.useHomeInvestmentFactor);
            int deployedArmies = nation.armies.Count((TIArmyState x) => x.investmentArmyFactor > 0f && !x.useHomeInvestmentFactor);
            int numNavies = nation.armies.Count((TIArmyState x) => x.deploymentType == DeploymentType.Naval && x.investmentNavyFactor > 0f);
            float upkeepHomeMult = TemplateManager.global.nationalInvestmentArmyFactorHome * nation.militaryTechLevel;
            float upkeepAwayMult = TemplateManager.global.nationalInvestmentArmyFactorAway * nation.militaryTechLevel;
            float upkeepNavyMult = TemplateManager.global.nationalInvestmentNavyFactor * nation.militaryTechLevel;
            float totalArmyHomeUpkeep = (float)armiesAtHome * upkeepHomeMult;
            float totalArmyAwayUpkeep = (float)deployedArmies * upkeepAwayMult;
            float totalNavyUpkeep = (float)numNavies * upkeepNavyMult;

            if (armiesAtHome > 0)
            {
                penaltyMilitary = true;
                stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.HomeArmiesPenalty", TIUtilities.FormatSmallNumber(upkeepHomeMult), TIUtilities.FormatSmallNumber(totalArmyHomeUpkeep)));
            }
            if (deployedArmies > 0)
            {
                penaltyMilitary = true;
                stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.AwayArmiesPenalty", TIUtilities.FormatSmallNumber(upkeepAwayMult), TIUtilities.FormatSmallNumber(totalArmyAwayUpkeep)));
            }
            if (numNavies > 0)
            {
                penaltyMilitary = true;
                stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.NaviesPenalty", TIUtilities.FormatSmallNumber(upkeepNavyMult), TIUtilities.FormatSmallNumber(totalNavyUpkeep)));
            }

            // Added additional tooltip that combines Home, Away, and Navy costs
            // Example text: The above penalties have a combined Investment Point penalty of {0}
            if (armiesAtHome > 0 || deployedArmies > 0)
            {
                stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.MilitaryTotalPenalty", TIUtilities.FormatSmallNumber(totalArmyHomeUpkeep + totalArmyAwayUpkeep + totalNavyUpkeep)));
            }

            if (penaltyMilitary && penaltyUnrest)
            {
                stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.OverallPenaltyToIP", TIUtilities.FormatSmallNumber(unrestPenalty + totalArmyHomeUpkeep + totalArmyAwayUpkeep + totalNavyUpkeep)));
            }

            __result = stringBuilder.ToString().Trim();


            return false; // Skip original method
        }
    }
}
