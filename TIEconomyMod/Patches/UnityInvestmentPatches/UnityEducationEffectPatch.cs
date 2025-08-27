// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
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



namespace TIEconomyMod
{
    /// <summary>
    /// Patch changes the knowledge effect of a unity investment to scale inversely with population size
    /// </summary>
    [HarmonyPatch(typeof(TINationState), nameof(TINationState.unityPriorityEducationChange), MethodType.Getter)]
    public static class UnityEducationEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetUnityPriorityEducationChangeOverwrite(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            const float BASE_EDUCATION = -0.01f;

            // 1/10 effect of Knowledge priority
            __result = Tools.EffectStrength(BASE_EDUCATION, __instance.population);


            return false; // Skip original method
        }
    }
}
