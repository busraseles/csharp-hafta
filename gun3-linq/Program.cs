using System;
using System.IO;
using System.Linq;
using System.Text.Json;

// Tohum (seed): 42 ile başlatılan Random her zaman aynı sırayla aynı sayıları üretir.
var rnd = new Random(42);

string[] kategoriler = ["Kırtasiye", "Elektronik", "Gıda", "Temizlik", "Oyuncak"];

// 1'den 10.000'e kadar ürün listesi oluşturuluyor
var urunler = Enumerable.Range(1, 10_000)
    .Select(i => new Product(
        Id: i,
        Name: $"Ürün {i}",
        Category: kategoriler[rnd.Next(kategoriler.Length)],
        Price: Math.Round((decimal)(rnd.NextDouble() * 1000), 2),
        Stock: rnd.Next(0, 50)))
    .ToList();

// JSON formatında diske yazıyoruz
File.WriteAllText("urunler.json", JsonSerializer.Serialize(urunler));
Console.WriteLine($"{urunler.Count} ürün yazıldı.");

// --- Rapor için Kontrol ve Ölçüm Kısmı ---
var dosyaBilgisi = new FileInfo("urunler.json");
double dosyaBoyutuKb = dosyaBilgisi.Length / 1024.0;
Console.WriteLine($"\n--- Rapor Bilgileri ---");
Console.WriteLine($"urunler.json Dosya Boyutu: {dosyaBoyutuKb:F2} KB ({dosyaBilgisi.Length} bayt)");

Console.WriteLine("\nİlk 3 Ürün:");
foreach (var p in urunler.Take(3))
{
    Console.WriteLine($"Id: {p.Id} | Ad: {p.Name} | Kategori: {p.Category} | Fiyat: {p.Price} TL | Stok: {p.Stock}");
}

// Ürün modelimiz (DTO / Record)
public record Product(int Id, string Name, string Category, decimal Price, int Stock);