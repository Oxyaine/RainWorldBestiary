# Title Sprite

[__Explanation__](https://oxyaine.github.io/RainWorldBestiary/articles/structure.html#title-sprite)

## "element_name" : string
This is the name of your title element in the atlas manager, make sure to load your title into the atlas manager, or nothing will happen.
You can do this in code using: `Futile.atlasManager.LoadImage()`

## "scale" : float
`Default = 1`

The scale multiplier for your image, this will affect the size of your image on screen.

## "x_offset" : int
While the sprite is automatically centered, the image might be just a little bit off, so this is here to allow you to add an offset to it, positive numbers send the image to the right, while negative sends it to the left.

## "y_offset" : int
Same logic with the X offset, you might need some extra distance from the top of the screen, or maybe less, so positive brings the image down, while negative lifts your sprite up.