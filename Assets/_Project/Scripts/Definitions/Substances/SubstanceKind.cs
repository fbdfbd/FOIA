namespace OneMoreSpoon.Game.Definitions
{
    public enum SubstanceKind
    {
        Twin,
        Stance,
        EdgeBlock_Stance,
        EdgeBlock_Tool,
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
                && kind != SubstanceKind.EdgeBlock_Stance
                && kind != SubstanceKind.Person_Replace
                && kind != SubstanceKind.Unique
                && kind != SubstanceKind.EdgeBlock_Tool
                && kind != SubstanceKind.Trash;
        }

        public static bool CanMerge(SubstanceKind kind)
        {
            return kind == SubstanceKind.Stance
                || kind == SubstanceKind.EdgeBlock_Stance;
        }

        public static bool CanEquipOnEdge(SubstanceKind kind)
        {
            return kind == SubstanceKind.EdgeBlock_Stance
                || kind == SubstanceKind.EdgeBlock_Tool;
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
            return false;
        }

        public static bool IsStanceLike(SubstanceKind kind)
        {
            return kind == SubstanceKind.Stance;
        }

        public static bool IsEdgeBlockLike(SubstanceKind kind)
        {
            return kind == SubstanceKind.EdgeBlock_Stance
                || kind == SubstanceKind.EdgeBlock_Tool;
        }

        public static bool IsEtcLike(SubstanceKind kind)
        {
            return kind == SubstanceKind.Unique
                || kind == SubstanceKind.Trash;
        }
    }
}
