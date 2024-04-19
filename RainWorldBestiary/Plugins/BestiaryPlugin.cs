namespace RainWorldBestiary.Plugins
{
    /// <summary>
    /// The base class for a custom bestiary plugin, that is only loaded if the bestiary is active
    /// </summary>
    public abstract class BestiaryPlugin
    {
        /// <summary>
        /// Called every frame
        /// </summary>
        public virtual void Update() { }
        /// <summary>
        /// Called when the plugin is loaded
        /// </summary>
        public virtual void Awake() { }
        /// <summary>
        /// Called when the plugin is loaded, after awake
        /// </summary>
        public virtual void Start() { }
        /// <summary>
        /// Called every fixed update, usually around 50 times a second
        /// </summary>
        public virtual void FixedUpdate() { }

        ///
        public BestiaryPlugin() { }
    }
}
