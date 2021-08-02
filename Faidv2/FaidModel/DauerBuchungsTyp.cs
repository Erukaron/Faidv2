using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidModel
{
    public enum DauerBuchungsTyp : short
    {
        Dummy,
        Sonntags,
        Montags,
        Dienstags,
        Mittwochs,
        Donnerstags,
        Freitags,
        Samstags,
        Woechentlich,
        Monatlich,
        Jaehrlich,
        /// <summary>
        /// Es können Daten angegeben werden, zu denen eine Verbuchung ausgeführt wird
        /// </summary>
        Benutzerdefiniert
    }
}
