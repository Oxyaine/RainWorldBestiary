# Structure
#
#
## Tabs

Tabs are categories / collections that entries are stored in, they appear as the first set of buttons you see when entering the Bestiary menu.

Tabs are categories that entries are stored in, they appear as the first set of buttons you see when entering the bestiary menu.
When adding entries, you can add your own tab, or you can use the `Modded` tab, for if your mod only adds a few entries.
To add entries to the `Modded` tab, make sure your tabs name is also `Modded` (*case sensitive*), two tabs with the same name will be merged.

### Merging Tabs

When merging tabs, the Bestiary adds all the entries from the new tab into the existing tab. To merge your entries into another tab, simply create a tab with the same name as an existing tab, if the name already exists, it'll merge the two tabs, this is your tabs name before it's translated, and this is not case sensitive.

If your tab has a custom Title Sprite, and the existing tab doesn't, your tabs title sprite will be used.

#
#

## Entries

Entries are the individual documents that hold all the text for the given creature. The entry will use the file name as the default name for the entry, this can however be overridden with the `name` property.

Entries also don't care what sub directory they are in, as the entry loader looks for all files in the tab folder, including entries in sub-directories.

#
#

## Description

The Description is an array of description modules, these modules have a unique unlock condition using unlock tokens; With these conditions, each part of a description is individually unlocked as the player makes discoveries about a creature.

Each module holds its text and an array of unlock tokens which determines what criteria should be met to unlock that module.

#
#

## Unlock Tokens

Unlock tokens are the way the bestiary determines what parts of descriptions can be unlocked, you can define your own unlock token to set a condition on when each module should be made available.

#
#

## Title Sprite

The title that appears at the top of a tab or entry while reading it. This usually shows the name of an entry, but really can be set to anything you want. If set to null or if the image isn't found in the atlas manager, an auto generated title will be placed instead.