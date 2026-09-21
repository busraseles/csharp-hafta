var gorevler = new List<string>();
var tamamlananlar = new HashSet<int>();

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
            var baslik = Console.ReadLine() ?? "";
            gorevler.Add(baslik);
            break;

        case "2":
            for (int i = 0; i < gorevler.Count; i++)
            {
                var isaret = tamamlananlar.Contains(i) ? "[x]" : "[ ]";
                Console.WriteLine($"{i}. {isaret} {gorevler[i]}");
            }
            break;

        case "3":
            Console.Write("Tamamlanacak numara: ");
            int tamamlanacakId = int.Parse(Console.ReadLine()!);
            tamamlananlar.Add(tamamlanacakId);
            break;

        case "4":
            Console.Write("Silinecek numara: ");
            if (int.TryParse(Console.ReadLine(), out int silinecekId) && silinecekId >= 0 && silinecekId < gorevler.Count)
            {
                gorevler.RemoveAt(silinecekId);
                tamamlananlar.Remove(silinecekId);
            }
            else
            {
                Console.WriteLine("Geçersiz numara!");
            }
            break;

        case "0":
            return;
    }
}