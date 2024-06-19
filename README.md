# README.md
## Operation Type
The operation this unlock token will perform against the current unlock value of the token. See more info [here](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#operation_against_value--operationtype).
Here is a list of all operations, like with [TokenType](https://github.com/Oxyaine/RainWorldBestiary?tab=readme-ov-file#unlock-token-types), you can set it as either the name or the id in the JSON file.

***0 = false, 1 = true***

* And = 0 : True if both the values are true
	- 0 : 0 = 0
	- 1 : 0 = 0
	- 0 : 1 = 0
	- 1 : 1 = 1
* OR = 1 : True if either of the inputs are true
	- 0 : 0 = 0
	- 1 : 0 = 1
	- 0 : 1 = 1
	- 1 : 1 = 1
* XOr = 2 : True if either of the inputs are true, but not when both inputs are true
	- 0 : 0 = 0
	- 1 : 0 = 1
	- 0 : 1 = 1
	- 1 : 1 = 0
* NAnd = 3 : True if either value is false
	- 0 : 0 = 1
	- 1 : 0 = 1
	- 0 : 1 = 1
	- 1 : 1 = 0
* Nor = 4 : True if neither value is true
	- 0 : 0 = 1
	- 1 : 0 = 0
	- 0 : 1 = 0
	- 1 : 1 = 0
* XAnd = 4 : True if both values are the same
	- 0 : 0 = 1
	- 1 : 0 = 0
	- 0 : 1 = 0
	- 1 : 1 = 1

## Special Data
Special data is data that can be added to unlock tokens to give some specifics to which interactions the player has had, and allows you to set more defined unlock logic for your modules. Below you can see a list of which data is automatically tracked and for which tokens.

* Player Grabbed: When the player grabs a creature
	- Adds "Dead" or "Alive" depending on the status of the creature when its grabbed.
* Observe Food: For when the player sees the creature want to eat something or eating something
	- Adds the unlock name of the creature that was attacked / eaten
* Observe Attacking: When the player observes the creature attacking another creature, excluding food
	- Adds the unlock name of the creature that was attacked
* Observe Rivals: When the player observes the creature fighting another creature as a rival, like two lizards fighting for territory
	- Adds the unlock name of the creature that this creature was competing with