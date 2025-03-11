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
    [HarmonyPatch(typeof(TINationState), "OnWelfarePriorityComplete")]
    public static class WelfareRegionEffectPatch
    {
        // The base threshold is stored as its own variable to allow quick fixing if the devs change the hardcoded requirement.
        public static int baseDecolonizeThreshold = 1000;
        public static int decolonizeThreshold = baseDecolonizeThreshold * 5;

        // This replaces hardcoded variables, in this case I'm increasing the requirements by 5x due to all the extra IP.
        // This does NOT change the localization's reported requirement to add a special region. That has to be done separately.
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == baseDecolonizeThreshold)
                {
                    instruction.operand = decolonizeThreshold;
                }
                yield return instruction;
            }
        }
    }
}