# Title Sprite
The title that appears at the top of a tab or entry while reading it. This usually shows the name of an entry, but really can be set to anything you want. If set to null or if the image isn't found in the atlas manager, an [auto generated title](https://github.com/Oxyaine/RainWorldBestiary/blob/master/Known%20Issues.md#issue---auto-generated-title) will be placed instead.

#### "element_name" : string
This is the name of your title element in the atlas manager, make sure to load your title into the atlas manager, or nothing will happen.
You can do this in code using: `Futile.atlasManager.LoadImage()`

#### "scale" : float
`Default = 1`
The scale multiplier for your image, this will affect the size of your image on screen.

#### "x_offset" : int
While the sprite is automatically centered, the image might be just a little bit off, so this is here to allow you to add an offset to it, positive numbers send the image to the right, while negative sends it to the left.

#### "y_offset" : int
Same logic with the X offset, you might need some extra distance from the top of the screen, or maybe less, so positive brings the image down, while negative lifts your sprite up.