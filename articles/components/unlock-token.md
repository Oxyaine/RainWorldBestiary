# Unlock Token

[__Explanation__](https://oxyaine.github.io/RainWorldBestiary/articles/structure.html#unlock-tokens)

## "operation_against_value" : OperationType
`Default = And`

The operation this unlock token will use to compare against the current unlock state of the module. 

For example, if all the previous tokens determine the module should be visible, it will compare the result of this token against that value to get the result.

To add more details, lets say all the previous tokens ended with a result of `true`, meaning the module should be visible, if this tokens value is `false`, and the operation type is `or`, that it will check if the current result of `true` OR if this tokens value of `false` are true. The same goes with the `and` operation, it will check if the current result of `true` AND this tokens value of `false` are true.
- You can check the template entries for some more details if this is still confusing.


## "token_type" : TokenType
`Default = None`

A TokenType that represents the token type of this unlock, you can see a list of token types in the misc category, as well as their respective names and id's. The token type determines what action should happen with a creature before this module is unlocked, such as the player killing the creature, or the other way around.
***To specify a specific token type you can either enter its name or its id, as either a string or an int.***

## "creature_id" : string
The name of this creature that this unlock token should check for, you don't want any creature killing the player to unlock this part of the description, so you set the creature id to say which creature specifically, same rules with the creature ID apply with the entry's unlock ID.

## "count" : byte
`Default = 1`

A number (max 255) that represents how many times the unlock token, defined by token_type and creature_id, should be registered before this token is considered valid. Examples include needing to kill 4-5 of the creature before this module is unlocked (which you would set the value to 4 or 5 depending on what your after), and so on.

## "special_data" : string[]
Special data is data that can be added to unlock tokens to give some specifics about which interactions the player should've had before this unlock token is valid, if all the strings in here are found in the registered unlock token, this will be considered valid.
