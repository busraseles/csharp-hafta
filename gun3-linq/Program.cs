using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// 1. Kısım: Ertelenmiş Çalışma (Deferred Execution) vs Anında Çalışma (Immediate Execution)
var sayilar = new List<int> { 1, 2, 3 };
var buyukler = sayilar.Where(s => s > 1);
sayilar.Add(10);
Console.WriteLine(string.Join(", ", buyukler));

var buyuklerListe = sayilar.Where(s => s > 1).ToList();
sayilar.Add(20);
Console.WriteLine(string.Join(", ", buyuklerListe));

// 2. Kısım: LINQ Filtresinin Çalışma Sayısı
var urunler = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText("urunler.json"))!;

int sayac = 0;
var sorgu = urunler.Where(u => { sayac++; return u.Stock == 0; });
var adet = sorgu.Count();
var ilk = sorgu.First();

Console.WriteLine($"Filtre {sayac} kez çalıştı.");

public record Product(int Id, string Name, string Category, decimal Price, int Stock);