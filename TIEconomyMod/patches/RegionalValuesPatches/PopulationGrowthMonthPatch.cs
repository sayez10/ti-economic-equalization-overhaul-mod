// SPDX-FileCopyrightText: Copyright © 2025 sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using System.Linq;
using UnityEngine;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch removes the education level reduction if the population of a region is reduced during the regular monthly update
    /// Mostly verbatim vanilla code, with only a few changes:
    /// Remove the random modifier to population changes (since I'm patching this method already)
    /// Replace code to call a private property setter, out of technical necessity
    /// Actually remove the education level reduction effect
    /// </summary>
    [HarmonyPatch(typeof(TIRegionState), nameof(TIRegionState.GrowPopulationByMonth))]
    internal static class GrowPopulationByMonthPatch
    {
        // FIXME: Replace this conventional prefix patch with a transpiler patch?
        [HarmonyPrefix]
        private static bool GrowPopulationByMonthOverwrite(TIRegionState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            double num = Mathd.Pow(1.0 + __instance.annualPopulationGrowth, 0.0833333358168602) - 1.0;

            // Not the point of this patch, but while here, we may as well reduce pointless RNG a tiny little bit
            // num += (double)global::UnityEngine.Random.Range(-0.000412f, 0.000412f);

            float populationInMillions = __instance.populationInMillions;

            // Old code to calculate the monthly population change
            // __instance.populationInMillions *= 1f + (float)num;
            // __instance.populationInMillions = Mathf.Max(__instance.populationInMillions, 0.001f);

            // New code to calculate the monthly population change...
            float changedPopulationInMillions = populationInMillions * (1f + (float)num);
            changedPopulationInMillions = Mathf.Max(changedPopulationInMillions, 0.001f);

            // ...and to commit that change
            Traverse traverse = Traverse.Create(__instance);
            traverse.Property("populationInMillions", null).SetValue(changedPopulationInMillions);

            // Back to vanilla code
            float num2 = __instance.populationInMillions - populationInMillions;
            GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
            {
                x.SetResourceIncomeDataDirty(new FactionResource[]
                {
                    FactionResource.Influence,
                    FactionResource.Research
                });
            });
            if (num2 < 0f)
            {
                __instance.nation.ModifyGDP(__instance.regionalPerCapitaGDP * (double)num2 * 1000000.0, TINationState.GDPChangeReason.GDPReason_PopulationChange);

                // And the whole point of this patch: Commenting out the code which reduces a region's education level if its population declines due to natural causes
                // __instance.nation.AddToEducation(Mathf.Clamp(num2 / 100f, -0.005f, 0f), TINationState.EducationChangeReason.EducationReason_PopulationLoss);
            }
            else
            {
                __instance.nation.ModifyGDP(__instance.regionalPerCapitaGDP * (double)num2 * 1000000.0, TINationState.GDPChangeReason.GDPReason_PopulationChange);
            }
            __instance.nation.SetPriorityEffectPopScaling();


            return false; // Skip original method
        }
    }
}
