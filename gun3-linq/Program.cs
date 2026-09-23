using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// 1. Kısım: Ertelenmiş Çalışma (Deferred Execution) vs Anında Çalışma (Immediate Execution)
var sayilar = new List<int> { 1, 2, 3 };

// Where tanımlandığı anda ÇALIŞMAZ, yalnızca sorgu planı hazırlanır (Deferred Execution)
var buyukler = sayilar.Where(s => s > 1);

// Listeye 10 ekleniyor
sayilar.Add(10);

// string.Join sorguyu iterate ettiği (tükettiği) anda Where çalışır.
// Dolayısıyla sonradan eklenen 10 da sorguya dahil olur.
Console.WriteLine("1. Çıktı (buyukler): " + string.Join(", ", buyukler));

// .ToList() çağrıldığı anda sorgu ANINDA çalışır ve sonuçlar bellekte yeni bir listeye kopyalanır (Immediate Execution)
var buyuklerListe = sayilar.Where(s => s > 1).ToList();

// Listeye 20 ekleniyor
sayilar.Add(20);

// buyuklerListe önceden sabitlendiği için 20 bu listeye yansımaz.
Console.WriteLine("2. Çıktı (buyuklerListe): " + string.Join(", ", buyuklerListe));

Console.WriteLine("--------------------------------------------------");

// 2. Kısım: LINQ Filtresinin Çalışma Sayısı ve Sayacın Mantığı
var jsonMetni = File.ReadAllText("urunler.json");
var urunler = JsonSerializer.Deserialize<List<Product>>(jsonMetni)!;

int sayac = 0;

// u.Stock == 0 olan ürünleri arayan sorgu şablonu (Henüz çalışmadı, sayac = 0)
var sorgu = urunler.Where(u => { sayac++; return u.Stock == 0; });

// Count(): Tüm listeyi baştan sona (10.000 eleman) tarar.
// sayac burada tam 10.000 artar.
var adet = sorgu.Count();

// First(): Baştan aramaya başlar, stoğu 0 olan İLK ürünü bulduğu anda durur.
var ilk = sorgu.First();

Console.WriteLine($"Stoğu 0 olan ilk ürün Id: {ilk.Id}, Ad: {ilk.Name}");
Console.WriteLine($"Filtre {sayac} kez çalıştı.");

public record Product(int Id, string Name, string Category, decimal Price, int Stock);