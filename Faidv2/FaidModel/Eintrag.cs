using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidModel
{
    [Serializable]
    public class Eintrag : EintragBase
    {
        #region Felder
        /// <summary>
        /// Typ der Verbuchung
        /// </summary>
        private BuchungsTyp _typ;

        /// <summary>
        /// Zeitpunkt der Verbuchung
        /// </summary>
        private DateTime _datum;

        /// <summary>
        /// Gibt an, ob die Verbuchung eine periodische Verbuchung war
        /// </summary>
        private bool _periodischeVerbuchung;
        #endregion Felder

        #region Konstruktor
        /// <summary>
        /// Legt einen neuen Eintrag an
        /// </summary>
        /// <param name="typ">Typ des Eintrags</param>
        /// <param name="datum">Datum der Kontoausführung</param>
        /// <param name="wert">Wert der Transaktion</param>
        /// <param name="kommentar">Kommentar zur Transaktion</param>
        /// <param name="periodischeVerbuchung">Gibt an, ob dies eine periodische Verbuchung ist</param>
        /// <throws><c>DateFutureException</c>, wenn das angegebene Datum in der Zukunft liegt</throws>
        public Eintrag(BuchungsTyp typ, DateTime datum, decimal wert, string kommentar, bool periodischeVerbuchung) : base(wert, kommentar)
        {
            if (datum > DateTime.Now)
                throw new DateFutureException(String.Format("Das angegebene Datum {0} liegt in der Zukunft!", datum));

            _typ = typ;
            _datum = datum;
            _periodischeVerbuchung = periodischeVerbuchung;
        }
        #endregion Konstruktor

        #region Eigenschaften
        /// <summary>
        /// Typ der Verbuchung
        /// </summary>
        public BuchungsTyp Typ { get => _typ; }

        /// <summary>
        /// Zeitpunkt der Verbuchung
        /// </summary>
        public DateTime Datum { get => _datum; }

        /// <summary>
        /// Gibt an, ob die Verbuchung eine periodische Verbuchung war
        /// </summary>
        public bool PeriodischeVerbuchung { get => _periodischeVerbuchung; }

        // Beim Hinzufügen neuer Eigenschaften müssen diese in Maske1.cs in der Kontoeigenschaft an das dgv gebunden werden!
        #endregion Eigenschaften
    }
}
