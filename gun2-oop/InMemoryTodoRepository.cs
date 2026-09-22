public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<TodoItem> _items = new();
    private int _sonId = 0;

    public IReadOnlyList<TodoItem> GetAll() => _items;

    public TodoItem? GetById(int id) => _items.FirstOrDefault(t => t.Id == id);

    public TodoItem Add(string title)
    {
        var item = new TodoItem(++_sonId, title);
        _items.Add(item);
        return item;
    }

    public bool Complete(int id)
    {
        var item = GetById(id);
        if (item is null)
            return false;

        item.Complete();
        return true;
    }

    public bool Remove(int id)
    {
        var item = GetById(id);
        if (item is null)
            return false;

        return _items.Remove(item);
    }
}