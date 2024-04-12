# Rain World Bestiary

The Bestiary is a mod that adds an encyclopedia of creatures to Rain World, this bestiary unlocks as you interact with creatures, carefully only displaying information the user could have figured out themselves.

If you have a suggestion, want to report a bug or issue (including performance issues), or ask help with something, feel free to submit an issue request on GitHub, [here](https://github.com/Oxyaine/RainWorldBestiary/issues)

Some Terminology:
* E

#

## First Things First
### To get a grasp on the Structure of how to add your own entries, you should download the template from the files above.
*This only applies if you are adding the entries through files*
This README.md file is just here to tell you everything that is in the mod, such as all the JSON components you can add, as well as which unlock tokens are automatically tracked, stuff like that.

Entries can be made in two ways, through code, and through JSON files placed in your mod.
- Accessing the code allows you to completely customize your entry, setting a custom unlock condition, custom module unlock conditions, or making a custom menu for your entries reading page. However there are some drawbacks, accessing the code usually makes the Bestiary a hard dependency, meaning your mod wont run unless the bestiary is also installed. You also would have to manually load and unload your entries.
- Using JSON Files gives you slightly less control, but gives the advantages of making the Bestiary completely optional, meaning if the Bestiary is installed, your entries will be visible and unlockable, otherwise if the Bestiary is not installed, they will remain hidden.

#
#
#

## Tabs

Tabs are categories that entries can be stored in, they appear as the first set of buttons you see when entering the bestiary menu.
When adding entries, you can add your own tab, or you can use the `Modded` tab, for if your mod only adds a few entries.
To add entries to the `Modded` tab, make sure your tabs name is also `Modded` (*case sensitive*), two tabs with the same name will be merged.

Tabs and entries are also organized the way they are loaded from the files (alphabetically), so you can prefix your files with numbers to change the order they appear, this wont matter if your merging with another tab, as it'll use whatever position the existing tab is in.
Entries don't care what sub directory they are in, as the entry loader looks for all files in the tab folder, including entries in sub-directories.


Merging tabs add all entries to the existing tab, If your tab has a custom [title image](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-sprite) and the existing tab doesn't, your tabs title image will be used; Same logic applies to [tab_menu_process_id](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#tab_menu_process_id--string).

### Tabs JSON Components

Tabs must be created using a JSON file, which can be used to give the tab a custom name and specify some additional details about the tab. The JSON file can contain the following elements:

#### "name" : string
`Default = (The name of the file)`
The name of the tab.

#### "title_image" : [TitleSprite](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-sprite)
`Default = null`
The image that is displayed at the top of the tab while viewing it, this is the title image that displays the name of the tab. You can see some more info [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#title-sprite).

#### "path" : string
`Default = null`
The local path to the folder that contains your entries, this is a local path from your mods folder, so if your tabs are in the folder `\*mod directory*\bestiary\mytab`, you would set this to `bestiary\mytab`, as your mods folder path is prepended.

#### "required_mods" : string[]
`Default = []`
All the mods (by their IDs) that have to be active for this tab to be visible, if one of the ids specified is not an active mod, the tab wont be visible.

#
#
#

## Structure - Entries

Entries' content is also loaded from JSON files. The name of the file determines the name of the entry (however this can be overridden with "name"), and the content of the file determines the rest of the entry's data.

#### "name" : string
`Default = (The name of the file)`
While by default the name of the file is the name of the entry, you can override the name with this property, and instead, this value you set will become the name of entry.

#### "unlock_id" : string
`Default = ""`
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

#### "color" : string*
`Default = dff5d6`
The hex string for a color, (*uses the last six characters of the string as the hex value*) that determines the color of the entry's button when its unlocked.

#### "description" : [Description](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#description)
The description of the entry, uses a custom class that can be found [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#description).

#
#
#

## Description
An entry's description is an array of description modules, each module can be given custom unlock behaviour using [unlock tokens](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token)

#### "unlock_id" : [UnlockToken](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token)
`Default = null`
The unlock token of this description module, use this if your module only has 1 condition, otherwise use ["unlock_ids"](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock__ids---unlocktoken), used to determine what requirements need to be met to unlock this part of the description

#### "unlock_ids" : [UnlockToken](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token)[]
`Default = []`
The unlock tokens of this description module, use ["operation_against_value"](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#operation_against_value) to determine which operation each module will use against the current value (defaults to and, meaning both this token and the previous token needs to be true for this entry to unlock). This is used to determine what requirements need to be met to unlock this part of the description

#### "body" : string
The text of this module, this gets run through the in game translator, so you can make the description something like "ENTRY_BATFLY_APPEARANCE", then define a translation into whatever language using the short strings dictionary in `text\text_eng\strings.txt`

#### "new_line" : bool
`Default = false`
Whether this module and the previous module should be separated by a new line '\n'. Otherwise just separates with a space.

#### "translate" : bool
`Default = true`
Whether to run the body of this module through the in-game translator, if no translation is found, then nothing will happen

#
#
#

## Title Sprite
The title that appears at the top of a tab or entry while reading it. This usually shows the name of an entry, but really can be set to anything you want. If set to null or if the image isn't found in the atlas manager, an [auto generated title](https://github.com/Oxyaine/RainWorldBestiary/blob/master/Known%20Issues.md#issue---auto-generated-title) will be placed instead.

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

#
#
#

## Unlock Token
Unlock tokens are the way the bestiary determines what parts of descriptions can be unlocked, you can define your own unlock token to set a condition on when your module should be made available.

#### "operation_against_value" : [OperationType](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#operation-type)
The operation this unlock token will perform against the current unlock value of the token. For example, if the previous two tokens where valid, that means the current unlock value is true, since by the logic of the two previous tokens, the module should be unlocked; Then if this unlock tokens operation is "or", that it will check if the current unlock value OR this value is true, then set the result as the new current unlock value. You can see all the operations [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#operation-type).

#### "token_type" : [TokenType](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token-types)
`Default = null`
A TokenType that represents the token type of this unlock, you can see a list of token types [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token-types) as well as their respective names and id's. The token type determines what action should happen with a creature before this module is unlocked, such as the player killing the creature, or the other way around.
***To specify a specific token type you can either enter its name or its id, as either a string or an int.***

#### "creature_id" : string
The name of this creature that this unlock token should check for, you don't want any creature killing the player to unlock this part of the description, so you set the creature id to say which creature specifically, same rules with the creature ID apply with the [entry's unlock ID](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock_id--string).

#### "count" : byte
`Default = 1`
A number (max 255) that represents how many times the unlock token, defined by token_type and creature_id, should be registered before this token is considered valid. Examples include needing to kill 4-5 of the creature before this module is unlocked (which you would set the value to 4 or 5 depending on what your after), and so on.

#### "special_data" : string[]
Special data is data that can be added to unlock tokens to give some specifics about which interactions the player should've had before this unlock token is valid, if all the strings in here are found in the registered unlock token, this will be considered valid. You can see more info [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#special-data).

#
#
#

## Operation Type
The operation this unlock token will perform against the current unlock value of the token. See more info [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#operation_against_value--operationtype).
Here is a list of all operations, like with [TokenType](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token-types), you can set it as either the name or the id in the JSON file.

***0 = false, 1 = true***

* And = 0 : True if both the values are true
	- 0 : 0 = 0
	- 1 : 0 = 0
	- 0 : 1 = 0
	- 1 : 1 = 1
* OR = 1 : True if either of the inputs are true
	- 0 : 0 = 0
	- 1 : 0 = 1
	- 0 : 1 = 1
	- 1 : 1 = 1
* XOr = 2 : True if either of the inputs are true, but not when both inputs are true
	- 0 : 0 = 0
	- 1 : 0 = 1
	- 0 : 1 = 1
	- 1 : 1 = 0
* NAnd = 3 : True if either value is false
	- 0 : 0 = 1
	- 1 : 0 = 1
	- 0 : 1 = 1
	- 1 : 1 = 0
* Nor = 4 : True if neither value is true
	- 0 : 0 = 1
	- 1 : 0 = 0
	- 0 : 1 = 0
	- 1 : 1 = 0
* XAnd = 4 : True if both values are the same
	- 0 : 0 = 1
	- 1 : 0 = 0
	- 0 : 1 = 0
	- 1 : 1 = 1

#
#
#

## Unlock Token Types

#### Unlock Tokens
Here is a full list of all unlock tokens, further down you can see a list of [which tokens are automatically tracked](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#automatic), although despite them being automatically tracked, you can also just track them manually, if you'd like.
***To specify a specific token type in a JSON file, you can either enter its name or its id, as either a string or an int.***

* None = 0 : This means this part of the description will always be visible if the entry is visible, however, unlike modules with no unlock token(s), this wont make the entry visible
* Tamed = 1 : For when the player tames the creature
* Evaded = 2 : For when the player evades the creature
* Snuck Past = 3 : For when the player sneaks past the creature
* Observed = 4 : For when the player sees the creature
* Observe Fear = 5 : For when the player sees the creature fear something
* Observe Food = 6 : For when the player sees the creature want to eat something or eating something
* Observe Hunting = 7 : For when the player gets hunted / chased by the creature
* Killed = 8 : When the creature gets killed by the player
* Impaled = 9 : When the creature gets impaled with a spear, by the player
* Stunned = 10 : When the creature gets stunned with a rock, by the player
* Killed Player = 11 : When the player is killed by the creature
* Grabbed Player = 12 : When the player gets grabbed by the creature
* Eaten = 13 : When the player eats the creature
* Observe Attraction = 14 : For when the player observes a creature being attracted to something, like batflies to batnip, etc
* Used As Lure = 15 : For when the player uses the creature to lure / distract something else
* Observe Hiding = 16 : For when the player observes a creature hiding
* Observe Behaviour = 17 : For when the player observes a creature doing a behaviour
* Heard Player = 18 : When a creature hears the player
* Player Grabbed = 19 : When the player grabs the creature
* Observe Rivals = 20 : When the player observes the creature fighting another creature as a rival, like two lizards fighting for territory
* Observe Attacking = 21 : When the player observes the creature attacking another creature


#### Automatic
Automatic unlock tokens are tokens that are automatically tracked and added by the mod.

***Please keep in mind, that most if not all of these tokens are only tracked for creatures that inherit from base game types, such as the `Creature` class , `AbstractCreature` class, and so on.***

Here is a list of all current automatic token types:
* Observe Food = 6 : For when the player sees the creature want to eat something or eating something
* Killed = 8 : When the creature gets killed by the player
* Impaled = 9 : When the creature gets impaled with a spear, by the player
* Stunned = 10 : When the creature gets stunned with a rock, by the player
* Killed Player = 11 : When the player is killed by the creature
* Grabbed Player = 12 : When the player gets grabbed by the creature
* Eaten = 13 : When the player eats the creature
* Heard Player = 18 : When the creature hears the player
* Player Grabbed = 19 : When the player grabs the creature
* Observe Rivals = 20 : When the player observes the creature fighting another creature as a rival, like two lizards fighting for territory
* Observe Attacking = 21 : When the player observes the creature attacking another creature, excluding food

#
#
#

## Special Data
Special data is data that can be added to unlock tokens to give some specifics to which interactions the player has had, and allows you to set more defined unlock logic for your modules. Below you can see a list of which data is automatically tracked and for which tokens.

* Player Grabbed: When the player grabs a creature
	- Adds "Dead" or "Alive" depending on the status of the creature when its grabbed.
* Observe Food: For when the player sees the creature want to eat something or eating something
	- Adds the unlock name of the creature that was attacked / eaten
* Observe Attacking: When the player observes the creature attacking another creature, excluding food
	- Adds the unlock name of the creature that was attacked
* Observe Rivals: When the player observes the creature fighting another creature as a rival, like two lizards fighting for territory
	- Adds the unlock name of the creature that this creature was competing with