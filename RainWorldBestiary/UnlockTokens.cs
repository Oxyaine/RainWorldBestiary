﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace RainWorldBestiary
{
    /// <summary>
    /// The type of unlock this UnlockToken targets
    /// </summary>
    public enum UnlockTokenType : byte
    {
        /// <summary>
        /// This means this part of the description will always be visible if the entry is visible, however, unlike modules with no unlock token(s), this wont make the entry visible
        /// </summary>
        None = 0,
        /// <summary>
        /// For when the player tames the creature
        /// </summary>
        Tamed = 1,
        /// <summary>
        /// For when the player evades the creature, by dodging an attack, climbing to a place it can't reach, etc
        /// </summary>
        Evaded = 2,
        /// <summary>
        /// For when the player sneaks past a creature
        /// </summary>
        SnuckPast = 3,
        /// <summary>
        /// For when the player sees the creature
        /// </summary>
        Observed = 4,
        /// <summary>
        /// For when the player sees the creature run away in fear
        /// </summary>
        ObserveFear = 5,
        /// <summary>
        /// For when the player sees the creature eating or hunting another creature
        /// </summary>
        ObserveFood = 6,
        /// <summary>
        /// For when the player is getting chased by a creature
        /// </summary>
        ObserveHunting = 7,
        /// <summary>
        /// When the creature is killed by the player
        /// </summary>
        /// <remarks>Automatically Tracked (for creatures inheriting from <see cref="Creature"/>)</remarks>
        Killed = 8,
        /// <summary>
        /// When the creature gets impaled with a spear, by the player
        /// </summary>
        /// <remarks>Automatically Tracked (for spears inheriting from <see cref="Spear"/>)</remarks>
        Impaled = 9,
        /// <summary>
        /// When the creature is stunned, by the player
        /// </summary>
        Stunned = 10,
        /// <summary>
        /// When the player is killed by the creature
        /// </summary>
        /// <remarks>Automatically Tracked (for creatures inheriting from <see cref="Creature"/>)</remarks>
        KilledPlayer = 11,
        /// <summary>
        /// When the player is grabbed by the creature
        /// </summary>
        /// <remarks>Automatically Tracked (for creatures inheriting from <see cref="Creature"/>)</remarks>
        GrabbedPlayer = 12,
        /// <summary>
        /// Whenever the creature gets eaten by the player
        /// </summary>
        Eaten = 13,
        /// <summary>
        /// For whenever the player observes a creature being attracted to something, such as batflies to batnip
        /// </summary>
        ObserveAttraction = 14,
        /// <summary>
        /// For whenever the player uses the creature to lure something else
        /// </summary>
        UsedAsLure = 15,
    }

    /// <summary>
    /// The base unlock token, used to register tokens so modules can be unlocked, inherited by <see cref="CreatureUnlockToken"/> which is used in <see cref="DescriptionModule"/> as the unlock ID
    /// </summary>
    public class UnlockToken : IEquatable<UnlockToken>
    {
        /// <summary>
        /// The type of token this module unlock targets
        /// </summary>
        [JsonIgnore]
        public UnlockTokenType TokenType = UnlockTokenType.None;

        [JsonProperty("token_type")]
        private string UnlockTokenType_JSON_STR
        {
            get => TokenType.ToString();
            set
            {
                if (Enum.TryParse(value, true, out UnlockTokenType val))
                    TokenType = val;
                else if (Enum.TryParse(value.Replace(" ", ""), true, out val))
                    TokenType = val;
            }
        }

        /// <summary>
        /// The amount of times this token has been registered, or needs to be registered
        /// </summary>
        [JsonProperty("count")]
        public byte Count = 1;

        [JsonProperty("value"), Obsolete("Use UnlockToken.Count instead")]
        private byte Value { set => Count = value; }

#if DEBUG
        /// <summary>
        /// Extra data that is tied to this token
        /// </summary>
        [JsonProperty("extra_data")]
        public List<string> ExtraData = new List<string>();
#endif

        ///
        [JsonConstructor]
        protected UnlockToken() { }
        /// <param name="tokenType">The type of token to look for</param>
        /// <param name="value">The amount of times this token should be registered before this is considered unlocked</param>
        public UnlockToken(UnlockTokenType tokenType, byte value = 1)
        {
            TokenType = tokenType;
            Count = value;
        }

        /// <summary>
        /// Checks if both objects are <see cref="UnlockToken"/>, then compares them using <see cref="Equals(UnlockToken)"/>
        /// </summary>
        /// <remarks><see cref="Equals(UnlockToken)"/> <inheritdoc cref="Equals(UnlockToken)"/></remarks>
        public override bool Equals(object obj) => obj is UnlockToken token && Equals(token);

        /// <remarks>
        /// Checks if the token type matches, ignores <see cref="Count"/>
        /// </remarks>
        public bool Equals(UnlockToken other) => TokenType.Equals(other.TokenType);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <returns>Token Type + Value</returns>
        public override string ToString()
        {
            return TokenType + " " + Count;
        }
    }
#if DEBUG

    /// <summary>
    /// A type of operation gate that can be applied to bools
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// True if both the values are true
        /// <code>0 - 0 = 0</code>
        /// <code>1 - 0 = 0</code>
        /// <code>0 - 1 = 0</code>
        /// <code>1 - 1 = 1</code>
        /// </summary>
        And = 0,
        /// <summary>
        /// True if either of the inputs are true
        /// <code>0 - 0 = 0</code>
        /// <code>1 - 0 = 1</code>
        /// <code>0 - 1 = 1</code>
        /// <code>1 - 1 = 1</code>
        /// </summary>
        Or = 1,
        /// <summary>
        /// True if either of the inputs are true, but not when both inputs are true
        /// <code>0 - 0 = 0</code>
        /// <code>1 - 0 = 1</code>
        /// <code>0 - 1 = 1</code>
        /// <code>1 - 1 = 0</code>
        /// </summary>
        XOr = 2,
        /// <summary>
        /// True if either value is false
        /// <code>0 - 0 = 1</code>
        /// <code>1 - 0 = 1</code>
        /// <code>0 - 1 = 1</code>
        /// <code>1 - 1 = 0</code>
        /// </summary>
        NAnd = 3,
        /// <summary>
        /// True if neither value is true
        /// <code>0 - 0 = 1</code>
        /// <code>1 - 0 = 0</code>
        /// <code>0 - 1 = 0</code>
        /// <code>1 - 1 = 0</code>
        /// </summary>
        NOr = 4,
        /// <summary>
        /// True if both values are the same
        /// <code>0 - 0 = 1</code>
        /// <code>1 - 0 = 0</code>
        /// <code>0 - 1 = 0</code>
        /// <code>1 - 1 = 1</code>
        /// </summary>
        XAnd = 5
    }

#endif
    /// <summary>
    /// An unlock token, that can be used to detect whether a <see cref="DescriptionModule"/> is unlocked, similar to <see cref="UnlockToken"/> but has a CreatureID string
    /// </summary>
    public class CreatureUnlockToken : UnlockToken, IEquatable<CreatureUnlockToken>, IEquatable<UnlockToken>
    {
        /// <summary>
        /// The ID of the creature this unlock token applies to
        /// </summary>
        [JsonProperty("creature_id")]
        public string CreatureID = string.Empty;

#if DEBUG

        /// <summary>
        /// The operation this unlock token will perform against the current unlock value of the token
        /// </summary>
        [JsonProperty("operation_against_value")]
        public OperationType OperationAgainstCurrentValue = OperationType.And;

#endif

        [JsonConstructor]
        private CreatureUnlockToken() { }
        /// <param name="creatureID">The ID of the creature that to look for</param>
        /// <param name="tokenType">The type of token to look for</param>
        /// <param name="value">The amount of times this token should be registered before this is considered unlocked</param>
        public CreatureUnlockToken(string creatureID, UnlockTokenType tokenType, byte value = 1)
            : base(tokenType, value) => CreatureID = creatureID;

        /// <summary>
        /// Checks if both objects are <see cref="CreatureUnlockToken"/>, then compares them using <see cref="Equals(CreatureUnlockToken)"/>
        /// </summary>
        /// <remarks><see cref="Equals(CreatureUnlockToken)"/> <inheritdoc cref="Equals(CreatureUnlockToken)"/></remarks>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case CreatureUnlockToken cToken: return Equals(cToken);
                case UnlockToken token: return Equals(token);
                default: return false;
            }
        }

        /// <remarks>
        /// Checks if the creature ID matches, then if the token type matches, ignores <see cref="UnlockToken.Count"/>
        /// </remarks>
        public bool Equals(CreatureUnlockToken other) => CreatureID.Equals(other.CreatureID) && TokenType.Equals(other.TokenType);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <returns>Creature ID + Token Type + Value</returns>
        public override string ToString()
        {
            return string.Join(" ", CreatureID, TokenType, Count);
        }
    }
}