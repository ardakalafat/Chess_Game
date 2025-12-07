# C# Console Chess Game ♟️ [EN | TR]

---

## C# Console Chess Engine (English Version)

This project is a console-based chess engine developed using the **C# programming language** for a 1st-year "Algorithms and Programming" course. It demonstrates fundamental concepts like **global/local variables, file operations (StreamWriter/StreamReader), functions, procedures, conditional logic, and matrix manipulation (8x8 board)**.

### 🚀 Features

- **Game Modes:**
  - **Text Mode:** Playing by entering coordinates (e.g., `e2 e4`).
  - **Cursor Mode:** Moving around the board using arrow keys for piece selection.
  - **Demo Mode:** Automatically reads and plays moves from the external `oyun.txt` file (Example of **File Operations**).

- **Core Chess Rules:**
  - ✅ **Castling (Rok):** Implemented using flag variables (`beyazSahOynadi`, etc.) and complex conditional logic.
  - ✅ **En Passant (Geçerken Alma):** Handled via the temporary global variable `enPassantSutun`.
  - ✅ **Pawn Promotion (Terfi):** Pawns are promoted upon reaching the final rank.
  - ✅ **Check Detection (Şah Kontrolü):** Uses the dedicated `SahTehditAltindaMi` function to prevent illegal moves (self-check).

- **Utilities:**
  - 💾 **Save / Load:** Implemented using **StreamWriter** and **StreamReader** to save and load the full game state (board + all game flags) to `kayit.txt`.
  - 📝 **Notation Tracking:** Moves are tracked and displayed on the side panel in algebraic notation (e.g., `1. e4 e5`).
  - 💡 **Hint System:** Provides suggestions for simple capturing moves.

### 🛠️ How to Run

1.  Clone the repository or download the source code.
2.  Open the project in Visual Studio.
3.  Run the application (F5 / Start) and select a mode (1, 2, or 3).

---
<div align="center">🇹🇷 TÜRKÇE VERSİYON 🇹🇷</div>
---

# C# Konsol Satranç Motoru

Bu proje, 1. sınıf "Algoritmalar ve Programlama" dersi için **C# programlama dili** kullanılarak geliştirilmiş, konsol tabanlı bir satranç oyun motorudur. **Global/Yerel değişkenler, dosya işlemleri (StreamWriter/StreamReader), fonksiyonlar, prosedürler, şartlı mantık ve matris (8x8 tahta) manipülasyonu** gibi temel konuları uygulamaktadır.

### 🚀 Özellikler

- **Oyun Modları:**
  - **Yazı Modu:** Koordinatları yazarak oynama (Örn: `e2 e4`).
  - **İmleç Modu:** Ok tuşları ile tahta üzerinde gezinerek taş seçme.
  - **Demo Modu:** Harici `oyun.txt` dosyasından hamleleri otomatik okuyup oynatır (**Dosya İşlemleri**'ne örnektir).

- **Temel Satranç Kuralları:**
  - ✅ **Rok (Castling):** `beyazSahOynadi` gibi kontrol bayrakları kullanılarak karmaşık koşullu mantıkla uygulanmıştır.
  - ✅ **Geçerken Alma (En Passant):** `enPassantSutun` global değişkeni ile yönetilmiştir.
  - ✅ **Piyon Terfisi (Promotion):** Son sıraya ulaşan piyonlar Vezir, Kale, Fil veya At'a terfi eder.
  - ✅ **Şah Kontrolü (Check Detection):** Özel `SahTehditAltindaMi` fonksiyonu ile şah tehdidi kontrol edilir ve kural dışı hamleler engellenir.

- **Yardımcı Araçlar:**
  - 💾 **Kaydet / Yükle:** **StreamWriter** ve **StreamReader** kullanılarak tüm oyun durumu (`kayit.txt` dosyasına) kaydedilip yüklenir.
  - 📝 **Notasyon Takibi:** Yapılan hamleler cebirsel notasyonla (Örn: `1. e4 e5`) yan panelde listelenir.
  - 💡 **İpucu Sistemi:** 'H' tuşuna basıldığında basit yeme hamleleri önerilir.

### 🛠️ Nasıl Çalıştırılır?

1.  Depoyu klonlayın veya kaynak kodu indirin.
2.  Visual Studio ile projeyi açın.
3.  Uygulamayı çalıştırın (F5 / Başlat) ve menüden istediğiniz modu (1, 2 veya 3) seçin.

---
*Geliştirici: Mehmet Arda Kalafat
