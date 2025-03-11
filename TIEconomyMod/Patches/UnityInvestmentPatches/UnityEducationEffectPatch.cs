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
    [HarmonyPatch(typeof(TINationState), "unityPriorityEducationChange", MethodType.Getter)]
    public static class UnityEducationEffectPatch
    {
        [HarmonyPrefix]
        public static bool GetUnityPriorityEducationChangeOverwrite(ref float __result, TINationState __instance)
        {
            //Patch changes the knowledge effect of a unity investment to scale inversely with population size

            // 1/10 effect of Knowledge priority.
            // Refer to EffectStrength() comments for explanation.
            __result = Tools.EffectStrength(-0.01f, __instance.population);



            return false; //Skip original getter
        }
    }
}
