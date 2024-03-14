# Rain World Bestiary
An in game bestiary that unlocks as you encounter creatures!

While there are summaries and comments for all functions and variables, there is no documentation (because I am unable to figure out how to make them), this file here is hopefuly good enough to help developers get started understanding how they can make their own entries in the bestiary.

# Quick Note
Whenever I change the code (in a way that would break dependent mods, for example renaming a variable / function / class, completely changing how a function runs, or changing the structure of how entries are saved), I'll try my best to leave the old code intact, but to mark it as obsolete, which should hopefuly prevent dependent mods from breaking.

# First Things First
Everything in the mod (to do with managing and working with entries), is stored in the `Bestiary` class, all settings, including remix menu settings are in the `BestiarySettings` class. Most things should be accessible from these classes, however if something is missing or you'd like something added, leave a issue request and I'll see what I can do.

# Details
*TODO*


# Unlock Token Types

**Automatic:**
Automatic unlock tokens are tokens that are automatically tracked and added by the mod.
* Killed = 8 : Whenever the player gets killed by a creature
* Impaled = 9 : When the creature gets impaled with a spear, by the player
* Stunned = 10 : When the creature gets stunned, by the player
* Killed Player = 11 : When the player is killed by the creature
* Grabbed Player = 12 : When the player gets grabbed by the creature


**Manual**
Manual tokens are tokens that the mod does not track by itself, and must be tracked by your mod. These are usually manual because they are specific to one creature, or one type of creature, so it cannot be tracked for all creatures without explicit implementation.
* Tamed = 1 : For when the player tames the creature
* Evaded = 2 : For when the player evades the creature
* Snuck Past = 3 : For when the player sneaks past the creature
* Observed = 4 : For when the player sees the creature
* Observe Fear = 5 : For when the player sees the creature fear something
* Observe Food = 6 : For when the player sees the creature want to eat something or eating something
* Observe Hunting = 7 : For when the player gets hunted / chased by the creature
* Eaten = 13 : For when the player eats the creature
* Observe Attraction = 14 : For when the player observes a creature being attracted to something