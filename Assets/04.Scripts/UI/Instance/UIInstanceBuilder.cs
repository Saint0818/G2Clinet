
public static class UIInstanceBuilder
{
    public static UIInstanceChapter.Data Build(ChapterData data)
    {
        return new UIInstanceChapter.Data
        {
            Title = data.Name,
            Desc = data.Explain
        };
    }
}
