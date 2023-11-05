using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace valasztas
{
    internal class Szavazatok
    {
        private byte _sorszam;
        private int _szavazatok;
        private string _vNev, _kNev, _part;
        public Szavazatok(byte sorszam, int szavazatok, string vNev, string kNev, string part) //Az első két adat a választókerület sorszáma és a képviselőjelöltre leadott szavazatokszáma. Ezt a jelölt vezeték- és utóneve, majd a jelöltet támogató párt hivatalos rövidítése követi. Független jelöltek esetében a párt rövidítése helyett egy kötőjel szerepel. Minden képviselőjelöltnek pontosan egy utóneve van.
        {
            _sorszam = sorszam;
            _szavazatok = szavazatok;
            _vNev = vNev;
            _kNev = kNev;
            _part = part;

        }
        public byte Sorszam { get { return _sorszam; } }
        public int Szavazat { get { return _szavazatok; } }
        public string Vnev { get { return _vNev; } }
        public string Knev  { get { return _kNev; } }
        public string Part { get { return _part; } }
    }
}
