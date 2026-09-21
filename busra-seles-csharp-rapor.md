# C# Staj Görevi - 1. Gün Raporu

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