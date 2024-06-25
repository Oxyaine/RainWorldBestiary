# Tabs

Tabs are categories that entries can be stored in, they appear as the first set of buttons you see when entering the bestiary menu.
When adding entries, you can add your own tab, or you can use the `Modded` tab, for if your mod only adds a few entries.
To add entries to the `Modded` tab, make sure your tabs name is also `Modded` (*case sensitive*), two tabs with the same name will be merged.

Tabs and entries are also organized the way they are loaded from the files (alphabetically), so you can prefix your files with numbers to change the order they appear, this wont matter if your merging with another tab, as it'll use whatever position the existing tab is in.
Entries don't care what sub directory they are in, as the entry loader looks for all files in the tab folder, including entries in sub-directories.

Merging tabs add all entries to the existing tab, If your tab has a custom Title Sprite and the existing tab doesn't, your tabs title image will be used.

### "name" : string
`Default = (The name of the file)`
The name of the tab.

### "title_sprite" : TitleSprite
`Default = null`
The image that is displayed at the top of the tab while viewing it, this is the title image that displays the name of the tab.

### "path" : string
`Default = null`
The local path to the folder that contains your entries, this is a local path from your mods folder, so if your tabs are in the folder `\*mod directory*\bestiary\mytab`, you would set this to `bestiary\mytab`, as your mods folder path is prepended.

### "required_mods" : string[]
`Default = []`
All the mods (by their IDs) that have to be active for this tab to be visible, if one of the ids specified is not an active mod, the tab wont be visible.
