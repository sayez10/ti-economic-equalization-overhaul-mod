using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;



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

            // Assign values to class variables.
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
            // Frankly, adding functionality for disabling this mod during runtime is absolutely unnecessary. This mod does a lot of things.
            // I'm doing this not particularly because it's necessary, but because it's good practice. Especially for transpilers.
            // So... basically, it's because I can.

            enabled = value;

            // Because transpilers can't just be turned off, all cached variables are refreshed.
            // This is what ensures the mod can shut itself off during runtime.
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

        // The following are settings fields.
        [DrawFields(DrawFieldMask.Public)]
        public class InvestmentPointsSettings
        {
            [Draw("IP per [n] billion GDP: (default: 100.0)", Min = 1.0, Precision = 1)] public double IPPerGDPBillions = 100.0;
            [Draw("1x investment scaling at per-capita GDP: (default: 30000.0)", Min = 1.0, Precision = 0)] public float baseEffectStrengthPCGDP = 30000.0f;
            [Draw("IP crutch up to base IP/month of: (default: 50.0)", Min = 0.0, Precision = 1)] public float crutchLimit = 50.0f;
            [Draw("IP crutch scaling aggressiveness: (default: 0.5)", DrawType.Slider, Max = 0, Min = 1.0, Precision = 1)] public float crutchAggressiveness = 0.5f;
        }

        [DrawFields(DrawFieldMask.Public)]
        public class ResearachProductionSettings
        {
            [Draw("Multiplier to overall research production: (default: 1.0)", Min = 0.0, Precision = 2)] public float researchOffset = 1.0f;
            [Draw("1x bonus at per-capita GDP: (default: 20000.0)", Min = 1.0, Precision = 0)] public float baseResearchAtPCGDP = 20000.0f;
            [Draw("Worst allowed penalty multiplier from low per-capita GDP: (default: 0.6)", DrawType.Slider, Min = 0.0, Max = 1.0, Precision = 2)] public float worstMalus = 0.6f;
        }

        /// <summary>
        ///
        /// </summary>
        public class Settings : UnityModManager.ModSettings, IDrawable
        {
            [Draw("Investment Points", Collapsible = true)] public InvestmentPointsSettings investmentPoints = new InvestmentPointsSettings();
            [Draw("Research Production", Collapsible = true)] public ResearachProductionSettings researchProduction = new ResearachProductionSettings();

            // These settings don't neatly fit into any category, and are thus ungrouped.
            [Draw("Percentage which control point costs are reduced by techs: (default: 0.15)", DrawType.Slider, Max = 0, Min = 0.2)] public float ControlPointCostReduction = 0.15f;
            [Draw("Region upgrade/decolonize/cleanup threshold multiplier, whole number: (default: 5, requires restart)", Min = 1)] public int regionUpgradeThresholdMult = 5;

            public override void Save(UnityModManager.ModEntry modEntry)
            {
                Save(this, modEntry);
            }

            // Class variables need to be refreshed whenver settings get changed. This allows changing settings without needing to restart.
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
