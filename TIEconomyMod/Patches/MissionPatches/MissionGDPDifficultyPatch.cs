using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace TIEconomyMod
{
    [HarmonyPatch(typeof(TIMissionModifier_TargetNationGDP), "GetModifier")]
    public static class MissionGDPDifficultyPatch
    {
        [HarmonyPrefix]
        public static bool GetModifierOverwrite(ref float __result, TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
        {
            //Patches the default amount of mission difficulty from national economy size for certain missions like control nation
            //Amount of difficulty for a given nation with a given GDP is to be identical in vanilla and this mod

            if (target == null)
            {
                __result = 0f;
            }
            TINationState tINationState = YoinkObjectToNation(attackingCouncilor.faction, target);
            if (tINationState != null)
            {
                float vanillaEcoScore = (float)Mathd.Pow(tINationState.economyScore * 100f, 0.33f); //This is the vanilla economyScore the country would have
                __result = vanillaEcoScore * TemplateManager.global.TIMissionModifier_TargetNationGDP_Multiplier;
            }


            return false; //Skip orignal method
        }

        // Not gonna lie, I have ChatGPT to thank for this.
        public static TINationState YoinkObjectToNation(object faction, TIGameState target)
        {
            // Get the MethodInfo for the protected static method 'ObjectToNation'
            MethodInfo method = AccessTools.Method(typeof(TIMissionModifier), "ObjectToNation", new Type[] { faction.GetType(), typeof(TIGameState) });
            if (method == null)
            {
                throw new Exception("Could not find method ObjectToNation");
            }
            // Call the method; note that since it is static, pass null as the instance
            object result = method.Invoke(null, new object[] { faction, target });
            return result as TINationState;
        }
    }
}
