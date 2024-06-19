# Unlock Token Types

### Unlock Tokens
Here is a full list of all unlock tokens, **further down you can see a list of which tokens are automatically tracked**, although despite them being automatically tracked, you can also just track them manually, if you'd like.
***To specify a specific token type in a JSON file, you can either enter its name or its id, as either a string or an int.***

* None = 0 : This means this part of the description will always be visible if the entry is visible, however, unlike modules with no unlock token(s), this wont make the entry visible
* Tamed = 1 : For when the player tames the creature
* Evaded = 2 : For when the player evades the creature
* Snuck Past = 3 : For when the player sneaks past the creature
* Observed = 4 : For when the player sees the creature
* Observe Fear = 5 : For when the player sees the creature fear something
* Observe Food = 6 : For when the player sees the creature want to eat something or eating something
* Observe Hunting = 7 : For when the player gets hunted / chased by the creature
* Killed = 8 : When the creature gets killed by the player
* Impaled = 9 : When the creature gets impaled with a spear, by the player
* Stunned = 10 : When the creature gets stunned with a rock, by the player
* Killed Player = 11 : When the player is killed by the creature
* Grabbed Player = 12 : When the player gets grabbed by the creature
* Eaten = 13 : When the player eats the creature
* Observe Attraction = 14 : For when the player observes a creature being attracted to something, like batflies to batnip, etc
* Used As Lure = 15 : For when the player uses the creature to lure / distract something else
* Observe Hiding = 16 : For when the player observes a creature hiding
* Observe Behaviour = 17 : For when the player observes a creature doing a behaviour
* Heard Player = 18 : When a creature hears the player
* Player Grabbed = 19 : When the player grabs the creature
* Observe Rivals = 20 : When the player observes the creature fighting another creature as a rival, like two lizards fighting for territory
* Observe Attacking = 21 : When the player observes the creature attacking another creature


### Automatic
Automatic unlock tokens are tokens that are automatically tracked and added by the mod.

***Please keep in mind, that most if not all of these tokens are only tracked for creatures that inherit from base game types, such as the `Creature` class , `AbstractCreature` class, and so on.***

Here is a list of all current automatic token types:
* Observe Food = 6 : For when the player sees the creature want to eat something or eating something
* Killed = 8 : When the creature gets killed by the player
* Impaled = 9 : When the creature gets impaled with a spear, by the player
* Stunned = 10 : When the creature gets stunned with a rock, by the player
* Killed Player = 11 : When the player is killed by the creature
* Grabbed Player = 12 : When the player gets grabbed by the creature
* Eaten = 13 : When the player eats the creature
* Heard Player = 18 : When the creature hears the player
* Player Grabbed = 19 : When the player grabs the creature
* Observe Rivals = 20 : When the player observes the creature fighting another creature as a rival, like two lizards fighting for territory
* Observe Attacking = 21 : When the player observes the creature attacking another creature, excluding food
