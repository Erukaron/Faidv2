using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faidv2.FaidModel
{
    [Serializable]
    public class Model
    {
        #region Delegaten und Events
        public delegate void KontostandAktualisiertEventHandler();
        public event KontostandAktualisiertEventHandler KontostandAktualisiert;
        #endregion Delegaten und Events

        #region Felder
        /// <summary>
        /// Versionsnummer der Daten
        /// </summary>
        public static string Version = "2.0";

        /// <summary>
        /// Name des Kontos
        /// </summary>
        private string _kontoname;

        /// <summary>
        /// Kontostand des Kontos
        /// </summary>
        private decimal _kontostand;

        /// <summary>
        /// Letzte Aktualisierung
        /// </summary>
        private DateTime _letztesUpdate;

        /// <summary>
        /// Datum des zuletzt ausgeführten periodischen Verbuchungen -> Einkommen, Ausgaben, Zinsen
        /// </summary>
        private DateTime _letztePeriodischeAusfuehrung;

        /// <summary>
        /// Liste über Einkommen
        /// </summary>
        private BindingList<DauerEintrag> _einkommen;

        /// <summary>
        /// Liste über Ausgaben
        /// </summary>
        private BindingList<DauerEintrag> _ausgaben;

        /// <summary>
        /// Liste über Kontobewegungen
        /// </summary>
        private ObservableCollection<Eintrag> _kontobewegung;

        /// <summary>
        /// Liste über Zinsen
        /// </summary>
        private BindingList<DauerEintrag> _zinsen;

        /// <summary>
        /// Weitere Daten für Erweiterungen
        /// </summary>
        private Dictionary<string, object> _weitereDaten;

        // ToDo: Bei Anlage neuer Felder unbedigt an DeepCopy denken!
        #endregion Felder

        #region Konstruktor
        /// <summary>
        /// Erstellt ein neues Konto
        /// </summary>
        /// <param name="kontoname">Kontoname des zu erstellenden Kontos</param>
        public Model(string kontoname)
        {
            Kontoname = kontoname;
            Kontostand = 0;
            LetztesUpdate = DateTime.Now;
            LetztePeriodischeAusfuehrung = DateTime.Now;
            Einkommen = new BindingList<DauerEintrag>();
            Ausgaben = new BindingList<DauerEintrag>();
            Kontobewegung = new ObservableCollection<Eintrag>();
            Zinsen = new BindingList<DauerEintrag>();
            WeitereDaten = new Dictionary<string, object>();
        }
        #endregion Konstruktor

        #region Eigenschaften
        /// <summary>
        /// Name des Kontos
        /// </summary>
        public string Kontoname { get => _kontoname; private set => _kontoname = value; }

        /// <summary>
        /// Kontostand des Kontos
        /// </summary>
        public decimal Kontostand 
        { 
            get => _kontostand; 
            private set
            {
                _kontostand = value;

                KontostandAktualisiert?.Invoke();
            }
        }

        /// <summary>
        /// Letzte Aktualisierung
        /// </summary>
        public DateTime LetztesUpdate { get => _letztesUpdate.Date; set => _letztesUpdate = value.Date; }

        /// <summary>
        /// Datum des zuletzt ausgeführten periodischen Verbuchungen -> Einkommen, Ausgaben, Zinsen
        /// </summary>
        public DateTime LetztePeriodischeAusfuehrung { get => _letztePeriodischeAusfuehrung.Date; set => _letztePeriodischeAusfuehrung = value.Date; }

        /// <summary>
        /// Liste über Einkommen
        /// </summary>
        public BindingList<DauerEintrag> Einkommen { get => _einkommen; private set => _einkommen = value; }

        /// <summary>
        /// Liste über Ausgaben
        /// </summary>
        public BindingList<DauerEintrag> Ausgaben { get => _ausgaben; private set => _ausgaben = value; }

        /// <summary>
        /// Liste über Kontobewegungen
        /// </summary>
        public ObservableCollection<Eintrag> Kontobewegung { get => _kontobewegung; private set => _kontobewegung = value; }

        /// <summary>
        /// Liste über Zinsen
        /// </summary>
        public BindingList<DauerEintrag> Zinsen { get => _zinsen; private set => _zinsen = value; }

        /// <summary>
        /// Weitere Daten für Erweiterungen
        /// </summary>
        public Dictionary<string, object> WeitereDaten
        {
            get
            {
                // ToDo: Weitere Daten hier für jeden neuen Key initialisieren
                //if (!_weitereDaten.ContainsKey(WeitereDatenIndexer.DATEN_KEY_?????))
                //    _weitereDaten.Add(WeitereDatenIndexer.DATEN_KEY_ ?????, (<TYPE>)INIT_VALUE);

                return _weitereDaten;
            }

            private set => _weitereDaten = value;
        }
        #endregion Eigenschaften

        #region Methoden
        /// <summary>
        /// Verbucht den Datensatz
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="eintrag">Zu verbuchender Datensatz</param>
        /// <throws><c>ArgumentException</c>, bei nicht zugelassenem Buchungstyp</throws>
        public void Verbuchen(Eintrag eintrag)
        {
            switch (eintrag.Typ)
            {
                case BuchungsTyp.Gleichsetzen:
                    Kontostand = eintrag.Wert;
                    break;
                case BuchungsTyp.Addition:
                    Kontostand += eintrag.Wert;
                    break;
                case BuchungsTyp.Subtraktion:
                    Kontostand -= eintrag.Wert;
                    break;
                default:
                    throw new ArgumentException(String.Format("Buchungstyp {0} nicht zulässig!", eintrag.Typ), "Typ");
            }

            Kontobewegung.Add(eintrag);
            LetztesUpdate = DateTime.Now.Date;
        }

        /// <summary>
        /// Berechnet den aktuellen Kontostand aus allen Bewegungen neu
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        public void NeuberechnungKontostandAusBewegung()
        {
            Kontostand = 0;

            foreach (Eintrag e in Kontobewegung)
            {
                switch (e.Typ)
                {
                    case BuchungsTyp.Gleichsetzen:
                        Kontostand = e.Wert;
                        break;
                    case BuchungsTyp.Addition:
                        Kontostand += e.Wert;
                        break;
                    case BuchungsTyp.Subtraktion:
                        Kontostand -= e.Wert;
                        break;
                }
            }

            LetztesUpdate = DateTime.Now.Date;
        }

        /// <summary>
        /// Erstellt eine deep Copy des aktuellen Objekts
        /// </summary>
        /// <returns>Deep Copy des Objekts</returns>
        public Model DeepCopy()
        {
            Model model = (Model)this.MemberwiseClone();
            model.KontostandAktualisiert = null; // Ansonsten würde bei Änderung des Kontostands die Maske des ursprünglichen Kontos aktualisiert werden
            model._einkommen = new BindingList<DauerEintrag>(_einkommen);
            model._ausgaben = new BindingList<DauerEintrag>(_ausgaben);
            model._kontobewegung = new ObservableCollection<Eintrag>(_kontobewegung);
            model._zinsen = new BindingList<DauerEintrag>(_zinsen);
            model._weitereDaten = new Dictionary<string, object>(_weitereDaten);

            return model;
        }
        #endregion Methoden
    }
}
