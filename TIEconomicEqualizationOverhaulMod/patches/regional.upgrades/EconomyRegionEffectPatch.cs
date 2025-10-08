// SPDX-FileCopyrightText: Copyright © 2024 - 2025 explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;



namespace TIEconomicEqualizationOverhaulMod
{
    /// <summary>
    /// Patch replaces hardcoded integers in OnEconomyPriorityComplete(), to increase the requirements by 5x due to all the extra IP
    /// This does NOT change the tooltip localization's reported threshold to add a special region, which has to be done separately by
    /// patching priorityTipStr() in PriorityTooltipPatch.cs
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.OnEconomyPriorityComplete))]
    internal static class EconomyRegionEffectPatch
    {
        // Rather than using a property variable, whose value is basically refreshed each time it's called,
        // it's instead refreshed only when mod settings are changed. In other words, they're cached.
        private static int _oilThreshold;
        private static int _miningThreshold;
        private static int _coreEcoThreshold;

        // This basically is a reference to the final threshold variables, which the post-transpiler code can call on
        internal static readonly FieldInfo getOilThreshold = AccessTools.Field(typeof(EconomyRegionEffectPatch), nameof(_oilThreshold));
        internal static readonly FieldInfo getMiningThreshold = AccessTools.Field(typeof(EconomyRegionEffectPatch), nameof(_miningThreshold));
        internal static readonly FieldInfo getCoreEcoThreshold = AccessTools.Field(typeof(EconomyRegionEffectPatch), nameof(_coreEcoThreshold));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == RegionalUpgradesShared.VANILLA_OIL_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, getOilThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == RegionalUpgradesShared.VANILLA_MINING_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, getMiningThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == RegionalUpgradesShared.VANILLA_CORE_ECO_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, getCoreEcoThreshold);
                }
                else
                {
                    yield return instruction;
                }
            }
        }



        internal static void Recalculate()
        {
            // If the mod is disabled, the vanilla value is inserted instead
            // This allows for the mod to be fully disabled during runtime
            _oilThreshold = (Main.enabled) ? RegionalUpgradesShared.VANILLA_OIL_THRESHOLD * RegionalUpgradesShared.REGION_UPGRADE_THRESHOLD_MULT : RegionalUpgradesShared.VANILLA_OIL_THRESHOLD;
            _miningThreshold = (Main.enabled) ? RegionalUpgradesShared.VANILLA_MINING_THRESHOLD * RegionalUpgradesShared.REGION_UPGRADE_THRESHOLD_MULT : RegionalUpgradesShared.VANILLA_MINING_THRESHOLD;
            _coreEcoThreshold = (Main.enabled) ? RegionalUpgradesShared.VANILLA_CORE_ECO_THRESHOLD * RegionalUpgradesShared.REGION_UPGRADE_THRESHOLD_MULT : RegionalUpgradesShared.VANILLA_CORE_ECO_THRESHOLD;
        }
    }
}
