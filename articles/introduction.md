# Introduction

The Bestiary aims to make entries slowly unlock as the player discovers things about the creature. We wanted to allow other modders to also create their own entries for the Bestiary, while making sure the Bestiary stays completely optional (so that no one is forced to download it even if their favorite additional creatures mod has entries). That did make the Bestiary somewhat complicated, so we've made this documentation site to hopefully guide other developers into creating their own entries.

## Entries Adding Methods
Entries can be made in two ways, through code, and through JSON files placed in your mod.

There are ways to add entries without making the Bestiary a dependency; With JSON files, this is done automatically, as the Bestiary can only load your entries if it is active; With code however, you must take an additional step, You can read more on how to do this [here](walkthrough.html#plugins).

### Through Code
While adding entries through code is possible, it's not fully supported, and it's recommended you add entries through JSON files. Adding entries through code does give slightly more customization, such as setting a custom unlock condition, and custom module unlock conditions.

### Through JSON Files
Using JSON Files gives you slightly less control, but is fully supported, and gives some advantages such as the bestiary managing the loading and unloading of your entries.