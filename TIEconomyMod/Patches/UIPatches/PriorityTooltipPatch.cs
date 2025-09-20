// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using System.Collections.Generic;
using System.Reflection.Emit;



namespace TIEconomyMod
{
    [HarmonyPatch(typeof(PriorityListItemController),  nameof(PriorityListItemController.priorityTipStr))]
    internal static class PriorityPatches
    {
        // This ensures that tooltip readouts of region upgrade IP requirements accurately reflect what the mod sets

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Refer to EconomyRegionEffectPatch.cs for specifics on what's happening here
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.VANILLA_OIL_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, EconomyRegionEffectPatch.getOilThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.VANILLA_MINING_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, EconomyRegionEffectPatch.getMiningThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.VANILLA_CORE_ECO_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, EconomyRegionEffectPatch.getCoreEcoThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.VANILLA_DECOLONIZE_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, WelfareRegionEffectPatch.getDecolonizeThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.VANILLA_DECONTAMINATE_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, EnvironmentRegionEffectPatch.getDecontaminateThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == Tools.VANILLA_LEGITIMIZE_THRESHOLD)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, GovernmentRegionEffectPatch.getLegitimizeThreshold);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
