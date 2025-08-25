# Economic Equalization Overhaul
This is a fan mod for the game Terra Invicta. It modifies the way national economies work, to better represent the way countries of different sizes compare.

The project was originally started by [Verdiss] (https://github.com/Verdiss). Development was later taken over by [explodoboy] (https://github.com/explodoboy). For now I am maintaining a fork while it's unclear if any other fork is still being actively maintained.


### Philosophy
In vanilla Terra Invicta, the way country size, country utility, and country control cost change relative to each other is problematic. A country that is twice the size is not twice as powerful, nor twice as expensive to control. Small countries are arbitrarily good at space programs and military, and large countries are arbitrarily good at welfare and education. These trade-offs are purely the result of game mechanic interactions, and do not capture any reasonable understanding of real life effects of national size, nor create any interesting game strategy trade-offs.

The primary cause of these bad relationships has to do with the cube-root relationship between country GDP and investment points, and with the up to 6 times multiplier on the control cost of a nation its number of control points.

As such, the primary function of this mod is to remove both of those issues. Investment points is a linear function of GDP, as is the control cost of a nation. Surrounding these core changes are a huge number of adjustments to the way national investments work, to account for this change and to otherwise even out the rate that a country's utility changes with its size. In this mod, a country that is twice as large is twice as good, but twice as expensive to control.

In short, the goal of the mod is to remove the unintuitive and unrealistic meta-strategizing that surrounds all choices regarding country management, country unifications, and country prioritization.


### Summary of Effects
-National monthly investment points are equal to 1 per 100 Billion GDP.

-Countries with a per capita GDP less than $15,000 receive up to a 30% penalty to investment points.

-The control cap cost of a nation is equal to its investment points, which is then split across each control point. Control cost  of a nation is no longer dependent on the number of control points, as in vanilla.

-Arrival International Relations, Unity Movements, Great Nations, Arrival Governance, and Accelerando each reduce the control cost of a nation to be to the power of 0.98, 0.95, 0.90, 0.85, or 0.80, depending on how many of these techs are researched. Larger nations benefit significantly more.

-Economy investments gives an amount of GDP, as opposed to an amount of GDP per capita (though the tooltip shows the distributed GDP per capita). This makes all economies grow at the same % rate, disregarding modifiers.

-GDP growth from economy investments has diminishing returns based on current GDP per capita. It is based on an exponential decay function of the country's GDP per capita, which gives a nation with 1000 gdp per capita nearly 6 times the growth as a country with 45000.

-Investment effects that impact demographic stats such as education, inequality, or cohesion are scaled inversely based on population size. You need 1000 times as many knowledge investments to increase education by 0.1 in a country with 1 billion people compared to a country with 1 million population.

-Education increases at a faster rate at lower education levels, and a slower rate at higher levels. This is an exponential decay function that gives 4 times the gain rate at 0 education compared to 10 education, and 50% of the gain at 15 education compared to 10.

-Military tech gain is increased by a factor depending on how far the nation is behind the global maximum tech level, a linear bonus of 50% extra gain per 1 tech level behind the current maximum.

-Small adjustments to the relationships between things such as education and GDP growth, broadly maintaining vanilla levels of impact.

-Other changes to investment effects such as spoils and funding amount to flatten out the amount gained per investment to be constant regardless of country size.

-Adjusted costs of many investments to reflect the greatly increased investment points available to most nations, or to simplify calculations.

-Adjusted upkeep cost of armies to be dependent on the host country's tech level. Armies cost a base upkeep of 1 IP, plus 2 IP per national tech level above 3.

-Research output of a nation rebalanced. A nation no longer receives a flat 7.5 + education monthly research, however its research also increases linearly with population, not at a ^1.1 rate as vanilla.


### Issues & the State of the Mod
This mod is sill in an early stage, and bugs, unintended behavior, and imbalances are all but guaranteed. If you encounter a bug, or feel strongly that something should be adjusted, either open an issue in this github repo or leave a comment on one of the mirror sites.

Current focuses for future updates are:

-Bug fixing

-Rebalancing numbers

-AI behavior adjustments


### Installation Info
Version 0.2.3.0 of this mod was built for Terra Invicta version 0.4.90 (the current beta-release branch).

This mod requires [Unity Mod Manager version 0.32.4](https://www.nexusmods.com/site/mods/21/?tab=description) to be installed on your Terra Invicta executable with the DoorstopProxy installation method.

This mod should not be added to an in-progress campaign

To install the mod:

1: Find the [Releases](https://github.com/sayez10/ti-economic-equalization-overhaul) page on this repository and download the appropriate version's zip file (not source).

EITHER 2A: Unpack the TIEconomyMod folder inside this zip into your Terra Invicta\Mods\Enabled folder.

OR 2B: Open the Unity Mod Manager executable, select Terra Invicta, go to the Mods tab, and drag the zip file into the directed box.

3: You should now have a Terra Invicta\Mods\Enabled\TIEconomyMod folder containing a ModInfo.json file, among other things. If so, the mod is correctly installed.

4: Now open the game and start a new game. If the investment point cost of the lower half investments appears unchanged (I.e. boost costs a vanilla 2 investment points, not 4), the game must be restarted until these values change (typically once).

UPDATING: When updating this mod, completely remove the old version of the mod and replace it with the new.


### Links
[Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=xxxxxxxxxx)

[Nexus](https://www.nexusmods.com/terrainvicta/mods/xxxxxxxxxx)
