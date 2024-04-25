// This is a basic C# file showing off how to make your own plugin for the bestiary
// Bestiary plugins work similar to other mods; Where you place a DLL file into the plugins folder;
// The only difference is, bestiary plugins are only loaded when the bestiary mod is active, keeping the bestiary optional.
// This File is just here to show what an example of the code would look like, this file should be compiled into a DLL file, and the DLL should be placed here instead

using RainWorldBestiary.Plugins;

namespace MyPlugin
{
    // We make our class inherit from BestiaryPlugin, which makes the bestiary automatically detect this class, initialize it, and run it as a plugin
    // If you make a constructor for your custom class, make sure it also has a default constructor with no parameters, or an exception will be thrown
    public class MyBestiaryPlugin : BestiaryPlugin
    {
        // Called when the plugin is loaded, awake is called before start
        public override void Awake()
        {
            // While you can put any code here, and it'll function as a normal BepinEX plugin, for this example we'll just initialize a custom class
            MyHooks.Initialize();
        }
    }

    public static class MyHooks
    {
        public static void Initialize()
        {
            // We will add some logic onto the Fly.Grabbed event, which is called when.... well.... a batfly is grabbed.
            // You don't need to do this normally, as creatures getting grabbed by the player is automatically tracked, this is just here for this example.
            // You can see all the automatically tracked behaviours in the README.md file on GitHub
            On.Fly.Grabbed += Fly_Grabbed;
        }

        public static void Fly_Grabbed(On.Fly.orig_Grabbed original, Fly self, Creature.Grasp grasp)
        {
            // We call the original method that defines the logic for when a batfly is grabbed
            original(self, grasp);

            // If the creature that grabbed the batfly is the player, we continue
            if (grasp.grabber is Player)
            {
                // We will add a new unlock token to the bestiary
                // Self is the creature that this unlock token is for, the bestiary automatically grabs the creature unlock name using Bestiary.GetCreatureUnlockName, however, you can also do:
                // Bestiary.AddOrIncreaseToken(Bestiary.GetCreatureUnlockName(self), UnlockTokenType.PlayerGrabbed);
                // UnlockTokenType.PlayerGrabbed is the type of token, this can be anything from the enum.
                // In this case, we want it to increase the token that tracks how many times the player has grabbed the creature.
                Bestiary.AddOrIncreaseToken(self, UnlockTokenType.PlayerGrabbed);
            }
        }
    }
}