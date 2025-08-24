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
    /// Patches the getter for control cost of a single control point for a nation
    /// Formula now uses a power function again
    /// The effect of five global technologies to reduce the control point cost of CPs by up to 75% was removed
    /// Unifying nations is now required to reduce the CP cost
    /// And the global techs to reduce CP cost aren't critical anymore
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "ControlPointMaintenanceCost", MethodType.Getter)]
    public static class ControlPointCostPatch
    {
        [HarmonyPostfix]
        public static void GetControlPointMaintenanceCostPostfix(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch
            if (!Main.enabled) { return; }

            // Will be 0 and should stay 0 if the nation's controller is the aliens
            if (__result != 0)
            {
                // Settings values cached for readability
                float controlPointCostMult = Main.settings.controlPointCostMult;

                // Total cost to control the entire nation. 1 cost per 1 IP
                float baseControlCost = __instance.economyScore;

                // Reverted the change from a power function to a flat multiplicative control point cost reduction
                const float COST_DECAY_EXPONENT = 0.7f;

                // Total cost is split across the control points
                __result = Mathf.Pow(baseControlCost, COST_DECAY_EXPONENT) * controlPointCostMult / __instance.numControlPoints;
            }
        }
    }
}
