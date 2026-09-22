// File ini disediakan dosen untuk mengecek progres level secara otomatis.
// JANGAN DIUBAH — perubahan pada file ini tidak akan dipakai saat penilaian
// (dosen menimpa ulang folder tests/ sebelum menjalankan grading).
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Pertemuan04;
using Xunit;

namespace Pertemuan04.Tests;

public class EnkapsulasiTests
{
    // ISBN-13 valid (digit cek benar) yang dipakai di seluruh test.
    private const string IsbnA = "9786020332956";
    private const string IsbnB = "9780306406157";
    private const string IsbnC = "9780134685991";
    private const string IsbnD = "9781234567897";

    // ---------- helper ----------

    private static void PastikanTidakAdaFieldPublik(Type tipe)
    {
        var fieldPublik = tipe.GetFields(BindingFlags.Public | BindingFlags.Instance);
        Assert.True(fieldPublik.Length == 0,
            $"Kelas {tipe.Name} masih punya field publik: {string.Join(", ", fieldPublik.Select(f => f.Name))}. " +
            "Jadikan private dan ekspos lewat properti.");
    }

    private static PropertyInfo AmbilProperti(Type tipe, string nama)
    {
        var prop = tipe.GetProperty(nama, BindingFlags.Public | BindingFlags.Instance);
        Assert.True(prop is not null, $"Properti publik '{nama}' tidak ditemukan di kelas {tipe.Name} (masih berupa field?)");
        return prop!;
    }

    private static bool SetterPublik(PropertyInfo p) => p.GetSetMethod(nonPublic: false) is not null;

    private static bool InitOnly(PropertyInfo p) =>
        p.SetMethod is not null &&
        p.SetMethod.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(IsExternalInit));

    // ---------- Level 1 ----------

    [Fact(DisplayName = "Level 1 - Field private + properti read-only + konstruktor")]
    public void Level1_FieldPrivateDanPropertiReadOnly()
    {
        PastikanTidakAdaFieldPublik(typeof(Buku));

        foreach (var nama in new[] { "Isbn", "Judul", "StokTotal", "StokTersedia" })
        {
            var prop = AmbilProperti(typeof(Buku), nama);
            Assert.True(prop.CanRead, $"Properti '{nama}' harus bisa dibaca");
            Assert.False(SetterPublik(prop), $"Properti '{nama}' tidak boleh punya setter publik");
        }

        var buku = new Buku(IsbnA, "Laskar Pelangi", 5);
        Assert.Equal(IsbnA, buku.Isbn);
        Assert.Equal("Laskar Pelangi", buku.Judul);
        Assert.Equal(5, buku.StokTotal);
        Assert.Equal(5, buku.StokTersedia);
    }

    // ---------- Level 2 ----------

    [Fact(DisplayName = "Level 2 - Validasi konstruktor (judul & stok)")]
    public void Level2_ValidasiKonstruktor()
    {
        string?[] judulTidakValid = { "", "   ", null };
        foreach (var judul in judulTidakValid)
        {
            var ex = Assert.ThrowsAny<ArgumentException>(() => new Buku(IsbnA, judul!, 3));
            Assert.False(string.IsNullOrWhiteSpace(ex.Message), "Pesan exception tidak boleh kosong");
        }

        Assert.ThrowsAny<ArgumentException>(() => new Buku(IsbnA, "Judul", -1));

        // stok 0 sah (buku terdaftar tapi belum ada eksemplar)
        var nol = new Buku(IsbnA, "Judul", 0);
        Assert.Equal(0, nol.StokTersedia);
    }

    // ---------- Level 3 ----------

    [Fact(DisplayName = "Level 3 - Pinjam() menjaga invarian stok")]
    public void Level3_Pinjam()
    {
        var buku = new Buku(IsbnA, "A", 2);

        buku.Pinjam();
        Assert.Equal(1, buku.StokTersedia);
        buku.Pinjam();
        Assert.Equal(0, buku.StokTersedia);

        Assert.Throws<InvalidOperationException>(() => buku.Pinjam());
        Assert.Equal(0, buku.StokTersedia);   // tidak boleh negatif
        Assert.Equal(2, buku.StokTotal);      // stok total tidak ikut berubah
    }

    // ---------- Level 4 ----------

    [Fact(DisplayName = "Level 4 - Kembalikan() menjaga invarian stok")]
    public void Level4_Kembalikan()
    {
        var buku = new Buku(IsbnA, "A", 2);
        buku.Pinjam();
        buku.Pinjam();

        buku.Kembalikan();
        Assert.Equal(1, buku.StokTersedia);
        buku.Kembalikan();
        Assert.Equal(2, buku.StokTersedia);

        // tidak boleh melebihi StokTotal
        Assert.Throws<InvalidOperationException>(() => buku.Kembalikan());
        Assert.Equal(2, buku.StokTersedia);
    }

    // ---------- Level 5 ----------

    [Fact(DisplayName = "Level 5 - Properti terhitung (PersentaseTersedia, Status)")]
    public void Level5_PropertiTerhitung()
    {
        var buku = new Buku(IsbnA, "A", 4);
        Assert.Equal(100.0, buku.PersentaseTersedia, 3);
        Assert.Equal("Tersedia", buku.Status);

        buku.Pinjam();
        Assert.Equal(75.0, buku.PersentaseTersedia, 3);

        buku.Pinjam(); buku.Pinjam(); buku.Pinjam();
        Assert.Equal(0.0, buku.PersentaseTersedia, 3);
        Assert.Equal("Habis", buku.Status);

        // StokTotal = 0 tidak boleh menghasilkan NaN
        var kosong = new Buku(IsbnB, "B", 0);
        Assert.Equal(0.0, kosong.PersentaseTersedia, 3);
        Assert.Equal("Habis", kosong.Status);

        // properti terhitung: tidak boleh bisa diisi dari luar
        Assert.False(SetterPublik(AmbilProperti(typeof(Buku), "PersentaseTersedia")));
        Assert.False(SetterPublik(AmbilProperti(typeof(Buku), "Status")));
    }

    // ---------- Level 6 ----------

    [Fact(DisplayName = "Level 6 - Validasi & normalisasi ISBN")]
    public void Level6_ValidasiIsbn()
    {
        // tanda '-' dan spasi dibuang
        Assert.Equal(IsbnB, new Buku("978-0-306-40615-7", "X", 1).Isbn);
        Assert.Equal(IsbnB, new Buku("978 0306 406157", "X", 1).Isbn);

        string?[] tidakValid =
        {
            null,
            "",
            "12345",                // terlalu pendek
            "97860203329561",       // 14 digit
            "97860203329AB",        // bukan angka
            "9786020332957",        // digit cek salah
        };
        foreach (var isbn in tidakValid)
        {
            Assert.ThrowsAny<ArgumentException>(() => new Buku(isbn!, "X", 1));
        }
    }

    // ---------- Level 7 ----------

    [Fact(DisplayName = "Level 7 - Enkapsulasi koleksi di Perpustakaan")]
    public void Level7_KoleksiTerlindungi()
    {
        PastikanTidakAdaFieldPublik(typeof(Perpustakaan));

        var prop = AmbilProperti(typeof(Perpustakaan), "DaftarBuku");
        Assert.False(SetterPublik(prop), "DaftarBuku tidak boleh punya setter publik");
        Assert.True(
            prop.PropertyType == typeof(IReadOnlyList<Buku>) ||
            prop.PropertyType == typeof(IReadOnlyCollection<Buku>) ||
            prop.PropertyType == typeof(IEnumerable<Buku>) ||
            prop.PropertyType == typeof(ReadOnlyCollection<Buku>),
            $"DaftarBuku bertipe {prop.PropertyType.Name}; gunakan tipe read-only (mis. IReadOnlyList<Buku>)");

        var perpus = new Perpustakaan();
        Assert.Empty(perpus.DaftarBuku);

        var a = new Buku(IsbnA, "A", 1);
        perpus.Tambah(a);
        perpus.Tambah(new Buku(IsbnB, "B", 1));
        Assert.Equal(2, perpus.DaftarBuku.Count);
        Assert.Equal(2, perpus.JumlahJudul);

        // ISBN kembar ditolak, null ditolak
        Assert.Throws<InvalidOperationException>(() => perpus.Tambah(new Buku(IsbnA, "Kembar", 1)));
        Assert.Throws<ArgumentNullException>(() => perpus.Tambah(null!));
        Assert.Equal(2, perpus.JumlahJudul);

        // pencarian
        Assert.Same(a, perpus.Cari(IsbnA));
        Assert.Null(perpus.Cari(IsbnC));

        // usaha merusak koleksi dari luar tidak boleh berhasil
        if (perpus.DaftarBuku is ICollection<Buku> koleksiLuar)
        {
            try { koleksiLuar.Add(new Buku(IsbnC, "Selundupan", 1)); } catch (NotSupportedException) { }
            try { koleksiLuar.Clear(); } catch (NotSupportedException) { }
        }
        Assert.Equal(2, perpus.DaftarBuku.Count);
        Assert.Equal(2, perpus.JumlahJudul);
        Assert.Null(perpus.Cari(IsbnC));
    }

    // ---------- Level 8 ----------

    [Fact(DisplayName = "Level 8 - Properti dengan validasi di setter")]
    public void Level8_SetterDenganValidasi()
    {
        var buku = new Buku(IsbnA, "A", 1);
        Assert.Equal(7, buku.BatasHariPinjam);   // nilai awal

        buku.BatasHariPinjam = 14;
        Assert.Equal(14, buku.BatasHariPinjam);

        buku.BatasHariPinjam = 1;
        buku.BatasHariPinjam = 30;
        Assert.Equal(30, buku.BatasHariPinjam);

        foreach (var tidakValid in new[] { 0, -5, 31, 100 })
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => buku.BatasHariPinjam = tidakValid);
            Assert.Equal(30, buku.BatasHariPinjam);   // nilai lama tidak berubah
        }
    }

    // ---------- Level 9 ----------

    [Fact(DisplayName = "Level 9 - AkunAnggota: init-only & state tersembunyi")]
    public void Level9_AkunAnggota()
    {
        // struktur
        var nomor = AmbilProperti(typeof(AkunAnggota), "NomorAnggota");
        Assert.False(SetterPublik(nomor), "NomorAnggota tidak boleh punya setter publik");

        var nama = AmbilProperti(typeof(AkunAnggota), "Nama");
        Assert.True(InitOnly(nama), "Nama harus init-only: ganti `set` menjadi `init`");

        var denda = AmbilProperti(typeof(AkunAnggota), "Denda");
        Assert.False(SetterPublik(denda), "Denda tidak boleh bisa diubah langsung dari luar (setter harus private)");

        // perilaku
        Assert.ThrowsAny<ArgumentException>(() => new AkunAnggota(""));
        Assert.ThrowsAny<ArgumentException>(() => new AkunAnggota("   "));

        var akun = new AkunAnggota("A001") { Nama = "Budi" };
        Assert.Equal("A001", akun.NomorAnggota);
        Assert.Equal("Budi", akun.Nama);
        Assert.Equal(0, akun.Denda);

        akun.TambahDenda(5000);
        akun.TambahDenda(2000);
        Assert.Equal(7000, akun.Denda);

        Assert.Equal(4000, akun.BayarDenda(3000));   // mengembalikan sisa
        Assert.Equal(4000, akun.Denda);

        Assert.Throws<InvalidOperationException>(() => akun.BayarDenda(9999));  // melebihi denda
        Assert.Equal(4000, akun.Denda);

        Assert.Throws<ArgumentOutOfRangeException>(() => akun.TambahDenda(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => akun.TambahDenda(-100));
        Assert.Throws<ArgumentOutOfRangeException>(() => akun.BayarDenda(0));
        Assert.Equal(4000, akun.Denda);

        Assert.Equal(0, akun.BayarDenda(4000));
    }

    // ---------- Level 10 (bonus) ----------

    [Fact(DisplayName = "Level 10 - Bonus: Perpustakaan.PinjamBuku (Tell, don't ask) & internal")]
    public void Level10_PinjamBukuDanInternal()
    {
        // pencatatan pinjaman hanya untuk kode di dalam pustaka: internal, BUKAN public
        var jenis = typeof(AkunAnggota);
        foreach (var nama in new[] { "CatatPinjam", "CatatKembali" })
        {
            Assert.Null(jenis.GetMethod(nama, BindingFlags.Public | BindingFlags.Instance));
            var m = jenis.GetMethod(nama, BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.True(m is not null && m.IsAssembly,
                $"Method {nama} harus ada dan bermodifier internal (tidak public, tidak private)");
        }
        var pinjaman = AmbilProperti(jenis, "JumlahPinjamanAktif");
        Assert.False(SetterPublik(pinjaman), "JumlahPinjamanAktif tidak boleh punya setter publik");

        // perilaku
        var perpus = new Perpustakaan();
        perpus.Tambah(new Buku(IsbnA, "A", 1));
        perpus.Tambah(new Buku(IsbnB, "B", 2));
        perpus.Tambah(new Buku(IsbnC, "C", 2));
        perpus.Tambah(new Buku(IsbnD, "D", 2));

        var akun = new AkunAnggota("A001");
        var lain = new AkunAnggota("A002");

        perpus.PinjamBuku(IsbnA, akun);
        Assert.Equal(1, akun.JumlahPinjamanAktif);
        Assert.Equal(0, perpus.Cari(IsbnA)!.StokTersedia);

        // stok habis -> ditolak, tidak ada yang tercatat
        Assert.Throws<InvalidOperationException>(() => perpus.PinjamBuku(IsbnA, lain));
        Assert.Equal(0, lain.JumlahPinjamanAktif);

        // ISBN tidak dikenal / akun null
        Assert.Throws<ArgumentException>(() => perpus.PinjamBuku("9780000000002", akun));
        Assert.Throws<ArgumentNullException>(() => perpus.PinjamBuku(IsbnB, null!));

        // batas pinjaman aktif = 3
        perpus.PinjamBuku(IsbnB, akun);
        perpus.PinjamBuku(IsbnC, akun);
        Assert.Equal(3, akun.JumlahPinjamanAktif);
        Assert.Throws<InvalidOperationException>(() => perpus.PinjamBuku(IsbnD, akun));
        Assert.Equal(2, perpus.Cari(IsbnD)!.StokTersedia);   // buku D tidak tersentuh

        // pengembalian
        perpus.KembalikanBuku(IsbnA, akun);
        Assert.Equal(2, akun.JumlahPinjamanAktif);
        Assert.Equal(1, perpus.Cari(IsbnA)!.StokTersedia);
        Assert.Throws<InvalidOperationException>(() => perpus.KembalikanBuku(IsbnD, lain)); // lain tidak sedang meminjam

        // denda menghalangi peminjaman
        lain.TambahDenda(1000);
        Assert.Throws<InvalidOperationException>(() => perpus.PinjamBuku(IsbnD, lain));
        lain.BayarDenda(1000);
        perpus.PinjamBuku(IsbnD, lain);
        Assert.Equal(1, lain.JumlahPinjamanAktif);
    }
}
