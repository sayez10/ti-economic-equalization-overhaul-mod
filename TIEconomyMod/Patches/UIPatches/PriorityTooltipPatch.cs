using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection.Emit;

namespace TIEconomyMod
{
    // THIS B###### TOOK FOREVER TO FIND OH MY GOD I HAD TWO INSTALL TWO SEPARATE PROGRAMS FOR THIS
    [HarmonyPatch(typeof(PriorityListItemController), "priorityTipStr")]
    public static class PriorityPatches
    {
        // This uses the variables from the assorted priority-region patches to reflect the higher IP needed for a special region interaction.
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == EconomyRegionEffectPatch.baseOilThreshold)
                {
                    instruction.operand = EconomyRegionEffectPatch.oilThreshold;
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == EconomyRegionEffectPatch.baseMiningThreshold)
                {
                    instruction.operand = EconomyRegionEffectPatch.miningThreshold;
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == EconomyRegionEffectPatch.baseEconomicThreshold)
                {
                    instruction.operand = EconomyRegionEffectPatch.economicThreshold;
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == WelfareRegionEffectPatch.baseDecolonizeThreshold)
                {
                    instruction.operand = WelfareRegionEffectPatch.decolonizeThreshold;
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == EnvironmentRegionEffectPatch.baseCleanupThreshold)
                {
                    instruction.operand = EnvironmentRegionEffectPatch.cleanupThreshold;
                }
                yield return instruction;
            }
        }
    }
}