using Newtonsoft.Json;
using System;
using System.Linq;

namespace RainWorldBestiary.Types
{
    /// <summary>
    /// A class representing an entry
    /// </summary>
    public class Entry : IEquatable<Entry>
    {
        /// <summary>
        /// The name of this entry
        /// </summary>
        public readonly string Name = string.Empty;
        /// <summary>
        /// The information of this entry, such as its unlock id, icon, scene to show while reading, and description
        /// </summary>
        public EntryInfo Info = new EntryInfo();

        /// <summary>
        /// The mod that owns this entry, also known as the mod this entry belongs to, is used for unloading the mod automatically when the owning mod gets disabled
        /// </summary>
        internal readonly string OwningModID = null;

        ///
        [JsonConstructor]
        public Entry() { }
        ///<param name="name"></param>
        /// <param name="owningModID">The ID of the mod (id that is set in `modinfo.json` file) that this entry belongs to, set this if you'd like this entry to automatically unload when the mod gets disabled</param>
        public Entry(string name, string owningModID = null) : this()
        {
            Name = name;
            OwningModID = owningModID;
        }
        /// <param name="info">The entry's info</param>
        /// <inheritdoc cref="Entry(string, string)"/>
        /// <param name="name"></param>
        /// <param name="owningModID"></param>
        public Entry(string name, EntryInfo info, string owningModID = null) : this(name, owningModID)
        {
            Name = name;
            Info = info;
        }
        /// <param name="name">The name of the entry</param>
        /// <param name="description">The main body of this entry</param>
        /// <param name="unlockID">The ID that will be used to determine whether this entry is unlocked or not</param>
        /// <param name="iconAtlasName">The name of the entry's icon in the atlas manager</param>
        /// <param name="lockedText">The text that is shown when pressing on the entry while its locked</param>
        /// <inheritdoc cref="Entry(string, string)"/>
        /// <param name="owningModID"></param>
        public Entry(string name, Description description, string unlockID = "", string iconAtlasName = "", string lockedText = EntryInfo.BaseLockedText, string owningModID = null)
            : this(name, owningModID)
        {
            Info = new EntryInfo() { UnlockID = unlockID, EntryIcon = iconAtlasName, Description = description, LockedText = lockedText };
        }
        /// <inheritdoc cref="Entry(string, Description, string, string, string, string)"/>
        public Entry(string name, string description, string unlockID = "", string iconAtlasName = "", string lockedText = EntryInfo.BaseLockedText, string owningModID = null)
            : this(name, owningModID)
        {
            Info = new EntryInfo() { UnlockID = unlockID, EntryIcon = iconAtlasName, LockedText = lockedText, Description = new Description(new DescriptionModule() { Body = description }) };
        }

        /// <summary>
        /// A default entry that's just an error, is always unlocked, and serves as a placeholder that appears when another entry cant be loaded
        /// </summary>
        public static Entry Error => new Entry("ERROR")
        {
            Info = new EntryInfo("Something went wrong with an entry, so this has been created as a warning.\nYou can check the log to see exactly what went wrong.\n", entryIcon: "illustrations\\bestiary\\icons\\error")
            {
                EntryUnlockedCondition = e => false,
                EntryColor = new HSLColor(0f, 0.8f, 0.6f)
            },
        };

        /// <summary>
        /// Checks if this entry is the same as another entry
        /// </summary>
        public bool Equals(Entry other) => OwningModID.Equals(other.OwningModID) && Name.Equals(other.Name) && Info.Equals(other.Info);
    }

    /// <summary>
    /// The contents of the entry file
    /// </summary>
    public class EntryInfo : IEquatable<EntryInfo>
    {
        /// <summary>
        /// A constant defining the default text that is shown when attempting to read a locked entry
        /// </summary>
        public const string BaseLockedText = "This entry is locked.";

        /// <summary>
        /// The name of this entry
        /// </summary>
        [JsonProperty("name")]
        internal readonly string Name = null;

        /// <summary>
        /// The ID of this entry, if the ID is found in the unlocked entries dictionary, this entry will be made visible
        /// </summary>
        [JsonProperty("unlock_id")]
        public string UnlockID = string.Empty;

        /// <summary>
        /// The condition that specifies whether this entry is visible or not, if this returns true, then the entry is visible. You can leave this as the default, or set your own custom condition.
        /// </summary>
        /// <remarks>Defaults to <see cref="DefaultEntryUnlockedCondition(EntryInfo)"/></remarks>
        [JsonIgnore]
        public Func<EntryInfo, bool> EntryUnlockedCondition = DefaultEntryUnlockedCondition;
        /// <summary>
        /// Checks whether any unlock tokens in <see cref="Bestiary"/> have the <see cref="UnlockTokenType"/> for <see cref="CreatureUnlockToken.CreatureID"/> with a value that is equal to or lower than the required value
        /// </summary>
        /// <returns>True if the entry should be locked, otherwise false</returns>
        public static bool DefaultEntryUnlockedCondition(EntryInfo info)
            => Bestiary.Settings.UnlockAllEntries.Value || Bestiary.IsCreatureUnlocked(info.UnlockID);

        /// <summary>
        /// Checks if this entry info's unlock id and icons match
        /// </summary>
        public bool Equals(EntryInfo other) => UnlockID.Equals(other.UnlockID) && EntryIcons.SequenceEqual(other.EntryIcons);

        /// <summary>
        /// Returns true if the entry is visible, else false
        /// </summary>
        [JsonIgnore]
        public bool EntryUnlocked => EntryUnlockedCondition == null || EntryUnlockedCondition(this);


        /// <summary>
        /// The text / tip that is shown when attempting to read the entry while its locked, this could be anything you want, leave blank for no message.
        /// </summary>
        [JsonProperty("locked_text")]
        public string LockedText = BaseLockedText;


        /// <summary>
        /// Whether the two icons specified by <see cref="EntryIcon"/> will be displayed either side of the title
        /// </summary>
        [JsonProperty("icons_next_to_title")]
        public bool IconsNextToTitle = true;


        /// <summary>
        /// The name of the sprite in the atlas manager that will be used as the entry icon
        /// </summary>
        [JsonProperty("entry_icon")]
        public string EntryIcon
        {
            set
            {
                EntryIcons = EntryIcons.Append(value).ToArray();
            }
        }

        /// <summary>
        /// The name of the sprites in the atlas manager that will be used as the entry's icons
        /// </summary>
        [JsonProperty("entry_icons")]
        public string[] EntryIcons = new string[0];

        /// <summary>
        /// The title image that gets displayed at the top when of the screen while reading the entry, if set to null, or if the image isn't found, some generated text will be placed instead
        /// </summary>
        /// <remarks>By title, I mean the name of the entry that is visible at the top while reading the entry</remarks>
        [JsonProperty("title_sprite")]
        public TitleSprite TitleSprite = null;


        /// <summary>
        /// The color of the entry's button and title image
        /// </summary>
        [JsonIgnore]
        public HSLColor EntryColor = new HSLColor(0.4f, 0.6f, 0.9f);


        [JsonProperty("color")]
        private string JSON_Color
        {
            get => "DFF5D6";
            set
            {
                if (!string.IsNullOrEmpty(value))
                    EntryColor = value.HexToColor().ToHSL();
            }
        }


        /// <summary>
        /// The body of this entry, when converted to string, only returns the parts of the entry that are visible
        /// </summary>
        [JsonProperty("description")]
        public Description Description = new Description();

        ///
        [JsonConstructor]
        public EntryInfo()
        {
        }
        /// <param name="description">The body of this entry</param>
        /// <param name="iD">The ID of this entry, if the ID is found in the unlocked entries dictionary, this entry will be made visible</param>
        /// <param name="lockedText">The text / tip that is shown when attempting to read the entry while its locked, this could be anything you want, leave blank for no message.</param>
        /// <param name="entryIcon">The name of the sprite in the atlas manager that will be used as the entry icon</param>
        public EntryInfo(Description description, string iD = "", string lockedText = BaseLockedText, string entryIcon = "")
        {
            UnlockID = iD;
            LockedText = lockedText;
            EntryIcon = entryIcon;
            Description = description;
        }
        /// <summary>
        /// Creates a new entry info with one description module that is by default visible
        /// </summary>
        /// <param name="description">The body of this entry</param>
        /// <param name="iD">The ID of this entry, if the ID is found in the unlocked entries dictionary, this entry will be made visible</param>
        /// <param name="lockedText">The text / tip that is shown when attempting to read the entry while its locked, this could be anything you want, leave blank for no message.</param>
        /// <param name="entryIcon">The name of the sprite in the atlas manager that will be used as the entry icon</param>
        public EntryInfo(string description, string iD = "", string lockedText = BaseLockedText, string entryIcon = "")
        {
            UnlockID = iD;
            LockedText = lockedText;
            EntryIcon = entryIcon;
            Description = new Description(description);
        }
    }
}
