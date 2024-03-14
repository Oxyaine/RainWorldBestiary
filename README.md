# Rain World Bestiary
An in game bestiary that unlocks as you encounter creatures!

While there are summaries and comments for all functions and variables, there is no documentation (because I am unable to figure out how to make them), this file here is hopefuly good enough to help developers get started understanding how they can make their own entries in the bestiary.

# Quick Note
Whenever I change the code (in a way that would break dependent mods, for example renaming a variable / function / class, completely changing how a function runs, or changing the structure of how entries are saved), I'll try my best to leave the old code intact, but to mark it as obsolete, which should hopefuly prevent dependent mods from breaking.

# First Things First
Everything in the mod (to do with managing and working with entries), is stored in the `Bestiary` class, all settings, including remix menu settings are in the `BestiarySettings` class. Most things should be accessible from these classes, however if something is missing or you'd like something added, leave a issue request and I'll see what I can do.


# Structure

Entries can be added to the bestiary without making the mod a dependency! This can be achieved using the built in entry loader, all you need to do is add a folder to your mod folder called `bestiary` (*case sensitive*). Any folders in the `bestiary` folder will be considered a [Tab](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#tabs), and any files in the tab will be considered entries, with the name of the file being the name of the entry.


# Tabs

Tabs are categories that entries can be stored in, they appear as the first set of buttons you see when entering the bestiary menu.
When adding entries, you can add your own tab, or use the `Modded` tab if your mod only adds a few entries, that don't really warrant an entire new tab to be added.
To add entries to the `Modded` tab, just call your tab folder `Modded` or set the name in your [tab's JSON file](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#structure---tabs) to `Modded` (*case sensitive*)
If your mod adds a tab with the same name as an existing tab, the tabs will be "merged", meaning your entries will be added to the existing tab. If your tab has a custom [title image](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-image) and the existing tab doesn't, your tabs title image will be used. Same logic applies to [`tab_menu_process_id`](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#menu-process).


# Structure - Tabs

Tabs can be given a JSON file to specify some additional details about the tab, or to separate the folder name and the tabs name. The JSON file can contain the following elements:

### "name"
The


# Structure - Entries




# Title Image




# Menu Process



# Unlock Token Types

Automatic unlock tokens are tokens that are automatically tracked and added by the mod. Here is a list of all automatic token types:
* Killed = 8 : When the player gets killed by a creature
* Impaled = 9 : When the creature gets impaled with a spear, by the player
* Stunned = 10 : When the creature gets stunned, by the player
* Killed Player = 11 : When the player is killed by the creature
* Grabbed Player = 12 : When the player gets grabbed by the creature


Manual tokens are tokens that the mod does not track by itself, and must be tracked by your mod. These are usually manual because they are specific to one creature, or one type of creature, so it cannot be tracked for all creatures without explicit implementation. Here is a list of all manual token types:
* Tamed = 1 : For when the player tames the creature
* Evaded = 2 : For when the player evades the creature
* Snuck Past = 3 : For when the player sneaks past the creature
* Observed = 4 : For when the player sees the creature
* Observe Fear = 5 : For when the player sees the creature fear something
* Observe Food = 6 : For when the player sees the creature want to eat something or eating something
* Observe Hunting = 7 : For when the player gets hunted / chased by the creature
* Eaten = 13 : For when the player eats the creature
* Observe Attraction = 14 : For when the player observes a creature being attracted to something