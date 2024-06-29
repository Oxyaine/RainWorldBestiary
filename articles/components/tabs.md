# Tabs

[__Explanation__](https://oxyaine.github.io/RainWorldBestiary/articles/structure.html#tabs)

## "name" : string
`Default = (*The name of the file*)`

The name of the tab.

## "title_sprite" : TitleSprite
The image that is displayed at the top of the tab while viewing it, this is the title image that displays the name of the tab.

You can read more on [title sprites here](title-sprite.html).

## "path" : string
`Default = null`

The local path to the folder that contains your entries, this is a local path from your mods folder, so if your tabs are in the folder `\*mod directory*\bestiary\mytab`, you would set this to `bestiary\mytab`, as your mods folder path is prepended.

## "required_mods" : string[]
`Default = []`

All the mods (by their IDs) that have to be active for this tab to be visible, if one of the ids specified is not an active mod, the tab wont be visible.
