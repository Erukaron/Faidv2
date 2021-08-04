using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidModel.Selektion
{
    public class SelektionGroesse : SelektionBase
    {
        #region ctor
        /// <summary>
        /// Initialisiert eine neue Instanz des Objekts
        /// </summary>
        /// <param name="ausschliessend"><paramref name="ausschliessend"/></param>
        /// <param name="kleiner">Gibt an, ob die Selektion auf Objekte angewendet wird, die einen kleineren Wert haben als der wert-Parameter</param>
        /// <param name="wert">selektierter Wert</param>
        public SelektionGroesse(bool ausschliessend, bool kleiner, decimal wert) : base(ausschliessend, kleiner ? SelektionTyp.kleiner : SelektionTyp.groesser, wert)
        {

        }
        #endregion ctor

        #region Eigenschaften
        /// <summary>
        /// Gibt an, ob Objkete selektiert werden, die kleiner als hier angegeben sind
        /// </summary>
        public bool IsKleiner
        {
            get
            {
                return Typ == SelektionTyp.kleiner;
            }
        }
        #endregion Eigenschaften

        #region Methoden
        /// <summary>
        /// Gibt den Wert zur Selektion zurück
        /// </summary>
        /// <returns><Selektionswert/returns>
        public decimal GetWert()
        {
            return (decimal)base.Wert;
        }

        /// <summary>
        /// Legt den Wert zur Selektion fest
        /// </summary>
        /// <param name="wert">Selektionswert</param>
        public void SetWert(decimal wert)
        {
            base.Wert = wert;
        }
        #endregion Methoden
    }
}
