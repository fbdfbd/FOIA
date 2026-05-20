namespace FOIA.Flow.Input
{
    public static class FlowDragPayload
    {
        public const string Staff = "staff";
        public const string Item = "item";
        public const string Document = "document";

        public static string Type { get; private set; }
        public static string Id { get; private set; }

        public static void Begin(string type, string id)
        {
            Type = type;
            Id = id;
        }

        public static void Clear()
        {
            Type = string.Empty;
            Id = string.Empty;
        }

        public static bool Is(string type)
        {
            return Type == type && !string.IsNullOrEmpty(Id);
        }
    }
}
