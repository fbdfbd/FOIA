using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.Presenter
{
    public sealed class SubstanceTitleProvider
    {
        public string GetTitle(SubstanceKind kind)
        {
            return kind switch
            {
                SubstanceKind.Twin => "Twin",
                SubstanceKind.Person => "직원",
                SubstanceKind.Person_Friend => "Friend",
                SubstanceKind.Person_Captive => "Captive",
                SubstanceKind.Person_Replace => "Replace",
                SubstanceKind.Person_Create => "Create",
                SubstanceKind.Stance => "탐사기록",
                SubstanceKind.EdgeBlock_Stance => "Edge Block",
                SubstanceKind.EdgeBlock_Tool => "엣지블록",
                SubstanceKind.Unique => "소장용 탐사기록",
                SubstanceKind.Trash => "Trash",
                _ => string.Empty
            };
        }
    }
}
