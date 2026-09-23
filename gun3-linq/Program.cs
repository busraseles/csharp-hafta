using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

// urunler.json dosyasından ürünleri okuyoruz
var urunler = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText("urunler.json"))!;

// Listeyi Dictionary'ye dönüştürüyoruz (Key: Id, Value: Product)
var sozluk = urunler.ToDictionary(u => u.Id);

// Sabit tohum (seed: 7) ile 1.000 adet aranacak Id listesi oluşturuyoruz
var rnd2 = new Random(7);
var arananlar = Enumerable.Range(0, 1_000).Select(_ => rnd2.Next(1, 10_001)).ToList();

// 1. Ölçüm: List üzerinde FirstOrDefault ile arama (O(N))
var sw = Stopwatch.StartNew();
foreach (var id in arananlar)
{
    _ = urunler.FirstOrDefault(u => u.Id == id);
}
sw.Stop();
Console.WriteLine($"List:       {sw.Elapsed.TotalMilliseconds:F2} ms");

// 2. Ölçüm: Dictionary üzerinde TryGetValue ile arama (O(1))
sw.Restart();
foreach (var id in arananlar)
{
    sozluk.TryGetValue(id, out _);
}
sw.Stop();
Console.WriteLine($"Dictionary: {sw.Elapsed.TotalMilliseconds:F2} ms");

public record Product(int Id, string Name, string Category, decimal Price, int Stock);