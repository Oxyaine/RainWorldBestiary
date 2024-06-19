# Description
An entry's description is an array of description modules, each module can be given custom unlock behaviour using [unlock tokens](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token)

#### "unlock_token" : [UnlockToken](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token)
`Default = null`
The unlock token of this description module, use this if your module only has 1 condition, otherwise use ["unlock_ids"](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock__ids---unlocktoken), used to determine what requirements need to be met to unlock this part of the description

#### "unlock_tokens" : [UnlockToken](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token)[]
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
