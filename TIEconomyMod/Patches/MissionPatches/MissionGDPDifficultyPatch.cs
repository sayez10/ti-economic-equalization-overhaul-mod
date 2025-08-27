// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

using System.Reflection;



namespace TIEconomyMod
{
    /// <summary>
    /// Patch the default amount of mission difficulty from national economy size for certain missions like control nation
    /// Amount of difficulty for a given nation with a given GDP is to be identical in vanilla and this mod
    /// </summary>
    [HarmonyPatch(typeof(TIMissionModifier_TargetNationGDP), nameof(TIMissionModifier_TargetNationGDP.GetModifier))]
    public static class MissionGDPDifficultyPatch
    {
        // Grab the reflection info for ObjectToNation(TIFactionState, TIGameState)
        // The function is protected, so it can't be run directly
        private static readonly MethodInfo objectToNationMethod = AccessTools.Method(typeof(TIMissionModifier), "ObjectToNation", new Type[] { typeof(TIFactionState), typeof(TIGameState) });

        [HarmonyPrefix]
        public static bool GetModifierOverwrite(ref float __result, TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
        {
            // If mod has been disabled, abort patch and use original method
            if (!Main.enabled) { return true; }

            __result = 0f;

            // End the overwrite here, skip original method
            if (target == null) { return false; }

            TINationState nation = YoinkObjectToNation(attackingCouncilor.faction, target);
            if (nation != null)
            {
                // This is the vanilla economyScore the country would have
                float vanillaEcoScore = (float)Math.Pow(nation.economyScore * 100f, 0.33f);
                __result = vanillaEcoScore * TemplateManager.global.TIMissionModifier_TargetNationGDP_Multiplier;
            }


            return false; // Skip original method
        }



        public static TINationState YoinkObjectToNation(TIFactionState faction, TIGameState target)
        {
            // In case the developers decide to change ObjectToNation()'s name or parameters, a check is done to confirm the method was actually acquired
            if (objectToNationMethod == null)
            {
                throw new MissingMethodException("Could not find method ObjectToNation");
            }

            // Call the method; note that since it is static, pass null as the instance
            object result = objectToNationMethod.Invoke(null, new object[] { faction, target });
            return result as TINationState;
        }
    }
}
