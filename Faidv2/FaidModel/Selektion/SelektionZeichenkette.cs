using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidModel.Selektion
{
    public class SelektionZeichenkette : SelektionBase
    {
        #region ctor
        /// <summary>
        /// Initialisiert eine neue Instanz des Objekts
        /// </summary>
        /// <param name="ausschliessend"><paramref name="ausschliessend"/></param>
        /// <param name="wert">selektierter Wert</param>
        public SelektionZeichenkette(bool ausschliessend = false, string wert = "") : base(ausschliessend, SelektionTyp.zeichenkette, wert)
        {

        }
        #endregion ctor

        #region Eigenschaften
        /// <summary>
        /// Gibt den Typ der Selektion an
        /// </summary>
        public override SelektionTyp Typ { get => SelektionTyp.zeichenkette; }
        #endregion Eigenschaften

        #region Methoden
        /// <summary>
        /// Gibt den Wert zur Selektion zurück
        /// </summary>
        /// <returns><Selektionswert/returns>
        public string GetWert()
        {
            return base.Wert.ToString();
        }

        /// <summary>
        /// Legt den Wert zur Selektion fest
        /// </summary>
        /// <param name="wert">Selektionswert</param>
        public void SetWert(string wert)
        {
            base.Wert = wert;
        }
        #endregion Methoden
    }
}
