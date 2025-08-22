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

        [DrawFields(DrawFieldMask.Public)]
        public class WelfareInvestmentSettings
        {
            [Draw("Base monthly inequality reduction, at 100% IP investment and 1x per-capita GDP investment rate: (default: -0.1)", Max = 0.0f, Precision = 2)] public float baseInequality = -0.1f;
        }

        [DrawFields(DrawFieldMask.Public)]
        public class EnvironmentInvestmentSettings
        {
            [Draw("Base sustainability change, at 100% IP investment and 1x per-capita GDP investment rate: (default: 0.1)", Min = 0.0, Precision = 2)] public float baseSustainability = 0.1f;
            [Draw("Multiplicative boost to sustainability change, per level of sustainability: (default: 0.1)", Min = 0.0, Precision = 2)] public float sustainabilityMultPerSustainabilityLevel = 0.1f;
            [Draw("Penalty to sustainability change, each time territory has been nuked: (default: 0.05)", Min = 0.0, Precision = 2)] public float penaltyPerNukedRegion = 0.05f;
            [Draw("Minimum allowed sustainability change multiplier, regardless of how many regions are nuked: (default: 0.5)", DrawType.Slider, Min = 0.0, Max = 1.0, Precision = 2)] public float maxPenaltyFromNukedRegions = 0.5f;
        }

        [DrawFields(DrawFieldMask.Public)]
        public class KnowledgeInvestmentSettings
        {
            [Draw("Base monthly education change, at 100% IP investment and 1x per-capita GDP investment rate: (default: 0.1)", Min = 0.0, Precision = 2)] public float baseEducation = 0.1f;
            [Draw("Maximum boost to education change, at education level 0: (default: 4.0)", Min = 0.0, Precision = 1)] public float maxScaleFactor = 4.0f;
            [Draw("Education growth diminishment multiplier: (default: 0.87)", DrawType.Slider, Min = 0.0, Max = 1.0, Precision = 2)] public float decayFactor = 0.87f;
            [Draw("Base monthly cohesion change, at 100% IP investment and 1x per-capita GDP investment rate: (default: 0.1)", Min = 0.0, Precision = 2)] public float baseCohesion = 0.1f;
        }

        [DrawFields(DrawFieldMask.Public)]
        public class GovernmentInvestmentSettings
        {
            [Draw("Base monthly democracy change, at 100% IP investment and 1x per-capita GDP investment rate: (default: 0.05)", Min = 0.0, Precision = 2)] public float baseDemocracy = 0.05f;
            [Draw("Multiplicative boost to democracy change, per level of education: (default: 0.1)", Min = 0.0, Precision = 2)] public float democracyMultPerEducationLevel = 0.1f;
        }

        [DrawFields(DrawFieldMask.Public)]
        public class UnityInvestmentSettings
        {
            [Draw("Base monthly cohesion change, at 100% IP investment and 1x per-capita GDP investment rate: (default: 1.0)", Min = 0.0, Precision = 2)] public float baseCohesion = 1.0f;
            [Draw("Penalty to cohesion change as a multiplier, per level of education and democracy: (default: 0.025)", Min = 0.0, Precision = 3)] public float cohesionPenaltyMultPerEducationAndDemocracyLevel = 0.025f;
            [Draw("Worst allowed penalty multiplier from high education and/or democracy level: (default: 0.5)", DrawType.Slider, Min = 0.0, Max = 1.0, Precision = 2)] public float worstMalus = 0.5f;
            [Draw("Base monthly educcation change, at 100% IP investment and 1x per-capita GDP investment rate: (default: -0.01)", Max = 0.0, Precision = 3)] public float baseEducation = -0.01f;
            [Draw("Multiplier to overall propaganda power: (default: 0.2)", Min = 0.0, Precision = 2)] public float propagandaOffset = 0.2f;
        }

        [DrawFields(DrawFieldMask.Public)]
        public class OppressionInvestmentSettings
        {
            [Draw("Base monthly unrest change, at 100% IP investment and 1x per-capita GDP investment rate: (default: -2.5)", Max = 0.0, Precision = 2)] public float baseUnrest = -2.5f;
            [Draw("Penalty to unrest change as a multiplier, per level of democracy: (default: 0.1)", DrawType.Slider, Min = 0.0, Max = 0.1, Precision = 2)] public float unrestPenaltyMultPerDemocracyLevel = 0.1f;
            [Draw("Base monthly cohesion change, at 100% IP investment, 1x per-capita GDP investment rate, and level 10 democracy: (default: -1.0)", Min = 1.0, Precision = 2)] public float baseCohesion = -1.0f;
            [Draw("The democracy level which the cohesion loss effect begins to take effect. If set to 10, cohesion loss is disabled: (default: 5.0)", DrawType.Slider, Min = 0.0, Max = 10.0, Precision = 1)] public float minDemocracyForCohesionChange = 5.0f;
            [Draw("Base monthly democracy change, at 100% IP investment and 1x per-capita GDP investment rate: (default: -0.0175)", Max = 0.0, Precision = 4)] public float baseDemocracy = -0.0175f;
        }

        [DrawFields(DrawFieldMask.Public)]
        public class FundingInvestmentSettings
        {
            [Draw("Flat increase to a country's monthly money resource output, per investment point: (default: 15.0)", Min = 0.0, Precision = 1)] public float fundingAmount = 15.0f;
        }

        [DrawFields(DrawFieldMask.Public)]
        public class MilitaryInvestmentSettings
        {
            [Draw("Base miltech change, at 100% IP investment and 1x per-capita GDP investment rate: (default: 0.025)", Min = 0.0, Precision = 3)] public float baseMiltech = 0.025f;
            [Draw("Extra boost to miltech change as a multiplier, per level behind the global limit: (default: 0.5)", Min = 0.0, Precision = 2)] public float miltechPerMiltechLevelBehind = 0.5f;
        }

        /// <summary>
        ///
        /// </summary>
        public class Settings : UnityModManager.ModSettings, IDrawable
        {
            [Draw("Investment Points", Collapsible = true)] public InvestmentPointsSettings investmentPoints = new InvestmentPointsSettings();
            [Draw("Research Production", Collapsible = true)] public ResearachProductionSettings researchProduction = new ResearachProductionSettings();
            [Draw("Welfare Investment", Collapsible = true)] public WelfareInvestmentSettings welfareInvestment = new WelfareInvestmentSettings();
            [Draw("Environment Investment", Collapsible = true)] public EnvironmentInvestmentSettings environmentInvestment = new EnvironmentInvestmentSettings();
            [Draw("Knowledge Investment", Collapsible = true)] public KnowledgeInvestmentSettings knowledgeInvestment = new KnowledgeInvestmentSettings();
            [Draw("Government Investment", Collapsible = true)] public GovernmentInvestmentSettings governmentInvestment = new GovernmentInvestmentSettings();
            [Draw("Unity Investment", Collapsible = true)] public UnityInvestmentSettings unityInvestment = new UnityInvestmentSettings();
            [Draw("Oppression Investment", Collapsible = true)] public OppressionInvestmentSettings oppressionInvestment = new OppressionInvestmentSettings();
            [Draw("Funding Investment", Collapsible = true)] public FundingInvestmentSettings fundingInvestment = new FundingInvestmentSettings();
            [Draw("Military Investment", Collapsible = true)] public MilitaryInvestmentSettings militaryInvestment = new MilitaryInvestmentSettings();

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
