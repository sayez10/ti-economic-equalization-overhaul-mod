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
    [HarmonyPatch(typeof(TINationState), "economyScore", MethodType.Getter)]
    public static class BaseInvestmentPointPatch
    {
        [HarmonyPrefix]
        public static bool GetEconomyScoreOverwrite(ref float __result, TINationState __instance)
        {
            // Patches the amount of Investment Points available to a nation

            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            // Linear scaling: E.g. 500 billion GDP / 100 billion = 5 IP/month
            __result = (float)(__instance.GDP / Tools.GDPPerIP);


            return false; // Skip original method
        }
    }
}
