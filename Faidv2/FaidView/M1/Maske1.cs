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
using Faidv2.FaidModel.Selektion;

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

        /// <summary>
        /// Element wird an Datagridview gebunden
        /// </summary>
        private BindingList<Eintrag> _bewegungen;

        /*
        /// <summary>
        /// Element wird an Datagridview gebunden
        /// </summary>
        private BindingList<DauerEintrag> _einkommen;

        /// <summary>
        /// Element wird an Datagridview gebunden
        /// </summary>
        private BindingList<DauerEintrag> _ausgaben;

        /// <summary>
        /// Element wird an Datagridview gebunden
        /// </summary>
        private BindingList<DauerEintrag> _zinsen;
        */

        /// <summary>
        /// Die Liste über die aktuelle Selektion
        /// </summary>
        private List<SelektionBase> _selektion;
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
            dgvBewegung.AlternatingRowsDefaultCellStyle.BackColor = Properties.Settings.Default.StandardZeilenHintergrund;
            dgvBewegung.RowsDefaultCellStyle.BackColor = Properties.Settings.Default.StandardWechselZeilenHintergrund;

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
                toolStripStatusLabelWert.Text = "0";
            };
            Ctrl.SpeichernFehler += (object sender, Exception e, string datei) => 
            { 
                toolStripStatusLabelDatei.Text = string.Format(M1Ressourcen.StatusSpeichernFehler, datei);
                MessageBox.Show(this, e.Message, M1Ressourcen.MsgBoxSpeichernFehlerUeberschrift, MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
        }

        /// <summary>
        /// Initiailisiert eine neue Instanz der Klasse
        /// </summary>
        /// <param name="c"><paramref name="c"/></param>
        /// <param name="konto">Zu ladendes Konto</param>
        public Maske1(Controller c, Model konto) : this(c)
        {
            AktualisiereKonto(konto);
        }
        #endregion Konstruktor

        #region Destruktor
        /// <summary>
        /// Destruktor
        /// </summary>
        ~Maske1()
        {
            DatenbasisEntbinden(Konto);
        }
        #endregion Destruktor

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

                    DatenbasisBinden(Konto);
                }
            }
        }

        /// <summary>
        /// Element wird an Datagridview gebunden
        /// </summary>
        private BindingList<Eintrag> Bewegungen { get => _bewegungen ?? new BindingList<Eintrag>(); }

        /*
        /// <summary>
        /// Element wird an Datagridview gebunden
        /// </summary>
        private BindingList<DauerEintrag> Einkommen { get => _einkommen ?? new BindingList<DauerEintrag>(); }

        /// <summary>
        /// Element wird an Datagridview gebunden
        /// </summary>
        private BindingList<DauerEintrag> Ausgaben { get => _ausgaben ?? new BindingList<DauerEintrag>(); }

        /// <summary>
        /// Element wird an Datagridview gebunden
        /// </summary>
        private BindingList<DauerEintrag> Zinsen { get => _zinsen ?? new BindingList<DauerEintrag>(); }
        */

        /// <summary>
        /// Die Liste über die aktuelle Selektion
        /// </summary>
        private List<SelektionBase> Selektion { get => _selektion ?? new List<SelektionBase>(); }
        #endregion Eigenschaften

        #region Menu Events
        #region Datei
        private void neuStripMenuItem1_Click(object sender, EventArgs e)
        {
            string eingabe = Interaction.InputBox(M1Ressourcen.InputKontoName, M1Ressourcen.TitelAllgemein, M1Ressourcen.StdKontoName);
            if (!eingabe.Equals(""))
            {
                Konto = Controller.NeuesKonto(eingabe);
                _kontoDateipfad = "";
            }
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
            if (_kontoDateipfad.Equals(""))
                speichernUnterToolStripMenuItem_Click(sender, e);
            else
                Ctrl.KontoSpeichern(Konto, _kontoDateipfad);
        }

        private void speichernUnterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_kontoDateipfad.Equals(""))
            {
                saveFileDialog.FileName = Path.GetFileName(_kontoDateipfad);
                saveFileDialog.InitialDirectory = Path.GetFullPath(_kontoDateipfad);
            }

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
        private void selektionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ctrl.SelektionBearbeiten(Selektion);

            // Test
            Selektion.Add(new SelektionGroesse(false, false, 50));

            List<Eintrag> arbeitskopie = new List<Eintrag>();

            // Einschließende Selektion über Zeichenkette
            foreach (SelektionZeichenkette sele in Selektion.Where(x => !x.IsAusschliessendeSelektion))
            {
                arbeitskopie.Concat(Bewegungen.Where(x => x.Kommentar.Equals(sele.GetWert())));
            }

            // Einschließende Selektion über Größe
            foreach (SelektionGroesse sele in Selektion.Where(x => !x.IsAusschliessendeSelektion))
            {
                if (sele.IsKleiner)
                    arbeitskopie.Concat(Bewegungen.Where(x => x.Wert < sele.GetWert()));
                else
                    arbeitskopie.Concat(Bewegungen.Where(x => x.Wert > sele.GetWert()));
            }

            arbeitskopie = arbeitskopie.Distinct().ToList();

            // Ausschließende Selektion über Zeichenkette
            foreach (SelektionZeichenkette sele in Selektion.Where(x => x.IsAusschliessendeSelektion))
            {
                arbeitskopie = arbeitskopie.Except(arbeitskopie.Where(x => x.Kommentar.Equals(sele.GetWert()))).ToList();
            }

            // Ausschließende Selektion über Größe
            foreach (SelektionGroesse sele in Selektion.Where(x => x.IsAusschliessendeSelektion))
            {
                if (sele.IsKleiner)
                    arbeitskopie = arbeitskopie.Except(arbeitskopie.Where(x => x.Wert < sele.GetWert())).ToList();
                else
                    arbeitskopie = arbeitskopie.Except(arbeitskopie.Where(x => x.Wert > sele.GetWert())).ToList();
            }

            Bewegungen.Clear();
            Bewegungen.Concat(arbeitskopie);
        }

        private void loeschenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabUebersicht.ContainsFocus)
            {
                string orig = toolStripStatusLabelDatei.Text;
                foreach (DataGridViewRow zeile in dgvBewegung.SelectedRows)
                {
                    Eintrag eintrag = zeile.DataBoundItem as Eintrag; // Eintrag oder null
                    if (eintrag is Eintrag) // Eintrag nur, wenn nicht null
                    {
                        toolStripStatusLabelDatei.Text = String.Format(M1Ressourcen.StorniereBuchung, eintrag.Kommentar);
                        Ctrl.Stornieren(Konto, eintrag);
                    }
                }
                toolStripStatusLabelDatei.Text = orig;
            }
            else 
            {
                DataGridView dgv = null;

                if (dgvEinnahmen.ContainsFocus)
                    dgv = dgvEinnahmen;
                else if (dgvAusgaben.ContainsFocus)
                    dgv = dgvAusgaben;
                else if (dgvZinsen.ContainsFocus)
                    dgv = dgvZinsen;

                foreach (DataGridViewRow zeile in dgv.SelectedRows)
                {
                    DauerEintrag eintrag = zeile.DataBoundItem as DauerEintrag; // Eintrag oder null
                    if (eintrag is DauerEintrag) // Eintrag nur, wenn nicht null
                    {
                        if (dgvEinnahmen.ContainsFocus)
                            Ctrl.EinkommenEntfernen(Konto, eintrag);
                        else if (dgvAusgaben.ContainsFocus)
                            Ctrl.AusgabenEntfernen(Konto, eintrag);
                        else if (dgvZinsen.ContainsFocus)
                            Ctrl.ZinsenEntfernen(Konto, eintrag);
                    }
                }
            }   
        }

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

        private void bearbeitenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (tabUebersicht.ContainsFocus)
                KorrekturGewaehlteBewegungen();
            else if (dgvEinnahmen.ContainsFocus)
                KorrekturGewaehlteEinnahmen();
            else if (dgvAusgaben.ContainsFocus)
                KorrekturGewaehlteAusgaben();
            else if (dgvZinsen.ContainsFocus)
                KorrekturGewaehlteZinsen();
        }
        #endregion Bearbeiten

        #region Einstellungen
        private void toolStripMenuItemDateierweiterung_Click(object sender, EventArgs e)
        {
            AlexHandyDandyAuxiliaryFunctions.FileAssociations.SetAssociation("fa2", "Faidv2.exe", "Faid v2 Kontodatei", Application.ExecutablePath);
        }
        #endregion Einstellungen
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
            dgvBewegung.Refresh();
        }

        private void dgvBewegung_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            KorrekturGewaehlteBewegungen();
        }

        private void dgvBewegung_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            Eintrag eintrag = dgvBewegung.Rows[e.RowIndex].DataBoundItem as Eintrag;

            if (   (eintrag.Typ == BuchungsTyp.Subtraktion && eintrag.Wert > 0)
                || (eintrag.Typ != BuchungsTyp.Subtraktion && eintrag.Wert < 0) )
                dgvBewegung.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Properties.Settings.Default.AbzugZeilenVordergrund;

            if (eintrag.PeriodischeVerbuchung)
            {
                if (dgvBewegung.Rows[e.RowIndex].InheritedStyle.BackColor == Properties.Settings.Default.StandardZeilenHintergrund)
                    dgvBewegung.Rows[e.RowIndex].DefaultCellStyle.BackColor = Properties.Settings.Default.PeridischerZeilenHintergrund;
                else if (dgvBewegung.Rows[e.RowIndex].InheritedStyle.BackColor == Properties.Settings.Default.StandardWechselZeilenHintergrund)
                    dgvBewegung.Rows[e.RowIndex].DefaultCellStyle.BackColor = Properties.Settings.Default.PeriodischerWechselZeilenHintergrund;
            }
            else if (eintrag.Typ == BuchungsTyp.Gleichsetzen)
            {
                if (dgvBewegung.Rows[e.RowIndex].InheritedStyle.BackColor == Properties.Settings.Default.StandardZeilenHintergrund)
                    dgvBewegung.Rows[e.RowIndex].DefaultCellStyle.BackColor = Properties.Settings.Default.GleichsetzenZeilenHintergrund;
                else if (dgvBewegung.Rows[e.RowIndex].InheritedStyle.BackColor == Properties.Settings.Default.StandardWechselZeilenHintergrund)
                    dgvBewegung.Rows[e.RowIndex].DefaultCellStyle.BackColor = Properties.Settings.Default.GleichsetzenWechselZeilenHintergrund;
            }
        }
        #endregion Bewegung

        #region Einkommen
        /// <summary>
        /// Aktualisiert das DGV für Einkommen
        /// </summary>
        public void DGVEinkommenAktualisieren()
        {
            dgvEinnahmen.Refresh();
        }

        private void dgvEinnahmen_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            KorrekturGewaehlteEinnahmen();
        }
        #endregion Einkommen

        #region Ausgaben
        /// <summary>
        /// Aktualisiert das DGV für Ausgaben
        /// </summary>
        public void DGVAusgabenAktualisieren()
        {
            dgvAusgaben.Refresh();
        }

        private void dgvAusgaben_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            KorrekturGewaehlteAusgaben();
        }
        #endregion Ausgaben

        #region Zinsen
        /// <summary>
        /// Aktualisiert das DGV für Zinsen
        /// </summary>
        public void DGVZinsenAktualisieren()
        {
            dgvZinsen.Refresh();
        }

        private void dgvZinsen_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            KorrekturGewaehlteZinsen();
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
            //if (e.Data.GetDataPresent(DataFormats.Serializable))
                e.Effect = DragDropEffects.Link;
            //else
            //    e.Effect = DragDropEffects.None;
        }
        #endregion Drag&Drop

        #region Tool Strip
        private void toolStripStatusLabelWert_TextChanged(object sender, EventArgs e)
        {
            if (decimal.Parse(toolStripStatusLabelWert.Text) < 0)
                toolStripStatusLabelWert.ForeColor = Properties.Settings.Default.AbzugZeilenVordergrund;
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

        #region Methoden
        /// <summary>
        /// Bindet die Datenbasis an die Maske
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        private void DatenbasisBinden(Model konto)
        {
            // Die drei Kommentare als Referenz
            //Konto.KontostandAktualisiert += delegate { toolStripStatusLabelWert.Text = Konto.Kontostand.ToString(); };
            //Konto.KontostandAktualisiert += () => { toolStripStatusLabelWert.Text = Konto.Kontostand.ToString(); };
            //Konto.KontostandAktualisiert += () => toolStripStatusLabelWert.Text = Konto.Kontostand.ToString();
            // Hier kann leider keine anonyme Methode verwendet werden, da ansonsten eine Entfernung dieser Methoe beim Entbinden nicht möglich wäre.
            // Dies ist aber notwendig, damit das Objekt serialisiert werden kann. Ansonsten würde M1 mit serialisiert werden.
            Konto.KontostandAktualisiert += AktualisiereKontostand;

            dgvBewegung.DataSource = konto.Kontobewegung;
            dgvBewegung.Columns[0].DataPropertyName = "Typ";
            dgvBewegung.Columns[1].DataPropertyName = "Erstellt";
            dgvBewegung.Columns[2].DataPropertyName = "Datum";
            dgvBewegung.Columns[3].DataPropertyName = "Wert";
            dgvBewegung.Columns[4].DataPropertyName = "Kommentar";

            // ToDo: dgvBewegung Zeilen grau färben bei periodischer buchung
            // ToDO: dgvEinnahmen/Ausgaben/Zinsen zusammen fassen -> Kennzeichen setzen
            dgvEinnahmen.DataSource = konto.Einkommen;
            dgvEinnahmen.Columns[0].DataPropertyName = "Erstellt";
            dgvEinnahmen.Columns[1].DataPropertyName = "Wert";
            dgvEinnahmen.Columns[2].DataPropertyName = "Kommentar";
            dgvAusgaben.DataSource = konto.Ausgaben;
            dgvAusgaben.Columns[0].DataPropertyName = "Erstellt";
            dgvAusgaben.Columns[1].DataPropertyName = "Wert";
            dgvAusgaben.Columns[2].DataPropertyName = "Kommentar";
            dgvZinsen.DataSource = konto.Zinsen;
            dgvZinsen.Columns[0].DataPropertyName = "Erstellt";
            dgvZinsen.Columns[1].DataPropertyName = "Wert";
            dgvZinsen.Columns[2].DataPropertyName = "Kommentar";
        }

        /// <summary>
        /// Entfernt die Datenbasisbindung an die Maske
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        private void DatenbasisEntbinden(Model konto)
        {
            Konto.KontostandAktualisiert -= AktualisiereKontostand;

            dgvBewegung.DataSource = null;
            dgvEinnahmen.DataSource = null;
            dgvAusgaben.DataSource = null;
            dgvZinsen.DataSource = null;
        }

        /// <summary>
        /// Event das ausgelöst wird, wenn der Kontostand aktualisiert wird
        /// </summary>
        private void AktualisiereKontostand()
        {
            toolStripStatusLabelWert.Text = Konto.Kontostand.ToString();
        }

        /// <summary>
        /// Korrekturaufruf für gewählte Bewegungen
        /// </summary>
        private void KorrekturGewaehlteBewegungen()
        {
            foreach (DataGridViewRow zeile in dgvBewegung.SelectedRows)
            {
                Eintrag eintrag = zeile.DataBoundItem as Eintrag; // Eintrag oder null
                if (eintrag is Eintrag) // Eintrag nur, wenn nicht null
                {
                    Ctrl.Korrektur(eintrag);
                }
            }
        }

        /// <summary>
        /// Korrekturaufruf für gewählte Einnahmen
        /// </summary>
        private void KorrekturGewaehlteEinnahmen() { KorrekturGewaehlteDauerbuchung(M2ModusTypen.Einkommen); }

        /// <summary>
        /// Korrekturaufruf für gewählte Einnahmen
        /// </summary>
        private void KorrekturGewaehlteAusgaben() { KorrekturGewaehlteDauerbuchung(M2ModusTypen.Ausgaben); }

        /// <summary>
        /// Korrekturaufruf für gewählte Einnahmen
        /// </summary>
        private void KorrekturGewaehlteZinsen() { KorrekturGewaehlteDauerbuchung(M2ModusTypen.Zinsen); }

        /// <summary>
        /// Korrekturaufruf für gewählte Dauerbuchungen
        /// </summary>
        /// <param name="typ">Dauerbuchungstyp</param>
        private void KorrekturGewaehlteDauerbuchung(M2ModusTypen typ)
        {
            DataGridView dgv = null;

            if (typ == M2ModusTypen.Einkommen)
                dgv = dgvEinnahmen;
            else if (typ == M2ModusTypen.Ausgaben)
                dgv = dgvAusgaben;
            else if (typ == M2ModusTypen.Zinsen)
                dgv = dgvZinsen;

            if (dgv != null)
            {
                foreach (DataGridViewRow zeile in dgv.SelectedRows)
                {
                    DauerEintrag eintrag = zeile.DataBoundItem as DauerEintrag; // Eintrag oder null
                    if (eintrag is DauerEintrag) // Eintrag nur, wenn nicht null
                    {
                        Ctrl.Korrektur(eintrag, typ);
                    }
                }
            }
        }
        #endregion Methoden
    }
}
