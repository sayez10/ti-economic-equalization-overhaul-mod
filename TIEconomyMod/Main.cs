// SPDX-FileCopyrightText: Copyright © 2022 - 2025 Verdiss, explodoboy, sayez10
//
// SPDX-License-Identifier: MIT

using HarmonyLib;
using UnityModManagerNet;

using System.Reflection;



namespace TIEconomyMod
{
    /// <summary>
    /// Controls loading and managing the mod
    /// </summary>
    public class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry mod;
        public static Settings settings;

        /// <summary>
        /// Entry point of the application (as per ModInfo.json), which applies the Harmony patches
        /// </summary>
        /// <param name="modEntry"></param>
        /// <returns></returns>
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            mod = modEntry;
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Disabled until I fully implement it
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            // Assign values to class variables
            settings.OnChange();
            return true;
        }

        /// <summary>
        /// Toggles the enabled state when the mod is toggled by the UMM interface or the TI interface
        /// </summary>
        /// <param name="modEntry"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            // Because transpilers can't just be turned off, all cached variables are refreshed
            // This is what ensures the mod can shut itself off during runtime
            settings.OnChange();
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        public class Settings : UnityModManager.ModSettings, IDrawable
        {
            [Draw("GDP in billions per IP, lower = more IPs: (default: 100.0)", Min = 1.0, Precision = 1)] public double GDPBillionsPerIP = 100f;
            [Draw("Multiplier for nations' research production: (default: 1.0)", Min = 0.0, Precision = 2)] public float researchMult = 1f;
            [Draw("Multiplier for control points cost: (default: 4.0)", Min = 0.1, Precision = 2)] public float controlPointCostMult = 4f;

            public override void Save(UnityModManager.ModEntry modEntry)
            {
                Save(this, modEntry);
            }

            // Class variables need to be refreshed whenver settings get changed
            // Allows changing settings without needing to restart
            public void OnChange()
            {
                Tools.Recalculate();
                EconomyRegionEffectPatch.Recalculate();
                EnvironmentRegionEffectPatch.Recalculate();
                WelfareRegionEffectPatch.Recalculate();
            }
        }
    }
}
