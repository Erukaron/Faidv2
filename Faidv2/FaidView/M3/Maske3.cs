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
        #region Konstanten
        /// <summary>
        /// Gibt den Index für die Checkbox "Ausschließende Selektion" an
        /// </summary>
        const int ZELLEN_INDEX_AUSSCHLIESSEND = 0;
        /// <summary>
        /// Gibt den Index für die ComboBox "Typ" an
        /// </summary>
        const int ZELLEN_INDEX_TYP = 1;
        /// <summary>
        /// Gibt den Index für das Wert-Feld an
        /// </summary>
        const int ZELLEN_INDEX_WERT = 2;
        #endregion Konstanten

        #region Felder
        /// <summary>
        /// Selektionsliste
        /// </summary>
        private BindingList<SelektionBase> _liste;

        /// <summary>
        /// Gibt an, ob die Daten übernommen werden sollen
        /// </summary>
        private bool _isDatenUebernommen;
        #endregion Felder

        #region ctor
        /// <summary>
        /// Initialisiert eine neue Instanz der Klasse
        /// </summary>
        public Maske3()
        {
            InitializeComponent();

            IsDatenUbernommen = false;

            dgvSelektion.AutoGenerateColumns = false;
            dgvSelektion.DataSource = Liste;
            //dgvSelektion.Columns[ZELLEN_INDEX_AUSSCHLIESSEND].DataPropertyName = "IsAusschliessend";
            //dgvSelektion.Columns[ZELLEN_INDEX_TYP].DataPropertyName = "Typ";
            //dgvSelektion.Columns[ZELLEN_INDEX_WERT].DataPropertyName = "Wert";
            
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der Klasse
        /// </summary>
        /// <param name="liste">Startliste</param>
        public Maske3(List<SelektionBase> liste) : this()
        {
            foreach (SelektionBase s in liste)
                Liste.Add(s);
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
                if (_liste is null)
                    _liste = new BindingList<SelektionBase>();

                return _liste;
            }
            private set => _liste = value; 
        }

        public bool IsDatenUbernommen { get => _isDatenUebernommen; private set => _isDatenUebernommen = value; }
        #endregion Eigenschaften

        #region Menu Events
        private void übernehmenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Das macht nicht das, was es soll...
            // Vor Verlassen sollte der User die Enter-Taste betätigen, damit Änderungen übernommen werden
            //dgvSelektion.CommitEdit(DataGridViewDataErrorContexts.Commit);
            //dgvSelektion.EndEdit();

            if (IsDGVFehler())
            {
                MessageBox.Show(this, M3Ressourcen.UebernahmeFehler, M3Ressourcen.UebernahmeFehlerUeberschrift, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                IsDatenUbernommen = true;
                Close();
            }
        }

        private void loeschenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell zelle in dgvSelektion.SelectedCells)
            {
                SelektionBase selektion = zelle.OwningRow.DataBoundItem as SelektionBase;
                Liste.Remove(selektion);
            }
        }

        private void neueWertselektionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Liste.Add(new SelektionGroesse());
        }

        private void neueZeichenselektionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Liste.Add(new SelektionZeichenkette());
        }
        #endregion Menu Events

        #region DGV Events
        private void dgvSelektion_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            SelektionBase selektion = (SelektionBase)dgvSelektion.Rows[e.RowIndex].DataBoundItem;
            DataGridViewCheckBoxCell check = (DataGridViewCheckBoxCell)dgvSelektion.Rows[e.RowIndex].Cells[ZELLEN_INDEX_AUSSCHLIESSEND];
            DataGridViewComboBoxCell combo = (DataGridViewComboBoxCell)dgvSelektion.Rows[e.RowIndex].Cells[ZELLEN_INDEX_TYP];
            DataGridViewTextBoxCell  text  = (DataGridViewTextBoxCell)dgvSelektion.Rows[e.RowIndex].Cells[ZELLEN_INDEX_WERT];

            check.Value = selektion.IsAusschliessendeSelektion;

            if (selektion.Typ == SelektionTyp.zeichenkette)
            {
                combo.Items.Add(M3Ressourcen.TypZeichenkette);
                combo.Value = M3Ressourcen.TypZeichenkette;

                text.Value = (selektion as SelektionZeichenkette).GetWert();
            }
            else
            {
                combo.Items.Add(M3Ressourcen.TypGroesser);
                combo.Items.Add(M3Ressourcen.TypKleiner);

                if (selektion.Typ == SelektionTyp.groesser)
                    combo.Value = M3Ressourcen.TypGroesser;
                else
                    combo.Value = M3Ressourcen.TypKleiner;

                 ((DataGridViewTextBoxCell)dgvSelektion.Rows[e.RowIndex].Cells[ZELLEN_INDEX_WERT]).Value = (selektion as SelektionGroesse).GetWert().ToString();
            }
        }

        private void dgvSelektion_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == ZELLEN_INDEX_WERT)
                WertValidieren(e);
        }

        private void dgvSelektion_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == ZELLEN_INDEX_AUSSCHLIESSEND)
                IsAusschliessendUebernehmen(e);
            else if (e.ColumnIndex == ZELLEN_INDEX_TYP)
                TypUebernehmen(e);
            else if (e.ColumnIndex == ZELLEN_INDEX_WERT)
                WertUebernehmen(e);
        }
        #endregion DGV Events

        #region Methoden
        /// <summary>
        /// Validiert die Wertspalte
        /// </summary>
        /// <param name="e">Event Argumente aus dem CellValidating Event</param>
        private void WertValidieren(DataGridViewCellValidatingEventArgs e)
        {
            SelektionBase selektion = (SelektionBase)dgvSelektion.Rows[e.RowIndex].DataBoundItem;
            if (selektion is SelektionGroesse)
            {
                string wert = (string)((DataGridViewTextBoxCell)dgvSelektion[e.ColumnIndex, e.RowIndex]).Value;
                decimal dummy;
                if (!decimal.TryParse(wert, out dummy))
                {
                    e.Cancel = true;
                    dgvSelektion[e.ColumnIndex, e.RowIndex].ErrorText = M3Ressourcen.FehlerWertNaN;
                    ((DataGridViewTextBoxCell)dgvSelektion[e.ColumnIndex, e.RowIndex]).Value = "0";
                }
            }
        }

        /// <summary>
        /// Uebernimmt den Wert für die Ausschließend-Eigenschaft nach der Validierung
        /// </summary>
        /// <param name="e">Event Argumente aus dem CellValidated Event</param>
        private void IsAusschliessendUebernehmen(DataGridViewCellEventArgs e)
        {
            if (!IsZelleFehler(e.ColumnIndex, e.RowIndex))
            {
                SelektionBase selektion = dgvSelektion.Rows[e.RowIndex].DataBoundItem as SelektionBase;
                if (((DataGridViewCheckBoxCell)dgvSelektion[e.ColumnIndex, e.RowIndex]).Value != null)
                    selektion.IsAusschliessendeSelektion = (bool)((DataGridViewCheckBoxCell)dgvSelektion[e.ColumnIndex, e.RowIndex]).Value;
            }
        }

        /// <summary>
        /// Uebernimmt den Typ für größer/kleiner Selektionen nach der Validierung
        /// </summary>
        /// <param name="e">Event Argumente aus dem CellValidated Event</param>
        private void TypUebernehmen(DataGridViewCellEventArgs e)
        {
            if (!IsZelleFehler(e.ColumnIndex, e.RowIndex))
            {
                SelektionGroesse selektion = dgvSelektion.Rows[e.RowIndex].DataBoundItem as SelektionGroesse;
                if (selektion != null)
                {
                    if (((string)((DataGridViewComboBoxCell)dgvSelektion[e.ColumnIndex, e.RowIndex]).Value).Equals(M3.M3Ressourcen.TypGroesser))
                        selektion.SetTyp(SelektionTyp.groesser);
                    else
                        selektion.SetTyp(SelektionTyp.kleiner);
                }
            }
        }

        /// <summary>
        /// Uebernimmt den Wert für die Selektion nach der Validierung
        /// </summary>
        /// <param name="e">Event Argumente aus dem CellValidated Event</param>
        private void WertUebernehmen(DataGridViewCellEventArgs e)
        {
            if (!IsZelleFehler(e.ColumnIndex, e.RowIndex))
            {
                SelektionBase selektion = dgvSelektion.Rows[e.RowIndex].DataBoundItem as SelektionBase;
                (selektion as SelektionZeichenkette)?.SetWert(((string)((DataGridViewTextBoxCell)dgvSelektion[e.ColumnIndex, e.RowIndex]).Value));
                (selektion as SelektionGroesse)?.SetWert(decimal.Parse(((string)((DataGridViewTextBoxCell)dgvSelektion[e.ColumnIndex, e.RowIndex]).Value)));
            }
        }

        /// <summary>
        /// Prüft, ob im dgv Fehler vorliegen
        /// </summary>
        /// <returns><c>true</c>, wenn Fehler vorliegen</returns>
        private bool IsDGVFehler()
        {
            bool fehler = false;
            foreach (DataGridViewRow zeile in dgvSelektion.Rows)
            {
                foreach (DataGridViewCell zelle in zeile.Cells)
                {
                    if (zelle.ErrorText.Length > 0)
                    {
                        fehler = true;
                        break;
                    }
                }

                if (fehler)
                    break;
            }

            return fehler;
        }

        /// <summary>
        /// Gibt an, ob die Zelle einen Fehlerstatus hat
        /// </summary>
        /// <param name="spalte">Spalte</param>
        /// <param name="zeile">Zeile</param>
        /// <returns><c>true</c>, wenn ein Fehlertext gesetzt ist</returns>
        private bool IsZelleFehler(int spalte, int zeile)
        {
            return dgvSelektion[spalte, zeile].ErrorText.Length > 0;
        }
        #endregion Methoden
    }
}
