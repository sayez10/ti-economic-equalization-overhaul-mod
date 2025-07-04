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
    [HarmonyPatch(typeof(TIGlobalValuesState), "AddSpoilsPriorityEnvEffect")]
    public static class SpoilsEnvironmentEffectPatch
    {
        [HarmonyPrefix]
        public static bool AddSpoilsPriorityEnvEffectOverwrite(TINationState nation, TIGlobalValuesState __instance)
        {
            //Overwrites the vanilla greenhouse gas emissions values for completing a spoils investment
            //Removes the scaling to emissions added in the vanilla function that does not work with this mod

            // If mod has been disabled, abort patch and use original method.
            if (!Main.enabled) { return true; }

            // Settings values are cached for readability.
            float greenhouseEmissionsOffset = Main.settings.spoilsInvestment.greenhouseEmissionsOffset;

            float regions = nation.currentResourceRegions;
            float regionMult = 0.5f;

            //CO2
            float baseCO2 = TemplateManager.global.SpoCO2_ppm;
            float resRegionCO2Mult = 1.0f + (regions * regionMult);
            __instance.AddCO2_ppm(baseCO2 * resRegionCO2Mult * greenhouseEmissionsOffset, GHGSources.SpoilsPriority);


            //CH4
            float baseCH4 = TemplateManager.global.SpoCH4_ppm;
            float resRegionCH4Mult = 1.0f + (regions * regionMult);
            __instance.AddCH4_ppm(baseCH4 * resRegionCH4Mult * greenhouseEmissionsOffset, GHGSources.SpoilsPriority);


            //N2O
            float baseN2O = TemplateManager.global.SpoN2O_ppm;
            float resRegionN2OMult = 1.0f + (regions * regionMult);
            __instance.AddN2O_ppm(baseN2O * resRegionN2OMult * greenhouseEmissionsOffset, GHGSources.SpoilsPriority);



            return false; //Skip the original method
        }
    }
}
