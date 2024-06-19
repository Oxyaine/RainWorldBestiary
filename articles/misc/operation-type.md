# Operation Type
The operation this unlock token will perform against the current unlock value of the token.
Here is a list of all operations, like with TokenType, you can set it as either the name or the id in the JSON file.

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
