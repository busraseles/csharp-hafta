using System.Text.Json.Serialization;

public class TodoItem
{
    public int Id { get; }
    public string Title { get; private set; }

    [JsonInclude]
    public bool IsDone { get; private set; }

    [JsonInclude]
    public DateTime CreatedAt { get; private set; }

    // Normal kodlarımız için kurucu
    public TodoItem(int id, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Başlık boş olamaz.", nameof(title));

        Id = id;
        Title = title.Trim();
        CreatedAt = DateTime.Now;
    }

    // JSON'dan geri yüklerken kullanılacak kurucu
    [JsonConstructor]
    public TodoItem(int id, string title, bool isDone, DateTime createdAt)
    {
        Id = id;
        Title = title;
        IsDone = isDone;
        CreatedAt = createdAt;
    }

    public void Complete() => IsDone = true;
}