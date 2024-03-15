# Rain World Bestiary
An in game bestiary that unlocks as you encounter creatures!

While there are summaries and comments for all functions and variables, there is no documentation (because I am unable to figure out how to make them), this file here is hopefully good enough to help developers get started understanding how they can make their own entries in the bestiary.

## Quick Note
Whenever I change the code (in a way that would break dependent mods, for example renaming a variable / function / class, completely changing how a function runs, or changing the structure of how entries are saved, etc), I'll try my best to leave the old code intact, but to mark it as obsolete, which should hopefully prevent dependent mods from breaking.

## First Things First
Everything in the mod (to do with managing and working with entries), is stored in the `Bestiary` class, all settings, including remix menu settings are in the `BestiarySettings` class. Most things should be accessible from these classes, however if something is missing or you'd like something added, leave a issue request and I'll see what I can do.


## Structure
***To get an example of the structure of things, or to have something to look at to help you follow along, you can download / look at the template in the files above.***

Entries can be added to the bestiary without making the mod a dependency! This can be achieved using the built in entry loader, all you need to do is add a folder to your mod folder called `bestiary` (*case sensitive*). Any folders in the `bestiary` folder will be considered a [Tab](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#tabs), and any files in the tab will be considered entries, with the name of the file being the name of the entry.


## Tabs

Tabs are categories that entries can be stored in, they appear as the first set of buttons you see when entering the bestiary menu.
When adding entries, you can add your own tab, or use the `Modded` tab if your mod only adds a few entries, that don't really warrant an entire new tab to be added.
To add entries to the `Modded` tab, just call your tab folder `Modded` or set the name in your [tab's JSON file](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#structure---tabs) to `Modded` (*case sensitive*)
If your mod adds a tab with the same name as an existing tab, the tabs will be "merged", meaning your entries will be added to the existing tab. If your tab has a custom [title image](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-image) and the existing tab doesn't, your tabs title image will be used. Same logic applies to [tab_menu_process_id](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#tab_menu_process_id--string).


## Structure - Tabs

Tabs can be given a JSON file to specify some additional details about the tab, or to separate the folder name and the tabs name. Most features should be accessible using the JSON format, however some behaviours, such as a custom tab menu have to be coded, which would make the bestiary a dependency. The JSON file can contain the following elements:

#### "name" : string
The name of the tab.

#### "title_image" : [TitleSprite](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-image)
The image that is displayed at the top of the tab while viewing it, this is the title image that displays the name of the tab. You can see some more info [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-image).

#### "tab_menu_process_id" : string
The value of the ProcessID that will be transmitted when the tab is pressed to open, this is part of how the game switches menu processes, you can read a few more details [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#menu-process).


## Structure - Entries

Entries' content is also loaded from JSON files. The name of the file determines the name of the entry, and the content of the file determines the rest of the entry's data. Like with tabs, most features should be accessible using the JSON format, however some behaviours are once again, only accessible through code, I will list everything to do with coding further down, but first, a list of entry's JSON elements:

#### "unlock_id" : string
The id of the creature that will be used to unlock this entry. You can see the ID by using `Bestiary.GetCreatureUnlockName()`, or by going to `AbstractCreature.creatureTemplate.name` of an instance of your creature, and removing the spaces.

#### "locked_text" : string
`Default = "This entry is locked."`
The text / tip that is shown when attempting to read the entry while its locked. Can be used to show a tip on where to find the creature and so on.
This gets run through the in game translator, so you can just give it an ID and add the translation logic for it in the short strings dictionary file.

#### "entry_icon" : string
The icon of the entry, use this if your entry only has 1 icon, otherwise use ["entry_icons"](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#entry_icons--string).
This is the name of the icon in the atlas manager, make sure to load all your custom icons into the atlas manager, or nothing will happen.
You can do this in code using:
`Futile.atlasManager.LoadImage()`


#### "entry_icons" : string[]
The multiple icons of your entry, use this if your entry has multiple icons. This is an array of the names of your icons in the atlas manager, once again, make sure to load all your custom icons into the atlas manager, or nothing will happen.


#### "icons_next_to_title" : bool
`Default = true`
Whether to show the entry's icon(s) next to the [entry's title](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-image) while reading the entry.

#### "title_sprite" : [TitleSprite](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-image)
The title image that gets displayed at the top of the screen while reading the entry, you can find more info on title sprites [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-image).


#### "description" : Description
The description of the entry, uses a custom class that can be found [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#description).


## Description
An entry's description is an array of description modules, each module can be given custom unlock behaviour using [unlock tokens](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token)

#### "unlock_id" : [UnlockToken](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token)
The unlock token of this description module, used to determine what requirements need to be met to unlock this part of the description

#### "text" : string
The text of this module, this gets run through the in game translator, and so you can make the description something like "ENTRY__BATFLY_APPEARANCE", then define a translation into whatever language using the short strings dictionary in `text\text_eng\strings.txt`



## Title Image
If set to null or if the image isn't found in the atlas manager, an [auto generated title](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#issue--auto-generated-title) will be placed instead.



## Menu Process
*TODO*

## Unlock Token
Unlock tokens are the way the bestiary determines what parts of descriptions can be unlocked, you can define your own unlock token to set a condition on when your module should be made available.

#### "token_type" : [TokenType](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token-types)
A number that represents the token type of this unlock, you can see a list of token types [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token-types) as well as their respective id's. The token type determines what action should happen with a creature before this module is unlocked, such as the player killing the creature, or the other way around.

#### "creature_id" : string
The name of this creature that this unlock token should check for, you don't want any creature killing the player to unlock this part of the description, so you set the creature id to say which creature specifically, same rules with the creature ID apply with the [entry's unlock ID](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock_id--string).

#### "value" : byte
A number (max 255) that represents how many times the unlock token, defined by token_type and creature_id, should be registered before this token is considered valid. Examples include needing to kill 4-5 of the creature before this module is unlocked (which you would set the value to 4 or 5 depending on what your after), and so on.


## Unlock Token Types

#### Automatic
Automatic unlock tokens are tokens that are automatically tracked and added by the mod. Here is a list of all automatic token types:
* Killed = 8 : When the player gets killed by a creature
* Impaled = 9 : When the creature gets impaled with a spear, by the player
* Stunned = 10 : When the creature gets stunned, by the player
* Killed Player = 11 : When the player is killed by the creature
* Grabbed Player = 12 : When the player gets grabbed by the creature


#### Manual
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


# Known Issues

## Issue - Auto Generated Title
The auto generated titles