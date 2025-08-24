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



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the democracy effect of an oppression investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "OppressionPriorityDemocracyChange", MethodType.Getter)]
    public static class OppressionDemocracyEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetOppressionPriorityDemocracyChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            float BASE_DEMOCRACY = -0.0175f;

            // About 35% of (base) Government effect
            __result = Tools.EffectStrength(BASE_DEMOCRACY, __instance.population);


            return false; // Skip original method
        }
    }
}
