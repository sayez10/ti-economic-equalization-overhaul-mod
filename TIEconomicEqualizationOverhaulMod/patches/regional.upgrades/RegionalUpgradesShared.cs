// SPDX-FileCopyrightText: Copyright © 2022 - 2025 sayez10
//
// SPDX-License-Identifier: MIT



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Defines some trivial constants which are shared by various files related to regional upgrades
    /// </summary>
    internal static class RegionalUpgradesShared
    {
        // More convenient aliases of vanilla constants for region upgrade thresholds which are used in multiple files of this mod
        // Also a single point of change if the vanilla devs ever make intrusive changes to the region upgrade costs
        internal const int VANILLA_OIL_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numEcosForCoreOilRegion;
        internal const int VANILLA_MINING_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numEcosForCoreMiningRegion;
        internal const int VANILLA_CORE_ECO_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numEcosForCoreEcoRegion;
        internal const int VANILLA_DECOLONIZE_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numWelfaresForDecolonizeTriggers;
        internal const int VANILLA_DECONTAMINATE_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numEnvironmentsToTriggerDecontaminate;
        internal const int VANILLA_LEGITIMIZE_THRESHOLD = PavonisInteractive.TerraInvicta.TINationState.numPrioritiesForLegitimize;

        // Multiplier to increase the number of investment completions required for region upgrades
        // Compensation for the significantly higher number of investment completions with this mod
        internal const int REGION_UPGRADE_THRESHOLD_MULT = 5;
    }
}
