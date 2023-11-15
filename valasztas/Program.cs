using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace valasztas
{
    internal class Program
    {
        public static List<Szavazatok> szavazat = new();
        public static readonly  int JogosultakSzama = 12345;
        static void Main(string[] args)
        {
            FeladatKiir(1, "Fájl beolvasása...");
            FajlFeltoltSQL();

            FeladatKiir(2, $"A helyhatósági választáson {szavazat.Count} képviselőjelölt indult.");

            Console.WriteLine("Adja meg a keresett képviselő vezetéknevét (üresen hagyva az érték 'Karfiol' lesz).");
            string input=Console.ReadLine();

            // string nev = input != "" ? input : "Karfiol Ede";
            string Vnev = !string.IsNullOrEmpty(input) ? input : "Karfiol";
            Console.WriteLine("Adja meg a keresett képviselő Keresztnevét (üresen hagyva az érték 'Ede' lesz).");
            input= Console.ReadLine();
            string Knev = !string.IsNullOrEmpty(input) ? input : "Ede";

            FeladatKiir(3,F3(Knev,Vnev));
            FeladatKiir(4, F4());
            FeladatKiir(5);
            foreach(string x in F5())
            {
                Console.WriteLine(x);
            }
            FeladatKiir(6);
            foreach (string x in F6())
            {
                Console.WriteLine(x);
            }
            FeladatKiir(7,"Adatok kiírása 'kepviselok.txt' fájlba...");
            F7();
        }

        static void FeladatKiir(byte fszam, string szoveg = "")
        {
            
            Console.WriteLine($"{fszam}. feladat:\t{szoveg}");
        }

        static void Fajlfeltolt()
        {
            try
            {
                FileStream fs = new FileStream("szavazatok.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string[] sor;
                while (!sr.EndOfStream)
                {
                    sor = sr.ReadLine().Split(' ');
                    szavazat.Add(new Szavazatok(Convert.ToByte( sor[0]),Convert.ToInt32(sor[1]), sor[2], sor[3], sor[4]));
                }
                fs.Close();
                sr.Close();
                Console.WriteLine("Sikeres feltöltés");
            }
            catch (Exception)
            {

                Console.WriteLine("Sikertelen feltöltés");
            }



        }

        static void FajlFeltoltSQL()
        {
            string connStr = "server=localhost;userid=root;password=;database=valasztasok";

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Kapcsolódás az adatbázishoz...");
                conn.Open();
                var sql = "SELECT * FROM szavazatok_txt";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr=cmd.ExecuteReader();
                while (rdr.Read())
                {
                    szavazat.Add(new Szavazatok(Convert.ToByte(rdr[0]), Convert.ToInt32(rdr[1]), rdr[2].ToString(), rdr[3].ToString(), rdr[4].ToString()));
                }
                Console.WriteLine("Sikeres kapcsolódás");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sikertelen kapcsolódás");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        static string F3(string knev, string vnev)
        {
            //METÓDUS SZINTAXIS:

            //var szavazatKapott = (from sz in szavazat
            //                      where sz.Knev == knev && sz.Vnev == vnev
            //                      select sz.Szavazat).Sum();


            //QUERY SZINTAXIS:

            var szavazatKapott = szavazat.Where(sz => sz.Knev == knev && sz.Vnev == vnev).Sum(sz => sz.Szavazat);

             return szavazatKapott != 0 ? $"{vnev} {knev} összesen {szavazatKapott} szavazatott kapott." :"Ilyen nevű képviselőjelölt nem szerepel a nyilvántartásban!";


                                
        }
        static string F4()
        {
            int szavazok=szavazat.Sum(sz=> sz.Szavazat);
            return $"A választáson {szavazok} állampolgár, a jogosultak {(double)szavazok/JogosultakSzama*100:0.00}%-a vett részt.";
            
        }



        static List<string> F5()
        {
            List<string> eredmenyek = new();
            int osszesSzavazat = szavazat.Sum(sz => sz.Szavazat);

            var partokSzerint = szavazat
                .GroupBy(sz => sz.Part == "-" ? "Független jelöltek" : sz.Part)
                .Select(csoport => new
                {
                    Part = csoport.Key,
                    SzavazatArany = Math.Round(csoport.Sum(sz => sz.Szavazat) / (double)osszesSzavazat * 100, 2)
                });

            foreach (var part in partokSzerint)
            {
                eredmenyek.Add($"{part.Part}= {part.SzavazatArany}%");
            }
            return eredmenyek;
        }

        static List<string> F6()
        {
            List<string> eredmenyek = new();

            var maxSzavazat = szavazat.Max(sz => sz.Szavazat);

            var legtobbSzavazatotKapottJeloltek = szavazat
                .Where(sz => sz.Szavazat == maxSzavazat)
                .Select(sz => new
                {
                    VezetekNev = sz.Vnev,
                    UtoNev = sz.Knev,
                    Part = sz.Part == "-" ? "független" : sz.Part
                });

            foreach (var jelolt in legtobbSzavazatotKapottJeloltek)
            {
                eredmenyek.Add($"{jelolt.VezetekNev} {jelolt.UtoNev}, {jelolt.Part}");
            }
            return eredmenyek;
        }

        static void F7()
        {
            //METÓDUS SZINTAXIS:

            //var kepviselok = szavazat
            //.GroupBy(sz => sz.Sorszam)
            //.Select(csoport => new
            //{
            //    Sorszam = csoport.Key,
            //    Gyoztes = csoport.OrderByDescending(sz => sz.Szavazat).First()
            //})
            //.OrderBy(k => k.Sorszam)  
            //.Select(k => $"{k.Sorszam} {k.Gyoztes.Vnev} {k.Gyoztes.Knev} {k.Gyoztes.Part.Replace("-", "független")}");



            //QUERY SZINTAXIS:
            var kepviselok = from sz in szavazat
                        group sz by sz.Sorszam into valasztokeruletGroup
                        let maxSzavazat = valasztokeruletGroup.Max(x => x.Szavazat)
                        select new
                        {
                            Sorszam = valasztokeruletGroup.Key,
                            Gyoztes = (from sz in valasztokeruletGroup
                                       where sz.Szavazat == maxSzavazat
                                       select sz).FirstOrDefault()
                        } into gyoztesek
                        orderby gyoztesek.Sorszam
                        select $"{gyoztesek.Sorszam} {gyoztesek.Gyoztes.Vnev} {gyoztesek.Gyoztes.Knev} {(gyoztesek.Gyoztes.Part == "-" ? "független" : gyoztesek.Gyoztes.Part)}";


            using (StreamWriter sw = new StreamWriter("kepviselok.txt"))
            {
                foreach (var kepviselo in kepviselok)
                {
                    sw.WriteLine(kepviselo);
                }
            }
        }
    }
}