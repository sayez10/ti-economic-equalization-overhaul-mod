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
            //Patches the amount of Investment Points available to a nation

            //ex: 500 billion GDP / 100 billion = 5 IP/month
            float baseIP = (float)(__instance.GDP / Tools.GDPPerIP);


            // Bonus to IP for nations with very low IP. Bonus becomes more aggressive as baseIP approaches 0.

            // How aggressive boost should be; higher values are more steep.
            float scaleFactor = 0.5f;
            // Minimum IP to trigger crutch; higher values are more steep.
            float minToTrigger = 5f;

            if(baseIP < minToTrigger)
            {
                // baseIP^(1-scaleFactor) * minToTrigger^scaleFactor
                __result = Mathf.Pow(baseIP, 1-scaleFactor) * Mathf.Pow(minToTrigger, scaleFactor);
            } else {
                // if IP over minimum, use linear scaling
                __result = baseIP;
            }

            return false; //Skip original getter
        }
    }
}
