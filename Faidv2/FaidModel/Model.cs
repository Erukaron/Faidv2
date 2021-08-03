using System;
using System.Collections.Generic;
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
        private BindingList<Eintrag> _kontobewegung;

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
            _kontoname = kontoname;
            _kontostand = 0;
            _letztesUpdate = DateTime.Now;
            _einkommen = new BindingList<DauerEintrag>();
            _ausgaben = new BindingList<DauerEintrag>();
            _kontobewegung = new BindingList<Eintrag>();
            _zinsen = new BindingList<DauerEintrag>();
            _weitereDaten = new Dictionary<string, object>();
        }
        #endregion Konstruktor

        #region Eigenschaften
        /// <summary>
        /// Name des Kontos
        /// </summary>
        public string Kontoname { get => _kontoname; }

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
        public DateTime LetztesUpdate { get => _letztesUpdate; set => _letztesUpdate = value; }

        /// <summary>
        /// Datum des zuletzt ausgeführten periodischen Verbuchungen -> Einkommen, Ausgaben, Zinsen
        /// </summary>
        public DateTime LetztePeriodischeAusfuehrung { get => _letztePeriodischeAusfuehrung; set => _letztePeriodischeAusfuehrung = value; }

        /// <summary>
        /// Liste über Einkommen
        /// </summary>
        public BindingList<DauerEintrag> Einkommen { get => _einkommen; }

        /// <summary>
        /// Liste über Ausgaben
        /// </summary>
        public BindingList<DauerEintrag> Ausgaben { get => _ausgaben; }

        /// <summary>
        /// Liste über Kontobewegungen
        /// </summary>
        public BindingList<Eintrag> Kontobewegung { get => _kontobewegung; }

        /// <summary>
        /// Liste über Zinsen
        /// </summary>
        public BindingList<DauerEintrag> Zinsen { get => _zinsen; }

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
            LetztesUpdate = DateTime.Now;
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

            LetztesUpdate = DateTime.Now;
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
            model._kontobewegung = new BindingList<Eintrag>(_kontobewegung);
            model._zinsen = new BindingList<DauerEintrag>(_zinsen);
            model._weitereDaten = new Dictionary<string, object>(_weitereDaten);

            return model;
        }
        #endregion Methoden
    }
}
