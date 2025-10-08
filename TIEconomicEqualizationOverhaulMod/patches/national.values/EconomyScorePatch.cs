// SPDX-FileCopyrightText: Copyright © 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using System.Runtime.CompilerServices;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Called every time after TINationState.ModifyGDP() completes
    /// That vanilla method is called every time a nation's GDP changes
    /// Overwrites the nation's economyScore member (= monthly IP), which had been set during ModifyGDP()
    /// Instead of GDPInBillions^0.33 like in Vanilla, we use linear scaling
    /// That can be further modified by a user-defined factor
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.ModifyGDP))]
    internal static class EconomyScorePatch
    {
        // These values are dynamically calculated inside a function
        // They're first calculated after the mod loads, and then whenever settings are changed
        private static float _theoreticalPopulation;
        private static double IPPerGDP;



        [HarmonyPostfix]
        private static void ModifyGDPPostfix(TINationState __instance)
        {
            // If mod has been disabled, abort patch
            if (!Main.enabled) { return; }

            // Linear scaling: E.g. 500 billion GDP * 0.000_000_000_01d = 5 IP/month
            float economyScore = (float)(__instance.GDP * IPPerGDP);

            Traverse traverse = Traverse.Create(__instance);
            traverse.Property("economyScore", null).SetValue(economyScore);
            __instance.SetDataDirty();
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float EffectStrength(float idealGainPerMonth, float population)
        {
            /*
             * Calculates the effect strength for inverse population scaling.
             *
             * A nation with 30k GDP per-capita will, if putting all of its focus on the relevant priority, increase a particular nation stat by [idealGainPerMonth].
             *
             * For example, if Welfare's Inequality reduction [idealGainPerMonth] is -0.1, and GDP/pc is 30k, then Inequality is reduced at a rate of 0.1 a month.
             *
             * If it's 60k, then the effect is 0.2 per month.
             * If it's 15k, then the effect is 0.05 per month.
             * And so on.
             *
             * The effect strength you will see in-game will likely be much, much less, unless the nation generates less than 1 IP a month.
             *
             * The following reasoning is used in the below equation:
             *
             * Let's say the country has a GDP of 100 billion. They generate 1 IP per month.
             * The GDP per capita is 30k. Therefore, they have a population of 3.33 million.
             *
             * This then is divided by the nation's population to get the final effect strength.
             *
             *
             * TLDR: If a nation has 30k GDP per capita, a stat will be changed by [idealGainPerMonth] a month.
             */

            return idealGainPerMonth * _theoreticalPopulation / population;
        }



        internal static void Recalculate()
        {
            const double IDEAL_POPULATION = 30_000d;

            IPPerGDP = 0.000_000_001d / Main.settings.GDPBillionsPerIP;

            // Declared outside of EffectStrength() because that function will be called VERY often
            // For an explanation as to why I did this, check the comments inside the function
            _theoreticalPopulation = (float)(1d / (IPPerGDP * IDEAL_POPULATION));
        }
    }
}
