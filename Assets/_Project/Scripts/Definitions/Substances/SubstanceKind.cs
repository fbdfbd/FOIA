namespace OneMoreSpoon.Game.Definitions
{
    public enum SubstanceKind
    {
        Twin,
        Stance,
        EdgeBlock_Stance,
        Person,
        Person_Friend,
        Person_Captive,
        Person_Replace,
        Person_Create,
        Unique,
        Trash,
    }

    public static class SubstanceKindRules
    {
        public static bool CanSpawnFlow(SubstanceKind kind)
        {
            return kind != SubstanceKind.Stance
                && kind != SubstanceKind.Person_Replace
                && kind != SubstanceKind.Unique
                && kind != SubstanceKind.Trash;
        }

        public static bool CanMerge(SubstanceKind kind)
        {
            return kind == SubstanceKind.Stance
                || kind == SubstanceKind.EdgeBlock_Stance;
        }

        public static bool CanEquipOnEdge(SubstanceKind kind)
        {
            return kind == SubstanceKind.EdgeBlock_Stance;
        }

        public static bool IsEncyclopediaKind(SubstanceKind kind)
        {
            return true;
        }

        public static bool IsPersonLike(SubstanceKind kind)
        {
            return kind == SubstanceKind.Twin
                || kind == SubstanceKind.Person
                || kind == SubstanceKind.Person_Friend
                || kind == SubstanceKind.Person_Captive
                || kind == SubstanceKind.Person_Replace
                || kind == SubstanceKind.Person_Create;
        }

        public static bool IsInfiniteStackKind(SubstanceKind kind)
        {
            return kind == SubstanceKind.Person
                || kind == SubstanceKind.Person_Friend
                || kind == SubstanceKind.Person_Captive
                || kind == SubstanceKind.Person_Replace
                || kind == SubstanceKind.Person_Create;
        }

        public static bool IsStanceLike(SubstanceKind kind)
        {
            return kind == SubstanceKind.Stance;
        }

        public static bool IsEdgeBlockLike(SubstanceKind kind)
        {
            return kind == SubstanceKind.EdgeBlock_Stance;
        }

        public static bool IsEtcLike(SubstanceKind kind)
        {
            return kind == SubstanceKind.Unique
                || kind == SubstanceKind.Trash;
        }

        public static string GetTitle(SubstanceKind kind)
        {
            return kind switch
            {
                SubstanceKind.Twin => "Twin",
                SubstanceKind.Person => "Person",
                SubstanceKind.Person_Friend => "Friend",
                SubstanceKind.Person_Captive => "Captive",
                SubstanceKind.Person_Replace => "Replace",
                SubstanceKind.Person_Create => "Create",
                SubstanceKind.Stance => "Stance",
                SubstanceKind.EdgeBlock_Stance => "Edge Block",
                SubstanceKind.Unique => "Unique",
                SubstanceKind.Trash => "Trash",
                _ => string.Empty
            };
        }
    }
}
