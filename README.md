# oop-nrp

Repo tugas mata kuliah **Pemrograman Berorientasi Objek (C#)**, dibuat dari template [`oop-if-its/oop-template`](https://github.com/oop-if-its/oop-template). Ganti judul di atas jadi nama repo kalian sendiri (`oop-nrp`, contoh: `oop-5025201012`).

## Aturan Umum

- Tugas tiap pertemuan disimpan di folder `pertemuan-XX/` pada repo ini.
- Commit message wajib menyebut level yang dicapai: `pertemuan-XX: level N selesai`.
- Deadline push: sebelum pertemuan berikutnya dimulai.
- Semua level dicek otomatis lewat `dotnet test` — baca `pertemuan-XX/SOAL.md` tiap minggu untuk detail levelnya.
- Struktur tiap `pertemuan-XX/` selalu punya dua project terpisah:
  - `src/` — kode kalian (boleh & wajib diedit). Bagian yang harus dikerjakan ditandai komentar `TODO(Level N)`; kode yang ditandai `SUDAH LENGKAP` jangan diubah.
  - `tests/` — test grading dari dosen (**jangan diubah** — perubahan di sini tidak dipakai saat penilaian, dosen menimpa ulang folder ini sebelum menjalankan grading).
- Jalankan `dotnet test pertemuan-XX/tests` dari root repo untuk mengecek progres kalian sendiri sebelum push (project test otomatis membangun ulang project `src/` yang direferensikannya).
- Semua tugas **berjenjang Level 1–10**: nilai mengikuti level tertinggi yang lolos **berurutan** dari 1, tapi kalian boleh mengerjakan tidak berurutan (`-v normal` menunjukkan status tiap level).

### Pertemuan 10–14 (studi kasus game)

Folder pertemuan blok game punya satu project tambahan:
- `app/` — aplikasi **Windows Forms** peraga yang menjalankan game kalian (hanya Windows). **Sudah lengkap, jangan diubah**, dan **tidak** ikut dinilai atau dibangun oleh `dotnet test`.
- Logika game di `src/` sengaja **tidak memakai Windows Forms** (menggambar lewat interface `IKanvas`), supaya bisa diuji otomatis di sistem operasi mana pun. Setelah levelnya cukup selesai, lihat game kalian hidup dengan:

```bash
dotnet run --project pertemuan-10/app
```

## Mengambil Pertemuan Baru Tiap Minggu

Repo ini **tidak otomatis sinkron** dengan template dosen. Begitu ada pertemuan baru, jalankan (ganti `pertemuan-03` sesuai minggu berjalan):

```bash
git fetch https://github.com/oop-if-its/oop-template.git main
git checkout FETCH_HEAD -- pertemuan-03
```

Perintah ini **aman dijalankan kapan pun** — tidak akan menimpa folder pertemuan lain yang sudah kalian kerjakan, karena hanya mengambil folder yang disebutkan. Setelah itu, commit folder barunya seperti biasa.

Kalau dosen memperbaiki sesuatu di pertemuan yang sudah dirilis (mis. ada bug di test), biasanya cukup ambil ulang file yang diperbaiki saja, bukan seluruh folder — akan diumumkan file mana yang berubah.

## Menjalankan Test Secara Lokal

Butuh [.NET SDK](https://dotnet.microsoft.com/download) (versi 8 ke atas) terinstal. Dari root repo:

```bash
dotnet test pertemuan-03/tests
```

Untuk melihat detail tiap level (lulus/gagal satu per satu), tambahkan `-v normal`:

```bash
dotnet test pertemuan-03/tests -v normal
```

---

## Identitas
- Nama: I Made Saskara Bawa
- NRP: 5025251205
- Kelas: PBO F

Bagian di atas diisi sekali di awal semester. Kalau pertemuan mendatang butuh section reflektif baru di README ini (heading yang dicek otomatis lewat test), section itu akan ditambahkan di sini — jangan ganti nama heading yang sudah ada.
