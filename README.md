# Rain World Bestiary

This file is here to hopefully help developers get started understanding how they can make their own entries in the bestiary.

You can [report any bugs or issues you encounter here](https://github.com/Oxyaine/RainWorldBestiary/issues), ***with performance issues, try to give as many details you can on where and when you where experiencing the bad performance.***

## First Things First
Everything in the mod (to do with managing and working with entries), is stored in the `Bestiary` class, all settings, including remix menu settings are in the `BestiarySettings` class. Most things should be accessible from these classes, however if something is missing or you'd like something added, leave a issue request and I'll see what I can do.


## Structure
***To get an example of the structure of things, or to have something to look at to help you follow along, you can download / look at the template in the files above.***

Entries can be added to the bestiary without making the mod a dependency! This can be achieved using the built in entry loader, all you need to do is add a folder to your mod folder called `bestiary` (*case sensitive*). Any folders in the `bestiary` folder will be considered a [Tab](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#tabs), and any files in the tab will be considered entries, with the name of the file being the name of the entry.


## Tabs

Tabs are categories that entries can be stored in, they appear as the first set of buttons you see when entering the bestiary menu.
When adding entries, you can add your own tab, or you can use the `Modded` tab if your mod only adds a few entries.
To add entries to the `Modded` tab, just call your tab folder `Modded` or set the name in your [tab's JSON file](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#structure---tabs) to `Modded` (*case sensitive*)
If your mod adds a tab with the same name as an existing tab, the tabs will be "merged", meaning your entries will be added to the existing tab. If your tab has a custom [title image](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-sprite) and the existing tab doesn't, your tabs title image will be used. Same logic applies to [tab_menu_process_id](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#tab_menu_process_id--string).
If your not using the tab's JSON file, the name of your folder will become the name of the tab, but with the JSON file, the name doesn't matter, as the resource manager will use the name set in the JSON file.
Tabs and entries are also organized the way they are loaded from the files (alphabetically), so you can prefix your folders with numbers to change the order they appear, this wont matter if your merging with another tab (as it'll use whatever position the existing tab is in).
Entries don't care what folder they are in, as the resource manager looks for all files in the tab folder, including entries in sub-directories.


## Structure - Tabs

Tabs can be given a JSON file to specify some additional details about the tab, or to separate the folder name and the tabs name. Most features should be accessible using the JSON format, however some behaviours, such as a custom tab menu have to be coded, which would make the bestiary a dependency. The JSON file can contain the following elements:

#### "name" : string
The name of the tab.

#### "title_image" : [TitleSprite](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-sprite)
`Default = null`
The image that is displayed at the top of the tab while viewing it, this is the title image that displays the name of the tab. You can see some more info [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-sprite).

#### "tab_menu_process_id" : string
`Default = The default menu for the bestiary`
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
Whether to show the entry's icon(s) next to the [entry's title](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-sprite) while reading the entry.

#### "title_sprite" : [TitleSprite](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-sprite)
`Default = null`
The title image that gets displayed at the top of the screen while reading the entry, you can find more info on title sprites [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-sprite).


#### "description" : Description
The description of the entry, uses a custom class that can be found [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#description).


## Description
An entry's description is an array of description modules, each module can be given custom unlock behaviour using [unlock tokens](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token)

#### "unlock_id" : [UnlockToken](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token)
`Default = null`
The unlock token of this description module, used to determine what requirements need to be met to unlock this part of the description

#### "text" : string
The text of this module, this gets run through the in game translator, and so you can make the description something like "ENTRY__BATFLY_APPEARANCE", then define a translation into whatever language using the short strings dictionary in `text\text_eng\strings.txt`

#### "new_line" : bool
`Default = false`
Whether this module and the previous module should be separated by a new line '\n', otherwise just separates with a space.



## Title Sprite
The title that appears at the top of a tab or entry while reading it. This usually shows the name of an entry, but really can be set to anything you want. If set to null or if the image isn't found in the atlas manager, an [auto generated title](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#issue--auto-generated-title) will be placed instead.

JSON Elements:

#### "element_name" : string
This is the name of your title element in the atlas manager, make sure to load your title into the atlas manager, or nothing will happen.
You can do this in code using: `Futile.atlasManager.LoadImage()`

#### "scale" : float
`Default = 1`
The scale multiplier for your image, this will affect the size of your image on screen.

#### "x_offset" : int
While the sprite is automatically centered, the image might be just a little bit off, so this is here to allow you to add an offset to it, positive numbers send the image to the right, while negative sends it to the left.

#### "y_offset" : int
Same logic with the X offset, you might need some extra distance from the top of the screen, or maybe less, so positive brings the image down, while negative lifts your sprite up.


## Menu Process
*TODO*

## Unlock Token
Unlock tokens are the way the bestiary determines what parts of descriptions can be unlocked, you can define your own unlock token to set a condition on when your module should be made available.

#### "token_type" : [TokenType](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token-types)
`Default = null`
A number that represents the token type of this unlock, you can see a list of token types [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token-types) as well as their respective id's. The token type determines what action should happen with a creature before this module is unlocked, such as the player killing the creature, or the other way around.

#### "creature_id" : string
The name of this creature that this unlock token should check for, you don't want any creature killing the player to unlock this part of the description, so you set the creature id to say which creature specifically, same rules with the creature ID apply with the [entry's unlock ID](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock_id--string).

#### "count" : byte
`Default = 1`
A number (max 255) that represents how many times the unlock token, defined by token_type and creature_id, should be registered before this token is considered valid. Examples include needing to kill 4-5 of the creature before this module is unlocked (which you would set the value to 4 or 5 depending on what your after), and so on.


## Unlock Token Types

#### Unlock Tokens
Here is a full list of all unlock tokens, further down you can see a list of [which tokens are automatically tracked](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#automatic), although despite them being automatically tracked, you can also just add them manually, if you'd like.

* Tamed = 1 : For when the player tames the creature
* Evaded = 2 : For when the player evades the creature
* Snuck Past = 3 : For when the player sneaks past the creature
* Observed = 4 : For when the player sees the creature
* Observe Fear = 5 : For when the player sees the creature fear something
* Observe Food = 6 : For when the player sees the creature want to eat something or eating something
* Observe Hunting = 7 : For when the player gets hunted / chased by the creature
* Killed = 8 : When the creature gets killed by the player
* Impaled = 9 : When the creature gets impaled with a spear, by the player
* Stunned = 10 : When the creature gets stunned, by the player
* Killed Player = 11 : When the player is killed by the creature
* Grabbed Player = 12 : When the player gets grabbed by the creature
* Eaten = 13 : For when the player eats the creature
* Observe Attraction = 14 : For when the player observes a creature being attracted to something, like batflies to batnip, etc
* Used As Lure = 15 : For when the player uses the creature to lure / distract something else


#### Automatic
Automatic unlock tokens are tokens that are automatically tracked and added by the mod.

***Please keep in mind, that most if not all of these tokens are only tracked for creatures that inherit from base game types, such as the `Creature` class , `AbstractCreature` class, and so on.***

Here is a list of all current automatic token types:
* Killed = 8 : When the creature gets killed by the player
* Impaled = 9 : When the creature gets impaled with a spear, by the player
* Killed Player = 11 : When the player is killed by the creature
* Grabbed Player = 12 : When the player gets grabbed by the creature


#
#

## Issue - Auto Generated Title
The auto generated titles look a little off, The spacing between the letters is weird and inconsistent, I'm not sure why this happens, I've tested my code other places and it seems to work, just something about the way the game draws sprites throws off some of the spacing and makes the characters wonky. I've countered this a bit with some extra code, so it's harder to notice the offsets, but it's still not perfect. I will continue working on solutions, but currently, this is an issue.


# Future Plans

I plan to continue keeping this mod up to date for a while, as well as bug fixes all around. I'll try to make it clear when I've stopped working on this project, and maybe someone else will take over the project and continue working on it, who knows. There's a couple things I still plan to add, here's a somewhat complete list of what I'm planning on changing / adding.

* Built in sprite loader, so you don't need to write any code to make bestiary entries.
* Improved title sprite generator. The [issue with them](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#issue--auto-generated-title) is annoying me.
* Ability to make custom unlock logic, without making the bestiary a dependency.
* Custom unlock tokens, so your not tied to the boundaries of what tokens I've added.
* Try to convert as many unlock tokens to be automated as possible.
* Make entries not just organized but categorized in the tabs
