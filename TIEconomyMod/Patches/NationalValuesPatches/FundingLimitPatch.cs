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



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the amount of investment points available to a nation to scale linearly with GDP
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "maxFunding_year", MethodType.Getter)]
    public static class FundingLimitPatch
    {
        [HarmonyPrefix]
        public static bool MaxFundingYearOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const double ANNUAL_FUNDING_PER_GDP = 0.00000005;

            __result = (float)(ANNUAL_FUNDING_PER_GDP * __instance.GDP);


            return false; // Skip original method
        }
    }
}
