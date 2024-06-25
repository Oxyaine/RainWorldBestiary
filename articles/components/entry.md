# Entries

Entries' content is also loaded from JSON files. The name of the file determines the name of the entry (however this can be overridden with "name"), and the content of the file determines the rest of the entry's data.

### "name" : string
`Default = (The name of the file)`
While by default the name of the file is the name of the entry, you can override the name with this property, and instead, this value you set will become the name of entry.

### "unlock_id" : string
`Default = ""`
The id of the creature that will be used to unlock this entry. You can see the ID by using `Bestiary.GetCreatureUnlockName()`, or by going to `AbstractCreature.creatureTemplate.name` of an instance of your creature, and removing the spaces.

### "locked_text" : string
`Default = "This entry is locked."`
The text / tip that is shown when attempting to read the entry while its locked. Can be used to show a tip on where to find the creature and so on.
This gets run through the in game translator, so you can just give it an ID and add the translation logic for it in the short strings dictionary file.

### "entry_icon" : string
The icon of the entry, use this if your entry only has 1 icon, otherwise use "entry_icons".
This is the name of the icon in the atlas manager, make sure to load all your custom icons into the atlas manager, or nothing will happen.
You can do this in code using:
`Futile.atlasManager.LoadImage()`

### "entry_icons" : string[]
The multiple icons of your entry, use this if your entry has multiple icons. This is an array of the names of your icons in the atlas manager, once again, make sure to load all your custom icons into the atlas manager, or nothing will happen.

### "icons_next_to_title" : bool
`Default = true`
Whether to show the entry's icon(s) next to the entry's title while reading the entry.

### "title_sprite" : TitleSprite
`Default = null`
The title image that gets displayed at the top of the screen while reading the entry, you can find more info on title sprites here.

### "color" : string*
`Default = dff5d6`
The hex string for a color, (*uses the last six characters of the string as the hex value*) that determines the color of the entry's button when its unlocked.

### "description" : Description
The description of the entry.