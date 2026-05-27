using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.Game.Rules
{
    public static class EdgeConnectionRule
    {
        public static bool CanConnect(
            NodeCategory fromCategory,
            NodeCategory toCategory,
            out string rejectReason)
        {
            rejectReason = string.Empty;

            if (fromCategory != NodeCategory.Unique &&
                toCategory != NodeCategory.Unique)
            {
                return true;
            }

            if (IsUniqueAllowedEndpoint(fromCategory) &&
                IsUniqueAllowedEndpoint(toCategory))
            {
                return true;
            }

            rejectReason = "UniqueNodeRequiresUniqueOrIO";
            return false;
        }

        private static bool IsUniqueAllowedEndpoint(NodeCategory category)
        {
            return category == NodeCategory.Unique ||
                category == NodeCategory.Input ||
                category == NodeCategory.Output;
        }
    }
}
