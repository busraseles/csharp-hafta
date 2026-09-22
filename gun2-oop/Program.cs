ITodoRepository repo = new JsonFileTodoRepository("todos.json");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Ekle 2) Listele 3) Tamamla 4) Sil 0) Çıkış");
    Console.Write("Seçim: ");
    var secim = Console.ReadLine();

    if (secim == "0") break;

    if (secim == "1")
    {
        Console.Write("Başlık: ");
        var baslik = Console.ReadLine() ?? "";
        try
        {
            var yeni = repo.Add(baslik);
            Console.WriteLine($"Eklendi: {yeni.Id}. {yeni.Title}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
    }
    else if (secim == "2")
    {
        var liste = repo.GetAll();
        if (liste.Count == 0) Console.WriteLine("Liste boş.");
        foreach (var item in liste)
        {
            var durum = item.IsDone ? "[x]" : "[ ]";
            Console.WriteLine($"{item.Id}. {durum} {item.Title}");
        }
    }
    else if (secim == "3")
    {
        Console.Write("Tamamlanacak Id: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (!repo.Complete(id)) Console.WriteLine("Görev bulunamadı.");
        }
    }
    else if (secim == "4")
    {
        Console.Write("Silinecek Id: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (!repo.Remove(id)) Console.WriteLine("Görev bulunamadı.");
        }
    }
}