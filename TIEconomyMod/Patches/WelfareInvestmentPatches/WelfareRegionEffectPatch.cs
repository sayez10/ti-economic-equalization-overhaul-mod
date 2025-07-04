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
    [HarmonyPatch(typeof(TINationState), "OnWelfarePriorityComplete")]
    public static class WelfareRegionEffectPatch
    {
        // Refer to EconomyRegionEffectPatch.cs for specifics on everything that's happening here.

        public static int baseDecolonizeThreshold = 1000;
        public static int decolonizeThreshold;
        public static readonly FieldInfo getDecolonizeThreshold = AccessTools.Field(typeof(WelfareRegionEffectPatch), nameof(decolonizeThreshold));

        // Replaces hardcoded variables in OnWelfarePriorityComplete(). The tooltip's stated threshold has to be updated separately.
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Refer to PriorityTooltipPatch.cs for details.
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == baseDecolonizeThreshold)
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
            decolonizeThreshold = (Main.enabled) ? baseDecolonizeThreshold * Main.settings.regionUpgradeThresholdMult : baseDecolonizeThreshold;
        }
    }
}