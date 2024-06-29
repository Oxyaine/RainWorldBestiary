# Entries

[__Explanation__](https://oxyaine.github.io/RainWorldBestiary/articles/structure.html#entries)

## "name" : string
`Default = (The name of the file)`

While by default the name of the file is the name of the entry, you can override the name with this property, and instead, this value you set will become the name of entry.

## "unlock_id" : string
The id of the creature that will be used to unlock this entry. You can see the ID by using `Bestiary.GetCreatureUnlockName()`, or by going to `AbstractCreature.creatureTemplate.name` of an instance of your creature, and removing the spaces.

## "locked_text" : string
`Default = "This entry is locked."`

The text / tip that is shown when attempting to read the entry while its locked. Can be used to show a tip on where to find the creature and so on.
This gets run through the in game translator, so you can just give it an ID and add the translation for it in the short strings dictionary file.

## "entry_icon" : string
The icon of the entry, use this if your entry only has 1 icon, otherwise use "entry_icons".
This is the path to the icon file starting from the root directory of your mod, excluding the file type at the end.

For example: if you have the image `batfly.png` in the `illustrations` folder, you would set this property to `illustrations/batfly`.

## "entry_icons" : string[]
The multiple icons of your entry, use this if your entry has multiple icons. Read more above in `entry_icon` for more details.

## "icons_next_to_title" : bool
`Default = true`

Whether to show the entry's icon(s) next to the entry's title while reading the entry.

## "title_sprite" : TitleSprite
The title sprite that gets displayed at the top of the screen while reading the entry.

You can read more on [title sprites here](title-sprite.html).

## "color" : string*
`Default = dff5d6`

The hex string for a color, (*uses the last six characters of the string as the hex value*) that determines the color of the entry's button when its unlocked.

## "description" : Description
The description of the entry. Uses the [description type](description.html)