using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



namespace TIEconomyMod
{
    [HarmonyPatch(typeof(TINationState), "OnEnvironmentPriorityComplete")]
    public static class EnvironmentRegionEffectPatch
    {
        // Refer to EconomyRegionEffectPatch.cs for specifics on everything that's happening here.

        public static int baseCleanupThreshold = 100;
        public static int cleanupThreshold;
        public static readonly FieldInfo getCleanupThreshold = AccessTools.Field(typeof(EnvironmentRegionEffectPatch), nameof(cleanupThreshold));

        // Replaces hardcoded variables in OnEnvironmentPriorityComplete(). The tooltip's stated threshold has to be updated separately.
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Refer to PriorityTooltipPatch.cs for details.
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == baseCleanupThreshold)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, getCleanupThreshold);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static void Recalculate()
        {
            cleanupThreshold = (Main.enabled) ? baseCleanupThreshold * Main.settings.regionUpgradeThresholdMult : baseCleanupThreshold;
        }
    }
}
