using Newtonsoft.Json;

namespace RainWorldBestiary.Types
{
    /// <summary>
    /// Represents an element in the atlas manager, but gives some more options to customize the scale and offset of the image from the default values
    /// </summary>
    public class TitleSprite
    {
        /// <summary>
        /// The name of the element in the atlas manager
        /// </summary>
        [JsonProperty("element_name")]
        public string ElementName = string.Empty;
        /// <summary>
        /// The scale of the image when drawn to the screen
        /// </summary>
        [JsonProperty("scale")]
        public float Scale = 1;
        /// <summary>
        /// The offset on the X axis from the default position
        /// </summary>
        [JsonProperty("x_offset")]
        public int XOffset = 0;
        /// <summary>
        /// THe offset on the Y axis from the default position
        /// </summary>
        [JsonProperty("y_offset")]
        public int YOffset = 0;

        [JsonConstructor]
        private TitleSprite() { }
        /// <inheritdoc cref="TitleSprite(string, float, int, int)"/>
        public TitleSprite(string elementName)
        {
            ElementName = elementName;
        }
        /// <param name="elementName">The name of the element in the atlas manager</param>
        /// <param name="scale">The scale multiplier of this icon</param>
        /// <param name="xOffset">The X Offset from the default position</param>
        /// <param name="yOffset">The Y Offset from the default position </param>
        public TitleSprite(string elementName, float scale, int xOffset = 0, int yOffset = 0) : this(elementName)
        {
            Scale = scale;
            XOffset = xOffset;
            YOffset = yOffset;
        }
    }
}
