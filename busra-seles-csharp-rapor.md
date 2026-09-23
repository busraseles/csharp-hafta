# C# Görevi - 1. Gün Raporu

**Tarih:** 21 Eylül 2026
**Hazırlayan:** Büşra Seleş
**Proje Adı:** gun1-konsol

## 1. Adım: Kurulum ve Doğrulama
| Soru | Cevabınız |
| :--- | :--- |
| .NET SDK sürümü | 10.0.401 |
| Kurulu runtime'lar | Microsoft.AspNetCore.App (8.0.31, 10.0.12), Microsoft.NETCore.App (8.0.31, 10.0.12), Microsoft.WindowsDesktop.App (8.0.31, 10.0.12) |
| Git sürümü | git version 2.53.0.windows.2 |
| Hangi editörü seçtiniz | Visual Studio 2026 Developer PowerShell |

- **Düşünün (SDK vs Runtime):** SDK derleyici, CLI araçları ve geliştirme kütüphanelerini içerir; kod geliştirilen makinede gereklidir. Runtime ise sadece derlenmiş ara kodun (IL) çalıştırılmasını sağlar; gereksiz kaynak tüketimini önlemek ve saldırı yüzeyini daraltmak için sunucuda sadece Runtime olması yeterlidir.

## 2. Adım: İlk Proje ve Ne Oluştuğu

### 1. `.csproj` Dosyasındaki Ayarlar Ne İşe Yarıyor?
- **`TargetFramework (net10.0)`:** Projenin hangi .NET sürümünün API'lerini ve çalışma ortamını hedef alacağını belirler. Bizim projemiz en güncel sürüm olan .NET 10 üzerinde derlenip çalışacaktır.
- **`Nullable (enable)`:** C# derleyicisinin C# 8.0 ile gelen "Nullable Reference Types" analiz özelliğini aktif eder. Değer almayan veya `null` olma riski taşıyan referans tiplerinde derleyicinin bizi önceden uyarmasını sağlayarak çalışma zamanında `NullReferenceException` alma ihtimalimizi en aza indirir.
- **`ImplicitUsings (enable)`:** C# 10 ile gelen bu özellik sayesinde `System`, `System.Collections.Generic`, `System.IO`, `System.Linq` gibi her C# projesinde neredeyse standart olarak kullanılan en temel `using` (namespace) bildirimleri dosya başlarına yazmaya gerek kalmadan proje genelinde otomatik olarak tanımlanır. Kod kalabalığını önler.

### 2. `bin/Debug/net10.0/` Altında Hangi Dosyalar Var? `.dll` ile `.exe` Neden İkisi Birden Var?
Klasör incelendiğinde şu temel dosyaların oluştuğu görüldü:
- `gun1-konsol.dll`: Yazdığımız C# kodunun Roslyn derleyicisi tarafından çevrildiği, platformdan bağımsız ara koddur (IL - Intermediate Language). Asıl iş mantığını ve derlenmiş kodlarımızı bu dosya taşır.
- `gun1-konsol.exe`: İşletim sistemine özel yerel başlatıcıdır (AppHost/Host Executable). Windows ortamında doğrudan çift tıklanarak veya terminalden adı yazılarak çalışabilmesi için .NET Runtime'ı ayağa kaldırıp arka plandaki `gun1-konsol.dll` dosyasını yükleyen hafif bir kabuk (loader) görevi görür.
- `gun1-konsol.pdb`: Hata ayıklama sembolleridir (Program Database). Kodda bir hata fırlatıldığında satır numarasını ve stack trace detaylarını görmemizi sağlar.
- `gun1-konsol.deps.json` ve `gun1-konsol.runtimeconfig.json`: Uygulamanın hangi runtime sürümüne ve hangi harici bağımlılıklara ihtiyaç duyduğunu belirten yapılandırma dosyalarıdır.

**Neden hem `.dll` hem `.exe` var?**  
Çünkü .NET platformlar arasıdır (cross-platform). Asıl derlenmiş mantık `.dll` içinde işletim sisteminden bağımsız olarak saklanır. Ancak Windows kullanıcılarının terminalden veya doğrudan tıklayarak uygulamayı rahatça çalıştırabilmesi için Windows'a özel bir çalıştırıcı olan `.exe` (AppHost) üretilir. Linux'ta derleseydik uzantısız bir ELF çalıştırılabilir dosyası ile yine aynı `.dll` yan yana oluşacaktı.

### 3. `bin` ve `obj` Klasörleri Repoya Girmeli mi? `dotnet new gitignore` Onlara Ne Yaptı?
- **Girmeli mi?:** Kesinlikle **girmemelidir**. Çünkü `bin` ve `obj` klasörleri derleme sonucu oluşan geçici dosyalardır (ara kodlar, çalıştırılabilir dosyalar, derleyici önbelleği vb.). Bu dosyalar kaynak koddan her an `dotnet build` komutuyla sıfırdan yeniden üretilebilir. Repoya eklenmeleri hem reponun boyutunu gereksiz şişirir hem de farklı işletim sistemlerinde veya farklı geliştirici makinelerinde derleme çakışmalarına (conflict) yol açar.
- **`dotnet new gitignore` ne yaptı?:** Kök dizinde oluşturduğumuz `.gitignore` dosyası, `[Bb]in/` ve `[Oo]bj/` desenlerini içerdiği için Git bu klasörleri otomatik olarak takip listesinden çıkardı (ignore etti). `git status` çalıştırdığımızda bu iki klasör `Untracked files` arasında hiç görünmedi; yalnızca `Program.cs` ve `.csproj` dosyaları Git tarafından algılandı.

### 4. Düşünün: `Program.cs`'te `class Program` veya `static void Main` Nereye Gitti?
- C# 9.0 ile hayatımıza giren **Top-Level Statements (Üst Düzey İfadeler)** özelliği sayesinde artık her konsol uygulamasında tekrarlanan `namespace`, `internal class Program` ve `static void Main(string[] args)` gibi standart şablon (boilerplate) kodları elle yazma zorunluluğu kalktı.
- Aslında bu yapılar hiçbir yere gitmedi veya dilden silinmedi. Biz kodu doğrudan yazdığımızda C# derleyicisi (Roslyn) derleme anında arka planda otomatik olarak görünmez bir `Program` sınıfı ve bir `Main` metodu oluşturup yazdığımız kodları bu metodun içine yerleştirir. Böylece gereksiz sözdizimi kalabalığı ortadan kalkarken arkada aynı nesne yönelimli C# yapısı eksiksiz çalışmaya devam eder.

## 3. Adım: Tiplerin Sürprizleri
| Satır | Tahmininiz | Gerçek Çıktı |
| :--- | :--- | :--- |
| `0.1 + 0.2` | 0.3 | 0,30000000000000004 |
| `0.1 + 0.2 == 0.3` | True | False |
| `0.1m + 0.2m == 0.3m` | True | True |
| `buyuk + 1` | Hata verir | -2147483648 |
| `7 / 2` | 3.5 | 3 |
| `7 / 2.0` | 3.5 | 3,5 |

- Kodda nokta yazılmasına rağmen yerel kültür (Türkçe) nedeniyle konsolda ondalık ayırıcı virgül (`,`) olarak basıldı.
- `checked(buyuk + 1)` yapıldığında sessizce negatif değere dönmek yerine `System.OverflowException: Arithmetic operation resulted in an overflow.` hatası fırlatılarak program durduruldu.
- **Düşünün:** `double` ikili tabanda kayan noktalı hesaplama yaptığı için ondalık basamaklarda hassasiyet kaybı yaşar. Finansal hesaplamalarda kuruş kaybı olmaması için 10 tabanlı tam hassasiyete sahip `decimal` kullanılır. Sayı taşmalarının sessiz kalıp hatalı işlem yapmaması için kritik operasyonlarda `checked` tercih edilir.

## 4. Adım: Null ve Derleyicinin Uyarıları

### 1. Karşılaşılan Derleyici Uyarıları (Kod ve Mesajlar)
Kodu ilk yazdığımızda `dotnet build` komutu başarılı bir derleme yapsa da derleyici terminalde iki adet sarı uyarı verdi:
- **`warning CS8600: Null sabit değeri veya olası null değeri, boş değer atanamaz türe dönüştürülüyor.`**  
  *Nerede oldu:* `Program.cs: 2. satır` (`string ad = Console.ReadLine();`)  
  *Nedeni:* `.NET` ortamında `Console.ReadLine()` metodunun geri dönüş tipi `string?` (yani null dönebilen) olarak tanımlıdır. Biz ise bu değeri `string` (null kabul etmeyen, non-nullable) bir değişkene atamaya çalıştığımız için derleyici bizi "buraya null gelebilir, dikkat et" diyerek uyardı.
- **`warning CS8602: Olası bir null başvurunun başvurma işlemi.`**  
  *Nerede oldu:* `Program.cs: 3. satır` (`Console.WriteLine($"Merhaba {ad.ToUpper()}");`)  
  *Nedeni:* Derleyici, `ad` değişkeninin null olma ihtimalini tespit ettiği için, bellekte karşılığı olmayan bir nesne üzerinden `.ToUpper()` metodunu çağırmaya çalıştığımızda programın patlayabileceğini haber verdi.

### 2. Ctrl + Z Denemesi ve Alınan Exception
- **Yapılan İşlem:** Program `dotnet run` ile çalıştırılıp `Adınız: ` sorulduğunda hiçbir karakter girmeden `Ctrl + Z` (girdi akışının sonu - EOF) yapılıp `Enter`'a basıldı. Bu durumda `Console.ReadLine()` metodu belleğe `null` değer döndürdü.
- **Alınan Hata (Exception):**  
  `Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.`
- **Hangi Satırda Patladı?:**  
  Hata tam olarak 3. satırdaki `ad.ToUpper()` ifadesinde gerçekleşti. Çünkü `ad` değişkeni `null` iken bir sınıf metoduna erişmeye çalıştık ve işletim sistemi boş referansı gösterdiği için çalışma zamanında (runtime) uygulama doğrudan çöktü.

### 3. Uyarıları Kaldırma Yöntemleri (Hangisi Çözüyor, Hangisi Susturuyor?)
Derleyicinin uyarılarını gidermek için denenen 3 farklı yöntemin analizi:

1. **`string? ad` ve `ad?.ToUpper()`:**  
   - *Durum:* **Gerçek Çözüm.**  
   - *Açıklama:* Değişkeni baştan `string?` yaparak null değer barındırabileceğini kabul ediyoruz. Çağrı esnasında null-conditional (`?.`) operatörü kullanarak "eğer `ad` null değilse `ToUpper()` çalıştır, null ise hiçbir şey yapma ve null dön" mantığını kuruyoruz. Kod her iki duruma da hazır hale gelir.
2. **`?? ""` (Null-Coalescing Operatörü):**  
   - *Durum:* **Gerçek Çözüm.**  
   - *Açıklama:* `Console.ReadLine() ?? ""` yazarak eğer girdi null gelirse bunun yerine boş bir metin (`""`) atanmasını sağlıyoruz. Böylece `ad` değişkeni hiçbir zaman null olmuyor ve 3. satırdaki `.ToUpper()` fonksiyonu güvenle çalışıyor.
3. **`Console.ReadLine()!` (Null-Forgiving / Dam Operatörü):**  
   - *Durum:* **Yalnızca Susturuyor (Tehlikeli).**  
   - *Açıklama:* Ünlem işareti derleyiciye *"Sen sus, ben ne yaptığımı biliyorum, burası kesinlikle null gelmeyecek"* demektir. Derleyicinin sarı uyarısı kaybolur; ancak çalışma anında kullanıcı yine `Ctrl + Z` basarsa arkada hiçbir güvenlik önlemi olmadığı için program yine aynı `NullReferenceException` ile çöker. Gerçek bir hata yönetimi sağlamaz, sadece uyarıyı gizler.

### 4. Düşünün: `TreatWarningsAsErrors` Neden Açılır? Hangi Projelerde Tercih Edilir?
- **Neden Açılır?:** Uyarılar (warning) derleme aşamasında kodu durdurmaz ve gözden kaçması çok kolaydır. Ancak özellikle null referans gibi uyarılar, göz ardı edildiğinde canlıya (production) çıkan yazılımın ortasında uygulamanın çökmesine (crash), kullanıcının ekranının kilitlenmesine veya veri tabanına yarım/hatalı işlem yapılmasına neden olur. `TreatWarningsAsErrors` (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`) açıldığında derleyici tüm uyarıları doğrudan birer derleme hatası (build error) gibi kabul eder ve kod derlenmez. Böylece ekipteki hiçbir geliştirici uyarıları görmezden gelerek canlıya eksik veya riskli kod gönderemez.
- **Hangi Projelerde Tercih Edilir?:** Bankacılık, e-ticaret, ödeme altyapıları, sağlık sistemleri gibi hataya tahammülü olmayan kritik kurumsal projelerde, büyük ekiplerin ortak çalıştığı kurumsal CI/CD süreçlerinde ve açık kaynak kütüphane geliştirme projelerinde kod kalitesini ve güvenliğini garanti altına almak için standart olarak tercih edilir.

## 5. Adım: Konsolda Todo
Program başarıyla çalıştırıldı; 3 görev eklendi, biri tamamlandı ve listelendi.

## 6. Adım: Kırın
1. Boş başlıkla görev eklenince doğrulanmadan listeye alındı ve `0. [ ] ` şeklinde boş satır oluştu.
2. Tamamla adımında harf (`iki`) girildiğinde:
   - `int.TryParse` kullanıldığında güvenli bir şekilde `false` döndü, "Geçersiz numara!" dedi ve uygulama çökmedi.
   - Kod geçici olarak `int.Parse`'a çevrildiğinde ise `System.FormatException: The input string 'iki' was not in a correct format.` exception'ı fırlatılarak program anında çöktü.
3. 99 silinmek istendiğinde indeks aralık kontrolü sayesinde program çökmeden "Geçersiz numara!" uyarısı verdi.
4. Sırayla A, B ve C eklendi; 1 numaralı B tamamlandı; 0 numaralı A silindi ve listelendi. Ekranda `0. [ ] B` ve `1. [x] C` görüldü.
- **Sonuç:** Tamamladığımız B görevi yerine C görevi tamamlanmış göründü.
- **Neden oldu:** Görevlerin kimliği sabit bir Id yerine listenin dinamik sıra numarası (`i`) olarak tutulduğu için 0. eleman silinince arkadaki elemanların indeksleri öne kaydı; HashSet içindeki 1 değeri bu kez C'ye denk geldi.

## Birinci Günün Sonu Değerlendirmesi

### 1. SDK, Runtime ve Derleyici: Hangisi Ne Yapar?
- **Derleyici (Roslyn):** Yazdığımız insan tarafından okunabilir C# kaynak kodunu sözdizimsel ve anlamsal analizden geçirir. Hata yoksa bu kodu doğrudan makine koduna değil; işlemciden ve işletim sisteminden bağımsız bir ara dile (CIL / IL - Common Intermediate Language) dönüştürerek `.dll` veya `.exe` içine paketler.
- **Runtime (CLR - Common Language Runtime):** Derleyicinin ürettiği bu IL kodunu işletim sistemi üzerinde fiilen çalıştıran yürütme motorudur. JIT (Just-In-Time) derleyicisi ile IL kodunu o anki işlemcinin makine diline çevirir. Bellek tahsisini (Memory Allocation), kullanılmayan nesnelerin temizlenmesini (Garbage Collection), tip güvenliğini ve istisna (exception) yönetimini üstlenir.
- **SDK (.NET Software Development Kit):** Geliştiricinin yazılım üretebilmesi için gereken tüm araç takımını kapsayan ana şemsiyedir. İçerisinde Roslyn derleyicisini, `dotnet` CLI araçlarını, MSBuild altyapısını, standart kütüphaneleri ve kodları test edip çalıştırabilmemiz için gerekli olan Runtime paketlerini bir arada barındırır.
- **Özet:** SDK kod üretmek ve inşa etmek için bilgisayarımızda bulunması gereken geliştirici paketidir; derleyici kodu ara forma çeviren dönüştürücüdür; Runtime ise bu ara formu canlı ortamda çalıştıran motordur.

### 2. `double` ile `decimal` Arasındaki Fark Nedir, Hangisini Nerede Kullanırsınız?
- **`double` (IEEE 754 Kayan Noktalı Sayı):**
  - Bellekte 64-bit yer kaplar ve ikili (base-2) sayı tabanında temsil edilir.
  - İkili taban yapısı gereği `0.1` veya `0.2` gibi basit ondalık kesirler ikili sistemde devirli sayılara dönüşür. Bu nedenle `0.1 + 0.2` toplandığında tam olarak `0.3` yerine `0.30000000000000004` üretilir ve mantıksal eşitlik (`== 0.3`) `False` döner.
  - Donanım (FPU) tarafından doğrudan desteklendiği için son derece hızlıdır ve çok geniş bir sayı aralığını kapsar.
  - **Kullanım Alanı:** Milimetrik kuruş hassasiyetinin aranmadığı, yüksek işlem hızının ve geniş değer aralığının kritik olduğu 3D grafik motorları, fizik simülasyonları, oyun geliştirme, veri bilimi ve mühendislik hesaplamalarında tercih edilir.
- **`decimal` (128-bit Yüksek Hassasiyetli Sabit/Ondalık Sayı):**
  - Bellekte 128-bit yer kaplar ve onluk (base-10) sayı tabanında temsil edilir.
  - Bizim günlük hayatta kullandığımız ondalık sistemle birebir çalıştığı için `0.1m + 0.2m == 0.3m` ifadesi kesinlikle `True` sonucunu verir; kuruş veya küsurat kaybı/yuvarlama sapması yaşanmaz.
  - Aritmetik işlemler yazılımsal düzeyde yönetildiği için `double` türüne göre hesaplama maliyeti daha yüksektir ve daha yavaş çalışır.
  - **Kullanım Alanı:** Yuvarlama hatalarının yasal veya finansal sorunlara yol açabileceği bankacılık sistemleri, e-ticaret sepet hesaplamaları, muhasebe, faturalandırma ve vergilendirme modüllerinde standart olarak kullanılır.

### 3. Nullable Uyarısı Bizi Neyden Korumaya Çalışıyor?
- Tony Hoare'un "milyar dolarlık hata" (billion-dollar mistake) olarak nitelendirdiği ve nesne yönelimli dillerde en çok karşılaşılan çökme sebebi olan **`NullReferenceException` (NRE)** hatasından korumaya çalışır.
- Normal şartlarda bir referans değişkeni `null` iken onun bir özelliğine (`Length`) veya metoduna (`ToUpper()`) erişmek derleme anında fark edilmez. Kod derlenir, dağıtıma çıkar ve canlıda kullanıcı beklenmeyen bir girdi verdiğinde uygulama aniden kilitlenir veya çöker.
- C#'ın Nullable Reference Types özelliği, daha biz kodu yazarken olası null senaryolarını statik kod analiziyle tespit eder. Bizi derleme aşamasında uyararak nesnenin bellekte var olduğundan emin olmaya, güvenli erişim operatörlerini (`?.`, `??`) kullanmaya veya null kontrolleri yazmaya zorlar. Bu sayede hatalar müşterinin önünde değil, geliştirme aşamasında yakalanır.

### 4. `Parse` ile `TryParse` Arasındaki Fark Nedir, Hangisini Ne Zaman Seçersiniz?
- **`int.Parse`:** Kendisine verilen metni doğrudan tamsayıya dönüştürmeye çalışır. Eğer girdi metninde sayı yerine harf, geçersiz bir sembol veya boşluk varsa işlem başarısız olur ve çalışma zamanında **`System.FormatException`** fırlatır; girdi çok büyükse **`OverflowException`** fırlatır. Bu da programın çökmesine yol açar.
- **`int.TryParse`:** İki parametre alır (girdi metni ve `out` değişkeni). Dönüşüm başarılı olursa `out` değişkenine değeri yazar ve geriye `true` döner. Girdi hatalı, geçersiz veya uyumsuz ise kesinlikle exception fırlatmaz; sadece geriye `false` döner ve programın akışı bozulmadan devam eder.
- **Seçim Kriteri:**
  - Eğer veri kaynağı bizim tam kontrolümüz altındaysa, doğruluğundan yüzde yüz emin olduğumuz dahili bir veri tabanı tablosundan veya şeması garanti bir API kontratından geliyorsa `Parse` tercih edilebilir (çünkü beklenmeyen bir bozulma varsa zaten sürecin durması gerekir).
  - Kullanıcıdan klavye ile alınan her türlü girdi (konsol ekranı, form alanları), harici dosya okumaları veya kontrolsüz API isteklerinde daima `TryParse` seçilmelidir. Exception fırlatıp yakalamak (.NET çalışma zamanında stack trace üretimi nedeniyle) maliyetli ve yavaş bir işlem olduğu için `TryParse` hem güvenli akış hem de yüksek performans sağlar.

### 5. 6. Adımın Dördüncü Maddesindeki Hata Neden Oldu?
- **Hatanın Kök Nedeni:** Görevlerin tamamlanma durumunu takip ederken her göreve ait kalıcı ve değişmez bir kimlik (`Id`) kullanmak yerine, listenin geçici ve dinamik sıra numarasını (`List<string>` indeksini) kullanmamızdan kaynaklandı.
- **Süreç Nasıl Bozuldu?:**
  1. Başlangıçta listeye sırayla `A` (indeks 0), `B` (indeks 1) ve `C` (indeks 2) görevleri eklendi.
  2. 1 numaralı görev olan `B` tamamlandı ve `tamamlananlar` setine `1` değeri eklendi.
  3. Ardından 0 numaralı görev olan `A` listeden silindi (`gorevler.RemoveAt(0)`).
  4. Liste dinamik bir dizi mantığıyla çalıştığı için `A` silinince arkadaki elemanlar öne doğru kaydı: Artık 0. indekste `B`, 1. indekste ise `C` yer aldı.
  5. Ancak `tamamlananlar` koleksiyonu hâlâ hafızasında `1` değerini tutuyordu. Listeleme döngüsü çalıştığında 1. indekste artık `C` oturduğu için sistem kontrolsüz bir şekilde `C` görevini tamamlanmış (`1. [x] C`), asıl tamamladığımız `B` görevini ise tamamlanmamış (`0. [ ] B`) olarak ekrana bastı.
- **Çıkarılan Ders:** Bir koleksiyondan eleman silinebilen veya sırası değişebilen sistemlerde varlıkların durumları asla dinamik dizi indeksine bağlanamaz. Her varlığın oluşturulduğu anda kendine özel, ömrü boyunca değişmeyen tekil bir anahtara (`Id`, `Guid`) ve durumu kendi bünyesinde taşıyan bir nesne yapısına (`TodoItem` sınıfı) sahip olması gerekir.

# C# Görevi - 2. Gün Raporu

**Tarih:** 22 Eylül 2026
**Hazırlayan:** Büşra Seleş
**Proje Adı:** gun2-oop

## 7. Adım: TodoItem Sınıfı

### 1. TodoItem.cs Kaynak Kodu

```csharp
using System.Text.Json.Serialization;

public class TodoItem
{
    public int Id { get; }
    public string Title { get; private set; }

    [JsonInclude]
    public bool IsDone { get; private set; }

    [JsonInclude]
    public DateTime CreatedAt { get; private set; }

    // Standart nesne üretim kurucusu
    public TodoItem(int id, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Başlık boş olamaz.", nameof(title));

        Id = id;
        Title = title.Trim();
        CreatedAt = DateTime.Now;
    }

    // JSON deserialization için kurucu
    [JsonConstructor]
    public TodoItem(int id, string title, bool isDone, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Başlık boş olamaz.", nameof(title));

        Id = id;
        Title = title;
        IsDone = isDone;
        CreatedAt = createdAt;
    }

    public void Complete() => IsDone = true;
}
```

### 2. Denemeler ve Gözlemler

#### `var t = new TodoItem(1, " ");` Deneyimi

**Sonuç:** Program çalışma zamanında (runtime) çöktü ve `System.ArgumentException: Başlık boş olamaz. (Parameter 'title')` hatası fırlattı.

**Açıklama:** Sınıf kurucusundaki `string.IsNullOrWhiteSpace(title)` kontrolü devreye girerek geçersiz, anlamsız veya sadece boşluktan oluşan veriye sahip bir nesnenin bellekte oluşturulmasını engelledi.

#### `t.IsDone = true;` Deneyimi

**Derleyici Hatası:** `CS0272: The property or indexer 'TodoItem.IsDone' cannot be used in this context because the set accessor is inaccessible.`

**Açıklama:** `IsDone` alanının erişim belirteci `private set` olarak tanımlandığı için sınıf dışından doğrudan değer ataması derleme aşamasında yasaklanmıştır.

#### IsDone'ın Setter'ı Private İken Complete() Metodunun Public Olma Sebebi

**Kapsülleme (Encapsulation):** Nesnenin iç durumunu korumak için durum değişiklikleri doğrudan alanlar üzerinden değil, kontrollü metotlar üzerinden yapılmalıdır. Dış dünya bir görevi sadece "tamamlayabilir" (`Complete()`); görevin tamamlanma mantığı, gerekiyorsa loglama veya ek iş kuralları nesnenin kendi içinde yönetilir.

### 3. Düşünün (İş Kuralının Yeri)

**Kural Neden Program.cs'te Değil de TodoItem İçinde?**

Tekrarlanabilirliği önlemek ve nesne bütünlüğünü garantiye almak için. `TodoItem` sınıfı bugün bir konsol uygulamasında (`gun2-oop`), perşembe günü bir Web API projesinde, gelecekte ise bir mobil uygulamada veya arka plan servisinde kullanılabilir. Validasyon `Program.cs` içine yazılsaydı, her yeni istemcide bu kontrolü baştan yazmak gerekirdi ve bir geliştiricinin kontrolü unutması durumunda sisteme bozuk veri sızabilirdi. Kural doğrudan sınıfın kurucusuna konulduğunda, nesne nereden üretilirse üretilsin geçersiz veriyle ayağa kalkması imkansız hale gelir.

---

## 8. Adım: Interface (ITodoRepository ve InMemory İncelemesi)

### 1. ITodoRepository.cs Kaynak Kodu

```csharp
public interface ITodoRepository
{
    IReadOnlyList<TodoItem> GetAll();
    TodoItem? GetById(int id);
    TodoItem Add(string title);
    bool Complete(int id);
    bool Remove(int id);
}
```

### 2. InMemoryTodoRepository.cs Kaynak Kodu

```csharp
public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<TodoItem> _items = new();
    private int _sonId = 0;

    public IReadOnlyList<TodoItem> GetAll() => _items;

    public TodoItem? GetById(int id) => _items.FirstOrDefault(t => t.Id == id);

    public TodoItem Add(string title)
    {
        var item = new TodoItem(++_sonId, title);
        _items.Add(item);
        return item;
    }

    public bool Complete(int id)
    {
        var item = GetById(id);
        if (item == null) return false;
        item.Complete();
        return true;
    }

    public bool Remove(int id)
    {
        var item = GetById(id);
        if (item == null) return false;
        return _items.Remove(item);
    }
}
```

### 3. Dünkü Hatanın Tekrar Test Edilmesi (A, B, C Senaryosu)

**Uygulanan Adımlar:**
1. A, B ve C görevleri eklendi (Id'ler sırasıyla: 1, 2, 3).
2. Id'si 2 olan B görevi tamamlandı (`[x]`).
3. Id'si 1 olan A görevi silindi.
4. Liste görüntülendi.

**Konsol Çıktısı:**
```
2. [x] B
3. [ ] C
```

**Hata Düzeldi mi? Neden?**

Evet, hata tamamen düzeldi. 1. günde görevler listenin dinamik indeks numaralarına (0, 1, 2) göre takip ediliyordu; baştan eleman silindiğinde tüm indeksler kayıyor ve tamamlanan görev yanlış eşleşiyordu. 8. adımda ise her görev nesnesi üretildiği anda kalıcı ve tekil bir Id değeri aldı. A silinse dahi B'nin Id'si 2 olarak sabit kaldı; arama ve tamamlama işlemleri Id üzerinden yürütüldüğü için indeks kayması yaşanmadı.

**Kurumdaki Karşılığı:**

İleride çalışılacak AtisCore mimarisinde bu sözleşmenin adı `ITodoDal` olup `DataAccess/Abstract` katmanında yer alır. Bellek üzerinde çalışan karşılığı `InMemoryTodoDal` iken, veritabanına yazan implementasyon `EfTodoDal` olarak adlandırılır. İsimler değişse de soyutlama ve somutlama mantığı birebir aynıdır.

---

## 9. Adım: İkinci Bir İmplementasyon (JsonFileTodoRepository)

### 1. JsonFileTodoRepository.cs Kaynak Kodu

```csharp
using System.Text.Json;

public class JsonFileTodoRepository : ITodoRepository
{
    private readonly string _dosyaYolu;
    private readonly List<TodoItem> _items = new();
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public JsonFileTodoRepository(string dosyaYolu)
    {
        _dosyaYolu = dosyaYolu;

        if (File.Exists(_dosyaYolu))
        {
            var json = File.ReadAllText(_dosyaYolu);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var yuklenen = JsonSerializer.Deserialize<List<TodoItem>>(json, _options);
                if (yuklenen != null)
                {
                    _items.AddRange(yuklenen);
                }
            }
        }
    }

    private void DosyayaKaydet()
    {
        var json = JsonSerializer.Serialize(_items, _options);
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
        if (item == null) return false;
        item.Complete();
        DosyayaKaydet();
        return true;
    }

    public bool Remove(int id)
    {
        var item = GetById(id);
        if (item == null) return false;
        var sonuc = _items.Remove(item);
        if (sonuc) DosyayaKaydet();
        return sonuc;
    }
}
```

### 2. Ölçüm ve Dosya Kanıtları

**Program.cs Değişimi (`git diff --stat`)**

Ölçüm Sonucu: Kod seviyesinde somut sınıf değişimi için yalnızca 1 satır değiştirilmiştir:

```diff
- ITodoRepository repo = new InMemoryTodoRepository();
+ ITodoRepository repo = new JsonFileTodoRepository("todos.json");
```

**Git Ölçümü:** Proje commit geçmişinde `git diff --stat HEAD~1 Program.cs` çalıştırıldığında, önceki committe sadece `TodoItem.cs` arşivlendiği için `Program.cs` yeni bir dosya olarak `1 file changed, 58 insertions(+)` şeklinde doğrulanmıştır.

**todos.json Dosya İçeriği**

`Get-Content todos.json` ile okunan gerçek JSON çıktısı:

```json
[
  {
    "Id": 1,
    "Title": "Kitap oku",
    "IsDone": true,
    "CreatedAt": "2026-09-22T11:45:12.8421054+03:00"
  }
]
```

### 3. Karşılaşılan Sürpriz ve Çözümü

**Soru:** Programı kapatıp açınca görevler geri geldi mi? Tamamlananlar tamamlanmış kaldı mı?

**İlk Gözlem:** Program kapatılıp açıldığında görev listelendi ancak tamamlanmış olan `[x]` işareti kaybolarak `[ ]` (false) olarak ekrana geldi.

**Nedeni:** 7. adımda `IsDone` özelliğinin setter'ı `private set` yapılmıştı. `JsonSerializer` harici bir araç olduğu için sınıf dışından private alana veri yazamadı ve hata vermeden sessizce varsayılan `false` değerinde bıraktı. Aynı şekilde `CreatedAt` için de dosyadaki eski tarih okunamıyordu.

**Çözüm:** Özelliklerin üzerine `[JsonInclude]` niteliği eklendi ve JSON ayrıştırma esnasında çalışan `[JsonConstructor]` kurucu metodu tanımlandı. Bu sayede hem `IsDone` değeri kalıcı olarak korundu hem de `CreatedAt` her açılışta baştan üretilmeyip dosyadaki orijinal anını muhafaza etti.

### 4. Dependency Inversion Analizi

Veri depolama mantığı bellekten fiziksel diske geçirilirken kullanıcı arayüzü, döngüler, menü ve validasyon kodlarının bulunduğu `Program.cs`'e hiç dokunulmadı.

`Program.cs`, somut veri erişim sınıflarına (`InMemoryTodoRepository` veya `JsonFileTodoRepository`) doğrudan bağımlı değildir; yalnızca `ITodoRepository` soyutlamasına bağımlıdır. Bu sayede sistem bileşenleri birbirine sıkı sıkıya bağlanmamış (loose coupling), tak-çıkar modüler bir yapı elde edilmiştir.

---

## 10. Adım: Record ve Class Karşılaştırması

### 1. Karşılaştırma Tablosu

| Satır | Tahmininiz | Gerçek Çıktı |
|---|---|---|
| `c1 == c2` | False | False |
| `r1 == r2` | True | True |
| `c1` | NoktaClass | NoktaClass |
| `r1` | NoktaRecord { X = 1, Y = 2 } | NoktaRecord { X = 1, Y = 2 } |

### 2. Düşünün (DTO ve Değer Eşitliği)

**DTO (Data Transfer Object) Ne Demek?**

Katmanlar arasında, servisler arasında ya da ağ üzerinden (örneğin Web API ile istemci arasında) yalnızca veri taşımak amacıyla kullanılan, iş mantığı ve davranış içermeyen veri taşıma yapılarıdır.

**Eşitliğin Nesnenin Kimliğine Değil Değerine Göre Çalışması Neden Doğal?**

Bir veritabanı varlığı (Entity) kendine has bir kimliğe (Id) sahiptir; iki ayrı sipariş nesnesi aynı tutara sahip olsa bile kimlikleri farklıdır. Ancak DTO yalnızca bir veri paketidir. Bir API uç noktasına gelen iki istek paketi `{ X: 1, Y: 2 }` içeriğine sahipse, bu paketlerin bellekte hangi adreste durduğuyla ilgilenilmez; taşıdıkları değerlerin özdeş olup olmadığına bakılır. `record` yapıları varsayılan olarak değer eşitliği (value-based equality) sağladığı için ek kod yazmadan `==` operatörüyle veri paketlerinin eşitliğini karşılaştırma imkanı sunar.

**ToString() Çıktı Farkı:**

`class` varsayılan olarak sadece tip adını ekrana basarken (`NoktaClass`), `record` derleyici tarafından otomatik üretilen formatlayıcı ile içindeki değerleri okunaklı biçimde basar (`NoktaRecord { X = 1, Y = 2 }`), bu da loglama süreçlerinde büyük avantaj sağlar.

---

## 11. Adım: Kırın (Hata ve Dayanıklılık Testleri)

### 1. Dört Denemenin Sonuçları

#### 1. ITodoRepository'ye `int Count();` Eklenmesi

**Sonuç:** `dotnet build` çalıştırıldığında derleme başarısız oldu ve 2 adet `CS0535` derleme hatası alındı.

**Etkilenen Dosyalar:**
```
InMemoryTodoRepository.cs(1,39): error CS0535: 'InMemoryTodoRepository', 'ITodoRepository.Count()' arabirim üyesini uygulamaz
JsonFileTodoRepository.cs(3,39): error CS0535: 'JsonFileTodoRepository', 'ITodoRepository.Count()' arabirim üyesini uygulamaz
```

**Çıkarım:** Bir interface'e yeni bir metot eklemek, o interface'i uygulayan istisnasız tüm sınıfları bozar. Bu durum Interface Segregation Principle (Arayüzlerin Ayrılması Prensibi) kuralının önemini gösterir: Arayüzler olabildiğince küçük ve tek amaca hizmet edecek şekilde tutulmalıdır.

#### 2. todos.json Dosyasının Elle Bozulması (Süslü Parantez Silinmesi)

**Exception Türü:** `System.Text.Json.JsonException` (İç hata: `System.Text.Json.JsonReaderException`).

**Hata Mesajı:** `']' is invalid without a matching open. Path: $[0] | LineNumber: 7 | BytePositionInLine: 0.`

**Açıklama:** JSON sözdizim kuralı ihlal edildiği için ayrıştırıcı (parser) metni çözümleyememiş ve çalışma zamanında çökmüştür.

#### 3. todos.json İçinde Başlığın Boş ("") Bırakılması

**Exception Türü:** `System.ArgumentException: Başlık boş olamaz. (Parameter 'title')`

**Hata Konumu:** `TodoItem.cs:line 30` kurucu metodu.

**7. Adımdaki Kural Dosyadan Okurken de Çalıştı mı?**

Evet, çalıştı. `JsonSerializer` nesneyi canlandırırken `[JsonConstructor]` ile işaretli kurucuyu çalıştırdığı için, 7. adımda yazdığımız validasyon kuralı dosya elle manipüle edilse dahi devreye girmiş ve geçersiz bir nesnenin belleğe alınmasını engellemiştir.

#### 4. todos.json Dosyasının Silinmesi

**Sonuç:** Program hiçbir hata vermeden, çökmeden açıldı.

**Açıklama:** `JsonFileTodoRepository` kurucusunda yazdığımız `if (File.Exists(_dosyaYolu))` kontrolü dosyanın olmadığını tespit etmiş ve sıfırdan boş bir liste ile güvenli bir şekilde çalışmayı başlatmıştır.

### 2. En Çok Şaşırtan Durum

2. denemedeki davranış en dikkat çekici sonuç oldu. Dışarıdan JSON dosyasındaki veri doğrudan değiştirilse bile, `[JsonConstructor]` sayesinde nesne yönelimli programlamanın getirdiği kapsülleme ve kurucu kısıtlarının `JsonSerializer` tarafından atlanmayıp iş kurallarının korunması, nesne bütünlüğünün ne kadar sağlam inşa edilebileceğini kanıtladı.

---

## İkinci Günün Sonu: Genel Değerlendirme

**Class ile Object Arasındaki Fark:**

Class (Sınıf), bir nesnenin sahip olacağı alanları, özellikleri ve metotları belirleyen soyut bir şablon, tasarım planıdır (`TodoItem`). Object (Nesne) ise bu şablondan `new` anahtar sözcüğü ile üretilmiş, bellekte (Heap) yer kaplayan somut bir örnektir (`new TodoItem(1, "Kitap oku")`).

**IsDone Neden Dışarıdan Değiştirilemiyor ve Bu Bize Ne Kazandırdı?**

Dışarıdan rastgele veya yetkisiz müdahaleleri önlemek, veri tutarlılığını sağlamak için `private set` yapılmıştır. Bu sayede durum değişikliği sadece `Complete()` metoduyla sınırlandırılmış, nesnenin her zaman geçerli bir iş mantığı durumunda kalması garanti edilmiştir.

**Interface Bize Somut Olarak Ne Kazandırdı?**

Veri saklama ortamını bellekten dosyaya taşırken kullanıcı arayüzü kodlarını barındıran `Program.cs` üzerinde sadece 1 satırlık değişiklik (`new InMemoryTodoRepository()` → `new JsonFileTodoRepository("todos.json")`) yapmamızı sağladı. Menü, listeleme ve tamamlama mekanizmaları arka plandaki depolama teknolojisinden tamamen bağımsız hale geldi.

**Ne Zaman Record, Ne Zaman Class?**

- **record:** Kimliği (Id) olmayan, değişmezlik (immutability) gerektiren, sadece veri paketlemek veya aktarmak için kullanılan DTO (Data Transfer Object) ve Request/Response modellerinde tercih edilir. Eşitlik değer odaklıdır.
- **class:** Durum (state) yöneten, davranış (behavior) barındıran, kendine ait bir yaşam döngüsü ve kimliği (Id) bulunan iş nesneleri (Entities, Services, Repositories) için kullanılır.

**Dependency Inversion'ın Kendi Cümlelerimizle Tanımı ve Projedeki Örneği:**

Yüksek seviyeli modüller (iş mantığı/UI), düşük seviyeli modüllere (veritabanı/dosya yazıcı) doğrudan bağımlı olmamalı; her iki grup da soyutlamalara (arayüzlere) bağımlı olmalıdır.

**Örnek:** Projemizdeki `Program.cs`, doğrudan somut olan `JsonFileTodoRepository` sınıfına sıkı sıkıya bağlanmamıştır; `ITodoRepository` arayüzüne bağımlıdır. Verinin bellekte mi, JSON dosyasında mı yoksa yarın bir veritabanında mı tutulduğu `Program.cs`'in sorumluluğunda değildir.


# C# Görevi - 3. Gün Raporu

**Tarih:** 23 Eylül 2026
**Hazırlayan:** Büşra Seleş
**Proje Adı:** gun3-linq


## Ürünler Dosyası Bilgisi

**İkinci ürün:** Gıda
**Üçüncü ürün:** Temizlik
**Arkadaşımınkiyle aynı:** Evet

---

## 13. Adım: LINQ Sorguları

Aynı tohum (seed: 42) kullanıldığı için üretilen veriler ve LINQ sorgu sonuçlarımız arkadaşımla birebir aynı çıkmıştır.

| # | Soru | Sizin Cevabınız | Arkadaşınızın Cevabı |
|---|------|------------------|------------------------|
| 1 | Kaç ürünün stoğu 0? | 195 | 195 |
| 2 | Her kategoride kaç ürün var? | Elektronik: 1960 adet<br>Gıda: 2029 adet<br>Kırtasiye: 1971 adet<br>Oyuncak: 1987 adet<br>Temizlik: 2053 adet | Temizlik: 2053<br>Gıda: 2029<br>Elektronik: 1960<br>Oyuncak: 1987<br>Kırtasiye: 1971 |
| 3 | Her kategorinin ortalama fiyatı (iki basamak) | Elektronik: 499,09 TL<br>Gıda: 494,43 TL<br>Kırtasiye: 505,65 TL<br>Oyuncak: 509,44 TL<br>Temizlik: 500,29 TL | Temizlik: 500,29 TL<br>Gıda: 494,43 TL<br>Elektronik: 499,09 TL<br>Oyuncak: 509,44 TL<br>Kırtasiye: 505,65 TL |
| 4 | En pahalı beş ürünün Id'leri | 5003, 6951, 6957, 4302, 448 | 5003, 6951, 6957, 4302, 448 |
| 5 | Fiyatı 500 ile 600 TL arasında olan Elektronik ürün sayısı | 194 (Sınırlar dahil: 194, Hariç: 194) | 194 |
| 6 | Toplam stok değeri (fiyat × stok) en yüksek kategori | Temizlik (25.558.657,24 TL) | Temizlik (25558657,24 TL) |

### Kanıt (Kullanılan LINQ Kodları)

1. **Soru:** `urunler.Count(u => u.Stock == 0);`
2. **Soru:** `urunler.GroupBy(u => u.Category).Select(g => new { Kategori = g.Key, Adet = g.Count() });`
3. **Soru:** `urunler.GroupBy(u => u.Category).Select(g => new { Kategori = g.Key, Ortalama = Math.Round(g.Average(u => u.Price), 2) });`
4. **Soru:** `urunler.OrderByDescending(u => u.Price).Take(5).Select(u => u.Id);`
5. **Soru:** `urunler.Count(u => u.Category == "Elektronik" && u.Price >= 500m && u.Price <= 600m);`
6. **Soru:** `urunler.GroupBy(u => u.Category).Select(g => new { Kategori = g.Key, Toplam = g.Sum(u => u.Price * u.Stock) }).OrderByDescending(x => x.Toplam).First();`

### Düşünün Sorusu Cevabı

**5. Sorudaki Belirsizlik:** "500 ile 600 TL arası" ifadesi sınırların dahil mi (`>= 500 && <= 600`) yoksa hariç mi (`> 500 && < 600`) olduğunu netleştirmez. Veri setinde tam olarak 500,00 TL ya da 600,00 TL olan bir elektronik ürün bulunmadığı için iki koşulda da sonuç 194 çıkmıştır. Ancak gerçek iş gereksinimlerinde bu sınır koşulları uyuşmazlıklara yol açabileceğinden kural baştan netleştirilmelidir.

---

## 14. Adım: LINQ Ne Zaman Çalışır?

| Kod Satırı | Tahmininiz | Gerçek Çıktı |
|---|---|---|
| `Console.WriteLine(string.Join(", ", buyukler));` | 2, 3 | 2, 3, 10 |
| `Console.WriteLine(string.Join(", ", buyuklerListe));` | 2, 3, 10 | 2, 3, 10 |

**Sayacın Değeri:** 10023

**Neden tam olarak 10023?:**
`sorgu.Count()` çağrısı tüm koleksiyonu tarayarak sayacı 10.000 artırdı. `sorgu.First()` çağrıldığında sorgu belleğe alınmadığı için en baştan tekrar çalıştı; stoğu 0 olan ilk ürünün Id'si 23 olduğu için 23 kontrol daha yapıp durdu. Toplamda filtre 10.000 + 23 = 10023 kez çalıştırıldı.

### Düşünün Sorularının Cevapları

1. **Veritabanına giden bir sorgu olsaydı Count() ve First() ayrı ayrı çağrıldığında veritabanına kaç kez gidilirdi?**
   İki kez gidilirdi. İlkinde `SELECT COUNT(*)...`, ikincisinde `SELECT TOP 1...` SQL sorguları ayrı ayrı veritabanına iletilirdi.

2. **Bir sorguyu neden bazen `.ToList()` ile sabitleriz?**
   Ertelenmiş çalışma (deferred execution) her çağrıda sorgunun baştan çalışmasına neden olur. Verinin değişmeyeceği durumlarda sorguyu belleğe alarak gereksiz CPU ve veritabanı maliyetlerinin önüne geçmek için `.ToList()` ile sabitleriz.

---

## 15. Adım: Ölçüm, Liste mi Sözlük mü?

Terminalde yapılan ölçüm sonuçları:

| Çalıştırma | Mod | List (ms) | Dictionary (ms) |
|---|---|---|---|
| 1 | Debug | 21,61 ms | 0,06 ms |
| 2 | Debug | 23,02 ms | 0,04 ms |
| 3 | Debug | 21,77 ms | 0,04 ms |
| 1 | Release | 18,70 ms | 0,05 ms |
| 2 | Release | 18,38 ms | 0,05 ms |
| 3 | Release | 18,48 ms | 0,05 ms |

### Düşünün Sorularının Cevapları

**Dictionary kaç kat hızlı?**
- Debug modunda List ortalaması yaklaşık 22,13 ms iken Dictionary ortalaması 0,046 ms civarındadır; yani Dictionary yaklaşık 480 kat daha hızlıdır.
- Release modunda List ortalaması 18,52 ms iken Dictionary 0,05 ms'dir; yani Dictionary yaklaşık 370 kat daha hızlıdır.
- Bunun sebebi: `List.FirstOrDefault` her aramada baştan başlayıp tek tek gezdiği için O(N) karmaşıklığa sahiptir; Dictionary ise hash tablosu ve anahtar (Key) eşleşmesiyle çalıştığı için O(1) sürede doğrudan adrese ulaşır.

**Ürün sayısı 10.000 yerine 1.000.000 yapılsaydı fark büyür mü, aynı mı kalır?**
- Fark katlanarak büyür.
- List araması doğrusal (O(N)) olduğundan veri 100 kat büyüdüğünde arama süresi de yaklaşık 100 kat artar (örneğin 18 ms yerine ~1800 ms sürer).
- Dictionary araması ise sabit zamanlı (O(1)) olduğundan eleman sayısı 10 bin de olsa 1 milyon da olsa arama süresi neredeyse hiç değişmez (~0,05 ms bandında kalır). Dolayısıyla aralarındaki hız farkı binlerce kata çıkar.

**13. adımdaki tablo aynı çıkarken bu tablo neden farklı çıkar?**
- 13. adım mantıksal sorgulardan oluşur; aynı tohum (seed: 42) ve aynı kurallar kullanıldığı sürece matematiksel sonuç her bilgisayarda birebir aynı olmak zorundadır.
- 15. adım ise milisaniye cinsinden donanımsal süre ölçümüdür. CPU frekansı, RAM hızı, çekirdek mimarisi, arka plandaki işletim sistemi yükü ve JIT derleyicisinin anlık çalışma performansı makineden makineye değiştiği için süreler ikimizde farklı çıkar.

---

## 16. Adım: async ve await

### Derleyici Uyarısı

- **Gözlem:** Terminalde herhangi bir derleyici uyarısı basılmadı; kod doğrudan derlenip çalıştı.
- **Beklenen / Teorik Durum:** `async` olarak tanımlanmış bir metot gövdesinde `await` barındırmadığında .NET derleyicisinin normal şartlarda `CS1998` koduyla metodun senkron çalışacağını bildirmesi beklenir.

### Ölçüm Tablosu

| Deneme | Tahmininiz (ms) | Ölçülen (ms) |
|---|---|---|
| `Task.Delay`, sırayla | 3000 ms | 3028 ms |
| `Task.Delay`, birlikte | 1000 ms | 1001 ms |
| `Thread.Sleep`, sırayla | 3000 ms | 3035 ms |
| `Thread.Sleep`, birlikte | 1000 ms | 3025 ms |

### Düşünün Sorularının Cevapları

1. **`Thread.Sleep` ile `WhenAll` neden artık bir işe yaramadı?**
   `Task.WhenAll`, arka plana devredilmiş asenkron görevlerin tamamlanmasını koordine eder. Ancak `Thread.Sleep(ms)` çağrıldığı thread'i doğrudan kilitler ve dondurur. Metot asenkron olarak arka plana bırakılamadığı için tüm çağrılar aynı thread üzerinde sırayla koşmak zorunda kaldı; bu nedenle `WhenAll` kullanılmasına rağmen süre yaklaşık 3000 ms (3025 ms) sürdü.

2. **Üçüncü çağrı ilk ikisinin sonucuna ihtiyaç duysaydı yine WhenAll kullanabilir miydiniz?**
   Üçünü birden aynı `WhenAll` bloğu içerisine veremezdik. İlk iki bağımsız çağrı `await Task.WhenAll(kullaniciTask, siparislerTask)` şeklinde paralel başlatılıp sonuçları beklenmeli, ardından elde edilen bu veriler üçüncü çağrıya parametre olarak geçilerek `await SahteDbCagrisi(...)` şeklinde sıralı koşturulmalıdır.

3. **Bir web sunucusunda, bir isteği beklerken thread'i bloklamak neden kötü?**
   Web sunucularında gelen HTTP isteklerini karşılayan sınırlı sayıda iş parçacığı (Thread Pool) bulunur. Bir veritabanı sorgusu veya harici API cevabı beklenirken thread `Thread.Sleep` ile bloklanırsa, CPU hiçbir hesaplama yapmadığı halde thread rehin tutulur. Trafik arttığında thread havuzu tükenir (**Thread Starvation**) ve sunucu yeni gelen istekleri işleyemez hale gelir. `async/await` mimarisinde ise I/O işlemi sürerken thread anında havuza geri döner ve başka kullanıcıların isteklerine yanıt verebilir.
