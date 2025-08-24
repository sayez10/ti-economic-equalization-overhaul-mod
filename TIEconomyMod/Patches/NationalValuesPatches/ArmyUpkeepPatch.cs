// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT



using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



namespace TIEconomyMod.InvestmentPointPatches
{
    /// <summary>
    /// Patch changes the IP upkeep of armies to be dependent on mil tech level of the owning nation
    /// </summary>
    [HarmonyPatch(typeof(TIArmyState), "investmentArmyFactor", MethodType.Getter)]
    public static class ArmyInvestmentUpkeepPatch
    {
        [HarmonyPrefix]
        public static bool GetInvestmentArmyFactorOverwrite(ref float __result, TIArmyState __instance)
        {
            // Multiply maintenance cost by miltech level
            if (!__instance.useHomeInvestmentFactor)
            {
                __result = TemplateManager.global.nationalInvestmentArmyFactorAway * __instance.homeNation.militaryTechLevel;
            }
            else
            {
                __result = TemplateManager.global.nationalInvestmentArmyFactorHome * __instance.homeNation.militaryTechLevel;
            }


            return false; // Skip original method
        }
    }
}
