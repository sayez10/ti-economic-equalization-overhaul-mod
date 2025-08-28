// SPDX-FileCopyrightText: Copyright © 2024 - 2025 explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;



namespace TIEconomyMod
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
        public static int oilThreshold;
        public static int miningThreshold;
        public static int economicThreshold;

        // This basically is a reference to the final threshold variables, which the post-transpiler code can call on
        public static readonly FieldInfo getOilThreshold = AccessTools.Field(typeof(EconomyRegionEffectPatch), nameof(oilThreshold));
        public static readonly FieldInfo getMiningThreshold = AccessTools.Field(typeof(EconomyRegionEffectPatch), nameof(miningThreshold));
        public static readonly FieldInfo getEconomicThreshold = AccessTools.Field(typeof(EconomyRegionEffectPatch), nameof(economicThreshold));

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.BASE_OIL_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, getOilThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.BASE_MINING_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, getMiningThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.BASE_ECONOMIC_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, getEconomicThreshold);
                }
                else
                {
                    yield return instruction;
                }
            }
        }



        public static void Recalculate()
        {
            // If the mod is disabled, the vanilla value is inserted instead
            // This allows for the mod to be fully disabled during runtime
            oilThreshold = (Main.enabled) ? Tools.BASE_OIL_THRESHOLD * Tools.REGION_UPGRADE_THRESHOLD_MULT : Tools.BASE_OIL_THRESHOLD;
            miningThreshold = (Main.enabled) ? Tools.BASE_MINING_THRESHOLD * Tools.REGION_UPGRADE_THRESHOLD_MULT : Tools.BASE_MINING_THRESHOLD;
            economicThreshold = (Main.enabled) ? Tools.BASE_ECONOMIC_THRESHOLD * Tools.REGION_UPGRADE_THRESHOLD_MULT : Tools.BASE_ECONOMIC_THRESHOLD;
        }
    }
}
