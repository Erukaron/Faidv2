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

namespace Faidv2.FaidView.M2
{
    public partial class Maske2 : Form
    {
        #region Felder
        /// <summary>
        /// Aufrufender Controller
        /// </summary>
        private Controller _controller;

        /// <summary>
        /// Zu verarbeitender Modus
        /// </summary>
        private M2ModusTypen _modusTyp;

        /// <summary>
        /// Zu verarbeitende Bewegung
        /// </summary>
        private BuchungsTyp _bewegungTyp;

        /// <summary>
        /// Kontodatei, für die Änderungen vorgenommen werden
        /// </summary>
        private Model _konto;

        /// <summary>
        /// Callback Funktion, die bei Übernahme der Daten ausgeführt wird
        /// </summary>
        private Controller.UebernehmeM2ErfassungHandler _uebernahmeErfassungCallback;

        /// <summary>
        /// Callback Funktion, die bei Übernahme der Daten ausgeführt wird
        /// </summary>
        private Controller.UebernehmeM2KorrekturHandler _uebernahmeKorrekturCallback;

        /// <summary>
        /// Zu korrigierender Eintrag
        /// </summary>
        private EintragBase _korrekturEintrag;
        #endregion Felder

        #region Konstruktor
        /// <summary>
        /// Initialisiert ein neues Objekt der Klasse
        /// </summary>
        /// <param name="c">Aufrufender Controller</param>
        private Maske2(Controller c)
        {
            InitializeComponent();

            _controller = c;
        }

        /// <summary>
        /// Initialisiert ein neues Objekt der Klasse 
        /// Bereitet die Maske vor
        /// </summary>
        /// <param name="c"><paramref name="c"/></param>
        /// <param name="modusTyp">Aufrufmodus</param>
        /// <param name="perdiodeTyp">Dauerbuchungstyp bei Periodischer Verbuchung</param>
        /// <param name="bewegungTyp">Bewegungstyp bei Bewegung</param>
        /// <param name="wert">Vorbelegung für Wert Feld</param>
        /// <param name="kommentar">Vorbelegung für Kommentar Feld</param>
        private Maske2(Controller c, M2ModusTypen modusTyp, DauerBuchungsTyp perdiodeTyp, BuchungsTyp bewegungTyp, decimal wert = 0, string kommentar = "") : this(c)
        {
            _modusTyp = modusTyp;

            textBoxWert.Text = wert.ToString();
            richTextBoxKommentar.Text = kommentar;

            // ToDo: Periode Combobox freischalten, wenn implementiert!
            comboBoxPeriode.Enabled = false;

            comboModus.Items.Add(M2Ressourcen.ModusEinkommen);
            comboModus.Items.Add(M2Ressourcen.ModusAusgaben);
            comboModus.Items.Add(M2Ressourcen.ModusZinsen);

            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeSonntag);
            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeMontag);
            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeDienstag);
            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeMittwoch);
            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeDonnerstag);
            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeFreitag);
            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeSamstag);
            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeWoechentlich);
            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeMonatlich);
            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeJaehrlich);
            comboBoxPeriode.Items.Add(M2Ressourcen.PeriodeBenutzerdefiniert);

            switch (modusTyp)
            {
                case M2ModusTypen.Bewegung:
                    comboModus.Enabled = false;
                    comboModus.Items.Add(M2Ressourcen.ModusBewegung);
                    comboModus.SelectedItem = M2Ressourcen.ModusBewegung;

                    comboBoxPeriode.Visible = false;

                    comboBewegung.Items.Add(M2Ressourcen.BewegungAdd);
                    comboBewegung.Items.Add(M2Ressourcen.BewegungSub);
                    comboBewegung.Items.Add(M2Ressourcen.BewegungGleich);
                    comboBewegung.Visible = true; // Standardmäßig ausgeblendet, nur angezeigt, wenn Bewegungstyp gefordert
                    break;

                case M2ModusTypen.Einkommen:
                    comboModus.SelectedItem = M2Ressourcen.ModusEinkommen;
                    break;

                case M2ModusTypen.Ausgaben:
                    comboModus.SelectedItem = M2Ressourcen.ModusAusgaben;
                    break;

                case M2ModusTypen.Zinsen:
                    comboModus.SelectedItem = M2Ressourcen.ModusZinsen;
                    break;
            }

            if (perdiodeTyp != DauerBuchungsTyp.Dummy)
            {
                switch (perdiodeTyp)
                {
                    case DauerBuchungsTyp.Sonntags:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeSonntag;
                        break;
                    case DauerBuchungsTyp.Montags:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeMontag;
                        break;
                    case DauerBuchungsTyp.Dienstags:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeDienstag;
                        break;
                    case DauerBuchungsTyp.Mittwochs:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeMittwoch;
                        break;
                    case DauerBuchungsTyp.Donnerstags:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeDonnerstag;
                        break;
                    case DauerBuchungsTyp.Freitags:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeFreitag;
                        break;
                    case DauerBuchungsTyp.Samstags:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeSamstag;
                        break;
                    case DauerBuchungsTyp.Woechentlich:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeWoechentlich;
                        break;
                    case DauerBuchungsTyp.Monatlich:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeMonatlich;
                        break;
                    case DauerBuchungsTyp.Jaehrlich:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeJaehrlich;
                        break;
                    case DauerBuchungsTyp.Benutzerdefiniert:
                        comboBoxPeriode.SelectedItem = M2Ressourcen.PeriodeBenutzerdefiniert;
                        break;
                }
            }

            if (bewegungTyp != BuchungsTyp.KeinBuchungstyp)
            {
                switch (bewegungTyp)
                {
                    case BuchungsTyp.Addition:
                        comboBewegung.SelectedItem = M2Ressourcen.BewegungAdd;
                        break;
                    case BuchungsTyp.Subtraktion:
                        comboBewegung.SelectedItem = M2Ressourcen.BewegungSub;
                        break;
                    case BuchungsTyp.Gleichsetzen:
                        comboBewegung.SelectedItem = M2Ressourcen.BewegungGleich;
                        break;
                }

                comboBewegung.Enabled = true;
            }
        }

        /// <summary>
        /// Initialisiert ein neues Objekt der Klasse 
        /// Bereitet die Maske vor
        /// Start im Erfassungsmodus
        /// </summary>
        /// <param name="c"><paramref name="c"/></param>
        /// <param name="konto">Zugrundeliegendes Konto</param>
        /// <param name="modusTyp"><paramref name="modusTyp"/></param>
        /// <param name="perdiodeTyp"><paramref name="periodeTyp"/></param>
        /// <param name="bewegungTyp"><paramref name="bewegungTyp"/></param>
        /// <param name="uebernahmeCallback">Callback Funktion für die Übernahme der Daten</param>
        /// <param name="wert"><paramref name="wert"/></param>
        /// <param name="kommentar"><paramref name="kommentar"/></param>
        private Maske2(Controller c, Model konto, M2ModusTypen modusTyp, DauerBuchungsTyp periodeTyp, BuchungsTyp bewegungTyp, Controller.UebernehmeM2ErfassungHandler uebernahmeCallback, decimal wert = 0, string kommentar = "")
            : this(c, modusTyp, periodeTyp, bewegungTyp, wert, kommentar)
        {
            _konto = konto;

            this.Text = M2Ressourcen.MaskeErfassung;

            _uebernahmeErfassungCallback = uebernahmeCallback;

            dateAnlage.Value = DateTime.Now;
        }

        /// <summary>
        /// Initialisiert ein neues Objekt der Klasse 
        /// Bereitet die Maske vor
        /// Start im Erfassungsmodus
        /// Erfassung : Periodischer Eintrag
        /// </summary>
        /// <param name="c"><paramref name="c"/></param>
        /// <param name="konto"><paramref name="konto"/></param>
        /// <param name="modusTyp"><paramref name="modusTyp"/></param>
        /// <param name="perdiodeTyp">Dauerbuchungstyp Vorbelegung</param>
        /// <param name="uebernahmeCallback"><paramref name="uebernahmeCallback"/></param>
        public Maske2(Controller c, Model konto, M2ModusTypen modusTyp, DauerBuchungsTyp perdiodeTyp, Controller.UebernehmeM2ErfassungHandler uebernahmeCallback) 
            : this(c, konto, modusTyp, perdiodeTyp, BuchungsTyp.KeinBuchungstyp, uebernahmeCallback)
        {
            
        }

        /// <summary>
        /// Initialisiert ein neues Objekt der Klasse 
        /// Bereitet die Maske vor
        /// Start im Erfassungsmodus
        /// Erfassung : Bewegung
        /// </summary>
        /// <param name="c"><paramref name="c"/></param>
        /// <param name="konto"><paramref name="konto"/></param>
        /// <param name="bewegungTyp">Bewegungstyp Vorbelegung</param>
        /// <param name="uebernahmeCallback"><paramref name="uebernahmeCallback"/></param>
        public Maske2(Controller c, Model konto, BuchungsTyp bewegungTyp, Controller.UebernehmeM2ErfassungHandler uebernahmeCallback ) 
            : this(c, konto, M2ModusTypen.Bewegung, DauerBuchungsTyp.Dummy, bewegungTyp, uebernahmeCallback)
        {
            dateDatum.Value = DateTime.Now;
        }

        /// <summary>
        /// Initialisiert ein neues Objekt der Klasse 
        /// Bereitet die Maske vor
        /// Start im Korrekturmodus
        /// </summary>
        /// <param name="c"><paramref name="c"/></param>
        /// <param name="eintragBase">Zu korrigierender Eintrag</param>
        /// <param name="modusTyp"><paramref name="modusTyp"/></param>
        /// <param name="perdiodeTyp"><paramref name="periodeTyp"/></param>
        /// <param name="bewegungTyp"><paramref name="bewegungTyp"/></param>
        /// <param name="uebernahmeCallback">Callback Funktion für die Übernahme der Daten</param>
        private Maske2(Controller c, EintragBase eintragBase, M2ModusTypen modusTyp, DauerBuchungsTyp periodeTyp, BuchungsTyp bewegungTyp, Controller.UebernehmeM2KorrekturHandler uebernahmeCallback)
            : this(c, modusTyp, periodeTyp, bewegungTyp, eintragBase.Wert, eintragBase.Kommentar)
        {
            this.Text = M2Ressourcen.MaskeKorrektur;

            _uebernahmeKorrekturCallback = uebernahmeCallback;

            dateAnlage.Value = eintragBase.Erstellt;

            _korrekturEintrag = eintragBase;
        }

        /// <summary>
        /// Initialisiert ein neues Objekt der Klasse 
        /// Bereitet die Maske vor
        /// Start im Korrekturmodus
        /// Korrektur : Periodischer Eintrag
        /// </summary>
        /// <param name="c"><paramref name="c"/></param>
        /// <param name="dauerEintrag">Zu korrigierender Dauereintrag</param>
        /// <param name="modusTyp"><paramref name="modusTyp"/></param>
        /// <param name="uebernahmeCallback"><paramref name="uebernahmeCallback"/></param>
        public Maske2(Controller c, DauerEintrag dauerEintrag, M2ModusTypen modusTyp, Controller.UebernehmeM2KorrekturHandler uebernahmeCallback)
            : this(c, dauerEintrag, modusTyp, dauerEintrag.Typ, BuchungsTyp.KeinBuchungstyp, uebernahmeCallback)
        {
            comboModus.Enabled = false;
        }

        /// <summary>
        /// Initialisiert ein neues Objekt der Klasse 
        /// Bereitet die Maske vor
        /// Start im Korrekturmodus
        /// Korrektur : Bewegung
        /// </summary>
        /// <param name="c"><paramref name="c"/></param>
        /// <param name="eintrag">Zu korrigierender Eintrag</param>
        /// <param name="uebernahmeCallback"><paramref name="uebernahmeCallback"/></param>
        public Maske2(Controller c, Eintrag eintrag, Controller.UebernehmeM2KorrekturHandler uebernahmeCallback)
            : this(c, eintrag, M2ModusTypen.Bewegung, DauerBuchungsTyp.Dummy, eintrag.Typ, uebernahmeCallback)
        {
            dateDatum.Value = eintrag.Datum;
        }

        #endregion Konstruktor

        #region Eigenschaften
        /// <summary>
        /// Aufrufender Controller
        /// </summary>
        private Controller Ctrl { get => _controller; }

        /// <summary>
        /// Aktuell gewählter Modus
        /// </summary>
        private M2ModusTypen ModusTyp
        {
            get
            {
                if (comboModus.SelectedItem == null)
                    return M2ModusTypen.Dummy;

                if (comboModus.SelectedItem.ToString().Equals(M2Ressourcen.ModusBewegung))
                    return M2ModusTypen.Bewegung;
                else if (comboModus.SelectedItem.ToString().Equals(M2Ressourcen.ModusEinkommen))
                    return M2ModusTypen.Einkommen;
                else if (comboModus.SelectedItem.ToString().Equals(M2Ressourcen.ModusAusgaben))
                    return M2ModusTypen.Ausgaben;
                else if (comboModus.SelectedItem.ToString().Equals(M2Ressourcen.ModusZinsen))
                    return M2ModusTypen.Zinsen;

                return M2ModusTypen.Dummy;
            }
        }

        /// <summary>
        /// Aktuell gewählter Bewegungstyp
        /// </summary>
        private BuchungsTyp BewegungTyp
        {
            get
            {
                if (comboBewegung.SelectedItem == null)
                    return BuchungsTyp.KeinBuchungstyp;

                if (comboBewegung.SelectedItem.ToString().Equals(M2Ressourcen.BewegungAdd))
                    return BuchungsTyp.Addition;
                else if (comboBewegung.SelectedItem.ToString().Equals(M2Ressourcen.BewegungSub))
                    return BuchungsTyp.Subtraktion;
                else if (comboBewegung.SelectedItem.ToString().Equals(M2Ressourcen.BewegungGleich))
                    return BuchungsTyp.Gleichsetzen;

                return BuchungsTyp.KeinBuchungstyp;
            }
        }

        /// <summary>
        /// Aktuell gewählter Dauerbuchungstyp
        /// </summary>
        private DauerBuchungsTyp DauerBuchungsTyp
        {
            get
            {
                if (comboBoxPeriode.SelectedItem == null)
                    return DauerBuchungsTyp.Dummy;

                if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeSonntag))
                    return DauerBuchungsTyp.Sonntags;
                else if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeMontag))
                    return DauerBuchungsTyp.Montags;
                else if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeDienstag))
                    return DauerBuchungsTyp.Dienstags;
                else if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeMittwoch))
                    return DauerBuchungsTyp.Mittwochs;
                else if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeDonnerstag))
                    return DauerBuchungsTyp.Donnerstags;
                else if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeFreitag))
                    return DauerBuchungsTyp.Freitags;
                else if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeSamstag))
                    return DauerBuchungsTyp.Samstags;
                else if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeWoechentlich))
                    return DauerBuchungsTyp.Woechentlich;
                else if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeMonatlich))
                    return DauerBuchungsTyp.Monatlich;
                else if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeJaehrlich))
                    return DauerBuchungsTyp.Jaehrlich;
                else if (comboBoxPeriode.SelectedItem.ToString().Equals(M2Ressourcen.PeriodeBenutzerdefiniert))
                    return DauerBuchungsTyp.Benutzerdefiniert;

                return DauerBuchungsTyp.Dummy;
            }
        }

        /// <summary>
        /// Gibt an, ob die Maske im Erfassungsmodus ist
        /// </summary>
        private bool IsErfassung { get => _uebernahmeErfassungCallback != null; }

        /// <summary>
        /// Gibt an, ob die Maske im Korrekturmodus ist
        /// </summary>
        private bool IsKorrektur { get => _uebernahmeKorrekturCallback != null; }
        #endregion Eigenschaften

        #region Button Events
        private void buttonSpeichern_Click(object sender, EventArgs e)
        {
            if (textBoxWert.Text.Equals(""))
            {
                MessageBox.Show(this, M2Ressourcen.MsgBoxKeinWert, M2Ressourcen.MsgBoxKeinWertUeberschrift, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (IsErfassung)
                    _uebernahmeErfassungCallback(_konto, ModusTyp, DauerBuchungsTyp, BewegungTyp, dateDatum.Value.Date, decimal.Parse(textBoxWert.Text), richTextBoxKommentar.Text);
                if (IsKorrektur)
                    _uebernahmeKorrekturCallback(_korrekturEintrag, ModusTyp, DauerBuchungsTyp, BewegungTyp, dateDatum.Value.Date, decimal.Parse(textBoxWert.Text), richTextBoxKommentar.Text);
                Close();
            }
        }

        private void buttonAbbrechen_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion Button Events
    }
}
