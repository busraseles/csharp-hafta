ITodoRepository repo = new JsonFileTodoRepository("todos.json");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Ekle 2) Listele 3) Tamamla 4) Sil 0) Çıkış");
    Console.Write("Seçim: ");
    var secim = Console.ReadLine();

    switch (secim)
    {
        case "1":
            Console.Write("Başlık: ");
            var baslik = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(baslik))
            {
                repo.Add(baslik);
            }
            break;

        case "2":
            foreach (var item in repo.GetAll())
            {
                var isaret = item.IsDone ? "[x]" : "[ ]";
                Console.WriteLine($"{item.Id}. {isaret} {item.Title}");
            }
            break;

        case "3":
            Console.Write("Tamamlanacak Id: ");
            if (int.TryParse(Console.ReadLine(), out int tamamlanacakId))
            {
                if (!repo.Complete(tamamlanacakId))
                    Console.WriteLine("Görev bulunamadı!");
            }
            else
            {
                Console.WriteLine("Geçersiz Id!");
            }
            break;

        case "4":
            Console.Write("Silinecek Id: ");
            if (int.TryParse(Console.ReadLine(), out int silinecekId))
            {
                if (!repo.Remove(silinecekId))
                    Console.WriteLine("Görev bulunamadı!");
            }
            else
            {
                Console.WriteLine("Geçersiz Id!");
            }
            break;

        case "0":
            return;
    }
}