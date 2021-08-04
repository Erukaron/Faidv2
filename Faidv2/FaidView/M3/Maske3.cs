using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Faidv2.FaidModel.Selektion;

namespace Faidv2.FaidView.M3
{
    public partial class Maske3 : Form
    {
        #region Felder
        /// <summary>
        /// Selektionsliste
        /// </summary>
        private BindingList<SelektionBase> _liste;
        #endregion Felder

        #region ctor
        /// <summary>
        /// Initialisiert eine neue Instanz der Klasse
        /// </summary>
        public Maske3()
        {
            InitializeComponent();

            Liste.Add(new SelektionZeichenkette(false, "Test"));

            dgvSelektion.AutoGenerateColumns = false;
            dgvSelektion.DataSource = Liste;
            dgvSelektion.Columns[0].DataPropertyName = "IsAusschliessend";
            dgvSelektion.Columns[1].DataPropertyName = "Typ";
            dgvSelektion.Columns[2].DataPropertyName = "Wert";
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der Klasse
        /// </summary>
        /// <param name="liste">Startliste</param>
        public Maske3(List<SelektionBase> liste) : this()
        {
            _liste = new BindingList<SelektionBase>(liste);
        }
        #endregion ctor

        #region Eigenschaften
        /// <summary>
        /// Selektionsliste
        /// </summary>
        public BindingList<SelektionBase> Liste 
        {
            get
            {
                return _liste ?? new BindingList<SelektionBase>();
            }
            private set => _liste = value; 
        }
        #endregion Eigenschaften
    }
}
