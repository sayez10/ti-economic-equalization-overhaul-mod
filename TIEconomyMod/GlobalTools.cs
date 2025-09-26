// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System.Runtime.CompilerServices;



namespace TIEconomyMod
{
    // This is a helper class used to streamline things, and centralize important variables
    internal static class Tools
    {
        // More convenient aliases of vanilla constants for region upgrade thresholds which are used in multiple files of this mod
        // Also a single point of change if the vanilla devs ever make intrusive changes to the region upgrade costs
        internal const int VANILLA_OIL_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numEcosForCoreOilRegion;
        internal const int VANILLA_MINING_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numEcosForCoreMiningRegion;
        internal const int VANILLA_CORE_ECO_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numEcosForCoreEcoRegion;
        internal const int VANILLA_DECOLONIZE_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numWelfaresForDecolonizeTriggers;
        internal const int VANILLA_DECONTAMINATE_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numEnvironmentsToTriggerDecontaminate;
        // Shared by both GovernmentRegionEffectPatch and UnityRegionEffectPatch
        internal const int VANILLA_LEGITIMIZE_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numPrioritiesForLegitimize;

        // Multiplier to increase the number of investment completions required for region upgrades
        // Compensation for the significantly higher number of investment completions with this mod
        internal const int REGION_UPGRADE_THRESHOLD_MULT = 5;

        // These values are dynamically calculated inside a function
        // They're first calculated after the mod loads, and then whenever settings are changed
        private static float _theoreticalPopulation;
        internal static double IPPerGDP
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
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
            double GDPPerIP = 1_000_000_000d * Main.settings.GDPBillionsPerIP;
            _theoreticalPopulation = (float)(GDPPerIP / IDEAL_POPULATION);
        }
    }
}
