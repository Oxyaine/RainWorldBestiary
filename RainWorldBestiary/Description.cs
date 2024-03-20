using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RainWorldBestiary
{
    /// <summary>
    /// A class representing an entries description, saved as a DescriptionModule array, but can be used as a string
    /// </summary>
    public class Description : IEnumerable<DescriptionModule>, ICollection<DescriptionModule>
    {
        readonly List<DescriptionModule> _values = new List<DescriptionModule>();

        ///
        [JsonConstructor]
        public Description() { }
        ///
        public Description(IEnumerable<DescriptionModule> modules)
        {
            _values = modules.ToList();
        }
        ///
        public Description(params DescriptionModule[] modules)
        {
            _values = modules.ToList();
        }
        /// <summary>
        /// Creates a new description with one module that defaults to unlocked
        /// </summary>
        public Description(string description)
        {
            _values = new List<DescriptionModule>() { new DescriptionModule() { Body = description } };
        }

        /// <summary>
        /// Gets the amount of description modules in this description
        /// </summary>
        public int Count => _values.Count;

        /// <summary>
        /// Gets the amount of description modules that are visible to the player
        /// </summary>
        public int UnlockedCount
        {
            get
            {
                int count = 0;
                foreach (DescriptionModule module in _values)
                {
                    if (!module.ModuleUnlocked)
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// This is unused in <see cref="Description"/>
        /// </summary>
        public bool IsReadOnly => ((ICollection<DescriptionModule>)_values).IsReadOnly;

        /// <summary>
        /// Gets or sets a module at the given index
        /// </summary>
        public DescriptionModule this[int index] { get => _values[index]; set => _values[index] = value; }



        /// <inheritdoc/>
        public IEnumerator<DescriptionModule> GetEnumerator() => _values.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

        /// <summary>
        /// Gets how much of the entry is locked (range 0 - 1)
        /// </summary>
        public float GetUnlockedPercentage() => UnlockedCount / (float)Count;

        /// <summary>
        /// Returns this description as a string where only the parts of the entry that are visible are added
        /// </summary>
        public override string ToString()
        {
            string result = string.Empty;

            foreach (DescriptionModule module in _values)
            {
                if (module.ModuleUnlocked)
                {
                    result += (module.NewLine ? "\n" : " ") + OptionInterface.Translate(module.ToString());
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Add(DescriptionModule item) => _values.Add(item);
        /// <inheritdoc/>
        public void Clear() => _values.Clear();
        /// <inheritdoc/>
        public bool Contains(DescriptionModule item) => _values.Contains(item);
        /// <inheritdoc/>
        public void CopyTo(DescriptionModule[] array, int arrayIndex) => _values.CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public bool Remove(DescriptionModule item) => _values.Remove(item);

        ///
        public static implicit operator Description(DescriptionModule[] modules) => new Description(modules);
        ///
        public static implicit operator Description(List<DescriptionModule> modules) => new Description(modules);
    }

    /// <summary>
    /// A piece of a description, can be given a lock ID to lock this part of an entries description
    /// </summary>
    public class DescriptionModule
    {

        /// <summary>
        /// The unlock token of this description module, used to determine what requirements need to be met to unlock this part of the description
        /// </summary>
        [JsonProperty("unlock_id")]
        public CreatureUnlockToken UnlockID
#if DEBUG
        {
            set
            {
                UnlockIDs = UnlockIDs.Append(value).ToArray();
            }
        }
#else
            = null;
#endif

#if DEBUG
        [JsonProperty("unlock_ids")]
        public CreatureUnlockToken[] UnlockIDs = new CreatureUnlockToken[0];
#endif

        /// <summary>
        /// The condition that specifies whether this entry is visible or not, if this returns true, then the entry is visible. You can leave this as the default, or set your own custom condition.
        /// </summary>
        /// <remarks>Defaults to <see cref="DefaultModuleUnlockedCondition(DescriptionModule)"/></remarks>
        [JsonIgnore]
        public Func<DescriptionModule, bool> ModuleUnlockedCondition = DefaultModuleUnlockedCondition;

        /// <summary>
        /// Checks if <see cref="UnlockID"/> is found in <see cref="Bestiary.ModuleUnlocks"/> by checking for the creature id and using <see cref="UnlockToken.Equals(UnlockToken)"/>
        /// </summary>
        public static bool DefaultModuleUnlockedCondition(DescriptionModule info)
#if DEBUG
        {
            if (BestiarySettings.UnlockAllEntries.Value)
                return true;

            bool unlocked = true;
            foreach (CreatureUnlockToken unlock in info.UnlockIDs)
            {
                switch (unlock.OperationAgainstCurrentValue)
                {
                    case OperationType.And:
                        unlocked = unlocked && CheckIfUnlockTokenUnlocked(unlock);
                        break;
                    case OperationType.Or:
                        unlocked = unlocked || CheckIfUnlockTokenUnlocked(unlock);
                        break;
                    case OperationType.XOr:
                        bool token = CheckIfUnlockTokenUnlocked(unlock);
                        unlocked = (unlocked && !token) || (!unlocked && token);
                        break;
                    case OperationType.NAnd:
                        unlocked = !(unlocked && CheckIfUnlockTokenUnlocked(unlock));
                        break;
                    case OperationType.NOr:
                        unlocked = !unlocked && !CheckIfUnlockTokenUnlocked(unlock);
                        break;
                    case OperationType.XAnd:
                        bool token2 = CheckIfUnlockTokenUnlocked(unlock);
                        unlocked = (unlocked && token2) || (!unlocked && !token2);
                        break;
                }
            }

            return unlocked;
        }

        /// <summary>
        /// Checks if this creature unlock token is null, the token type is none, or if it returns true when run through <see cref="Bestiary.IsUnlockTokenValid(CreatureUnlockToken)"/>.
        /// </summary>
        /// <returns>True if either of the conditions above is met</returns>
        public static bool CheckIfUnlockTokenUnlocked(CreatureUnlockToken unlockToken)
            => unlockToken == null || unlockToken.TokenType == UnlockTokenType.None || Bestiary.IsUnlockTokenValid(unlockToken);
#else
            => BestiarySettings.UnlockAllEntries.Value || info.UnlockID == null || info.UnlockID.TokenType == UnlockTokenType.None || Bestiary.IsUnlockTokenValid(info.UnlockID);
#endif
        /// <summary>
        /// Returns true if the module is unlocked, else false
        /// </summary>
        [JsonIgnore]
        public bool ModuleUnlocked => ModuleUnlockedCondition == null || ModuleUnlockedCondition(this);

        /// <summary>
        /// Whether this module contributes to making the entry visible
        /// </summary>
        /// <remarks>Checks if the unlock id TokenType is none, if it is, this returns false, meaning this module doesn't contribute to unlocking the entire entry, see <see cref="UnlockTokenType.None"/> for more info</remarks>
        [JsonIgnore]
        public bool ContributesToEntryUnlock => UnlockID == null || UnlockID.TokenType != UnlockTokenType.None;

        [JsonProperty("text"), Obsolete("Use DescriptionModule.Body Instead")]
        private string Text { set => Body = value; }

        /// <summary>
        /// The text of this part of the entries description
        /// </summary>
        [JsonProperty("body")]
        public string Body = string.Empty;

        /// <summary>
        /// Whether this module and the previous module should be separated by a new line '\n', otherwise just separates with a space.
        /// </summary>
        [JsonProperty("new_line")]
        public bool NewLine = false;


        /// <inheritdoc cref="DescriptionModule(string, bool)"/>
        [JsonConstructor]
        public DescriptionModule() { }
        /// <inheritdoc cref="DescriptionModule(string, CreatureUnlockToken, bool)"/>
        public DescriptionModule(string body, bool newLine = false)
        {
            Body = body;
            NewLine = newLine;
        }
        /// <param name="body">The text of this part of the entries description</param>
        /// <param name="unlockToken">The unlock token of this description module, used to determine what requirements need to be met to unlock this part of the description</param>
        /// <param name="newLine">Whether this module and the previous module should be separated by a new line '\n', otherwise just separates with a space.</param>
        public DescriptionModule(string body, CreatureUnlockToken unlockToken, bool newLine = false) : this(body, newLine)
        {
            UnlockID = unlockToken;
        }

        /// <inheritdoc/>
        public override string ToString() => Body;
    }
}
