using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Faidv2.FaidController;
using Faidv2.FaidModel;
using Microsoft.VisualBasic;
using System.IO;
using Faidv2.FaidView.M2;

namespace Faidv2.FaidView.M1
{
    public partial class Maske1 : Form
    {
        #region Felder
        /// <summary>
        /// Aufrufender Controller
        /// </summary>
        private Controller _controller;

        /// <summary>
        /// Konto, das als Datenbasis dient
        /// </summary>
        private Model _konto;

        /// <summary>
        /// Dateipfad zur aktuellen Kontodatei
        /// </summary>
        private string _kontoDateipfad = "";
        #endregion Felder

        #region Konstruktor
        /// <summary>
        /// Initiailisiert eine neue Instanz der Klasse
        /// </summary>
        /// <param name="c">Aufrufendes Controller objekt</param>
        public Maske1(Controller c)
        {
            InitializeComponent();

            _controller = c;
            toolStripStatusLabelDatei.Text = M1Ressourcen.StatusKeinKonto;

            dgvBewegung.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBewegung.AutoGenerateColumns = false;

            dgvEinnahmen.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEinnahmen.AutoGenerateColumns = false;

            dgvAusgaben.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAusgaben.AutoGenerateColumns = false;

            dgvZinsen.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvZinsen.AutoGenerateColumns = false;

            Ctrl.LadenFehler += (object sender, Exception e, string datei) => 
            { 
                toolStripStatusLabelDatei.Text = string.Format(M1Ressourcen.StatusLadefehler, datei);
                MessageBox.Show(this, e.Message, M1Ressourcen.MsgBoxLadeFehlerUeberschrift, MessageBoxButtons.OK, MessageBoxIcon.Error);
                _kontoDateipfad = ""; // Im Fehlerfall zurücksetzen
            };
            Ctrl.SpeichernFehler += (object sender, Exception e, string datei) => 
            { 
                toolStripStatusLabelDatei.Text = string.Format(M1Ressourcen.StatusSpeichernFehler, datei);
                MessageBox.Show(this, e.Message, M1Ressourcen.MsgBoxSpeichernFehlerUeberschrift, MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
        }
        #endregion Konstruktor

        #region Eigenschaften
        /// <summary>
        /// Aufrufender Controller
        /// </summary>
        private Controller Ctrl { get => _controller; }

        /// <summary>
        /// Konto, das als Datenbasis dient
        /// Beim setzen / entfernen des Kontos wird das standard Tab aktiviert, der Inhalt des dgv gelöscht und das Tab gesperrt
        /// </summary>
        private Model Konto 
        { 
            get => _konto; 
            set
            {
                _konto = value;
                if (_konto == null)
                {
                    tabUebersicht.Select();
                    dgvBewegung.Rows.Clear();
                    tabMain.Enabled = false;
                    toolStripStatusLabelDatei.Text = M1Ressourcen.StatusKeinKonto;
                }
                else
                {
                    tabMain.Enabled = true;
                    tabUebersicht.Select();
                    toolStripStatusLabelDatei.Text = Konto.Kontoname;
                    toolStripStatusLabelWert.Text = Konto.Kontostand.ToString();

                    // Die beiden Kommentare als Referenz
                    //Konto.KontostandAktualisiert += delegate { toolStripStatusLabelWert.Text = Konto.Kontostand.ToString(); };
                    //Konto.KontostandAktualisiert += () => { toolStripStatusLabelWert.Text = Konto.Kontostand.ToString(); };
                    Konto.KontostandAktualisiert += () => toolStripStatusLabelWert.Text = Konto.Kontostand.ToString();

                    dgvBewegung.DataSource = Konto.Kontobewegung;
                    dgvBewegung.Columns[0].DataPropertyName = "Typ";
                    dgvBewegung.Columns[1].DataPropertyName = "Erstellt";
                    dgvBewegung.Columns[2].DataPropertyName = "Datum";
                    dgvBewegung.Columns[3].DataPropertyName = "Wert";
                    dgvBewegung.Columns[4].DataPropertyName = "Kommentar";

                    // ToDo: dgvBewegung Zeilen grau färben bei periodischer buchung
                    // ToDO: dgvEinnahmen/Ausgaben/Zinsen zusammen fassen -> Kennzeichen setzen
                    dgvEinnahmen.DataSource = Konto.Einkommen;
                    dgvAusgaben.DataSource = Konto.Ausgaben;
                    dgvZinsen.DataSource = Konto.Zinsen;
                }
            }
        }
        #endregion Eigenschaften

        #region Menu Events
        #region Datei
        private void hinzufuegenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabUebersicht.ContainsFocus)
                Ctrl.Erfassung(Konto, BuchungsTyp.Addition);
            else if (dgvEinnahmen.ContainsFocus)
                Ctrl.Erfassung(Konto, M2ModusTypen.Einkommen, DauerBuchungsTyp.Monatlich);
            else if (dgvAusgaben.ContainsFocus)
                Ctrl.Erfassung(Konto, M2ModusTypen.Ausgaben, DauerBuchungsTyp.Monatlich); 
            else if (dgvZinsen.ContainsFocus)
                Ctrl.Erfassung(Konto, M2ModusTypen.Zinsen, DauerBuchungsTyp.Monatlich);
        }

        private void neuStripMenuItem1_Click(object sender, EventArgs e)
        {
            Konto = Controller.NeuesKonto(Interaction.InputBox(M1Ressourcen.InputKontoName, M1Ressourcen.TitelAllgemein, M1Ressourcen.StdKontoName));
            _kontoDateipfad = "";
        }

        private void oeffnenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dErg = openFileDialog.ShowDialog();
            if (dErg == DialogResult.OK)
            {
                _kontoDateipfad = openFileDialog.FileName;
                Konto = Ctrl.KontoLaden(openFileDialog.FileName);
            }
        }

        private void speichernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_kontoDateipfad.Equals(""))
                speichernUnterToolStripMenuItem_Click(sender, e);
            else
                Ctrl.KontoSpeichern(Konto, _kontoDateipfad);
        }

        private void speichernUnterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.FileName = Path.GetFileName(_kontoDateipfad);
            saveFileDialog.InitialDirectory = Path.GetFullPath(_kontoDateipfad);

            DialogResult dErg = saveFileDialog.ShowDialog();
            if (dErg == DialogResult.OK)
            {
                _kontoDateipfad = saveFileDialog.FileName;
                Ctrl.KontoSpeichern(Konto, _kontoDateipfad);
            }
        }

        private void toolStripMenuItemKonvertierung_Click(object sender, EventArgs e)
        {
            DialogResult dErg = openFileDialogKonvertierung.ShowDialog();
            if (dErg == DialogResult.OK)
            {
                Model konto;
                string fehler = Ctrl.KonvertierenV1_4ZuV2(openFileDialogKonvertierung.FileName, out konto);
                konto.NeuberechnungKontostandAusBewegung();
                if (fehler.Equals(string.Empty))
                {
                    DialogResult d = saveFileDialog.ShowDialog();
                    if (d == DialogResult.OK)
                    {
                        Ctrl.KontoSpeichern(konto, saveFileDialog.FileName);
                    }
                }
                else
                {
                    MessageBox.Show(this, fehler, M1Ressourcen.MsgBoxKonvertierungFehlerUeberschrift, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion Datei

        #region Bearbeiten
        #endregion Bearbeiten
        #endregion Menu Events

        #region Data Grid Views
        /// <summary>
        /// Aktualisiert alle Data Grid Views
        /// </summary>
        public void DGVsAktualisieren()
        {
            DGVBewegungAktualisieren();
            DGVEinkommenAktualisieren();
            DGVAusgabenAktualisieren();
            DGVZinsenAktualisieren();
        }

        #region Bewegung
        /// <summary>
        /// Aktualisiert das DGV für Bewegungen
        /// </summary>
        public void DGVBewegungAktualisieren()
        {
            /*
            BindingSource source = new BindingSource();
            foreach (Eintrag e in Konto.Kontobewegung)
                source.Add(e);

            dgvBewegung.DataSource = source;
            */
            // ToDo: DataSource anschauen und dann damit arbeiten, statt die Matrix immer neu zu beschreiben
        }
        #endregion Bewegung

        #region Einkommen
        /// <summary>
        /// Aktualisiert das DGV für Einkommen
        /// </summary>
        public void DGVEinkommenAktualisieren()
        {

        }
        #endregion Einkommen

        #region Ausgaben
        /// <summary>
        /// Aktualisiert das DGV für Ausgaben
        /// </summary>
        public void DGVAusgabenAktualisieren()
        {

        }
        #endregion Ausgaben

        #region Zinsen
        /// <summary>
        /// Aktualisiert das DGV für Zinsen
        /// </summary>
        public void DGVZinsenAktualisieren()
        {

        }
        #endregion Zinsen

        #endregion Data Grid Views

        #region Sonstige Masken Events
        #region Drag&Drop
        private void Maske1_DragDrop(object sender, DragEventArgs e)
        {
            string[] dateiListe = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (dateiListe?.Length > 0)
            {
                _kontoDateipfad = dateiListe[0];
                Konto = Ctrl.KontoLaden(_kontoDateipfad);
            }
        }

        private void Maske1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Serializable))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }
        #endregion Drag&Drop

        #region Tool Strip
        private void toolStripStatusLabelWert_TextChanged(object sender, EventArgs e)
        {
            if (decimal.Parse(toolStripStatusLabelWert.Text) < 0)
                toolStripStatusLabelWert.ForeColor = Color.Red;
            else
                toolStripStatusLabelWert.ForeColor = Color.Black;
        }
        #endregion Tool Strip
        #endregion Sonstige Masken Events

        #region Controller Kommunikation
        /// <summary>
        /// Aktualisiert das Konto
        /// </summary>
        /// <param name="konto">Konto</param>
        public void AktualisiereKonto(Model konto)
        {
            Konto = konto;
        }
        #endregion Controller Kommunikation
    }
}
