# Special Data
Special data is data that can be added to unlock tokens to give some specifics to which interactions the player has had, and allows you to set more defined unlock logic for your modules.

Below you can see a list of which data is automatically tracked and for which tokens.

* Player Grabbed: When the player grabs a creature
	- Adds "Dead" or "Alive" depending on the status of the creature when its grabbed.
* Observe Food: For when the player sees the creature want to eat something or eating something
	- Adds the unlock name of the creature that was attacked / eaten
* Observe Attacking: When the player observes the creature attacking another creature, excluding food
	- Adds the unlock name of the creature that was attacked
* Observe Rivals: When the player observes the creature fighting another creature as a rival, like two lizards fighting for territory
	- Adds the unlock name of the creature that this creature was competing with