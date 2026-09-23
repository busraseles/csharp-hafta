using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

async Task<string> SahteDbCagrisi(string ad, int ms)
{
    Thread.Sleep(ms);
    return ad;
}

// 1. Ölçüm: Sırayla
var sw = Stopwatch.StartNew();
await SahteDbCagrisi("kullanıcı", 1000);
await SahteDbCagrisi("siparişler", 1000);
await SahteDbCagrisi("ürünler", 1000);
Console.WriteLine($"Thread.Sleep Sırayla: {sw.ElapsedMilliseconds} ms");

// 2. Ölçüm: Birlikte
sw.Restart();
await Task.WhenAll(
    SahteDbCagrisi("kullanıcı", 1000),
    SahteDbCagrisi("siparişler", 1000),
    SahteDbCagrisi("ürünler", 1000));
Console.WriteLine($"Thread.Sleep Birlikte: {sw.ElapsedMilliseconds} ms");