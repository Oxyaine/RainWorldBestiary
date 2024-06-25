# Structure

## Tabs

Tabs are categories that entries are stored in, they appear as the first set of buttons you see when entering the bestiary menu.
When adding entries, you can add your own tab, or you can use the `Modded` tab, for if your mod only adds a few entries.
To add entries to the `Modded` tab, make sure your tabs name is also `Modded` (*case sensitive*), two tabs with the same name will be merged.

### Merging Tabs

Merging tabs add all entries to the existing tab, If your tab has a custom Title Sprite and the existing tab doesn't, your tabs title image will be used.

## Entries

Entries don't care what sub directory they are in, as the entry loader looks for all files in the tab folder, including entries in sub-directories.
Entries' content is also loaded from JSON files. The name of the file determines the name of the entry (however this can be overridden with "name"), and the content of the file determines the rest of the entry's data.

## Description

An entry's description is an array of description modules, each module can be given custom unlock behaviour using unlock tokens

## Description Modules


## Unlock Tokens

Unlock tokens are the way the bestiary determines what parts of descriptions can be unlocked, you can define your own unlock token to set a condition on when your module should be made available.

## Title Sprite

The title that appears at the top of a tab or entry while reading it. This usually shows the name of an entry, but really can be set to anything you want. If set to null or if the image isn't found in the atlas manager, an auto generated title will be placed instead.