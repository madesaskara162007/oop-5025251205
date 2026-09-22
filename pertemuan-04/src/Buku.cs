// File ini berisi TODO -- lihat SOAL.md untuk kontrak lengkap tiap level.
// Bagian bertanda TODO(Level N) adalah tugas kalian; kode lain sudah
// disediakan. Jangan mengubah nama/tipe yang sudah ada kecuali TODO
// memintanya secara eksplisit.

namespace Pertemuan04;

public class Buku
{
    // TODO(Level 1): field PUBLIK di bawah ini melanggar enkapsulasi (siapa pun
    //   bisa mengubahnya sembarangan). Jadikan field PRIVATE (awali nama dengan
    //   _) lalu ekspos lewat properti read-only: public get, tanpa setter
    //   publik. Nama properti tetap Isbn, Judul, StokTotal, StokTersedia.
    private string _isbn = "";
    private string _judul = "";
    private int _stokTotal;
    private int _stokTersedia;

    public string Isbn => _isbn;
    public string Judul => _judul;
    public int StokTotal => _stokTotal;
    public int StokTersedia => _stokTersedia;

    // TODO(Level 8): properti di bawah ini menerima nilai apa saja. Beri nilai
    //   awal 7 dan tambahkan logika validasi di accessor set (perlu field
    //   pendukung): nilai harus 1..30, di luar itu lempar
    //   ArgumentOutOfRangeException dan JANGAN mengubah nilai lama.
    private int _batasHariPinjam = 7;

    public int BatasHariPinjam
    {
        get => _batasHariPinjam;
        set
        {
            if (value is < 1 or > 30)
                throw new ArgumentOutOfRangeException(nameof(value), "Batas hari harus 1 sampai 30.");

            _batasHariPinjam = value;
        }
    }

    // TODO(Level 2): validasi di AWAL konstruktor -- judul null/kosong/spasi
    //   saja atau stokTotal negatif -> lempar ArgumentException
    //   (ArgumentOutOfRangeException juga boleh); jangan ada state yang berubah
    //   kalau ditolak.
    // TODO(Level 6): validasi & normalisasi ISBN -- buang tanda '-' dan spasi;
    //   hasilnya harus tepat 13 digit angka dengan digit cek ISBN-13 yang benar;
    //   kalau tidak, lempar ArgumentException. Isbn menyimpan versi TANPA '-'.
    public Buku(string isbn, string judul, int stokTotal)
    {
        // TODO(Level 1): isi Isbn, Judul, StokTotal dari parameter; StokTersedia
        //   awal = stokTotal.
        if (string.IsNullOrWhiteSpace(judul))
            throw new ArgumentException("Judul wajib diisi.", nameof(judul));

        if (stokTotal < 0)
            throw new ArgumentOutOfRangeException(nameof(stokTotal), "Stok tidak boleh negatif.");

        var isbnBersih = isbn?.Replace("-", "").Replace(" ", "");
        if (isbnBersih is null || isbnBersih.Length != 13 ||
            isbnBersih.Any(digit => digit < '0' || digit > '9'))
            throw new ArgumentException("ISBN harus terdiri dari 13 digit.", nameof(isbn));

        var jumlahChecksum = 0;
        for (var indeks = 0; indeks < isbnBersih.Length; indeks++)
        {
            var digit = isbnBersih[indeks] - '0';
            jumlahChecksum += digit * (indeks % 2 == 0 ? 1 : 3);
        }

        if (jumlahChecksum % 10 != 0)
            throw new ArgumentException("Checksum ISBN tidak valid.", nameof(isbn));

        _isbn = isbnBersih;
        _judul = judul;
        _stokTotal = stokTotal;
        _stokTersedia = stokTotal;
    }

    public void Pinjam()
    {
        // TODO(Level 3): kurangi StokTersedia satu. Kalau stok sudah 0, lempar
        //   InvalidOperationException dan biarkan stok tetap.
        if (_stokTersedia == 0)
            throw new InvalidOperationException("Stok buku habis.");

        _stokTersedia--;
    }

    public void Kembalikan()
    {
        // TODO(Level 4): tambah StokTersedia satu. Kalau stok sudah sama dengan
        //   StokTotal (tidak ada yang sedang dipinjam), lempar
        //   InvalidOperationException dan biarkan stok tetap.
        if (_stokTersedia == _stokTotal)
            throw new InvalidOperationException("Semua buku sudah tersedia.");

        _stokTersedia++;
    }

    // Level 5: properti TERHITUNG -- tanpa field pendukung, tanpa setter.
    public double PersentaseTersedia
    {
        get
        {
            // TODO(Level 5): kembalikan StokTersedia / StokTotal * 100 (double).
            //   Kalau StokTotal = 0 kembalikan 0 (bukan NaN).
            return _stokTotal == 0 ? 0 : (double)_stokTersedia / _stokTotal * 100;
        }
    }

    public string Status
    {
        get
        {
            // TODO(Level 5): kembalikan "Tersedia" kalau StokTersedia > 0,
            //   selain itu "Habis".
            return _stokTersedia > 0 ? "Tersedia" : "Habis";
        }
    }

}
