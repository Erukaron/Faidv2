using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidModel
{
    public enum DauerBuchungsTyp : short
    {
        Dummy = -1,
        Sonntags = 0,
        Montags = 1,
        Dienstags = 2,
        Mittwochs = 3,
        Donnerstags = 4,
        Freitags = 5,
        Samstags = 6,
        Woechentlich = 16,
        Monatlich = 32,
        Jaehrlich = 128,
        /// <summary>
        /// Es können Daten angegeben werden, zu denen eine Verbuchung ausgeführt wird
        /// </summary>
        Benutzerdefiniert = 255
    }
}
