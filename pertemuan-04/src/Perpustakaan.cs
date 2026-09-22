// File ini berisi TODO -- lihat SOAL.md untuk kontrak lengkap tiap level.
// Bagian bertanda TODO(Level N) adalah tugas kalian; kode lain sudah
// disediakan. Jangan mengubah nama/tipe yang sudah ada kecuali TODO
// memintanya secara eksplisit.

namespace Pertemuan04;

public class Perpustakaan
{
    // TODO(Level 7): koleksi di bawah ini PUBLIK -- pihak luar bisa Add/Clear
    //   seenaknya, melewati aturan Tambah(). Simpan daftar di field PRIVATE
    //   (List<Buku>) dan ekspos DaftarBuku sebagai properti read-only bertipe
    //   IReadOnlyList<Buku> (atau IReadOnlyCollection/IEnumerable) yang tidak
    //   bisa dipakai untuk mengubah koleksi asli.
    public List<Buku> DaftarBuku = new();

    public int JumlahJudul => DaftarBuku.Count;

    public void Tambah(Buku buku)
    {
        // TODO(Level 7): buku null -> ArgumentNullException; ISBN yang sudah ada
        //   di koleksi -> InvalidOperationException; selain itu tambahkan ke
        //   koleksi.
        throw new NotImplementedException("Level 7 belum diimplementasikan");
    }

    public Buku? Cari(string isbn)
    {
        // TODO(Level 7): kembalikan buku dengan Isbn yang sama persis (apa
        //   adanya, tanpa normalisasi), atau null kalau tidak ada.
        throw new NotImplementedException("Level 7 belum diimplementasikan");
    }

    public void PinjamBuku(string isbn, AkunAnggota akun)
    {
        // TODO(Level 10): "Tell, don't ask" -- Perpustakaan memutuskan semuanya.
        //   akun null -> ArgumentNullException; ISBN tidak ada ->
        //   ArgumentException; akun.Denda > 0 -> InvalidOperationException;
        //   akun.JumlahPinjamanAktif sudah sama dengan AkunAnggota.MaksPinjaman
        //   -> InvalidOperationException; selain itu panggil buku.Pinjam()
        //   (boleh melempar kalau stok habis) lalu akun.CatatPinjam().
        throw new NotImplementedException("Level 10 belum diimplementasikan");
    }

    public void KembalikanBuku(string isbn, AkunAnggota akun)
    {
        // TODO(Level 10): akun null -> ArgumentNullException; ISBN tidak ada ->
        //   ArgumentException; akun.JumlahPinjamanAktif = 0 ->
        //   InvalidOperationException; selain itu panggil buku.Kembalikan() lalu
        //   akun.CatatKembali().
        throw new NotImplementedException("Level 10 belum diimplementasikan");
    }
}
