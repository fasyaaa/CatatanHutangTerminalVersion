using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace KoneksiDB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string strKoneksi = @"Server = 127.0.0.1; Database = TokoDB;UserID = root; Password = Fasya2210;port = 3306";
            string strKoneksi2 = @"Server = 127.0.0.1; Database = PencatatanHutang;UserID = root; Password = Fasya2210;port = 3306";
            Program pr = new Program();
            MySqlConnection conn = null;
            while (true)
            {
                try
                {
                    Console.WriteLine("\nCATAT HUTANG CUSTOMER ANDA");
                    Console.WriteLine("1. Koneksi Menggunakan SQL Server Autentication");
                    Console.WriteLine("2. Buat Database Pencatatan Hutang");
                    Console.WriteLine("3. Buat Seluruh Table Pencatatan Hutang");
                    Console.WriteLine("4. Buat Seluruh Database Fiskal");
                    Console.WriteLine("5. Connect Ke Database Pencatatan Hutang");
                    Console.WriteLine("6. MENU");
                    Console.WriteLine("7. PELANGGAN");
                    Console.WriteLine("8. HUTANG");
                    Console.WriteLine("9. EXIT");
                    Console.WriteLine("\nEnter your Choice (1 - 10): ");
                    char ch = Convert.ToChar(Console.ReadLine());
                    switch (ch)
                    {
                        case '1':
                            {
                                try
                                {
                                    MySqlConnection koneksi = new MySqlConnection();
                                    koneksi.ConnectionString = strKoneksi;
                                    koneksi.Open();
                                    string MySqlState = koneksi.State.ToString();
                                    if (MySqlState == "open")
                                    {
                                        koneksi.Close();
                                    }
                                    Console.WriteLine("Koneksi Berhasil");
                                    Console.ReadLine();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Periksa Kembali Server Anda!\n" + ex.Message.ToString());
                                    Console.ReadLine();
                                }
                            }
                            break;
                        case '2':
                            {
                                MySqlConnection koneksi = new MySqlConnection();
                                koneksi.ConnectionString = strKoneksi;

                                string str = "CREATE DATABASE PencatatanHutang ";
                                MySqlCommand cmd = new MySqlCommand(str, koneksi);
                                try
                                {
                                    koneksi.Open();
                                    cmd.ExecuteNonQuery();
                                    Console.WriteLine("Database Berhasil Dibuat");
                                    Console.ReadLine();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Terjadi Kesalahan! Cek Ulang Server Anda!\n " + ex.Message.ToString());
                                    Console.ReadLine();
                                }
                            }
                            break;
                        case '3':
                            {
                                MySqlConnection koneksi = new MySqlConnection();
                                koneksi.ConnectionString = strKoneksi2;

                                string str = @"CREATE TABLE Kasir(
                                             Id_Kasir INT(2) PRIMARY KEY AUTO_INCREMENT,
                                             Nama_Kasir VARCHAR(30)
                                             );

                                             CREATE TABLE Menu(
                                             Id_Menu INT(2) PRIMARY KEY AUTO_INCREMENT,
                                             Nama_Menu VARCHAR(30),
                                             Harga FLOAT
                                             );

                                             CREATE TABLE Pelanggan (
                                             Id_Pelanggan INT(2) PRIMARY KEY AUTO_INCREMENT,
                                             Nama_Pelanggan VARCHAR(30),
                                             No_HP CHAR(13),
                                             CONSTRAINT CHK_No_HP CHECK (No_HP REGEXP '^[0-9]+$')
                                             );


                                             CREATE TABLE Hutang(
                                             Id_Hutang INT(2) PRIMARY KEY AUTO_INCREMENT,
                                             Id_Menu INT(2),
                                             Id_Pelanggan INT(2),
                                             Banyaknya CHAR(2),
                                             Tanggal DATETIME,
                                             Jumlah_Hutang FLOAT,
                                             CONSTRAINT `Id_Menu` FOREIGN KEY (`Id_Menu`) REFERENCES `Menu` (`Id_Menu`),
                                             CONSTRAINT `Id_Pelanggan` FOREIGN KEY (`Id_Pelanggan`) REFERENCES `Pelanggan` (`Id_Pelanggan`)
                                             );";
                                MySqlCommand cmd = new MySqlCommand(str, koneksi);
                                try
                                {
                                    koneksi.Open();
                                    cmd.ExecuteNonQuery();
                                    Console.WriteLine("Seluruh Tabel Berhasil Dibuat");
                                    Console.ReadLine();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Terjadi Kesalahan! Cek Ulang Server Anda!\n" + ex.Message.ToString());
                                    Console.ReadLine();
                                }
                            }
                            break;
                        case '4':
                            {
                                MySqlConnection koneksi = new MySqlConnection();
                                koneksi.ConnectionString = strKoneksi2;

                                // Urut dari Store Procedure, Trigger, Index, View, Hak Akses User
                                string str = @"CREATE PROCEDURE `CRUD_Hutang` ( 
                                    IN action VARCHAR(10),
                                    IN id INT,
                                    IN Id_Pelanggan INT,
                                    IN Harga FLOAT,
                                    IN Banyaknya CHAR(2)
                                    )
                                    BEGIN
                                    IF action = 'Create' THEN
                                        INSERT INTO Hutang(Id_Pelanggan, Harga, Banyaknya) VALUES (Id_Pelanggan, Harga, Banyaknya);
                                    ELSEIF action = 'Read' THEN
                                        SELECT * FROM Hutang WHERE id = id;
                                    ELSEIF action = 'Update' THEN
                                        UPDATE Hutang SET Id_Pelanggan = Id_Pelanggan, Harga = Harga, Banyaknya = Banyaknya WHERE id = id;
                                    ELSEIF action = 'Delete' THEN
                                        DELETE FROM Hutang WHERE id = id;
                                    END IF;
                                    END;

                                    CREATE TRIGGER `Hitung_Hutang` BEFORE INSERT ON `Hutang` FOR EACH ROW
                                                        BEGIN
                                                        DECLARE Harga_Menu FLOAT;
                                                        SELECT Harga INTO Harga_Menu FROM Menu WHERE Id_Menu = NEW.Id_Menu;
                                                        SET NEW.Jumlah_Hutang = Harga_Menu * NEW.Banyaknya;
                                                        END;

                                    CREATE INDEX idx_Pelanggan ON Hutang(Id_Pelanggan);
                                    CREATE INDEX idx_Tanggal ON Hutang(Tanggal);

                                    CREATE VIEW `Average_Hutang` AS
                                                 SELECT P.Id_Pelanggan, SUM(M.Harga * H.Banyaknya) AS Total_Hutang
                                                 FROM Hutang H
                                                 INNER JOIN Menu M ON H.Id_Menu = M.Id_Menu
                                                 INNER JOIN Pelanggan P ON H.Id_Pelanggan = P.Id_Pelanggan
                                                 GROUP BY P.Id_Pelanggan;


                                    ";
                                MySqlCommand cmd = new MySqlCommand(str, koneksi);
                                try
                                {
                                    koneksi.Open();
                                    cmd.ExecuteNonQuery();
                                    Console.WriteLine("Seluruh Database Fiskal Berhasil Dibuat");

                                    string strUser = "CREATE USER 'Kasir'@'localhost' IDENTIFIED BY 'password';";
                                    string strPrivilege = "GRANT ALL PRIVILEGES ON PencatatanHutang.* TO 'Kasir'@'localhost';";
                                    string strFlush = "FLUSH PRIVILEGES;";

                                    MySqlCommand cmdUser = new MySqlCommand(strUser, koneksi);
                                    MySqlCommand cmdPrivilege = new MySqlCommand(strPrivilege, koneksi);
                                    MySqlCommand cmdFlush = new MySqlCommand(strFlush, koneksi);

                                    cmdUser.ExecuteNonQuery();
                                    cmdPrivilege.ExecuteNonQuery();
                                    cmdFlush.ExecuteNonQuery();
                                    Console.WriteLine("Hak Akses User 'Kasir' Berhasil Dibuat");

                                    Console.ReadLine();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Terjadi Kesalahan! Cek Ulang Server Anda!\n" + ex.Message.ToString());
                                    Console.ReadLine();
                                }
                            }
                            break;
                        case '5':
                            {
                                string strKoneksiCatatHutang = @"Server=127.0.0.1;Database=PencatatanHutang;UserID=root;Password=Fasya2210;port=3306";
                                conn = new MySqlConnection(strKoneksiCatatHutang);
                                try
                                {
                                    conn.Open();
                                    if (conn.State == System.Data.ConnectionState.Open)
                                    {
                                        Console.Clear();    //ngilangin biar bagus
                                        Console.WriteLine("Terhubung ke database berhasil!!!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Gagal terhubung ke database.");
                                    }
                                }
                                finally
                                {
                                    if (conn != null)
                                    {
                                        conn.Close();
                                    }
                                }
                            }
                            break;
                        case '6':
                            {
                                //Display Submenu
                                Console.Clear();
                                Console.WriteLine("\nMENU");
                                Console.WriteLine("1. Menambah Data Menu");
                                Console.WriteLine("2. Melihat Data Menu");
                                Console.WriteLine("3. Update Data Menu");
                                Console.WriteLine("4. Cari Menu");
                                Console.WriteLine("5. Hapus Data Menu");
                                Console.WriteLine("6. Kembali");
                                Console.WriteLine("Masukkan Pilihan (1 - 6) : ");
                                char ch2 = Convert.ToChar(Console.ReadLine());
                                switch (ch2)
                                {
                                    case '1':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Input Data Menu");
                                            Console.WriteLine("Nama Menu : ");
                                            string Nama_Menu = Console.ReadLine();
                                            Console.WriteLine("Harga : ");
                                            string Harga = Console.ReadLine();
                                            if (string.IsNullOrEmpty(Nama_Menu) || string.IsNullOrEmpty(Harga))
                                            {
                                                Console.WriteLine("Input tidak boleh kosong!");
                                            }
                                            //Tidak boleh menggunakan angka
                                            else if (Nama_Menu.Any(char.IsDigit))
                                            {
                                                Console.WriteLine("Nama Menu tidak boleh menggunakan angka");
                                            }
                                            else
                                            {
                                                Console.WriteLine("\n---------------------------------\n");
                                                Console.WriteLine("Nama : " + Nama_Menu);
                                                Console.WriteLine("Harga : " + Harga);
                                                Console.WriteLine("\nApakah data sudah benar? (Y/N)");
                                                string konfir = Console.ReadLine();

                                                if (konfir == "Y" || konfir == "y")
                                                {
                                                    try
                                                    {
                                                        pr.insertMenu(Nama_Menu, Harga, conn);
                                                    }
                                                    catch
                                                    {
                                                        Console.WriteLine("\nAnda Tidak Memiliki " +
                                                                       "Akses Untuk Menambah Data");
                                                    }
                                                }
                                                else
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Penambahan Data Dibatalkan!");
                                                }
                                                break;
                                            }

                                        }
                                        break;
                                    case '2':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Menu");
                                            Console.WriteLine();
                                            pr.bacaMenu(conn);
                                        }
                                        break;
                                    case '3':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Update Data Menu\n");
                                            Console.WriteLine("Masukkan Id Menu yang ingin diupdate : ");
                                            string UpdateMenu = Console.ReadLine();
                                            Console.WriteLine("Masukkan harga baru: ");
                                            string HargaNew = Console.ReadLine();
                                            try
                                            {
                                                pr.updateMenu(UpdateMenu, HargaNew, conn);
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Terjadi Kesalahan Saat Mengupdate Menu\n");
                                            }
                                            break;
                                        }
                                        break;
                                    case '4':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Cari Data Menu");
                                            Console.WriteLine("1. Berdasarkan Nama");
                                            Console.WriteLine("2. Berdasarkan ID");
                                            char ch3 = Convert.ToChar(Console.ReadLine());
                                            switch (ch3)
                                            {
                                                case '1':
                                                    {
                                                        Console.WriteLine("\nMasukkan Nama Menu yang ingin dicari :\n");
                                                        string NamaMenuCari = Console.ReadLine();
                                                        try
                                                        {
                                                            pr.cariMenuNama(NamaMenuCari, conn);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Terjadi kesalahan saat mencari data Menu: " + ex.Message);
                                                        }
                                                        break;
                                                    }
                                                case '2':
                                                    {
                                                        Console.WriteLine("\nMasukkan ID Menu yang ingin dicari :\n");
                                                        string IdMenuCari = Console.ReadLine();
                                                        try
                                                        {
                                                            pr.cariMenuId(IdMenuCari, conn); // Saya berasumsi ada method yang bernama cariMenuId, sesuai dengan nama ID yang ingin dicari.
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Terjadi kesalahan saat mencari data Menu: " + ex.Message);
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        Console.Clear();
                                                        Console.WriteLine("\nOpsi Tidak Valid");
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                        break;
                                    case '5':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Hapus Data Menu\n");
                                            Console.WriteLine("Masukkan Id Menu yang ingin dihapus : ");
                                            string HapusMenu = Console.ReadLine();
                                            if (string.IsNullOrEmpty(HapusMenu))
                                            {
                                                Console.WriteLine("Input tidak boleh kosong!");
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nApakah anda yakin ingin menghapus " + HapusMenu + "? (Y/N)");
                                                string konfir = Console.ReadLine();

                                                if (konfir == "Y" || konfir == "y")
                                                {
                                                    try
                                                    {
                                                        pr.hapusMenu(HapusMenu, conn);
                                                    }
                                                    catch
                                                    {
                                                        Console.WriteLine("Terjadi Kesalahan Saat Menghapus Menu\n");
                                                    }
                                                }
                                                else
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Data Batal Dihapus!");

                                                }
                                                break;
                                            }
                                        }
                                        break;
                                    case '6':
                                        conn.Close();
                                        Console.Clear();
                                        Main(new string[0]);
                                        return;
                                    default:
                                        {
                                            Console.Clear();
                                            Console.WriteLine("\nOpsi Tidak Valid");
                                        }
                                        break;
                                }
                            }
                            break;

                        case '7':
                            {
                                //DISPLAY SUB PELANGGAN
                                Console.Clear();
                                Console.WriteLine("\nPELANGGAN");
                                Console.WriteLine("1. Menambah Data Pelanggan");
                                Console.WriteLine("2. Melihat Data Pelanggan");
                                Console.WriteLine("3. Update Data Pelanggan");
                                Console.WriteLine("4. Search Data Pelanggan");
                                Console.WriteLine("5. Hapus Data Pelanggan");
                                Console.WriteLine("6. Kembali");
                                Console.WriteLine("Masukkan Pilihan (1 - 6) : ");
                                char ch2 = Convert.ToChar(Console.ReadLine());
                                switch (ch2)
                                {
                                    case '1':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Input Data Pelanggan");
                                            Console.WriteLine("Nama Pelanggan : ");
                                            string Nama_Pelanggan = Console.ReadLine();
                                            Console.WriteLine("Nomer Hp : ");
                                            string NomerHp = Console.ReadLine();

                                            //validasi ketika nomer telp tidak di isi pake angka
                                            if (!NomerHp.All(char.IsDigit))
                                            {
                                                Console.WriteLine("Hanya Bisa Di Isi Menggunakan Angka !!!");
                                                break;
                                            }
                                            //validasi ketika data tidak di isi
                                            if (string.IsNullOrEmpty(Nama_Pelanggan) || string.IsNullOrEmpty(NomerHp))
                                            {
                                                Console.WriteLine("Input tidak boleh kosong!");
                                            }
                                            else if (Nama_Pelanggan.Any(char.IsDigit))
                                            {
                                                Console.WriteLine("Nama Pelanggan tidak boleh menggunakan angka");
                                            }
                                            else { 
                                                Console.WriteLine("\n---------------------------------\n");
                                            Console.WriteLine("Nama : " + Nama_Pelanggan);
                                            Console.WriteLine("NO HP : " + NomerHp);
                                            Console.WriteLine("\nApakah data sudah benar? (Y/N)");
                                            string konfir = Console.ReadLine();

                                            if (konfir == "Y" || konfir == "y")
                                            {
                                                try
                                                {
                                                    pr.insertPelanggan(Nama_Pelanggan, NomerHp, conn);
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("\nAnda Tidak Memiliki " +
                                                                   "Akses Untuk Menambah Data");
                                                }
                                            }
                                            else
                                            {
                                                Console.Clear();
                                                Console.WriteLine("Penambahan Data Dibatalkan!");
                                            }
                                            break;
                                            }

                                        }
                                        break;
                                    case '2':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Pelanggan");
                                            Console.WriteLine();
                                            pr.bacaPelanggan(conn);
                                        }
                                        break;
                                    case '3':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Update Data Pelanggan\n");
                                            Console.WriteLine("Masukkan Id Pelanggan yang ingin diupdate : ");
                                            string UpdatePelanggan = Console.ReadLine();
                                            Console.WriteLine("Masukkan Nomer Hp baru : ");
                                            string NewNoHp = Console.ReadLine();
                                            if (string.IsNullOrEmpty(UpdatePelanggan) || string.IsNullOrEmpty(NewNoHp))
                                            {
                                                Console.WriteLine("Input tidak boleh kosong!");
                                            }
                                            else
                                            {

                                            try
                                            {
                                                pr.updatePelanggan(UpdatePelanggan, NewNoHp, conn);
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Terjadi Kesalahan Saat Mengupdate Nomer Telpon\n");
                                            }
                                            break;
                                            }
                                        }
                                        break;
                                    case '4':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Cari Data Pelanggan");
                                            Console.WriteLine("Masukkan Id Pelanggan yang Dicari: ");
                                            string IdPelangganCari = Console.ReadLine();

                                            try
                                            {
                                                pr.cariPelanggan(IdPelangganCari, conn);
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine("Terjadi kesalahan saat mencari data pelanggan: " + ex.Message);
                                            }
                                            break;
                                        }
                                        break;
                                    case '5':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Hapus Data Pelanggan\n");
                                            Console.WriteLine("Masukkan Id Pelanggan yang ingin dihapus : ");
                                            string HapusPelanggan = Console.ReadLine();
                                            if (string.IsNullOrEmpty(HapusPelanggan))
                                            {
                                                Console.WriteLine("Input tidak boleh kosong!");
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nApakah anda yakin ingin menghapus " + HapusPelanggan + "? (Y/N)");
                                                string konfir = Console.ReadLine();

                                                if (konfir == "Y" || konfir == "y")
                                                {
                                                    try
                                                    {
                                                        pr.hapusPelanggan(HapusPelanggan, conn);
                                                    }
                                                    catch
                                                    {
                                                        Console.WriteLine("Terjadi Kesalahan Saat Menghapus Nama\n");
                                                    }
                                                }
                                                else
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Data Batal Dihapus!");

                                                }
                                                break;
                                            }
                                        }
                                        break;
                                    case '6':
                                        conn.Close();
                                        Console.Clear();
                                        Main(new string[0]);
                                        return;
                                    default:
                                        {
                                            Console.Clear();
                                            Console.WriteLine("\nOpsi Tidak Valid");
                                        }
                                        break;
                                }
                            }
                            break;
                        case '8': //HUTANG
                            {
                                //DISPLAY SUB HUTANG
                                Console.Clear();
                                Console.WriteLine("\nHUTANG");
                                Console.WriteLine("1. Menambah Data Hutang");
                                Console.WriteLine("2. Melihat Data Hutang");
                                Console.WriteLine("3. Update Data Hutang");
                                Console.WriteLine("4. Cari Data Hutang");
                                Console.WriteLine("5. Hapus Data Hutang");
                                Console.WriteLine("6. Kembali");
                                Console.WriteLine("Masukkan Pilihan (1 - 6) : ");
                                char ch2 = Convert.ToChar(Console.ReadLine());
                                switch (ch2)
                                {
                                    case '1':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Input Data Hutang");
                                            Console.WriteLine("Id Pelanggan : ");
                                            string Id_Pelanggan = Console.ReadLine();
                                            Console.WriteLine("Id Menu : ");
                                            string Id_Menu = Console.ReadLine();
                                            Console.WriteLine("Masukkan Kuantitas : ");
                                            string Banyaknya = Console.ReadLine();
                                            string Jumlah_Hutang = "";

                                            DateTime Tanggal = DateTime.Now;

                                            if (string.IsNullOrEmpty(Id_Pelanggan) || string.IsNullOrEmpty(Id_Menu) || string.IsNullOrEmpty(Banyaknya))
                                            {
                                                Console.WriteLine("Input tidak boleh kosong!");
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    pr.insertHutang(Id_Pelanggan, Id_Menu, Banyaknya, Tanggal, conn);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("\nAnda Tidak Memiliki " +
                                                                   "Akses Untuk Menambah Data");
                                                }
                                            }
                                            break;

                                        }
                                        break;
                                    case '2':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Hutang");
                                            Console.WriteLine();
                                            pr.bacaHutang(conn);
                                        }
                                        break;
                                    case '3':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Update Data Hutang\n");
                                            Console.WriteLine("Pilih bagian Update");
                                            Console.WriteLine("1. Update Bagian Menu");
                                            Console.WriteLine("2. Update Bagian Banyaknya");
                                            Console.WriteLine("3. Kembali");
                                            char ch3 = Convert.ToChar(Console.ReadLine());
                                            switch(ch3)
                                            {
                                                case '1':
                                                    {
                                                        Console.Clear();
                                                        Console.WriteLine("Masukkan Id Hutang : ");
                                                        string UpdateHutangMenu = Console.ReadLine();
                                                        Console.WriteLine("Masukkan Id Menu baru : ");
                                                        string NewMenu = Console.ReadLine();
                                                        if (string.IsNullOrEmpty(UpdateHutangMenu) || string.IsNullOrEmpty(NewMenu))
                                                        {
                                                            Console.WriteLine("Input tidak boleh kosong!");
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                pr.updateHutangMenu(UpdateHutangMenu, NewMenu, conn);
                                                            }
                                                            catch
                                                            {
                                                                Console.WriteLine("Terjadi Kesalahan saat Mengupdate Menu\n");
                                                            }
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                case '2':
                                                    {
                                                        Console.Clear();
                                                        Console.WriteLine("Masukkan Id Hutang");
                                                        string UpdateHutangJml = Console.ReadLine();
                                                        Console.WriteLine("Masukkan Jumlah baru : ");
                                                        string NewBanyaknya = Console.ReadLine();
                                                        if (string.IsNullOrEmpty(UpdateHutangJml) || string.IsNullOrEmpty(NewBanyaknya))
                                                        {
                                                            Console.WriteLine("Input data tidak boleh kosong");
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                pr.updateHutangJml(UpdateHutangJml, NewBanyaknya, conn);
                                                            }
                                                            catch
                                                            {
                                                                Console.WriteLine("Terjadi Kesalahan saat Mengupdate kuantitas\n");
                                                            }
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                case '3':                                                     
                                                    conn.Close();
                                                    Console.Clear();
                                                    Main(new string[0]);
                                                    return;
                                                default:
                                                    {
                                                        Console.Clear();
                                                        Console.WriteLine("\nOpsi Tidak Valid");
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    case '4':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Cari Data Hutang");
                                            Console.WriteLine("Masukkan Id Hutang yang Dicari: ");
                                            string Id_Hutang = Console.ReadLine();

                                            try
                                            {
                                                pr.cariHutang(Id_Hutang, conn);
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine("Terjadi kesalahan saat mencari data hutang: " + ex.Message);
                                            }
                                            break;
                                        }
                                        break;
                                    case '5':
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Hapus Data Hutang\n");
                                            Console.WriteLine("Masukkan Id Hutang yang ingin dihapus : ");
                                            string HapusHutang = Console.ReadLine();
                                            if (string.IsNullOrEmpty(HapusHutang))
                                            {
                                                Console.WriteLine("Input tidak boleh kosong!");
                                            }
                                            else
                                            {
                                                //bisa ditambahkan y/n
                                                try
                                                {
                                                    pr.hapusHutang(HapusHutang, conn);
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("Terjadi Kesalahan Saat Menghapus Hutang\n");
                                                }
                                                break;
                                            }
                                        }
                                        break;
                                    case '6':
                                        conn.Close();
                                        Console.Clear();
                                        Main(new string[0]);
                                        return;
                                    default:
                                        {
                                            Console.Clear();
                                            Console.WriteLine("\nOpsi Tidak Valid");
                                        }
                                        break;
                                }
                            }
                            break;
                        case '9':
                            {
                                // Kode untuk keluar dari program
                                Environment.Exit(0);
                            }
                            break;
                        default:
                            {
                                Console.WriteLine("\nOpsi Tidak Valid");
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Terjadi Kesalahan! " + ex.Message.ToString());
                    Console.ReadLine();
                }


            }
        }


        // VOID MENU
        public void insertMenu(string Nama_Menu, string Harga, MySqlConnection con)
        {
            try
            {
                // Periksa apakah Nama_Menu sudah ada dalam database
                string checkQuery = "SELECT COUNT(*) FROM Menu WHERE Nama_Menu = @namaMenu";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@namaMenu", Nama_Menu);
                con.Open();
                int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                con.Close();

                if (existingCount > 0)
                {
                    Console.WriteLine("Menu dengan nama yang sama sudah ada dalam database.");
                    Console.ReadLine();
                }
                else
                {
                    string str = "INSERT INTO Menu (Nama_Menu, Harga) VALUES (@namaMenu, @harga)";

                    MySqlCommand cmd = new MySqlCommand(str, con);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@namaMenu", Nama_Menu);
                    cmd.Parameters.AddWithValue("@harga", float.Parse(Harga));

                    con.Open(); // Buka koneksi sebelum eksekusi perintah SQL
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Data Menu berhasil ditambahkan");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Gagal menambahkan data Menu");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi kesalahan saat menambahkan data Menu: " + ex.Message);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close(); // Tutup koneksi setelah eksekusi perintah SQL selesai
                }
            }
        }



        public void bacaMenu(MySqlConnection con)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT Id_Menu, Nama_Menu, Harga FROM Menu", con);
                con.Open();
                MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        Console.Write(r.GetValue(i) + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                r.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi kesalahan saat membaca data menu: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public void updateMenu(string UpdateMenu, string HargaNew, MySqlConnection con)
        {
            try
            {
                // Mendapatkan data menu berdasarkan ID
                string getMenuQuery = "SELECT * FROM Menu WHERE Id_Menu = @idMenu";
                MySqlCommand getMenuCmd = new MySqlCommand(getMenuQuery, con);
                getMenuCmd.Parameters.AddWithValue("@idMenu", UpdateMenu);
                con.Open();
                MySqlDataReader reader = getMenuCmd.ExecuteReader();

                // Memeriksa apakah data menu ditemukan
                if (reader.Read())
                {
                    // Mendapatkan nilai dari kolom-kolom yang dibutuhkan
                    string idMenu = reader["Id_Menu"].ToString();
                    string namaMenu = reader["Nama_Menu"].ToString();
                    string hargaLama = reader["Harga"].ToString();

                    // Menampilkan informasi menu yang akan diupdate
                    Console.WriteLine($"Anda akan mengupdate menu dengan ID: {idMenu}");
                    Console.WriteLine($"Nama Menu: {namaMenu}");
                    Console.WriteLine($"Harga Lama: {hargaLama}");
                    Console.WriteLine($"Harga Baru: {HargaNew}");
                    Console.WriteLine("Apakah Anda yakin ingin melanjutkan update? (Y/N)");
                    string confirmation = Console.ReadLine();

                    if (confirmation.ToUpper() == "Y" || confirmation.ToUpper() == "y")
                    {
                        // Melakukan update harga menu
                        string updateQuery = "UPDATE Menu SET Harga = @harga WHERE Id_Menu = @idMenu";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, con);
                        updateCmd.Parameters.AddWithValue("@idMenu", idMenu);
                        updateCmd.Parameters.AddWithValue("@harga", float.Parse(HargaNew));
                        reader.Close(); // Menutup reader sebelum menjalankan perintah update
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Data Menu berhasil diupdate");
                        }
                        else
                        {
                            Console.WriteLine("Menu tidak ditemukan");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Update menu dibatalkan.");
                    }
                }
                else
                {
                    Console.WriteLine("Menu tidak ditemukan");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saat mengupdate data Menu : " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }


        public void cariMenuNama(string NamaMenu, MySqlConnection con)
        {
            string query = "SELECT Nama_Menu, Harga FROM Menu WHERE Nama_Menu = @Nama_Menu";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Nama_Menu", NamaMenu);

            con.Open();
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                Console.WriteLine("Data Menu yang Cocok:");

                while (reader.Read())
                {
                    Console.WriteLine($"Nama Menu: {reader.GetString(0)} - Harga: {reader.GetFloat(1)}");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Data Menu tidak ditemukan.");
            }

            reader.Close();
            con.Close();
        }
        //revisi jumat
        public void cariMenuId(string IdMenu, MySqlConnection con)
        {
            string query = "SELECT Nama_Menu, Harga FROM Menu WHERE Id_Menu = @idMenu";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Idmenu", IdMenu);

            con.Open();
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                Console.WriteLine("Data Menu yang Cocok:");

                while (reader.Read())
                {
                    Console.WriteLine($"Nama Menu: {reader.GetString(0)} - Harga: {reader.GetFloat(1)}");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Data Menu tidak ditemukan.");
            }

            reader.Close();
            con.Close();
        }

        public void hapusMenu(string Id_Menu, MySqlConnection con)
        {
            con.Open();
            string str = "DELETE FROM Menu WHERE Id_Menu = @im";
            MySqlCommand cmd = new MySqlCommand(str, con);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Add(new MySqlParameter("@im", Id_Menu));
            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                Console.WriteLine("Data Berhasil Dihapus");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Data Tidak Ditemukan");
                Console.ReadLine();
            }
            con.Close();
        }


        // VOID PELANGGAN

        public void insertPelanggan(string Nama_Pelanggan, string NomerHp, MySqlConnection con)
        {
            con.Open();
            string str = "";
            str = "INSERT INTO Pelanggan (Nama_Pelanggan, No_HP) VALUES (@np, @nhp)";

            MySqlCommand cmd = new MySqlCommand(str, con);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Add(new MySqlParameter("@np", Nama_Pelanggan));
            cmd.Parameters.Add(new MySqlParameter("@nhp", NomerHp));
            cmd.ExecuteNonQuery();
            Console.WriteLine("Data Pelanggan berhasil ditambahkan");
            Console.ReadLine();
            con.Close();
        }

        public void bacaPelanggan(MySqlConnection con)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT Id_Pelanggan, Nama_Pelanggan, No_HP FROM Pelanggan", con);
                con.Open();
                MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        Console.Write(r.GetValue(i) + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                r.Close();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi kesalahan saat membaca data pelanggan: " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public void updatePelanggan(string UpdatePelanggan, string NomerHpNew, MySqlConnection con)
        {
            try
            {
                string getPelangganQuery = "SELECT * FROM Pelanggan WHERE Id_Pelanggan = @ip";
                MySqlCommand getPelangganCmd = new MySqlCommand(getPelangganQuery, con);
                getPelangganCmd.Parameters.AddWithValue("@ip", UpdatePelanggan);
                con.Open();
                MySqlDataReader reader = getPelangganCmd.ExecuteReader();

                if (reader.Read())
                {
                    string idPelanggan = reader["Id_Pelanggan"].ToString();
                    string Nama_Pelanggan = reader["Nama_Pelanggan"].ToString();
                    string NomerHp = reader["No_HP"].ToString();

                    Console.WriteLine($"Anda akan mengupdate pelanggan dengan ID: {idPelanggan}");
                    Console.WriteLine($"Nama Pelanggan: {Nama_Pelanggan}");
                    Console.WriteLine($"Nomer Hp Lama: {NomerHp}");
                    Console.WriteLine($"Nomer Hp Baru: {NomerHpNew}");
                    Console.WriteLine("Apakah Anda yakin ingin melanjutkan update? (Y/N)");
                    string confirmation = Console.ReadLine();
                    if (confirmation.ToUpper() == "Y" || confirmation.ToUpper() == "y")
                    {
                        string str = "UPDATE Pelanggan SET No_HP = @nhp WHERE Id_Pelanggan = @ip";
                        MySqlCommand cmd = new MySqlCommand(str, con);
                        cmd.Parameters.AddWithValue("@ip", UpdatePelanggan);
                        cmd.Parameters.AddWithValue("@nhp", NomerHpNew);
                        reader.Close();
                        int rowsAffected2 = cmd.ExecuteNonQuery();

                        if (rowsAffected2 > 0)
                        {
                            Console.WriteLine("Data Pelanggan berhasil diupdate");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Pelanggan tidak ditemukan");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Update Pelanggan Dibatalkan");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Pelanggan Tidak Ditemukan");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saat mengupdate data Pelanggan : " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                con.Close();
            }
        }

        public void cariPelanggan(string Id_Pelanggan, MySqlConnection con)
        {
            string query = "SELECT Nama_Pelanggan, No_Hp FROM Pelanggan WHERE Id_Pelanggan = @ip";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ip", Id_Pelanggan);

            con.Open();
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                Console.WriteLine("Data Pelanggan yang Cocok:");


                while (reader.Read())
                {
                    Console.WriteLine($"Nama: {reader.GetString(0)} - No. HP: {reader.GetString(1)}");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Data Pelanggan tidak ditemukan.");
                Console.ReadLine();
            }

            reader.Close();
            con.Close();
        }

        public void hapusPelanggan(string IdPelanggan, MySqlConnection con)
        {
            con.Open();
            string str = "DELETE FROM Pelanggan WHERE Id_Pelanggan = @ip";
            MySqlCommand cmd = new MySqlCommand(str, con);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Add(new MySqlParameter("@ip", IdPelanggan));
            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                Console.WriteLine("Data Berhasil Dihapus");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Data Tidak Ditemukan");
                Console.ReadLine();
            }
            con.Close();
        }

        // VOID HUTANG
        public void insertHutang(string Id_Pelanggan, string Id_Menu, string Banyaknya, DateTime Tanggal, MySqlConnection con)
        {
            try
            {
                string queryGetPelanggan = "SELECT Nama_Pelanggan FROM Pelanggan WHERE Id_Pelanggan = @idPelanggan";
                MySqlCommand cmdGetPelanggan = new MySqlCommand(queryGetPelanggan, con);
                cmdGetPelanggan.Parameters.AddWithValue("@idPelanggan", Id_Pelanggan);

                string queryGetMenu = "SELECT Nama_Menu FROM Menu WHERE Id_Menu = @idMenu";
                MySqlCommand cmdGetMenu = new MySqlCommand(queryGetMenu, con);
                cmdGetMenu.Parameters.AddWithValue("@idMenu", Id_Menu);

                con.Open();
                string namaPelanggan = cmdGetPelanggan.ExecuteScalar().ToString();
                string namaMenu = cmdGetMenu.ExecuteScalar().ToString();

                string queryGetPrice = "SELECT Harga FROM Menu WHERE Id_Menu = @idMenu";
                MySqlCommand cmdGetPrice = new MySqlCommand(queryGetPrice, con);
                cmdGetPrice.Parameters.AddWithValue("@idMenu", Id_Menu);
                float hargaMenu = Convert.ToSingle(cmdGetPrice.ExecuteScalar());

                // Menghitung jumlah hutang
                float jumlahHutang = hargaMenu * Convert.ToSingle(Banyaknya);

                Console.WriteLine("Pencatatan Hutang ");
                Console.WriteLine($"Nama pelanggan : {namaPelanggan}");
                Console.WriteLine($"Nama menu : {namaMenu}");
                Console.WriteLine($"Banyaknya: {Banyaknya}");
                Console.WriteLine($"Tanggal: {Tanggal}");
                Console.WriteLine($"Jumlah Hutang : {jumlahHutang}");
                Console.WriteLine("Apakah Anda yakin ingin melanjutkan? (Y/N)");
                string confirmation = Console.ReadLine();

                if (confirmation.ToUpper() == "Y" || confirmation.ToUpper() == "y")
                {
                    string query = "INSERT INTO Hutang (Id_Pelanggan, Id_Menu, Banyaknya, Tanggal, Jumlah_Hutang) VALUES (@idPelanggan, @idMenu, @banyaknya, @tanggal, @jumlahHutang)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@idPelanggan", Id_Pelanggan);
                    cmd.Parameters.AddWithValue("@idMenu", Id_Menu);
                    cmd.Parameters.AddWithValue("@banyaknya", Banyaknya);
                    cmd.Parameters.AddWithValue("@tanggal", Tanggal);
                    cmd.Parameters.AddWithValue("@jumlahHutang", jumlahHutang);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Data Hutang berhasil ditambahkan");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Gagal menambahkan data Hutang");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Penambahan Hutang Dibatalkan");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                con.Close();
            }
        }

        public void bacaHutang(MySqlConnection con)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(@"SELECT H.Id_Hutang, H.Tanggal, P.Nama_Pelanggan, H.Id_Menu, M.Nama_Menu AS Menu, M.Harga, H.Banyaknya, H.Jumlah_Hutang
                    FROM Hutang H
                    INNER JOIN Pelanggan P ON H.Id_Pelanggan = P.Id_Pelanggan
                    INNER JOIN Menu M ON H.Id_Menu = M.Id_Menu;", con);
                con.Open();
                MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        Console.Write(r.GetValue(i) + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                r.Close();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi kesalahan saat membaca data menu: " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public void updateHutangMenu(string UpdateHutangMenu, string NewMenu, MySqlConnection con)
        {
            try
            {
                con.Open();

                string getHutangQuery = "SELECT * FROM Hutang WHERE Id_Hutang = @ih";
                MySqlCommand getHutangCmd = new MySqlCommand(getHutangQuery, con);
                getHutangCmd.Parameters.AddWithValue("@ih", UpdateHutangMenu);
                MySqlDataReader reader = getHutangCmd.ExecuteReader();

                if (reader.Read())
                {
                    string idHutang = reader["Id_Hutang"].ToString();
                    string idMenu = reader["Id_Menu"].ToString();
                    string idPelanggan = reader["Id_Pelanggan"].ToString();
                    string banyaknya = reader["Banyaknya"].ToString();
                    DateTime tanggal = Convert.ToDateTime(reader["Tanggal"]);
                    float jumlahHutang = Convert.ToSingle(reader["Jumlah_Hutang"]);

                    reader.Close();

                    string getNewMenuNameQuery = "SELECT Nama_Menu FROM Menu WHERE Id_Menu = @mn";
                    MySqlCommand getNewMenuNameCmd = new MySqlCommand(getNewMenuNameQuery, con);
                    getNewMenuNameCmd.Parameters.AddWithValue("@mn", NewMenu);
                    string newMenuName = getNewMenuNameCmd.ExecuteScalar().ToString();

                    string getMenuPriceQuery = "SELECT Harga FROM Menu WHERE Id_Menu = @mn";
                    MySqlCommand getMenuPriceCmd = new MySqlCommand(getMenuPriceQuery, con);
                    getMenuPriceCmd.Parameters.AddWithValue("@mn", NewMenu);
                    float hargaMenuBaru = Convert.ToSingle(getMenuPriceCmd.ExecuteScalar());
                    float jumlahHutangBaru = hargaMenuBaru * Convert.ToSingle(banyaknya);

                    Console.WriteLine($"Update hutang dengan ID: {idHutang}");
                    Console.WriteLine($"ID Menu: {idMenu}");
                    Console.WriteLine($"ID Pelanggan: {idPelanggan}");
                    Console.WriteLine($"Tanggal: {tanggal}\n");
                    Console.WriteLine($"Id Menu Baru: {NewMenu}");
                    Console.WriteLine($"Nama Menu: {newMenuName}");
                    Console.WriteLine($"Harga Menu Baru: {hargaMenuBaru}");
                    Console.WriteLine($"Banyaknya: {banyaknya}");
                    Console.WriteLine($"Jumlah Hutang : {jumlahHutangBaru}");
                    Console.WriteLine("Apakah Anda yakin ingin melanjutkan update? (Y/N)");
                    string confirmation = Console.ReadLine();
                    if (confirmation.ToUpper() == "Y" || confirmation.ToUpper() == "y")
                    {
                        reader.Close();

                        string str = "UPDATE Hutang SET Id_Menu = @mn WHERE Id_Hutang = @ih";
                        MySqlCommand cmd = new MySqlCommand(str, con);
                        cmd.Parameters.AddWithValue("@ih", UpdateHutangMenu);
                        cmd.Parameters.AddWithValue("@mn", NewMenu);
                        reader.Close();
                        int rowsAffected2 = cmd.ExecuteNonQuery();

                        if (rowsAffected2 > 0)
                        {
                            Console.WriteLine("Data Hutang berhasil diupdate");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Hutang tidak ditemukan");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Update Hutang Dibatalkan");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Hutang Tidak Ditemukan");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saat mengupdate data Hutang : " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                con.Close();
            }
        }



        public void updateHutangJml(string UpdateHutangJml, string NewBanyaknya, MySqlConnection con)
        {
            string getMenuInfoQuery = "SELECT h.Id_Menu, m.Harga FROM Hutang h JOIN Menu m ON h.Id_Menu = m.Id_Menu WHERE h.Id_Hutang = @ih";

            MySqlCommand cmd = new MySqlCommand(getMenuInfoQuery, con);
            cmd.Parameters.AddWithValue("@ih", UpdateHutangJml);

            try
            {
                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int idMenu = reader.GetInt32("Id_Menu");
                    float hargaMenu = reader.GetFloat("Harga");

                    reader.Close();

                    // Menghitung jumlah hutang baru
                    float newJumlahHutang = Convert.ToInt32(NewBanyaknya) * hargaMenu;

                    Console.WriteLine($"Update hutang dengan ID: {UpdateHutangJml}");
                    Console.WriteLine($"ID Menu: {idMenu}");
                    Console.WriteLine($"Banyaknya Baru: {NewBanyaknya}");
                    Console.WriteLine($"Harga Menu: {hargaMenu}");
                    Console.WriteLine($"Jumlah Hutang Baru: {newJumlahHutang}");
                    Console.WriteLine("Apakah Anda yakin ingin melanjutkan update? (Y/N)");
                    string confirmation = Console.ReadLine();
                    if (confirmation.ToUpper() == "Y" || confirmation.ToUpper() == "y")
                    {

                        //string str = "UPDATE Menu SET Harga = @harga WHERE Nama_Menu = @namaMenu";
                        string str = "UPDATE Hutang SET Banyaknya = @bn, Jumlah_Hutang = @jh WHERE Id_Hutang = @ih";

                        MySqlCommand cmd2 = new MySqlCommand(str, con);
                        cmd2.Parameters.AddWithValue("@bn", NewBanyaknya);
                        cmd2.Parameters.AddWithValue("@jh", newJumlahHutang);
                        cmd2.Parameters.AddWithValue("@ih", UpdateHutangJml);

                        int rowsAffected = cmd2.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Data Hutang berhasil diupdate");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Hutang tidak ditemukan");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Hutang tidak ditemukan");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Hutang tidak ditemukan");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saat mengupdate data Hutang : " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                con.Close();
            }
        }


        public void cariHutang(string Id_Hutang, MySqlConnection con)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(@"SELECT H.Id_Hutang, H.Tanggal, P.Nama_Pelanggan, H.Id_Menu, M.Nama_Menu AS Menu, M.Harga, H.Banyaknya,
                H.Jumlah_Hutang FROM Hutang H INNER JOIN Pelanggan P ON H.Id_Pelanggan = P.Id_Pelanggan INNER JOIN Menu M ON H.Id_Menu = M.Id_Menu WHERE Id_Hutang = @ih;", con);
                cmd.Parameters.AddWithValue("@ih", Id_Hutang);
                con.Open();
                MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        Console.Write(r.GetValue(i) + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                r.Close();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi kesalahan saat membaca data menu: " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public void hapusHutang(string HapusHutang, MySqlConnection con)
        {
            try
            {
                string getHutangQuery = "SELECT Hutang.Id_Hutang, Hutang.Tanggal, Hutang.Jumlah_Hutang, Pelanggan.Nama_Pelanggan " +
                                        "FROM Hutang " +
                                        "INNER JOIN Pelanggan ON Hutang.Id_Pelanggan = Pelanggan.Id_Pelanggan " +
                                        "WHERE Hutang.Id_Hutang = @ih";
                MySqlCommand getHutangCmd = new MySqlCommand(getHutangQuery, con);
                getHutangCmd.Parameters.AddWithValue("@ih", HapusHutang);
                con.Open();
                MySqlDataReader reader = getHutangCmd.ExecuteReader();

                if (reader.Read())
                {
                    //untuk membaca data yang ada di database
                    string idHutang = reader["Id_Hutang"].ToString();
                    string tanggal = reader["Tanggal"].ToString();
                    string jumlahHutang = reader["Jumlah_Hutang"].ToString();
                    string namaPelanggan = reader["Nama_Pelanggan"].ToString();

                    Console.WriteLine($"Hapus data hutang dengan ID: {idHutang}\n");
                    Console.WriteLine($"Nama Pelanggan: {namaPelanggan}");
                    Console.WriteLine($"Tanggal: {tanggal}");
                    Console.WriteLine($"Jumlah Hutang: {jumlahHutang}");
                    Console.WriteLine("Apakah Anda yakin ingin melanjutkan penghapusan? (Y/N)");
                    string confirmation = Console.ReadLine();
                    if (confirmation.ToUpper() == "Y" || confirmation.ToUpper() == "y")
                    {
                        reader.Close();
                        string deleteQuery = "DELETE FROM Hutang WHERE Id_Hutang = @ih";
                        MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, con);
                        deleteCmd.Parameters.AddWithValue("@ih", HapusHutang);
                        int rowsAffected = deleteCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Data Hutang berhasil dihapus");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Data Hutang tidak ditemukan");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Penghapusan data Hutang dibatalkan");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Data Hutang Tidak Ditemukan");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saat menghapus data Hutang : " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                con.Close();
            }
        }
    }
}