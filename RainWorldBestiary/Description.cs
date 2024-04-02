using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RainWorldBestiary
{
    /// <summary>
    /// A class representing an entries description, saved as a DescriptionModule array, but can be used as a string
    /// </summary>
    public class Description : IEnumerable<DescriptionModule>, ICollection<DescriptionModule>
    {
        // The values of this description, all of the description modules
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
        /// Copy Operator
        /// </summary>
        public Description(Description other)
        {
            _values = new List<DescriptionModule>(other._values);
        }

        /// <summary>
        /// Gets the amount of description modules in this description
        /// </summary>
        public int Count => _values.Count;

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
        /// Returns this description as a string where only the parts of the entry that are visible are added
        /// </summary>
        public override string ToString()
        {
            string result = string.Empty;

            foreach (DescriptionModule module in _values)
            {
                if (module.ModuleUnlocked)
                {
                    result += string.Concat(module.NewLine ? "\n" : " ", module.ToString());
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
        // A set only property so you can just say unlock_id in the json file and specify one creature unlock token
        [JsonProperty("unlock_id")]
        private CreatureUnlockToken UnlockID
        {
            set
            {
                if (value != null)
                    UnlockIDs = UnlockIDs.Append(value).ToArray();
            }
        }

        /// <summary>
        /// All the unlock tokens of this description module, used to determine what requirements need to be met to unlock this part of the description
        /// </summary>
        [JsonProperty("unlock_ids")]
        public CreatureUnlockToken[] UnlockIDs = new CreatureUnlockToken[0];

        /// <summary>
        /// The condition that specifies whether this entry is visible or not, if this returns true, then the entry is visible. You can leave this as the default, or set your own custom condition.
        /// </summary>
        /// <remarks>Defaults to <see cref="DefaultModuleUnlockedCondition(DescriptionModule)"/></remarks>
        [JsonIgnore]
        public Func<DescriptionModule, bool> ModuleUnlockedCondition = DefaultModuleUnlockedCondition;

        /// <summary>
        /// Checks all the UnlockIDs to determine if this module is visible or not
        /// </summary>
        public static bool DefaultModuleUnlockedCondition(DescriptionModule info)
        {
            if (BestiarySettings.UnlockAllEntries.Value)
                return true;

            bool unlocked = true;
            foreach (CreatureUnlockToken unlock in info.UnlockIDs)
            {
                bool thisValue = CheckIfUnlockTokenValid(unlock);

                switch (unlock.OperationAgainstCurrentValue)
                {
                    case OperationType.And:
                        unlocked = unlocked && thisValue;
                        break;
                    case OperationType.Or:
                        unlocked = unlocked || thisValue;
                        break;
                    case OperationType.XOr:
                        unlocked = (unlocked && !thisValue) || (!unlocked && thisValue);
                        break;
                    case OperationType.NAnd:
                        unlocked = !(unlocked && thisValue);
                        break;
                    case OperationType.NOr:
                        unlocked = !unlocked && !thisValue;
                        break;
                    case OperationType.XAnd:
                        unlocked = (unlocked && thisValue) || (!unlocked && !thisValue);
                        break;
                }
            }

            return unlocked;
        }

        /// <summary>
        /// Checks if this creature unlock token is null, the token type is none, or if it returns true when run through <see cref="Bestiary.IsUnlockTokenValid(CreatureUnlockToken)"/>.
        /// </summary>
        /// <returns>True if either of the conditions above is met</returns>
        public static bool CheckIfUnlockTokenValid(CreatureUnlockToken unlockToken)
            => unlockToken == null || unlockToken.TokenType == UnlockTokenType.None || Bestiary.IsUnlockTokenValid(unlockToken);

        /// <summary>
        /// Returns true if the module is unlocked, else false
        /// </summary>
        [JsonIgnore]
        public bool ModuleUnlocked => ModuleUnlockedCondition == null || ModuleUnlockedCondition(this);

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

        /// <summary>
        /// Whether this modules text will get run through the in game translator, if no translation is found, then the text will be placed
        /// </summary>
        [JsonProperty("translate")]
        public bool Translate = true;

        /// <summary>
        /// The Color of the Unlock Pip for this Module
        /// </summary>
        [JsonIgnore]
        public Color UnlockPipColor = Color.white;


        internal enum PredefinedColors
        {
            None,
            AlwaysAvailable,
            Combat,
            Appearance,
            Hunting,
            Utility,
            Behaviour,
        }
        internal static Color GetColorFromPredefined(PredefinedColors color)
        {
            switch (color)
            {
                case PredefinedColors.AlwaysAvailable:
                    return new Color(0.25f, 1f, 0.25f);
                case PredefinedColors.Combat:
                    return new Color(1f, 0.6f, 0.5f);
                case PredefinedColors.Appearance:
                    return new Color(0.6f, 1f, 0.9f);
                case PredefinedColors.Hunting:
                    return new Color(0.6f, 1f, 0.6f);
                case PredefinedColors.Utility:
                    return new Color(1f, 0.6f, 1f);
                case PredefinedColors.Behaviour:
                    return new Color(0.5f, 0.5f, 1f);
                default:
                    return Color.white;
            }
        }


        [JsonProperty("unlock_pip_color")]
        private string UnlockPipColor_JSON
        {
            get => UnlockPipColor.RGBToHexString();
            set
            {
                if (Enum.TryParse(value, true, out PredefinedColors result))
                    UnlockPipColor = GetColorFromPredefined(result);
                else
                    UnlockPipColor = value.HexToColor();
            }
        }

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

        /// <summary>
        /// Copy Operator
        /// </summary>
        public DescriptionModule(DescriptionModule other)
        {
            other.UnlockIDs.CopyTo(UnlockIDs, 0);
            ModuleUnlockedCondition = other.ModuleUnlockedCondition;
            Body = new string(other.Body.ToCharArray());
            NewLine = other.NewLine;
            Translate = other.Translate;

        }

        /// <summary>
        /// Returns <see cref="Body"/>, if <see cref="Translate"/> is true, <see cref="Body"/> will get run through the in game translator first
        /// </summary>
        /// <returns><see cref="Body"/>, translated if <see cref="Translate"/> is true</returns>
        public override string ToString() => Translate ? Translator.Translate(Body) : Body;
    }
}
