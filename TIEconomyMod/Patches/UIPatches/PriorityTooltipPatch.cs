using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection.Emit;
using System.Collections;
using System.Reflection;



namespace TIEconomyMod
{
    // THIS PRICK TOOK FOREVER TO FIND OH MY GOD I HAD TO INSTALL TWO SEPARATE PROGRAMS TO FIND THIS ACCURSED FUNCTION
    // I REGRET NOTHING
    [HarmonyPatch(typeof(PriorityListItemController), "priorityTipStr")]
    public static class PriorityPatches
    {
        // This ensures that tooltip readouts of region 'upgrade' IP requirements accurately reflect what the mod sets.

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Refer to EconomyRegionEffectPatch.cs for specifics on what's happening here.
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == EconomyRegionEffectPatch.baseOilThreshold)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, EconomyRegionEffectPatch.getOilThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == EconomyRegionEffectPatch.baseMiningThreshold)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, EconomyRegionEffectPatch.getMiningThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == EconomyRegionEffectPatch.baseEconomicThreshold)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, EconomyRegionEffectPatch.getEconomicThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == WelfareRegionEffectPatch.baseDecolonizeThreshold)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, WelfareRegionEffectPatch.getDecolonizeThreshold);
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == EnvironmentRegionEffectPatch.baseCleanupThreshold)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, EnvironmentRegionEffectPatch.getCleanupThreshold);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
