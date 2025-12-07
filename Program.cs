using System;
using System.IO;

internal class Program
{
    // global değişkenler atadık main dışında olduğumuz için
    // Her yerden ulaşabilelim diye buraya koyduk
    static char[,] board = new char[8, 8];

    // oyun modları için 1 yazı modu 2 imleç modu 3 demo mod 1 ve 2 play modun içinde zaten 
    static int oyunModu = 1;

    // KareSec fonksiyonu geriye değer döndüremiyor diye bunları global yaptık
    // Seçilen yerin koordinatları burada duracak
    static int secilenSatirGlobal = 0;
    static int secilenSutunGlobal = 0;

    // Demo modu için gerekli değişkenler atadık
    static string[] demoHamleler;
    static int demoSira = 0;

    // Rok Bayrakları eğer mesela beyazsahoynandı true dönerse artık beyaz şah rok atamayacak demek
    static bool beyazSahOynadi = false;
    static bool siyahSahOynadi = false;
    static bool beyazKaleSolOynadi = false;
    static bool beyazKaleSagOynadi = false;
    static bool siyahKaleSolOynadi = false;
    static bool siyahKaleSagOynadi = false;

    // En Passant için
    static int enPassantSutun = -1;

    // Notasyon (sağda çıkan tahta hamleleri e4 e5 gibi)
    static string[] notasyonListesi = new string[200];
    static int notasyonSayisi = 0;

    // ŞAH KONTROL FONKSİYONU
    static bool SahTehditAltindaMi(bool beyazSahMi)
    {
        // 1. Şahın yerini buluyoruz bütün tabloyu tarayarak
        int kR = -1;
        int kC = -1;
        char arananSah = ' ';

        if (beyazSahMi == true)
        {
            arananSah = 'K';
        }
        else
        {
            arananSah = 'k';
        }

        for (int i = 0; i < 8; i++) // tüm tahta taranır
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == arananSah)
                {
                    kR = i;
                    kC = j;
                    break;
                }
            }
        }

        if (kR == -1)
        {
            return false;
        }

        // 2. Tehditleri Kontrol Ettiriyoruz

        // A) PİYON TEHDİDİ
        // Beyaz şahsa, yukarıdaki siyah piyonlara bakıyoruz çünkü tabloda beyaz şahımız aşağıdaki (0) satırda
        // Siyah şahsa, aşağıdaki beyaz piyonlara bakıyoruz aynı şekilde
        if (beyazSahMi == true)
        {
            if (kR > 0 && kC > 0)
            {
                if (board[kR - 1, kC - 1] == 'p')
                {
                    return true;
                }
            }
            if (kR > 0 && kC < 7)
            {
                if (board[kR - 1, kC + 1] == 'p')
                {
                    return true;
                }
            }
        }
        else
        {
            if (kR < 7 && kC > 0)
            {
                if (board[kR + 1, kC - 1] == 'P')
                {
                    return true;
                }
            }
            if (kR < 7 && kC < 7)
            {
                if (board[kR + 1, kC + 1] == 'P')
                {
                    return true;
                }
            }
        }

        // B) AT TEHDİDİ (8 L noktası) burada yaptığımız şey şahı at gibi düşünerek etrafında gidebildiği karelerde at var mı diye bakmak şah çekildi mi at tarafından sorduğumuz soru bu
        char dusmanAt = ' ';
        if (beyazSahMi == true)
        {
            dusmanAt = 'n';
        }
        else
        {
            dusmanAt = 'N';
        }

        int[] atR = { -2, -2, -1, -1, 1, 1, 2, 2 };
        int[] atC = { -1, 1, -2, 2, -2, 2, -1, 1 };

        for (int i = 0; i < 8; i++)
        {
            int tR = kR + atR[i];
            int tC = kC + atC[i];
            if (tR >= 0 && tR < 8 && tC >= 0 && tC < 8)
            {
                if (board[tR, tC] == dusmanAt)
                {
                    return true;
                }
            }
        }

        // C) KALE VE VEZİR TEHDİDİ (Yatay ve Dikey) burada yatay ve dikey bakıyoruz aslında kale için zaten bunu yapacaktık vezir de aynı yönlere baktığı için(ayrıca çapraz da bakar onu da bundan sonra yapacağız) aradan çıkartmak amacıyla veziri de kaleyle kullandık
        char dusmanKale = ' ';
        char dusmanVezir = ' ';
        if (beyazSahMi == true)
        {
            dusmanKale = 'r';
            dusmanVezir = 'q';
        }
        else
        {
            dusmanKale = 'R';
            dusmanVezir = 'Q';
        }

        int[] yonR = { -1, 1, 0, 0 };
        int[] yonC = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int tR = kR;
            int tC = kC;
            while (true)
            {
                tR += yonR[i];
                tC += yonC[i];

                if (tR < 0 || tR > 7 || tC < 0 || tC > 7)
                {
                    break;
                }

                char tas = board[tR, tC];
                if (tas == '\0' || tas == '.')
                {
                    continue;
                }

                if (tas == dusmanKale || tas == dusmanVezir)
                {
                    return true;
                }

                break;
            }
        }

        // D) FİL VE VEZİR TEHDİDİ (Çaprazlar) burada da vezirle fil kullanıyoruz ve vezir de bitmiş oluyor kale ile filin hareketlerinin birleşimi vezir ediyor hem filde hem kalede vezir için ayrı ayrı baktık hareketlere
        char dusmanFil = ' ';
        if (beyazSahMi == true)
        {
            dusmanFil = 'b';
        }
        else
        {
            dusmanFil = 'B';
        }

        int[] capR = { -1, -1, 1, 1 };
        int[] capC = { -1, 1, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int tR = kR;
            int tC = kC;
            while (true)
            {
                tR += capR[i];
                tC += capC[i];

                if (tR < 0 || tR > 7 || tC < 0 || tC > 7)
                {
                    break;
                }

                char tas = board[tR, tC];
                if (tas == '\0' || tas == '.')
                {
                    continue;
                }

                if (tas == dusmanFil || tas == dusmanVezir)
                {
                    return true;
                }

                break;
            }
        }

        // E) DÜŞMAN ŞAH (Yan yana gelemezler)
        char dusmanSah = ' ';
        if (beyazSahMi == true)
        {
            dusmanSah = 'k';
        }
        else
        {
            dusmanSah = 'K';
        }

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int tR = kR + i;
                int tC = kC + j;
                if (tR >= 0 && tR < 8 && tC >= 0 && tC < 8)
                {
                    if (board[tR, tC] == dusmanSah)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    //      KAYDETME (s tuşuna bakmak burada file operations konusundan streamwriter kullandık yani txt dosyası oluşturuyor bizim için oyunun klasörüne ve hamlelerimizi sanki demo modda yapacağımız gibi yazı şeklinde(e4 e5) gibi yazıyor
    static void OyunuKaydet(bool siraBeyaz)
    {
        // Yazma işlemi için StreamWriter kullanıyoruz (fileoperations konusundan)
        StreamWriter yazici = new StreamWriter("kayit.txt");
        yazici.WriteLine(siraBeyaz);
        yazici.WriteLine(beyazSahOynadi);
        yazici.WriteLine(siyahSahOynadi);
        yazici.WriteLine(beyazKaleSolOynadi);
        yazici.WriteLine(beyazKaleSagOynadi);
        yazici.WriteLine(siyahKaleSolOynadi);
        yazici.WriteLine(siyahKaleSagOynadi);
        yazici.WriteLine(enPassantSutun);

        // Tahtayı satır satır yazalım
        for (int i = 0; i < 8; i++)
        {
            string satir = "";
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == '\0')
                {
                    satir = satir + "."; // Boş yerlere nokta koyalım
                }
                else
                {
                    satir = satir + board[i, j];
                }
            }
            yazici.WriteLine(satir);
        }
        yazici.Close();
        Console.WriteLine("Oyun 'kayit.txt' dosyasına kaydedildi!");
    }

    //     YÜKLEME (l harfi ile load yapıyoruz kayıt txt dosyamızı yürürlüğe sokuyoruz da denilebilir)
    static bool OyunuYukle()
    {
        if (File.Exists("kayit.txt") == false)
        {
            Console.WriteLine("Kayıtlı oyun bulunamadı!");
            return true;
        }

        string[] satirlar = File.ReadAllLines("kayit.txt");

        // 1. Sıra bilgisi
        bool yuklenenSira = true;
        if (satirlar[0] == "True")
        {
            yuklenenSira = true;
        }
        else
        {
            yuklenenSira = false;
        }

        // 2. Rok ve En Passant bilgileri ( burada kontrol ediyoruz oynanan beyaz şah siyah şah kale var mı bakılıyor çünkü satranç kuralları gereği rok oynanmayan şah ve kale üzerinde gerçekleşebilir)
        if (satirlar[1] == "True")
        {
            beyazSahOynadi = true;
        }
        else
        {
            beyazSahOynadi = false;
        }

        if (satirlar[2] == "True")
        {
            siyahSahOynadi = true;
        }
        else
        {
            siyahSahOynadi = false;
        }

        if (satirlar[3] == "True")
        {
            beyazKaleSolOynadi = true;
        }
        else
        {
            beyazKaleSolOynadi = false;
        }

        if (satirlar[4] == "True")
        {
            beyazKaleSagOynadi = true;
        }
        else
        {
            beyazKaleSagOynadi = false;
        }

        if (satirlar[5] == "True")
        {
            siyahKaleSolOynadi = true;
        }
        else
        {
            siyahKaleSolOynadi = false;
        }

        if (satirlar[6] == "True")
        {
            siyahKaleSagOynadi = true;
        }
        else
        {
            siyahKaleSagOynadi = false;
        }

        enPassantSutun = Convert.ToInt32(satirlar[7]);

        // 3. Tahtayı dolduruyoruz
        int dosyaSatirNo = 8;
        for (int i = 0; i < 8; i++)
        {
            string okunanSatir = satirlar[dosyaSatirNo];
            for (int j = 0; j < 8; j++)
            {
                char karakter = okunanSatir[j];
                if (karakter == '.')
                {
                    board[i, j] = '\0';
                }
                else
                {
                    board[i, j] = karakter;
                }
            }
            dosyaSatirNo++;
        }
        Console.WriteLine("Oyun yüklendi!");
        return yuklenenSira;
    }

    static void SetupBoard()
    {
        // piyonlar
        for (int i = 0; i < 8; i++)
        {
            board[6, i] = 'P'; // beyaz piyon
            board[1, i] = 'p'; // siyah piyon
        }

        // beyaz taşlar(piyon hariç)
        board[7, 0] = 'R';
        board[7, 1] = 'N';
        board[7, 2] = 'B';
        board[7, 3] = 'Q';
        board[7, 4] = 'K';
        board[7, 5] = 'B';
        board[7, 6] = 'N';
        board[7, 7] = 'R';
        // Siyah taşlar(piyon hariç)
        board[0, 0] = 'r';
        board[0, 1] = 'n';
        board[0, 2] = 'b';
        board[0, 3] = 'q';
        board[0, 4] = 'k';
        board[0, 5] = 'b';
        board[0, 6] = 'n';
        board[0, 7] = 'r';
    }

    static void PrintBoard(int cursorR, int cursorC, int selectedR, int selectedC)
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("   a b c d e f g h      --- HAMLE GEÇMİŞİ ---"); // Koordinatları üste aldım hizası bozulmasın diye
        Console.WriteLine();

        for (int r = 0; r < 8; r++)
        {
            Console.Write((8 - r) + "  "); //8'den başlayıp aşağıya doğru gidecek

            for (int c = 0; c < 8; c++)
            {
                // Renklendirme mantığı (İmleç Mavi, Seçili Kırmızı)
                if (r == cursorR && c == cursorC)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (r == selectedR && c == selectedC)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ResetColor(); // sıfırladık
                }

                if (board[r, c] == '\0') // eğer kare boşsa kontrolü
                {
                    Console.Write(". "); // boşsa . koy
                }
                else
                {
                    Console.Write(board[r, c] + " "); // else yani boş değilse değerini koy ve boşluk bırak
                }
                Console.ResetColor(); // Rengi sıfırla
            }

            // --- SAĞ TARAFA NOTASYON ---
            Console.Write("     ");
            int beyazIndex = r * 2;
            int siyahIndex = r * 2 + 1;

            if (beyazIndex < notasyonSayisi)
            {
                Console.Write((r + 1) + ". " + notasyonListesi[beyazIndex]);
                if (siyahIndex < notasyonSayisi)
                {
                    Console.Write("   " + notasyonListesi[siyahIndex]);
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    static void KareSec(int oncekiR, int oncekiC) //cursorr movement yapıyoruz(yön tuşları hareketi mod 2 için)
    {
        int x = 0; int y = 0;
        if (oncekiR != -1)
        {
            y = oncekiR; x = oncekiC;
        }
        while (true)
        {
            PrintBoard(y, x, oncekiR, oncekiC);
            Console.WriteLine("Ok tuşlarıyla gezin, ENTER ile seçin.");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                if (y > 0) y--;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                if (y < 7) y++;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                if (x > 0) x--;
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                if (x < 7) x++;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                secilenSatirGlobal = y;
                secilenSutunGlobal = x;
                break;
            }
        }
    }

    static void Move()// en uzun fonksiyonumuz neredeyse bütün satranç kurallarını içinde barındıran fonksiyon 
    {
        // Sıra takibi için değişken tutucam (True Beyaz False Siyah)kod bitince sil kendime not
        bool siraBeyaz = true;

        while (true)
        {
            // Sıranın kimde olduğunu gösterelim
            if (siraBeyaz)
            {
                Console.WriteLine("\n--- SIRA BEYAZDA (Büyük Harfler) ---");
            }
            else
            {
                Console.WriteLine("\n--- SIRA SİYAHDA (Küçük Harfler) ---");
            }

            int sutun1 = 0, c1 = 0, sutun2 = 0, c2 = 0;
            int r1 = 0, r2 = 0;
            int yeniEnPassantSutun = -1;

            if (oyunModu == 1)
            {
                Console.Write("Oynayacağınız taş sütun (veya 's' save, 'l' load): ");
                string inputF1 = Console.ReadLine();

                if (inputF1 == "s")
                {
                    OyunuKaydet(siraBeyaz);
                    continue;
                }
                if (inputF1 == "l")
                {
                    siraBeyaz = OyunuYukle();
                    PrintBoard(-1, -1, -1, -1);
                    continue;
                }
                if (inputF1 == "hint" || inputF1 == "ipucu") // burada h yerine hint ya da ipucu ile tuttuk öbür türlü h ile tuttsak bu sefer h sütununa ulaşamıyorduk bu şekilde bir çözüm ürettik
                {
                    IpucuVer(siraBeyaz);
                    Console.WriteLine("Devam etmek için tuşa basın...");
                    Console.ReadKey();
                    PrintBoard(-1, -1, -1, -1);
                    continue;
                }

                // boş basarsa hata vermesin başa dönsün
                if (inputF1 == "")
                {
                    continue;
                }
                char f1 = inputF1[0];
                // mod 1 için soru soruyoruz
                Console.Write("Oynayacağınız taş hangi satırda: "); 
                r1 = Convert.ToInt32(Console.ReadLine());

                Console.Write("Hangi sütuna oynanacak: ");
                string inputF2 = Console.ReadLine();
                if (inputF2 == "") continue;
                char f2 = inputF2[0];

                Console.Write("Hangi satıra oynanacak: ");
                r2 = Convert.ToInt32(Console.ReadLine());

                // Biraz uğraştırıcı ve uzun oldu tek tek tutmayı seçtiğim için , sütunda sorulan ve string değer alınan inputu sayıya çevirdik böyle daha rahat anlayabiliyoruz
                if (f1 == 'a')
                {
                    c1 = 0;
                }
                else if (f1 == 'b')
                {
                    c1 = 1;
                }
                else if (f1 == 'c')
                {
                    c1 = 2;
                }
                else if (f1 == 'd')
                {
                    c1 = 3;
                }
                else if (f1 == 'e')
                {
                    c1 = 4;
                }
                else if (f1 == 'f')
                {
                    c1 = 5;
                }
                else if (f1 == 'g')
                {
                    c1 = 6;
                }
                else if (f1 == 'h')
                {
                    c1 = 7;
                }

                if (f2 == 'a')
                {
                    c2 = 0;
                }
                else if (f2 == 'b')
                {
                    c2 = 1;
                }
                else if (f2 == 'c')
                {
                    c2 = 2;
                }
                else if (f2 == 'd')
                {
                    c2 = 3;
                }
                else if (f2 == 'e')
                {
                    c2 = 4;
                }
                else if (f2 == 'f')
                {
                    c2 = 5;
                }
                else if (f2 == 'g')
                {
                    c2 = 6;
                }
                else if (f2 == 'h')
                {
                    c2 = 7;
                }

                // burada satıra çevirdim
                sutun1 = 8 - r1;
                sutun2 = 8 - r2;
            }
            else if (oyunModu == 2) // oyunmodu 2 için karesec fonksiyonu çalışır imleç hareketi
            {
                KareSec(-1, -1); sutun1 = secilenSatirGlobal; c1 = secilenSutunGlobal; r1 = 8 - sutun1;
                KareSec(sutun1, c1); sutun2 = secilenSatirGlobal; c2 = secilenSutunGlobal; r2 = 8 - sutun2;
            }
            else if (oyunModu == 3) // File Operations: Kayıtlı oyun.txt dosyasından hamleler oynatılır
            {
                Console.WriteLine("Hamle için SPACE (Boşluk) tuşuna basın...");

                // Önce tuşu okuyoruz
                ConsoleKeyInfo tus = Console.ReadKey(true);

                // Space olmadığı sürece yeni tuş iste (Döngü burada döner)
                while (tus.Key != ConsoleKey.Spacebar)
                {
                    tus = Console.ReadKey(true);
                }

                if (demoSira >= demoHamleler.Length)
                {
                    Console.WriteLine("Demo bitti.");
                    break;
                }

                string satir = demoHamleler[demoSira];
                Console.WriteLine("Oynanan: " + satir);
                demoSira++;

                char f1 = satir[0];
                string sR1 = satir[1].ToString();
                r1 = Convert.ToInt32(sR1);

                char f2 = satir[3];
                string sR2 = satir[4].ToString();
                r2 = Convert.ToInt32(sR2);

                if (f1 == 'a')
                {
                    c1 = 0;
                }
                else if (f1 == 'b')
                {
                    c1 = 1;
                }
                else if (f1 == 'c')
                {
                    c1 = 2;
                }
                else if (f1 == 'd')
                {
                    c1 = 3;
                }
                else if (f1 == 'e')
                {
                    c1 = 4;
                }
                else if (f1 == 'f')
                {
                    c1 = 5;
                }
                else if (f1 == 'g')
                {
                    c1 = 6;
                }
                else if (f1 == 'h')
                {
                    c1 = 7;
                }

                if (f2 == 'a')
                {
                    c2 = 0;
                }
                else if (f2 == 'b')
                {
                    c2 = 1;
                }
                else if (f2 == 'c')
                {
                    c2 = 2;
                }
                else if (f2 == 'd')
                {
                    c2 = 3;
                }
                else if (f2 == 'e')
                {
                    c2 = 4;
                }
                else if (f2 == 'f')
                {
                    c2 = 5;
                }
                else if (f2 == 'g')
                {
                    c2 = 6;
                }
                else if (f2 == 'h')
                {
                    c2 = 7;
                }

                sutun1 = 8 - r1;
                sutun2 = 8 - r2;
            }

            // Seçilen taşı değişkene alalım(kontrol edicez büyük harf mi küçük harf mi)
            char secilenTas = board[sutun1, c1];

            // seçilen yer boş mu diye kontrol ediyorum
            if (secilenTas == '\0' || secilenTas == ' ' || secilenTas == '.')
            {
                Console.WriteLine("HATA: Seçtiğiniz kare boş! Lütfen tekrar deneyin.");
                if (oyunModu == 2) { Console.ReadKey(); }
                continue;
            }

            // Sıra beyazda mı siyahda mı kontrol için büyük küçük harfleri kullandım büyükler beyaz
            bool tasBeyazMi = char.IsUpper(secilenTas);
            if (siraBeyaz == true && tasBeyazMi == false)
            {
                Console.WriteLine("Sıra beyazda ama siz Siyah taş seçtiniz.(beyaz taş seçin)");
                if (oyunModu == 2) { Console.ReadKey(); }
                continue;
            }
            if (siraBeyaz == false && tasBeyazMi == true)
            {
                Console.WriteLine("HATA: Sıra siyahda ama siz beyaz taş seçtiniz.(siyah taş seçin)");
                if (oyunModu == 2) { Console.ReadKey(); }
                continue;
            }

            //     NOTASYON HAZIRLIĞI 
            string hamleNotasyonu = "";
            bool ozelNotasyonVar = false;

            //                    Piyon hareketleri kontrolleri 

            // Piyon (P)
            if (secilenTas == 'P')
            {
                if (c1 == c2)
                {
                    // 1. Durum Sadece 1 kare ileri gitmek istiyor(önü dolu mu bakalım)
                    if (r2 == r1 + 1)
                    {
                        // Önü dolu mu bakıyorum
                        if (board[sutun2, c2] != '\0' && board[sutun2, c2] != '.')
                        {
                            Console.WriteLine("HATA: Piyonun önü dolu!");
                            if (oyunModu == 2) { Console.ReadKey(); }
                            continue;
                        }
                    }
                    // 2. Durum: Başlangıçta 2 kare ileri gitmek istiyor(2 önü de boş mu bakalım)
                    else if (r1 == 2 && r2 == 4)
                    {
                        if (board[sutun2, c2] != '\0' || board[sutun1 - 1, c1] != '\0')
                        {
                            Console.WriteLine("HATA: Piyonun yolu kapalı, 2 kare atlayamaz!");
                            if (oyunModu == 2) { Console.ReadKey(); }
                            continue;
                        }
                        yeniEnPassantSutun = c1;
                    }
                    else
                    {
                        Console.WriteLine("HATA: Beyaz Piyon bu şekilde hareket edemez.");
                        if (oyunModu == 2) { Console.ReadKey(); }
                        continue;
                    }
                }
                else
                {
                    if (r2 == r1 + 1 && Math.Abs(c1 - c2) == 1)
                    {
                        if (board[sutun2, c2] != '\0' && board[sutun2, c2] != '.')
                        {
                            // Yeme işlemi 
                        }
                        else if (board[sutun2, c2] == '\0' && c2 == enPassantSutun && r1 == 5)
                        {
                            // En Passant
                            board[sutun1, c2] = '\0';
                            Console.WriteLine("En Passant!");
                            hamleNotasyonu = "ex" + (char)('a' + c2) + (r2);
                            ozelNotasyonVar = true;
                        }
                        else
                        {
                            Console.WriteLine("HATA: Piyon çapraz sadece taş yemek için gidebilir!");
                            if (oyunModu == 2) { Console.ReadKey(); }
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("HATA: Piyon sadece ileri düz veya çapraz gidebilir.");
                        if (oyunModu == 2) { Console.ReadKey(); }
                        continue;
                    }
                }
            }
            // Siyah Piyon (p)
            else if (secilenTas == 'p')
            {
                if (c1 == c2)
                {
                    if (r2 == r1 - 1)
                    {
                        if (board[sutun2, c2] != '\0' && board[sutun2, c2] != '.')
                        {
                            Console.WriteLine("HATA: Piyonun önü dolu!");
                            if (oyunModu == 2) { Console.ReadKey(); }
                            continue;
                        }
                    }
                    else if (r1 == 7 && r2 == 5)
                    {
                        if (board[sutun2, c2] != '\0' || board[sutun1 + 1, c1] != '\0')
                        {
                            Console.WriteLine("HATA: Piyonun yolu kapalı!");
                            if (oyunModu == 2) { Console.ReadKey(); }
                            continue;
                        }
                        yeniEnPassantSutun = c1;
                    }
                    else
                    {
                        Console.WriteLine("HATA: Siyah Piyon bu şekilde hareket edemez.");
                        if (oyunModu == 2) { Console.ReadKey(); }
                        continue;
                    }
                }
                else
                {
                    if (r2 == r1 - 1 && Math.Abs(c1 - c2) == 1)
                    {
                        if (board[sutun2, c2] != '\0' && board[sutun2, c2] != '.')
                        {
                            // Yeme işlemi
                        }
                        else if (board[sutun2, c2] == '\0' && c2 == enPassantSutun && r1 == 4)
                        {
                            board[sutun1, c2] = '\0';
                            Console.WriteLine("En Passant!");
                            hamleNotasyonu = (char)('a' + c1) + "x" + (char)('a' + c2) + (r2);
                            ozelNotasyonVar = true;
                        }
                        else
                        {
                            Console.WriteLine("HATA: Piyon çapraz sadece taş yemek için gidebilir!");
                            if (oyunModu == 2) { Console.ReadKey(); }
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("HATA: Siyah piyon hatalı hareket etti.");
                        if (oyunModu == 2) { Console.ReadKey(); }
                        continue;
                    }
                }
            }
            // At (Knight)
            else if (secilenTas == 'N' || secilenTas == 'n')
            {
                int fark1 = Math.Abs(r1 - r2);
                int fark2 = Math.Abs(c1 - c2);
                if (fark1 * fark2 != 2)
                {
                    Console.WriteLine("HATA: At L çizer");
                    if (oyunModu == 2) { Console.ReadKey(); }
                    continue;
                }
            }
            // Şah ve Rok
            else if (secilenTas == 'K' || secilenTas == 'k')
            {
                int deltaRow = Math.Abs(r1 - r2);
                int deltaCol = Math.Abs(c1 - c2);

                if (deltaRow <= 1 && deltaCol <= 1)
                {
                    // ROK
                }
                else if (deltaRow == 0 && deltaCol == 2)
                {
                    bool rokBasarili = false;
                    if (secilenTas == 'K')
                    {
                        if (beyazSahOynadi == true)
                        {
                            Console.WriteLine("HATA: Şah oynadı!");
                            continue;
                        }

                        if (c2 > c1)
                        {
                            if (beyazKaleSagOynadi == true || board[7, 5] != '\0' || board[7, 6] != '\0')
                            {
                                Console.WriteLine("HATA: Rok olmaz!");
                                continue;
                            }
                            board[7, 5] = 'R';
                            board[7, 7] = '\0';
                            rokBasarili = true;
                            hamleNotasyonu = "0-0";
                            ozelNotasyonVar = true;
                        }
                        else
                        {
                            if (beyazKaleSolOynadi == true || board[7, 1] != '\0' || board[7, 2] != '\0' || board[7, 3] != '\0')
                            {
                                Console.WriteLine("HATA: Rok olmaz!");
                                continue;
                            }
                            board[7, 3] = 'R';
                            board[7, 0] = '\0';
                            rokBasarili = true;
                            hamleNotasyonu = "0-0-0";
                            ozelNotasyonVar = true;
                        }
                    }
                    else
                    {
                        if (siyahSahOynadi == true)
                        {
                            Console.WriteLine("HATA: Şah oynadı!");
                            continue;
                        }

                        if (c2 > c1)
                        {
                            if (siyahKaleSagOynadi == true || board[0, 5] != '\0' || board[0, 6] != '\0')
                            {
                                Console.WriteLine("HATA: Rok olmaz!");
                                continue;
                            }
                            board[0, 5] = 'r';
                            board[0, 7] = '\0';
                            rokBasarili = true;
                            hamleNotasyonu = "0-0";
                            ozelNotasyonVar = true;
                        }
                        else
                        {
                            if (siyahKaleSolOynadi == true || board[0, 1] != '\0' || board[0, 2] != '\0' || board[0, 3] != '\0')
                            {
                                Console.WriteLine("HATA: Rok olmaz!");
                                continue;
                            }
                            board[0, 3] = 'r';
                            board[0, 0] = '\0';
                            rokBasarili = true;
                            hamleNotasyonu = "0-0-0";
                            ozelNotasyonVar = true;
                        }
                    }
                    if (rokBasarili == false)
                    {
                        if (oyunModu == 2) { Console.ReadKey(); }
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("HATA: Şah hareketi yanlış");
                    if (oyunModu == 2) { Console.ReadKey(); }
                    continue;
                }
            }
            // Kale
            else if (secilenTas == 'R' || secilenTas == 'r')
            {
                if (r1 != r2 && c1 != c2)
                {
                    Console.WriteLine("HATA: Kale düz gider");
                    if (oyunModu == 2) { Console.ReadKey(); }
                    continue;
                }

                bool yol = false;
                if (r1 == r2)
                {
                    int k1 = Math.Min(c1, c2) + 1;
                    int k2 = Math.Max(c1, c2);
                    for (int k = k1; k < k2; k++)
                    {
                        if (board[sutun1, k] != '\0')
                        {
                            yol = true;
                        }
                    }
                }
                else
                {
                    int k1 = Math.Min(sutun1, sutun2) + 1;
                    int k2 = Math.Max(sutun1, sutun2);
                    for (int k = k1; k < k2; k++)
                    {
                        if (board[k, c1] != '\0')
                        {
                            yol = true;
                        }
                    }
                }

                if (yol == true)
                {
                    Console.WriteLine("HATA: Yol kapalı");
                    if (oyunModu == 2) { Console.ReadKey(); }
                    continue;
                }
            }
            // Fil
            else if (secilenTas == 'B' || secilenTas == 'b')
            {
                if (Math.Abs(sutun1 - sutun2) != Math.Abs(c1 - c2))
                {
                    Console.WriteLine("HATA: Fil çapraz gider");
                    if (oyunModu == 2) { Console.ReadKey(); }
                    continue;
                }

                int rs = 0;
                if (sutun2 > sutun1)
                {
                    rs = 1;
                }
                else
                {
                    rs = -1;
                }

                int cs = 0;
                if (c2 > c1)
                {
                    cs = 1;
                }
                else
                {
                    cs = -1;
                }

                int cr = sutun1 + rs;
                int cc = c1 + cs;
                bool yol = false;

                while (cr != sutun2)
                {
                    if (board[cr, cc] != '\0')
                    {
                        yol = true;
                    }
                    cr += rs; cc += cs;
                }

                if (yol == true)
                {
                    Console.WriteLine("HATA: Yol kapalı");
                    if (oyunModu == 2) { Console.ReadKey(); }
                    continue;
                }
            }
            // Vezir
            else if (secilenTas == 'Q' || secilenTas == 'q')
            {
                bool duz = (r1 == r2 || c1 == c2);
                bool cap = (Math.Abs(sutun1 - sutun2) == Math.Abs(c1 - c2));

                if (!duz && !cap)
                {
                    Console.WriteLine("HATA: Vezir düz veya çapraz");
                    if (oyunModu == 2) { Console.ReadKey(); }
                    continue;
                }

                bool yol = false;
                if (duz)
                {
                    if (r1 == r2)
                    {
                        int k1 = Math.Min(c1, c2) + 1;
                        int k2 = Math.Max(c1, c2);
                        for (int k = k1; k < k2; k++)
                        {
                            if (board[sutun1, k] != '\0')
                            {
                                yol = true;
                            }
                        }
                    }
                    else
                    {
                        int k1 = Math.Min(sutun1, sutun2) + 1;
                        int k2 = Math.Max(sutun1, sutun2);
                        for (int k = k1; k < k2; k++)
                        {
                            if (board[k, c1] != '\0')
                            {
                                yol = true;
                            }
                        }
                    }
                }
                else
                {
                    int rs = 0;
                    if (sutun2 > sutun1)
                    {
                        rs = 1;
                    }
                    else
                    {
                        rs = -1;
                    }

                    int cs = 0;
                    if (c2 > c1)
                    {
                        cs = 1;
                    }
                    else
                    {
                        cs = -1;
                    }

                    int cr = sutun1 + rs;
                    int cc = c1 + cs;

                    while (cr != sutun2)
                    {
                        if (board[cr, cc] != '\0')
                        {
                            yol = true;
                        }
                        cr += rs; cc += cs;
                    }
                }
                if (yol)
                {
                    Console.WriteLine("HATA: Yol kapalı");
                    if (oyunModu == 2) { Console.ReadKey(); }
                    continue;
                }
            }

            char hedef = board[sutun2, c2];
            if (hedef != '\0' && char.IsUpper(hedef) == char.IsUpper(secilenTas))
            {
                Console.WriteLine("HATA: Kendi taşını yiyemezsin");
                if (oyunModu == 2) { Console.ReadKey(); }
                continue;
            }

            //     NOTASYON KAYIT
            if (ozelNotasyonVar == false)
            {
                string parca = "";
                if (secilenTas == 'P' || secilenTas == 'p')
                {
                    if (hedef != '\0' || c1 != c2)
                    {
                        parca = ((char)('a' + c1)).ToString();
                    }
                }
                else
                {
                    parca = char.ToUpper(secilenTas).ToString();
                }

                string xIsareti = "";
                if (hedef != '\0' && hedef != '.')
                {
                    xIsareti = "x";
                }

                hamleNotasyonu = parca + xIsareti + ((char)('a' + c2)) + r2;
            }
            notasyonListesi[notasyonSayisi] = hamleNotasyonu;
            notasyonSayisi++;

            //     HAMLE UYGULA
            board[sutun2, c2] = board[sutun1, c1];
            board[sutun1, c1] = '\0';

            //     ŞAH KONTROLÜ
            if (SahTehditAltindaMi(siraBeyaz) == true)
            {
                Console.WriteLine("HATA: Şahınız tehdit altında! Bu hamleyi yapamazsınız.");
                // Geri alma
                board[sutun1, c1] = secilenTas;
                board[sutun2, c2] = hedef;

                if (oyunModu == 2)
                {
                    Console.ReadKey();
                }
                notasyonSayisi--;
                continue;
            }

            //     BAYRAKLARI GÜNCELLE
            if (secilenTas == 'K')
            {
                beyazSahOynadi = true;
            }
            if (secilenTas == 'k')
            {
                siyahSahOynadi = true;
            }
            if (secilenTas == 'R')
            {
                if (sutun1 == 7 && c1 == 0)
                {
                    beyazKaleSolOynadi = true;
                }
                if (sutun1 == 7 && c1 == 7)
                {
                    beyazKaleSagOynadi = true;
                }
            }
            if (secilenTas == 'r')
            {
                if (sutun1 == 0 && c1 == 0)
                {
                    siyahKaleSolOynadi = true;
                }
                if (sutun1 == 0 && c1 == 7)
                {
                    siyahKaleSagOynadi = true;
                }
            }

            enPassantSutun = yeniEnPassantSutun;

            //     TERFİ DURUMU
            if (board[sutun2, c2] == 'P' && sutun2 == 0)
            {
                if (oyunModu == 3)
                {
                    board[sutun2, c2] = 'Q';
                }
                else
                {
                    Console.WriteLine("Terfi: Q, R, B, N");
                    string c = Console.ReadLine();
                    if (c == "R")
                    {
                        board[sutun2, c2] = 'R';
                    }
                    else if (c == "B")
                    {
                        board[sutun2, c2] = 'B';
                    }
                    else if (c == "N")
                    {
                        board[sutun2, c2] = 'N';
                    }
                    else
                    {
                        board[sutun2, c2] = 'Q';
                    }
                }
            }
            if (board[sutun2, c2] == 'p' && sutun2 == 7)
            {
                if (oyunModu == 3)
                {
                    board[sutun2, c2] = 'q';
                }
                else
                {
                    Console.WriteLine("Terfi: q, r, b, n");
                    string c = Console.ReadLine();
                    if (c == "r")
                    {
                        board[sutun2, c2] = 'r';
                    }
                    else if (c == "b")
                    {
                        board[sutun2, c2] = 'b';
                    }
                    else if (c == "n")
                    {
                        board[sutun2, c2] = 'n';
                    }
                    else
                    {
                        board[sutun2, c2] = 'q';
                    }
                }
            }

            //      MAT KONTROLÜ UYARISI
            bool digerSira = false;
            if (siraBeyaz == true)
            {
                digerSira = false;
            }
            else
            {
                digerSira = true;
            }

            if (SahTehditAltindaMi(digerSira) == true)
            {
                Console.WriteLine("DİKKAT: ŞAH ÇEKİLDİ!");
                Console.WriteLine("Devam etmek için bir tuşa basın..."); // Kullanıcı görsün diye mesaj
                Console.ReadKey(); // Bekleyecek tuşa basınca devam edecek
            }

            PrintBoard(-1, -1, -1, -1);
            siraBeyaz = !siraBeyaz;
        }
    }

    static void Main(string[] args) //Mainimiz
    {
        Console.WriteLine("DEU-EE CHESS 2025");
        Console.WriteLine("1. Yazı Modu");
        Console.WriteLine("2. İmleç Modu");
        Console.WriteLine("3. Demo Modu");
        Console.Write("Seçiminiz: ");
        string secim = Console.ReadLine();

        if (secim == "2")
        {
            oyunModu = 2;
        }
        else if (secim == "3")
        {
            oyunModu = 3;
            if (File.Exists("oyun.txt"))
            {
                demoHamleler = File.ReadAllLines("oyun.txt");
            }
            else
            {
                Console.WriteLine("oyun.txt yok!");
                Console.ReadKey();
                return;
            }
        }
        else
        {
            oyunModu = 1;
        }

        SetupBoard();
        PrintBoard(-1, -1, -1, -1);
        Move();
    }
    //     İPUCU (HINT) FONKSİYONU
    static void IpucuVer(bool siraBeyaz)
    {
        Console.WriteLine("\n--- İPUÇLARI (Yeme İhtimalleri) ---");
        bool ipucuBulundu = false;

        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                char tas = board[r, c];
                if (tas == '\0')
                {
                    continue;
                }
                if (tas == '.')
                {
                    continue;
                }

                // Sadece sırası gelen oyuncunun taşlarına bakacak
                bool tasBeyaz = char.IsUpper(tas);
                if (tasBeyaz != siraBeyaz)
                {
                    continue;
                }

                //  bu taş neler yiyebilir sorusuna bakıyoruz (ipucu) hint için

                // 1. PİYON
                if (tas == 'P' || tas == 'p')
                {
                    int yon = 0;

                    if (siraBeyaz == true)
                    {
                        yon = -1;
                    }
                    else
                    {
                        yon = 1;
                    }

                    // Sağ ve Sol çapraz (-1 ve 1)
                    int[] dc = { -1, 1 };


                    for (int k = 0; k < 2; k++)
                    {
                        int d = dc[k];
                        int tr = r + yon;
                        int tc = c + d;

                        if (tr >= 0 && tr < 8 && tc >= 0 && tc < 8)
                        {
                            char hedef = board[tr, tc];
                            // Normal yeme veya En Passant
                            bool normalYeme = false;
                            if (hedef != '\0' && char.IsUpper(hedef) != siraBeyaz)
                            {
                                normalYeme = true;
                            }

                            bool enPassant = false;
                            if (hedef == '\0' && tc == enPassantSutun)
                            {
                                if (siraBeyaz && r == 3)
                                {
                                    enPassant = true;
                                }
                                if (!siraBeyaz && r == 4)
                                {
                                    enPassant = true;
                                }
                            }

                            if (normalYeme || enPassant)
                            {
                                Console.WriteLine("İpucu: Piyon (" + ((char)('a' + c)) + (8 - r) + ") yiyebilir -> " + ((char)('a' + tc)) + (8 - tr));
                                ipucuBulundu = true;
                            }
                        }
                    }
                }
                // 2. AT (KNIGHT)
                else if (tas == 'N' || tas == 'n')
                {
                    int[] dr = { -2, -2, -1, -1, 1, 1, 2, 2 };
                    int[] dc = { -1, 1, -2, 2, -2, 2, -1, 1 };
                    for (int i = 0; i < 8; i++)
                    {
                        int tr = r + dr[i];
                        int tc = c + dc[i];
                        if (tr >= 0 && tr < 8 && tc >= 0 && tc < 8)
                        {
                            char hedef = board[tr, tc];
                            if (hedef != '\0' && char.IsUpper(hedef) != siraBeyaz)
                            {
                                Console.WriteLine("İpucu: At (" + ((char)('a' + c)) + (8 - r) + ") yiyebilir -> " + ((char)('a' + tc)) + (8 - tr));
                                ipucuBulundu = true;
                            }
                        }
                    }
                }
                // 3. ŞAH (KING)
                else if (tas == 'K' || tas == 'k')
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == 0 && j == 0) continue;
                            int tr = r + i;
                            int tc = c + j;
                            if (tr >= 0 && tr < 8 && tc >= 0 && tc < 8)
                            {
                                char hedef = board[tr, tc];
                                if (hedef != '\0' && char.IsUpper(hedef) != siraBeyaz)
                                {
                                    Console.WriteLine("İpucu: Şah yiyebilir -> " + ((char)('a' + tc)) + (8 - tr));
                                    ipucuBulundu = true;
                                }
                            }
                        }
                    }
                }
                // 4. UZUN MENZİLLİLER (KALE, FİL, VEZİR)
                else
                {
                    int[] dr = new int[0];
                    int[] dc = new int[0];

                    if (tas == 'R' || tas == 'r') // Kale
                    {
                        dr = new int[] { -1, 1, 0, 0 };
                        dc = new int[] { 0, 0, -1, 1 };
                    }
                    else if (tas == 'B' || tas == 'b') // Fil
                    {
                        dr = new int[] { -1, -1, 1, 1 };
                        dc = new int[] { -1, 1, -1, 1 };
                    }
                    else // Vezir
                    {
                        dr = new int[] { -1, 1, 0, 0, -1, -1, 1, 1 };
                        dc = new int[] { 0, 0, -1, 1, -1, 1, -1, 1 };
                    }

                    for (int i = 0; i < dr.Length; i++)
                    {
                        int tr = r;
                        int tc = c;
                        while (true)
                        {
                            tr += dr[i];
                            tc += dc[i];

                            if (tr < 0 || tr > 7 || tc < 0 || tc > 7)
                            {
                                break;
                            }

                            char hedef = board[tr, tc];
                            if (hedef == '\0' || hedef == '.')
                            {
                                continue;
                            }

                            // Dolu kareye çarptık
                            if (char.IsUpper(hedef) != siraBeyaz)
                            {
                                // Düşmansa yiyebiliriz
                                Console.WriteLine("İpucu: " + tas + " (" + ((char)('a' + c)) + (8 - r) + ") yiyebilir -> " + ((char)('a' + tc)) + (8 - tr));
                                ipucuBulundu = true;
                            }
                            break; // Yol bitti
                        }
                    }
                }
            }
        }

        if (!ipucuBulundu)
        {
            Console.WriteLine("Şu an yiyebileceğiniz bir taş görünmüyor.");
        }
        Console.WriteLine("-----------------------------------");
    }
}