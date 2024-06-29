# Description

[__Explanation__](https://oxyaine.github.io/RainWorldBestiary/articles/structure.html#description)

## "unlock_token" : UnlockToken
`Default = null`

The unlock token of this description module, use this if your module only has 1 condition, otherwise use "unlock_tokens". Used to determine what requirements need to be met to unlock this part of the description.

You can read more on [__unlock tokens here__](unlock-token.html)

## "unlock_tokens" : UnlockToken[]
`Default = []`

The unlock tokens of this description module, use "operation_against_value" to determine which operation each module will use against the current value (defaults to and, meaning both this token and the previous token needs to be true for this entry to unlock).

You can read more info in the `unlock_token` property above

## "body" : string
The text of this module, this gets run through the in game translator, so you can make the description something like "ENTRY_BATFLY_APPEARANCE", then define a translation into whatever language using the short strings dictionary in `text\text_eng\strings.txt`

## "new_line" : bool
`Default = false`

Whether this module and the previous module should be separated by a new line '\n'. Otherwise just separates with a space.

## "translate" : bool
`Default = true`

Whether to run the body of this module through the in-game translator, if no translation is found, then nothing will happen
