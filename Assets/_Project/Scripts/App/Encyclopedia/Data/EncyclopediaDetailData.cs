namespace OneMoreSpoon.App.Encyclopedia.Data
{
    public abstract class EncyclopediaDetailData
    {
        public string Title { get; }
        public string Description { get; }

        protected EncyclopediaDetailData(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }
}
