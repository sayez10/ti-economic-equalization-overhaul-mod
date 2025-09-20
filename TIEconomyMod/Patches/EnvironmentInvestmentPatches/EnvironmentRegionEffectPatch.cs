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
    /// Patch replaces hardcoded integers in OnEnvironmentPriorityComplete(), to increase the requirements by 5x due to all the extra IP
    /// This does NOT change the tooltip localization's reported threshold to add a special region, which has to be done separately by
    /// patching priorityTipStr() in PriorityTooltipPatch.cs
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.OnEnvironmentPriorityComplete))]
    internal static class EnvironmentRegionEffectPatch
    {
        // Rather than using a property variable, whose value is basically refreshed each time it's called,
        // it's instead refreshed only when mod settings are changed. In other words, they're cached.
        private static int _decontaminateThreshold;

        // This basically is a reference to the final threshold variables, which the post-transpiler code can call on
        internal static readonly FieldInfo getDecontaminateThreshold = AccessTools.Field(typeof(EnvironmentRegionEffectPatch), nameof(_decontaminateThreshold));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.VANILLA_DECONTAMINATE_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, getDecontaminateThreshold);
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
            _decontaminateThreshold = (Main.enabled) ? Tools.VANILLA_DECONTAMINATE_THRESHOLD * Tools.REGION_UPGRADE_THRESHOLD_MULT : Tools.VANILLA_DECONTAMINATE_THRESHOLD;
        }
    }
}
