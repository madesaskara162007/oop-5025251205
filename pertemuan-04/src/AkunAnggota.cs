// File ini berisi TODO -- lihat SOAL.md untuk kontrak lengkap tiap level.
// Bagian bertanda TODO(Level N) adalah tugas kalian; kode lain sudah
// disediakan. Jangan mengubah nama/tipe yang sudah ada kecuali TODO
// memintanya secara eksplisit.

namespace Pertemuan04;

public class AkunAnggota
{
    public const int MaksPinjaman = 3;

    public string NomorAnggota { get; }

    // TODO(Level 9): Nama hanya boleh diisi saat objek dibuat (ganti set ->
    //   init). Denda TIDAK boleh diubah dari luar kelas sama sekali (setter
    //   private) -- perubahannya hanya lewat TambahDenda()/BayarDenda().
    public string Nama { get; init; } = "";
    public int Denda { get; private set; }

    // TODO(Level 10): JumlahPinjamanAktif hanya boleh diubah dari dalam kelas
    //   (setter private), dan pencatatannya lewat method internal (bukan public)
    //   di bawah -- hanya kode di dalam pustaka (Perpustakaan) yang boleh
    //   memanggilnya, bukan kode pemakai dari luar.
    public int JumlahPinjamanAktif { get; private set; }

    public AkunAnggota(string nomorAnggota)
    {
        // TODO(Level 9): nomorAnggota null/kosong/spasi -> ArgumentException;
        //   selain itu isi NomorAnggota.
        if (string.IsNullOrWhiteSpace(nomorAnggota))
            throw new ArgumentException("Nomor anggota wajib diisi.", nameof(nomorAnggota));

        NomorAnggota = nomorAnggota;
    }

    public void TambahDenda(int rupiah)
    {
        // TODO(Level 9): rupiah <= 0 -> ArgumentOutOfRangeException; selain itu
        //   tambahkan ke Denda.
        if (rupiah <= 0)
            throw new ArgumentOutOfRangeException(nameof(rupiah), "Denda harus lebih besar dari nol.");

        Denda += rupiah;
    }

    public int BayarDenda(int rupiah)
    {
        // TODO(Level 9): rupiah <= 0 -> ArgumentOutOfRangeException; rupiah >
        //   Denda -> InvalidOperationException (denda tidak berubah); selain itu
        //   kurangi Denda dan KEMBALIKAN sisa denda.
        if (rupiah <= 0)
            throw new ArgumentOutOfRangeException(nameof(rupiah), "Pembayaran harus lebih besar dari nol.");

        if (rupiah > Denda)
            throw new InvalidOperationException("Pembayaran melebihi denda.");

        Denda -= rupiah;
        return Denda;
    }

    internal void CatatPinjam()
    {
        // TODO(Level 10): naikkan JumlahPinjamanAktif satu.
        JumlahPinjamanAktif++;
    }

    internal void CatatKembali()
    {
        // TODO(Level 10): turunkan JumlahPinjamanAktif satu (tidak boleh di
        //   bawah 0).
        if (JumlahPinjamanAktif > 0)
            JumlahPinjamanAktif--;
    }
}
