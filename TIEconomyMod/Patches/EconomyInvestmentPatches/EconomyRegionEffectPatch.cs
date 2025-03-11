using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TIEconomyMod
{
    [HarmonyPatch(typeof(TINationState), "OnEconomyPriorityComplete")]
    public static class EconomyRegionEffectPatch
    {
        // The base threshold is stored as its own variable to allow quick fixing if the devs change the hardcoded requirement.
        public static int baseOilThreshold = 500;
        public static int baseMiningThreshold = 750;
        public static int baseEconomicThreshold = 1200;

        public static int oilThreshold = baseOilThreshold * 5;
        public static int miningThreshold = baseMiningThreshold * 5;
        public static int economicThreshold = baseEconomicThreshold * 5;

        // This replaces hardcoded variables, in this case I'm increasing the requirements by 5x due to all the extra IP.
        // This does NOT change the localization's reported requirement to add a special region. That has to be done separately.
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == baseOilThreshold)
                {
                    instruction.operand = oilThreshold;
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == baseMiningThreshold)
                {
                    instruction.operand = miningThreshold;
                }
                else if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == baseEconomicThreshold)
                {
                    instruction.operand = economicThreshold;
                }
                yield return instruction;
            }
        }
    }
}