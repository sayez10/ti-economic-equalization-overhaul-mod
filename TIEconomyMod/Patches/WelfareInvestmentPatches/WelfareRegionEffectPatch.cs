// SPDX-FileCopyrightText: Copyright © 2024 - 2025 explodoboy, sayez10
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
using System.Reflection.Emit;
using System.Reflection;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch replaces hardcoded integers in OnWelfarePriorityComplete(), to increase the requirements by 5x due to all the extra IP
    /// This does NOT change the tooltip localization's reported threshold to add a special region, which has to be done separately by
    /// patching priorityTipStr() in PriorityTooltipPatch.cs
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "OnWelfarePriorityComplete")]
    public static class WelfareRegionEffectPatch
    {
        // Rather than using a property variable, whose value is basically refreshed each time it's called,
        // it's instead refreshed only when mod settings are changed. In other words, they're cached.
        public static int decolonizeThreshold;

        // This basically is a reference to the final threshold variables, which the post-transpiler code can call on
        public static readonly FieldInfo getDecolonizeThreshold = AccessTools.Field(typeof(WelfareRegionEffectPatch), nameof(decolonizeThreshold));

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.BASE_DECOLONIZE_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, getDecolonizeThreshold);
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
            decolonizeThreshold = (Main.enabled) ? Tools.BASE_DECOLONIZE_THRESHOLD * Tools.REGION_UPGRADE_THRESHOLD_MULT : Tools.BASE_DECOLONIZE_THRESHOLD;
        }
    }
}
