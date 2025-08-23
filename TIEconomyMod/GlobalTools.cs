using UnityEngine;



namespace TIEconomyMod
{
    // This is a helper class used to streamline things, and centralize important variables.
    public static class Tools
    {
        // The base threshold is stored as its own variable to allow quick fixing if the devs change the hardcoded requirement
        public const int BASE_OIL_THRESHOLD = 500;
        public const int BASE_MINING_THRESHOLD = 750;
        public const int BASE_ECONOMIC_THRESHOLD = 1200;
        public const int BASE_DECOLONIZE_THRESHOLD = 1000;
        public const int BASE_CLEANUP_THRESHOLD = 100;

        public const int REGION_UPGRADE_THRESHOLD_MULT = 5;

        // These values are dynamically calculated inside a function.
        // They're first calculated after the mod loads, and then whenever settings are changed.
        public static double GDPPerIP;
        private static float theoreticalPopulation;

        public static float EffectStrength(float idealGainPerMonth, float population)
        {
            /*
             * Calculates the effect strength for inverse population scaling.
             *
             * A nation with 30k GDP per-capita will, if putting all of its focus on the relevant priority, increase a particular nation stat by [idealGainPerMonth].
             *
             * For example, if Welfare's Inequality reduction [idealGainPerMonth] is -0.1, and GDP/pc is 30k, then Inequality is reduced at a rate of 0.1 a month.
             *
             * If it's 60k, then the effect is 0.2 per month.
             * If it's 15k, then the effect is 0.05 per month.
             * And so on.
             *
             * The effect strength you will see in-game will likely be much, much less, unless the nation generates less than 1 IP a month.
             *
             * The following reasoning is used in the below equation:
             *
             * Let's say the country has a GDP of 100 billion. They generate 1 IP per month.
             * The GDP per capita is 30k. Therefore, they have a population of 3.33 million.
             *
             * This then is divided by the nation's population to get the final effect strength.
             *
             *
             * TLDR: If a nation has 30k GDP per capita, a stat will be changed by [idealGainPerMonth] a month.
             */

            float effectStrength = idealGainPerMonth * theoreticalPopulation;

            return effectStrength / population;
        }

        public static void Recalculate()
        {
            // 1 billion * setting value
            GDPPerIP = 1000000000 * Main.settings.GDPBillionsPerIP;

            // These are declared outside of EffectStrength() because that function will be called several times. Also, it's readable IMO.
            // For an explanation as to why I did this, check the comments inside the function.
            theoreticalPopulation = (float)GDPPerIP / 30000f;
        }
    }
}
