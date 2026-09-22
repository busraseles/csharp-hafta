using System.Text.Json;

public class JsonFileTodoRepository : ITodoRepository
{
    private readonly string _dosyaYolu;
    private readonly List<TodoItem> _items = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public JsonFileTodoRepository(string dosyaYolu)
    {
        _dosyaYolu = dosyaYolu;

        if (File.Exists(_dosyaYolu))
        {
            var json = File.ReadAllText(_dosyaYolu);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var yuklenenler = JsonSerializer.Deserialize<List<TodoItem>>(json, _jsonOptions);
                if (yuklenenler is not null)
                {
                    _items.AddRange(yuklenenler);
                }
            }
        }
    }

    private void DosyayaKaydet()
    {
        var json = JsonSerializer.Serialize(_items, _jsonOptions);
        File.WriteAllText(_dosyaYolu, json);
    }

    public IReadOnlyList<TodoItem> GetAll() => _items;

    public TodoItem? GetById(int id) => _items.FirstOrDefault(t => t.Id == id);

    public TodoItem Add(string title)
    {
        int yeniId = _items.Count > 0 ? _items.Max(t => t.Id) + 1 : 1;
        var item = new TodoItem(yeniId, title);
        _items.Add(item);
        DosyayaKaydet();
        return item;
    }

    public bool Complete(int id)
    {
        var item = GetById(id);
        if (item is null)
            return false;

        item.Complete();
        DosyayaKaydet();
        return true;
    }

    public bool Remove(int id)
    {
        var item = GetById(id);
        if (item is null)
            return false;

        var sonuc = _items.Remove(item);
        if (sonuc)
            DosyayaKaydet();

        return sonuc;
    }
}