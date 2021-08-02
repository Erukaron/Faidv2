using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidModel
{
    [Serializable]
    public class DauerEintrag : EintragBase
    {
        #region Felder
        /// <summary>
        /// Typ der Einkommen/Ausgaben
        /// </summary>
        private DauerBuchungsTyp _typ;

        /// <summary>
        /// Falls als Typ Benutzerdefiniert gewählt ist, werden hier die Zeitpunkte gespeichert, zu denen eine Verbuchung ausgeführt wird
        /// </summary>
        private List<DateTime> _benutzerdefinierteZeitpunkte;
        #endregion Felder

        #region Konstruktor
        /// <summary>
        /// Legt einen neuen Dauereintrag an
        /// </summary>
        /// <param name="typ">Typ des Dauereintrags -> Täglich, Wöchentlich, Monatlich, Jährlich oder Benutzerdefiniert</param>
        /// <param name="wert">Wert für jede Verbuchung</param>
        /// <param name="kommentar">Kommentar zu jeder Verbuchung</param>
        public DauerEintrag(DauerBuchungsTyp typ, decimal wert, string kommentar) : base(wert, kommentar)
        {
            if (typ != DauerBuchungsTyp.Monatlich)
                throw new ArgumentException(String.Format("Aktuell ist nur der monatliche Typ zugelassen!"), "typ");

            _typ = typ;

            if (_typ == DauerBuchungsTyp.Benutzerdefiniert)
                _benutzerdefinierteZeitpunkte = new List<DateTime>();
        }
        #endregion Konstruktor

        #region Eigenschaften
        /// <summary>
        /// Typ der Einkommen/Ausgaben
        /// </summary>
        public DauerBuchungsTyp Typ { get => _typ; }

        /// <summary>
        /// Falls als Typ Benutzerdefiniert gewählt ist, werden hier die Zeitpunkte gespeichert, zu denen eine Verbuchung ausgeführt wird
        /// </summary>
        public List<DateTime> BenutzerdefinierteZeitpunkte { get => _benutzerdefinierteZeitpunkte; }
        #endregion Eigenschaften
    }
}
