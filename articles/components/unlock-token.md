# Unlock Token
Unlock tokens are the way the bestiary determines what parts of descriptions can be unlocked, you can define your own unlock token to set a condition on when your module should be made available.

#### "operation_against_value" : OperationType
`Default = And`
The operation this unlock token will perform against the current unlock value of the token. For example, if the previous two tokens where valid, that means the current unlock value is true, since by the logic of the two previous tokens, the module should be unlocked; Then if this unlock tokens operation is "or", that it will check if the current unlock value OR this value is true, then set the result as the new current unlock value.

#### "token_type" : TokenType
`Default = null`
A TokenType that represents the token type of this unlock, you can see a list of token types in the misc category, as well as their respective names and id's. The token type determines what action should happen with a creature before this module is unlocked, such as the player killing the creature, or the other way around.
***To specify a specific token type you can either enter its name or its id, as either a string or an int.***

#### "creature_id" : string
The name of this creature that this unlock token should check for, you don't want any creature killing the player to unlock this part of the description, so you set the creature id to say which creature specifically, same rules with the creature ID apply with the entry's unlock ID.

#### "count" : byte
`Default = 1`
A number (max 255) that represents how many times the unlock token, defined by token_type and creature_id, should be registered before this token is considered valid. Examples include needing to kill 4-5 of the creature before this module is unlocked (which you would set the value to 4 or 5 depending on what your after), and so on.

#### "special_data" : string[]
Special data is data that can be added to unlock tokens to give some specifics about which interactions the player should've had before this unlock token is valid, if all the strings in here are found in the registered unlock token, this will be considered valid.
