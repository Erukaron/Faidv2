using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidView.M2
{
    /// <summary>
    /// Stellt Typen für M2 bereit, die den Modus angeben
    /// </summary>
    public enum M2ModusTypen : ushort
    {
        Dummy,
        Bewegung,
        Einkommen,
        Ausgaben,
        Zinsen
    }
}
