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
        private Controller.UebernehmeM2Handler _uebernahmeCallback;
        #endregion Felder

        #region Konstruktor
        /// <summary>
        /// Initiailisiert eine neue Instanz der Klasse
        /// </summary>
        /// <param name="c">Aufrufendes Controller objekt</param>
        /// <param name="konto">Kontodatei</param>
        /// <param name="modusTyp">Modus, in dem die Maske aufgerufen werden soll</param>
        /// <param name="uebernahmeCallback">Callback für die Verarbeitung der erstellten Daten</param>
        public Maske2(Controller c, Model konto, M2ModusTypen modusTyp, DauerBuchungsTyp perdiodeTyp, Controller.UebernehmeM2Handler uebernahmeCallback)
        {
            InitializeComponent();

            _controller = c;
            _modusTyp = modusTyp;
            _uebernahmeCallback = uebernahmeCallback;
            _konto = konto;

            dateAnlage.Value = DateTime.Now;

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

        /// <summary>
        /// Initiailisiert eine neue Instanz der Klasse
        /// </summary>
        /// <param name="c">Aufrufendes Controller objekt</param>
        /// <param name="modusTyp">Modus, in dem die Maske aufgerufen werden soll</param>
        /// <param name="bewegungTyp">Art der Bewegung</param>
        /// <throws><c>ArgumentException</c>, wenn der <c>modusTyp</c> nicht Bewegung ist</throws>
        public Maske2(Controller c, Model konto, M2ModusTypen modusTyp, Controller.UebernehmeM2Handler uebernahmeCallback, BuchungsTyp bewegungTyp) : this(c, konto, modusTyp, DauerBuchungsTyp.Dummy, uebernahmeCallback)
        {
            if (modusTyp == M2ModusTypen.Bewegung)
                _bewegungTyp = bewegungTyp;
            else
                throw new ArgumentException(string.Format("Zur Angabe eines Bewegungstypen muss der Modus Bewegung gewählt werden!"), "modusTypen");

            comboBewegung.Items.Add(M2Ressourcen.BewegungAdd);
            comboBewegung.Items.Add(M2Ressourcen.BewegungSub);
            comboBewegung.Items.Add(M2Ressourcen.BewegungGleich);

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
                _uebernahmeCallback(_konto, ModusTyp, DauerBuchungsTyp, BewegungTyp, dateDatum.Value, decimal.Parse(textBoxWert.Text), richTextBoxKommentar.Text);
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
