namespace HackerNews.Application.Models;
public class Story
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }

    public override bool Equals(object obj)
    {
        Story? story = obj as Story;
        if (story == null) return false;
        return Id == story.Id && Title == story.Title && Url == story.Url;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Title, Url);
    }
}