using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidModel.Selektion
{
    public class SelektionBase
    {
        #region Felder
        /// <summary>
        /// Gibt an, ob die Selektions einschließend oder ausschließend ist
        /// </summary>
        private bool _isAusschliessendeSelektion;

        /// <summary>
        /// Gibt den Typ der Selektion an
        /// </summary>
        private SelektionTyp _typ;

        /// <summary>
        /// Beim Selektionstyp Zeichenkette, gibt dies den selektierten Wert (decimal) an
        /// Beim Selektionstyp größer/kleiner, gibt dies den selektierten Wert (decimal) an
        /// </summary>
        private object _wert;
        #endregion Felder

        #region ctor
        /// <summary>
        /// Initialisiert eine neue Instanz des Objekts
        /// </summary>
        /// <param name="ausschliessend">Gibt an, ob die Selektions einschließend oder ausschließend ist</param>
        /// <param name="typ">Gibt den Typ der Selektion an</param>
        /// <param name="wert">
        /// Beim Selektionstyp Zeichenkette, gibt dies den selektierten Wert (string) an
        /// Beim Selektionstyp größer/kleiner, gibt dies den selektierten Wert (decimal) an
        /// </param>
        protected SelektionBase(bool ausschliessend, SelektionTyp typ, object wert)
        {
            IsAusschliessendeSelektion = ausschliessend;
            Typ = typ;
            Wert = wert;
        }
        #endregion ctor

        #region Eigenschaften
        /// <summary>
        /// Gibt an, ob die Selektions einschließend oder ausschließend ist
        /// </summary>
        public bool IsAusschliessendeSelektion { get => _isAusschliessendeSelektion; set => _isAusschliessendeSelektion = value; }

        /// <summary>
        /// Gibt den Typ der Selektion an
        /// </summary>
        public virtual SelektionTyp Typ { get => _typ; protected set => _typ = value; }

        /// <summary>
        /// Beim Selektionstyp Zeichenkette, gibt dies den selektierten Wert (decimal) an
        /// Beim Selektionstyp größer/kleiner, gibt dies den selektierten Wert (decimal) an
        /// </summary>
        protected virtual object Wert { get => _wert; set => _wert = value; }
        #endregion Eigenschaften
    }
}
