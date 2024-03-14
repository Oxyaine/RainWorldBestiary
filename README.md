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
* Killed (8) - Whenever the player gets killed by a creature


**Manual**