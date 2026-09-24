public interface ITodoRepository
{
    IReadOnlyList<TodoItem> GetAll();
    TodoItem? GetById(int id);
    TodoItem Add(string title);
    bool Complete(int id);
    bool Remove(int id);
}