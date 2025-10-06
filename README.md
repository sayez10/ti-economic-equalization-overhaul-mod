# Economic Equalization Overhaul Mod
This is a fan mod for the game *Terra Invicta*. It modifies the way national economies work, to better represent the way countries of different sizes compare.

The project was originally started by [Verdiss](https://github.com/Verdiss/TI-Economic-Equalization-Overhaul). Development was later taken over by [explodoboy](https://github.com/explodoboy/Economic-Equalization-Overhaul). For now I am maintaining a fork while it's unclear if any other fork is still being actively maintained.


### Philosophy
In vanilla Terra Invicta, the way country size, country utility, and country control cost change relative to each other is problematic. A country that is twice the size is not twice as powerful, nor twice as expensive to control. Small countries are arbitrarily good at space programs and military, and large countries are arbitrarily good at welfare and education. These trade-offs are purely the result of game mechanic interactions, and do not capture any reasonable understanding of real life effects of national size, nor create any interesting game strategy trade-offs.

The primary cause of these bad relationships has to do with the cube-root relationship between country GDP and investment points, and with the up to 6 times multiplier on the control cost of a nation its number of control points.

As such, the primary function of this mod is to remove both of those issues. Investment points is a linear function of GDP, as is the control cost of a nation. Surrounding these core changes are a huge number of adjustments to the way national investments work, to account for this change and to otherwise even out the rate that a country's utility changes with its size. In this mod, a country that is twice as large is twice as good, but twice as expensive to control.

In short, the goal of the mod is to remove the unintuitive and unrealistic meta-strategizing that surrounds all choices regarding country management, country unification, and country prioritization.


### Summary of Effects
-National monthly investment points are equal to 1 per 100 Billion GDP.

-The formula to calculate the control point cost of a nation uses a power function again, like vanilla, and unlike older versions of this mod. However, the parameters have been changed compared to vanilla. Nations with high GDP now require far fewer control points per generated IP than nations with low GDP.

-Economy investments give an amount of GDP, as opposed to an amount of GDP per capita (though the tooltip shows the distributed GDP per capita). This makes all economies grow at the same % rate, disregarding modifiers.

-GDP growth from economy investments has diminishing returns based on current GDP per capita. It is based on an exponential decay function of the country's GDP per capita, which gives a nation with 1,000 GDP per capita nearly 6 times the growth as a country with 45,000.

-Investment effects that impact demographic stats such as education, inequality, or cohesion are scaled inversely based on population size. You need 1,000 times as many knowledge investments to increase education by 0.1 in a country with 1 billion people compared to a country with 1 million population.

-Education increases at a faster rate at lower education levels, and a slower rate at higher levels. This is an exponential decay function that gives 4 times the gain rate at 0 education compared to 10 education, and 50% of the gain at 15 education compared to 10.

-Military tech gain no longer depends on the population of a nation. Instead a military tech has a fixed cost per level, so large and wealthy nations can increase their military tech much faster than small and poor nations. Existing armies and navies increase the cost and thus slow down military technology progress. A 10% bonus to the military priority per level of education has been added. Nations which are far behind the global maximum tech level gain a linear bonus of 50% extra gain per 1 tech level behind the current maximum.

-The algorithm used to calculate the military technology level of a merged nation after absorbing another nation has been changed. Instead of weighing the military tech levels of both nations with their number of owned regions, the larger military technology value is used as starting point. If both military technology values are within 0.5 points, the merged nation's value will be increased by up to 0.5 to represent the often different capabilities of military forces with a roughly comparable technological level and the synergy of combining them. Existing armies and navies might give a malus to the merged nation's military technology value to represent the need to possibly replace or upgrade equipment and to train personnel. [Yard1](https://github.com/Yard1) and his mod [Unification Pop Based Miltech Calculation](https://github.com/Yard1/TerraInvicta-PopBasedMiltechCalculation) provided inspiration for this change, for the general design of the patch, and for the postfix patch in particular, although I chose a completely different approach for the prefix patch and the actual formula to calculate the merged nation's military tech level. Thank you!

-Small adjustments to the relationships between things such as education and GDP growth, broadly maintaining vanilla levels of impact.

-Other changes to investment effects such as spoils and funding amount to flatten out the amount gained per investment to be constant regardless of country size and to attempt to re-balance the money income from nations. The goal has been to make funding the best option to invest in for sustained income and to keep spoils useful to gain a large amount of money in a short time, but incurring significant damage to that nation.

-The maximum annual income from funding in a nation has been increased by a factor of 10 to match the faster increase of the funding priority and to compensate for the nerf to spoils.

-The public opinion effects of unity and spoils priority investment completions now scale inversely with the nation's democracy and education levels.

-Democracy now reduces the lower bound of a nation's corruption by 0.5% per level, down to 0% at 10 democracy

-Existing inequality above 5 can now increase the further inequality gain from completing the economy priority, by up to 100% at inequality level 9. On the other hand, inequality below 5 now reduses the further inequality gain from completing the economy priority, by up to 100% (to 0) at inequality level 1.

-Adjusted costs of many investments to reflect the greatly increased investment points available to most nations and the increased frequency of priority completions, or to simplify calculations.

-Adjusted upkeep cost of armies and navies to be dependent on the host country's tech level.

-Some changes have been made to the calculation of regions' population growth: The bonus from GDP per capita is no longer capped at 180k. The minimum global temperature anomaly required for population growth to decline has been reduced from 8 K to 2 K. The starting population growth modifiers of nations now also expire over a duration of 25 years (not noticeable in vanilla because all nations have a modifier of 0; but mods might change these values). A cap has been added to the malus from education (at level 8). Cardinal_Z's mod "Population Growth Rework" provided inspiration for the latter change. Thank you!

-Lifetime extension technologies of the faction controlling an executive CP in a nation now increase the population growth rate of all regions in that nation. This is supposed to model the initial population growth during times of quickly rising life expectancy.

-A region's education is no longer reduced if its population declines due to natural causes during the monthly update.

-Corruption in a nation now reduces the positive effects of many investment completitions: Income from Spoils, income increase from Funding, GDP growth from Economy, sustainability increase from Environment, democracy increase from government, education increase from Knowledge, cohesion move towards five from Knowledge, inequality reuction from Welfare, unrest reduction from Unity, miltech increase from Military. It also reduces a nation's research output.

-Research output of nations has been re-balanced. A nation no longer receives a flat 7.5 + education monthly research, however its research also increases linearly with population, not at a ^1.1 rate as vanilla. The limit of the quadratic scaling with education > 12 (introduced with vanilla 0.4.1) has been removed. Low democracy (below 5) now reduces research output and the bonus of democracy above 5 on research output has been slightly increased).

-Completing economy priority investments now also increases the price of fissiles, antimatter and exotics on the global market by a very small amount.

-The direct investment code has been rewritten almost completely. The additional scaling with population (on top of that in the actual investment completion methods) was removed. The double-dipping of several effects was removed. All effects now use the same scaling for direct investments and priority investment completions. Nations with low GDP per capita now benefit for a lower cost for economy direct investments. To compensate for the much weaker effects of investment completitions in this mod, the cost of direct investments has been reduced significantly and the annual limit on direct investments in a nation has been increased substantially.

-The limit of annual direct investments in a nation now scales linearly with GDP. To help develop small and especially poor nations, the limit of every nation is increased by a flat amount of 200.

-A few variables have been made configurable in the Unity Mod Manager settings menu: IP output, research production, and control point cost.


### Issues & the State of the Mod
This mod is sill in an early stage, and bugs, unintended behavior, and imbalances are all but guaranteed. If you encounter a bug, or feel strongly that something should be adjusted, either open an issue in this GitHub repository or leave a comment on one of the mirror sites.

Current focuses for future updates are:

-Bug fixing

-Rebalancing numbers

-AI behavior adjustments


### Installation Info
Version 0.2.3.0 of this mod was built for Terra Invicta version 0.4.102 (the current experimental branch).

This mod requires [Unity Mod Manager version 0.32.4](https://www.nexusmods.com/site/mods/21/?tab=description) to be installed on your Terra Invicta executable with the DoorstopProxy installation method.

This mod can be safely added or removed during an ongoing campaign.

To install the mod:

1: Find the [Releases](https://github.com/sayez10/ti-economic-equalization-overhaul-mod) page on this repository and download the appropriate version's .zip file (not source).

EITHER 2A: Unpack the TIEconomicEqualizationOverhaulMod folder inside this .zip file into your Terra Invicta\Mods\Enabled folder.

OR 2B: Open the Unity Mod Manager executable, select Terra Invicta, go to the Mods tab, and drag the .zip file into the directed box.

3: You should now have a Terra Invicta\Mods\Enabled\TIEconomicEqualizationOverhaulMod folder containing a ModInfo.json file, among other things. If so, the mod is correctly installed.

UPDATING: When updating this mod, completely remove the old version of the mod and replace it with the new one.


### Links
[Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=xxxxxxxxxx)

[Nexus](https://www.nexusmods.com/terrainvicta/mods/xxxxxxxxxx)
