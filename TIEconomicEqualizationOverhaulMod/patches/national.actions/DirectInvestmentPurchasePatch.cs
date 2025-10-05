// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch removes all scaling with population size or GDP
    /// Patch removes all scaling with military level (double-dipping in vanilla)
    /// Patch removes all scaling with cost of investments (in IP)
    /// Cost of almost all investments is now constant, with very few exceptions
    /// Funding is a special case, the cost in influence depends directly on the financial benefits
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.InvestmentPointDirectPurchasePrice))]
    internal static class DirectInvestmentPurchasePatch
    {
        private enum CurrencyType
        {
            Money,
            Influence,
            Ops,
            None
        }

        // Lookup table for the base cost of all investments
        // In a few cases, these are modified later
        private static readonly float[,] _priorityDirectInvestmentCost = {
            { 500f,  25f,   0f }, // Economy
            { 500f, 100f,   0f }, // Welfare
            { 500f, 100f,   0f }, // Environment
            { 250f, 500f,   0f }, // Knowledge
            { 250f, 500f,   0f }, // Government
            { 250f, 500f,   0f }, // Unity
            {   0f, 100f,  25f }, // Oppression
            {   0f,   5f,   0f }, // Funding
            {   0f, 0.5f,   0f }, // Spoils
            { 100f, 500f,   0f }, // Civilian_InitiateSpaceflightProgram
            { 100f, 500f,   0f }, // LaunchFacilities
            { 100f, 500f,   0f }, // MissionControl
            { 500f, 100f,  25f }, // Military_FoundMilitary
            { 500f, 100f,  25f }, // Military
            { 500f, 100f,  25f }, // Military_BuildArmy
            { 500f, 100f,  25f }, // Military_BuildNavy
            { 500f, 100f,   5f }, // Military_InitiateNuclearProgram
            { 500f, 100f,   5f }, // Military_BuildNuclearWeapons
            { 500f, 100f,  10f }, // Military_BuildSpaceDefenses
            { 500f, 100f,  10f }  // Military_BuildSTOSquadron
        };



        [HarmonyPrefix]
        private static bool InvestmentPointDirectPurchasePriceOverwrite(ref TIResourcesCost __result, TINationState __instance, PriorityType priority, in TIFactionState faction)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            float costMoney = _priorityDirectInvestmentCost[(int)priority, (int)CurrencyType.Money];
            float costInfluence = _priorityDirectInvestmentCost[(int)priority, (int)CurrencyType.Influence];
            float costOps = _priorityDirectInvestmentCost[(int)priority, (int)CurrencyType.Ops];

            // Fix up anything that can't be included in a simple lookup-table
            switch (priority)
            {
            case PriorityType.Economy:
                break;
            case PriorityType.Funding:
                costInfluence *= __instance.spaceFundingPriorityIncomeChange;
                break;
//            case PriorityType.Spoils:
//                costInfluence *= __instance.spoilsPriorityMoney;
//                break;
            }

            // If the user set a nation IP bonus/malus at campaign start, we also use that factor to decrease/increase the cost of direct investments
            float customizationNationalIPMult = 1f / (TIGlobalValuesState.Customizations.usingCustomizations ? TIGlobalValuesState.Customizations.nationalIPMultiplier : 1f);

            float corruptionMult = 1f + (__instance.corruption * 0.75f);

            TIResourcesCost resourcesCost = new TIResourcesCost();

            if (costMoney > 0f)
            {
                costMoney *= customizationNationalIPMult;
                costMoney *= corruptionMult;
                costMoney *= 1f - TIEffectsState.SumEffectsModifiers(Context.DirectInvestGlobalDiscount_Money_PCT, faction, costMoney);

                if (costMoney > 0f)
                {
                    resourcesCost.AddCost(FactionResource.Money, costMoney);
                }
            }

            if (costInfluence > 0f)
            {
                costInfluence *= customizationNationalIPMult;
                costInfluence *= corruptionMult;
                costInfluence *= 1f - TIEffectsState.SumEffectsModifiers(Context.DirectInvestGlobalDiscount_Influence_PCT, faction, costInfluence);

                // 50% discount after a change of executive control
                if (__instance.SkipDirectInvestInfluenceCost(faction))
                {
                    costInfluence *= 0.5f;
                }

                // Up to 50% discount if we control CPs in a nation
                costInfluence *= 1f - (TemplateManager.global.maxInvestmentPointDiscountfromControlPoints * __instance.CouncilControlPointFraction(faction, false, false));

                if (costInfluence > 0f)
                {
                    resourcesCost.AddCost(FactionResource.Influence, costInfluence);
                }
            }

            if (costOps > 0f)
            {
                costOps *= customizationNationalIPMult;
                costOps *= 1f - TIEffectsState.SumEffectsModifiers(Context.DirectInvestGlobalDiscount_Ops_PCT, faction, costOps);

                if (costOps > 0f)
                {
                    resourcesCost.AddCost(FactionResource.Operations, costOps);
                }
            }

            resourcesCost = resourcesCost.MultiplyCost(1f / (1f + faction.cachedPriorityBonuses[priority]));

            __result = resourcesCost.MultiplyCost(1f / (1f + __instance.NationalPriorityBonuses(priority)));


            return false; // Skip original method
        }
    }
}
