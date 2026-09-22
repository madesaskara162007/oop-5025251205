# Pertemuan 4 — Encapsulation & Data Hiding: Buku yang Terlindungi

Tugas ini melatih **enkapsulasi**: menyembunyikan data (`private`), membuka akses hanya lewat "pintu" yang dijaga (properti, method), dan memastikan objek **tidak pernah berada dalam keadaan yang tidak sah** (invarian). Studi kasusnya kelanjutan perpustakaan dari pertemuan 3: `Buku`, `Perpustakaan`, dan `AkunAnggota`.

Semua level dicek otomatis lewat `dotnet test`. Nilai mengikuti level tertinggi yang lolos **berurutan** dari 1 — kerjakan sejauh kemampuan, boleh tidak berurutan (jalankan `dotnet test pertemuan-04/tests -v normal` untuk lihat status tiap level apa adanya).

**Struktur folder yang wajib diikuti** (sudah disiapkan di starter, jangan diubah namanya):
```
pertemuan-04/
  SOAL.md
  src/
    Pertemuan04.csproj
    Buku.cs              <- edit (Level 1-6, 8)
    Perpustakaan.cs      <- edit (Level 7, 10)
    AkunAnggota.cs       <- edit (Level 9, 10)
  tests/
    Pertemuan04.Tests.csproj
    EnkapsulasiTests.cs   <- JANGAN diubah, ini test grading dosen
```

**Cara membaca starter.** Berbeda dari pertemuan 3, sebagian kode starter sengaja berisi desain yang **buruk** (mis. field `public`) — itu titik awal refaktor kalian, bukan kesalahan. Tiap bagian yang harus kalian kerjakan ditandai komentar `TODO(Level N)`; nama kelas, properti, dan method yang sudah ada dipakai test, jadi jangan diganti kecuali TODO memintanya. Jalankan `dotnet test pertemuan-04/tests` dari root repo.

---

## Level 1 — Field Private + Properti Read-Only + Konstruktor

Di `Buku.cs`, keempat anggota `Isbn`, `Judul`, `StokTotal`, `StokTersedia` awalnya berupa **field publik** — siapa pun bisa menulis `buku.StokTersedia = -999`. Perbaiki:

- Ubah menjadi field **private** (awali nama dengan `_`, mis. `_judul`).
- Ekspos lewat **properti** dengan nama yang sama (`Isbn`, `Judul`, `StokTotal`, `StokTersedia`): boleh dibaca dari luar, **tidak** boleh punya setter publik.
- Lengkapi konstruktor `Buku(string isbn, string judul, int stokTotal)` supaya mengisi keempat nilai: `StokTersedia` di awal **sama dengan** `stokTotal`.

Test juga memastikan tidak ada field publik yang tersisa di `Buku`.

## Level 2 — Validasi Konstruktor

Di **awal** konstruktor `Buku`, tolak data yang tidak sah dengan `ArgumentException` (`ArgumentOutOfRangeException` dan `ArgumentNullException` juga diterima, karena keduanya turunannya), dengan pesan yang tidak kosong:

- `judul` bernilai `null`, kosong, atau hanya spasi.
- `stokTotal` negatif. (`stokTotal` = 0 **sah**.)

## Level 3 — `Pinjam()`

Setiap pemanggilan `Pinjam()` mengurangi `StokTersedia` satu. Kalau stok sudah 0, lempar `InvalidOperationException` dan **biarkan stok tetap 0** (tidak boleh negatif). `StokTotal` tidak ikut berubah.

## Level 4 — `Kembalikan()`

Setiap pemanggilan `Kembalikan()` menambah `StokTersedia` satu. `StokTersedia` tidak boleh melebihi `StokTotal`: kalau semua eksemplar sudah ada di perpustakaan, lempar `InvalidOperationException` dan biarkan stok tetap.

## Level 5 — Properti Terhitung

Tambahkan isi dua properti **terhitung** (tanpa field pendukung, tanpa setter — nilainya selalu dihitung dari data lain, jadi tidak mungkin "tidak sinkron"):

- `double PersentaseTersedia` = `StokTersedia / StokTotal * 100`. Kalau `StokTotal` = 0, kembalikan `0` (bukan `NaN`). Hati-hati pembagian bilangan bulat!
- `string Status` = `"Tersedia"` jika `StokTersedia > 0`, selain itu `"Habis"`.

## Level 6 — Validasi & Normalisasi ISBN

Lengkapi pengolahan `isbn` di konstruktor `Buku`:

1. Buang semua tanda `-` dan spasi (`"978-0-306-40615-7"` → `"9780306406157"`). Properti `Isbn` menyimpan versi **bersih**.
2. Hasilnya harus tepat **13 digit angka**.
3. Digit cek ISBN-13 harus benar: jumlahkan digit ke-1, 2, 3, … 13 dengan bobot berselang-seling **1, 3, 1, 3, …**; total harus habis dibagi 10.
   Contoh `9786020332956`: `9·1 + 7·3 + 8·1 + 6·3 + 0·1 + 2·3 + 0·1 + 3·3 + 3·1 + 2·3 + 9·1 + 5·3 + 6·1 = 110` → valid.

ISBN `null` atau yang tidak memenuhi salah satu syarat di atas → `ArgumentException` (turunannya boleh).

## Level 7 — Melindungi Koleksi di `Perpustakaan`

Di `Perpustakaan.cs`, `DaftarBuku` awalnya `public List<Buku>` — pihak luar bisa `Add`/`Clear` seenaknya dan melewati aturan `Tambah()`. Perbaiki:

- Simpan daftar di field **private** `List<Buku>`.
- `DaftarBuku` menjadi properti **read-only** bertipe `IReadOnlyList<Buku>` (`IReadOnlyCollection<Buku>`, `IEnumerable<Buku>`, atau `ReadOnlyCollection<Buku>` juga diterima) tanpa setter publik. Test mencoba merusak koleksi dari luar — koleksi asli harus tetap utuh.
- `Tambah(Buku buku)`: `null` → `ArgumentNullException`; ISBN yang sudah ada → `InvalidOperationException`.
- `Cari(string isbn)`: kembalikan buku dengan `Isbn` yang sama persis, atau `null`.
- `JumlahJudul` sudah disediakan (memakai `DaftarBuku.Count`).

## Level 8 — Properti dengan Validasi di Setter

Tambahkan validasi pada `Buku.BatasHariPinjam` (lama peminjaman dalam hari). Nilai awalnya **7**. Setter tetap `public`, tetapi menolak nilai di luar rentang **1..30** dengan `ArgumentOutOfRangeException` — dan **nilai lama tidak boleh berubah** kalau ditolak. Kalian butuh field pendukung (*backing field*) dan accessor `set` yang punya logika.

## Level 9 — `AkunAnggota`: Init-Only & State Tersembunyi

Di `AkunAnggota.cs`:

- `Nama` hanya boleh diisi **saat objek dibuat** (`new AkunAnggota("A001") { Nama = "Budi" }`) tapi tidak boleh diubah sesudahnya: ganti accessor `set` dengan **`init`**.
- `Denda` **tidak boleh** diubah dari luar kelas sama sekali: setter-nya `private`. Perubahannya hanya lewat method:
  - `TambahDenda(int rupiah)` — `rupiah <= 0` → `ArgumentOutOfRangeException`; selain itu menambah `Denda`.
  - `int BayarDenda(int rupiah)` — `rupiah <= 0` → `ArgumentOutOfRangeException`; `rupiah > Denda` → `InvalidOperationException` (denda tidak berubah); selain itu mengurangi `Denda` dan **mengembalikan sisa denda**.
- Konstruktor `AkunAnggota(string nomorAnggota)`: `null`/kosong/spasi → `ArgumentException`; selain itu mengisi `NomorAnggota` (get-only).

## Level 10 — Bonus: `Perpustakaan.PinjamBuku` & Modifier `internal`

Prinsip *Tell, Don't Ask*: pihak luar cukup **memerintah** (`PinjamBuku`), bukan bertanya-tanya lalu memutuskan sendiri. Lengkapi di `Perpustakaan`:

- `PinjamBuku(string isbn, AkunAnggota akun)` — urutan pemeriksaan: `akun` null → `ArgumentNullException`; ISBN tidak ada → `ArgumentException`; `akun.Denda > 0` → `InvalidOperationException`; `akun.JumlahPinjamanAktif` sudah sama dengan `AkunAnggota.MaksPinjaman` (3) → `InvalidOperationException`; selain itu `buku.Pinjam()` (yang sendirinya bisa melempar kalau stok habis) lalu `akun.CatatPinjam()`.
- `KembalikanBuku(string isbn, AkunAnggota akun)` — `akun` null → `ArgumentNullException`; ISBN tidak ada → `ArgumentException`; `akun.JumlahPinjamanAktif` = 0 → `InvalidOperationException`; selain itu `buku.Kembalikan()` lalu `akun.CatatKembali()`.

Di `AkunAnggota`, perbaiki modifier-nya:
- `JumlahPinjamanAktif`: setter `private`.
- `CatatPinjam()` dan `CatatKembali()` awalnya `public` — ubah menjadi **`internal`** (hanya bisa dipanggil kode di dalam pustaka yang sama, bukan pemakai dari luar). `CatatPinjam` menaikkan `JumlahPinjamanAktif` satu; `CatatKembali` menurunkannya satu (tidak pernah di bawah 0).
