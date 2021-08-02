using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidModel
{
    [Serializable]
    public class EintragBase
    {
        #region Felder
        /// <summary>
        /// Erstellungszeitpunkt
        /// </summary>
        private DateTime _erstellt;

        /// <summary>
        /// Zu verbuchender Wert
        /// </summary>
        private decimal _wert;

        /// <summary>
        /// Kommentar zur Verbuchung
        /// </summary>
        private string _kommentar;
        #endregion Felder

        #region Konstruktor
        /// <summary>
        /// Legt einen neuen Eintrag an
        /// </summary>
        /// <param name="wert">Wert für jede Verbuchung</param>
        /// <param name="kommentar">Kommentar zu jeder Verbuchung</param>
        public EintragBase(decimal wert, string kommentar)
        {
            _erstellt = DateTime.Now;
            _wert = wert;
            _kommentar = kommentar;
        }
        #endregion Konstruktor

        #region Eigenschaften
        /// <summary>
        /// Erstellungszeitpunkt
        /// </summary>
        public DateTime Erstellt { get => _erstellt; }

        /// <summary>
        /// Zu verbuchender Wert
        /// </summary>
        public decimal Wert { get => _wert; }

        /// <summary>
        /// Kommentar zur Verbuchung
        /// </summary>
        public string Kommentar { get => _kommentar; }
        #endregion Eigenschaften
    }
}
