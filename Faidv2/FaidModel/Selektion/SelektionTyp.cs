using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Faidv2.FaidView.M3;

namespace Faidv2.FaidModel.Selektion
{
    /// <summary>
    /// Gibt Typen zur Selektion an
    /// </summary>
    public enum SelektionTyp
    {
        zeichenkette,
        groesser,
        kleiner
    }

    public class SelektionTypen
    {
        public static string[] Bezeichnung = { M3Ressourcen.TypZeichenkette, M3Ressourcen.TypGroesser, M3Ressourcen.TypKleiner };
    }
}
