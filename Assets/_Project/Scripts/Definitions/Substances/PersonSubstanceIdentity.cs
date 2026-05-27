namespace OneMoreSpoon.Game.Definitions
{
    public readonly struct PersonSubstanceIdentity
    {
        public PersonSubstanceIdentity(string characterKey, PersonSubstanceState state)
        {
            CharacterKey = characterKey;
            State = state;
        }

        public string CharacterKey { get; }
        public PersonSubstanceState State { get; }
    }

    public enum PersonSubstanceState
    {
        Normal,
        Friend,
        Captive,
        Create,
        Replace,
    }

    public static class PersonSubstanceIdentityParser
    {
        private const string PersonPrefix = "person_";
        private const string FriendPrefix = "friend_";
        private const string CaptivePrefix = "captive_";
        private const string CreatePrefix = "create_";
        private const string ReplacePrefix = "replace_";

        public static bool TryParse(string substanceId, out PersonSubstanceIdentity identity)
        {
            identity = default;

            if (string.IsNullOrWhiteSpace(substanceId) ||
                !substanceId.StartsWith(PersonPrefix, System.StringComparison.Ordinal))
            {
                return false;
            }

            var value = substanceId.Substring(PersonPrefix.Length);
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (TryParseStatePrefix(value, FriendPrefix, PersonSubstanceState.Friend, out identity))
                return true;

            if (TryParseStatePrefix(value, CaptivePrefix, PersonSubstanceState.Captive, out identity))
                return true;

            if (TryParseStatePrefix(value, CreatePrefix, PersonSubstanceState.Create, out identity))
                return true;

            if (TryParseStatePrefix(value, ReplacePrefix, PersonSubstanceState.Replace, out identity))
                return true;

            identity = new PersonSubstanceIdentity(value, PersonSubstanceState.Normal);
            return true;
        }

        private static bool TryParseStatePrefix(
            string value,
            string prefix,
            PersonSubstanceState state,
            out PersonSubstanceIdentity identity)
        {
            identity = default;

            if (!value.StartsWith(prefix, System.StringComparison.Ordinal))
                return false;

            var characterKey = value.Substring(prefix.Length);
            if (string.IsNullOrWhiteSpace(characterKey))
                return false;

            identity = new PersonSubstanceIdentity(characterKey, state);
            return true;
        }
    }
}
