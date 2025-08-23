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
    /// </summary>
    [HarmonyPatch(typeof(TINationState), "ControlPointMaintenanceCost", MethodType.Getter)]
    public static class ControlPointCostPatch
    {
        [HarmonyPostfix]
        public static void GetControlPointMaintenanceCostPostfix(ref float __result, TINationState __instance)
        {
            // If mod has been disabled, abort patch.
            if (!Main.enabled) { return; }

            // Will be 0 and should stay 0 if the nation's controller is the aliens
            if (__result != 0)
            {
                // Settings values are cached for readability
                float controlPointCostMult = Main.settings.controlPointCostMult;

                // Total cost to control the entire nation. 1 cost per 1 IP
                float baseControlCost = __instance.economyScore;

                // Reverted the change from a power function to a flat multiplicative control point cost reduction
                // Also removed the control point cost reducing effect of five global technologies
                // Instead reduced the exponent to further reduce the control cost, in particular of large/rich nations
                // These now have a much high IP/CP ratio and are much more efficient to control that multiple small nations
                // Effects: Unification is clearly more efficient now and the global techs to reduce CP cost aren't critical anymore
                const float COST_DECAY_EXPONENT = 0.7f;

                // Total cost is split across the control points
                __result = Mathf.Pow(baseControlCost, COST_DECAY_EXPONENT) * controlPointCostMult / __instance.numControlPoints;
            }
        }
    }
}
