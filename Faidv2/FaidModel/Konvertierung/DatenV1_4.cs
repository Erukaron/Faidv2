using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexHandyDandyAuxiliaryFunctions;

namespace Faid
{
    [Serializable]
    public class DatenV1_4
    {
        #region Fields (unverschlüsselt)

        public static String version = "1.4";

        /// <summary>
        /// Hash über das eingegebene Passwort
        /// </summary>
        private String _passwortHash;

        /// <summary>
        /// Name des Kontos
        /// </summary>
        private String _kontoName;

        #endregion Fields (unverschlüsselt)

        #region Fields (Verschlüsselt)

        /// <summary>
        /// Der aktuelle Kontostand
        /// </summary>
        public double _kontostand;

        /// <summary>
        /// Zeitpunkt der letzten Aktualisierung (Verbuchung von Einkommen/Ausgaben)
        /// </summary>
        public DateTime _letztesUpdate;

        /// <summary>
        /// Einkommen auf monatlicher Basis
        /// </summary>
        public Dictionary<String, double> _einkommen;

        /// <summary>
        /// Ausgaben auf monatlicher Basis
        /// </summary>
        public Dictionary<String, double> _ausgaben;

        /// <summary>
        /// Liste über getätigte Kontobewegungen
        /// </summary>
        public List<String> _kontobewegungen;

        public Dictionary<string, object> _weitereDaten;

        #endregion Fields (Verschlüsselt)

        #region Constructor

        internal DatenV1_4(String kontoName, String passwortHash = "")
        {
            _passwortHash = passwortHash;
            _kontoName = kontoName;

            _kontostand = 0;
            _letztesUpdate = DateTime.Now;
            _einkommen = new Dictionary<string, double>();
            _ausgaben = new Dictionary<string, double>();
            _kontobewegungen = new List<string>();
            _weitereDaten = new Dictionary<string, object>();
        }

        #endregion Constructor

        #region Properties

        internal String PasswortHash
        {
            get
            {
                return _passwortHash;
            }

            set
            {
                _passwortHash = value;
            }
        }

        internal String KontoName
        {
            get
            {
                return _kontoName;
            }
        }

        internal double Kontostand
        {
            get
            {
                return _kontostand;
            }

            set
            {
                _kontostand = value;
            }
        }

        internal DateTime LetztesUpdate
        {
            get
            {
                return _letztesUpdate;
            }

            set
            {
                _letztesUpdate = value;
            }
        }

        internal Dictionary<String, double> Einkommen { get { return _einkommen; } }

        internal Dictionary<String, double> Ausgaben { get { return _ausgaben; } }

        internal List<String> Kontobewegungen
        {
            get
            {
                return _kontobewegungen;
            }
        }

        /// <summary>
        /// Noch zu verbuchende DatenV1_3, die nicht verbucht werden konnte, da Passwort nicht eingegeben wurde
        /// </summary>
        internal Dictionary<String, double> ZuVerbuchen { get; set; }

        internal Dictionary<string, object> WeitereDaten
        {
            get
            {
                if (!_weitereDaten.ContainsKey(WeitereDatenIndexer.DATEN_KEY_ZINSEN))
                    _weitereDaten.Add(WeitereDatenIndexer.DATEN_KEY_ZINSEN, (double)0);

                return _weitereDaten;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Setzt den Kontostand auf den angegebenen Wert
        /// </summary>
        /// <param name="k">Neuer Wert für den Kontostand</param>
        /// <param name="manuell">Falls dieses Kennzeichen true ist, wird eine Kontobewegung erzeugt</param>
        public void SetKontostand(double k, bool manuell)
        {
            _kontostand = k;
            _letztesUpdate = DateTime.Now;


            if (manuell) //Nicht, falls automatisch verbucht wird (mntl.)
                _kontobewegungen.Add("= Kontostand manuell gesetzt: " + k);
        }

        /// <summary>
        /// Erhöht den Kontostand und schreibt die Bewegung
        /// </summary>
        /// <param name="bewBez">Bezeichnung der in die Kontobewegungsliste einzutragenden Bewegung</param>
        /// <param name="k">Zu erhöhender Wert</param>
        /// <param name="padding">Leerzeichen vor dem Wert</param>
        public void addKontostand(String bewBez, double k, int padding = 0)
        {
            _kontostand += k;
            _letztesUpdate = DateTime.Now;

            string bewegung = "+ " + bewBez + ": ";
            for (int i = padding; i > 0; i--)
                bewegung += " ";
            bewegung += k;

            _kontobewegungen.Add(bewegung);
        }

        /// <summary>
        /// Verringert den Kontostand und schreibt die Bewegung
        /// </summary>
        /// <param name="bewBez">Bezeichnung der in die Kontobewegungsliste einzutragenden Bewegung</param>
        /// <param name="k">Zu verringernder Wert</param>
        /// <param name="padding">Leerzeichen vor dem Wert</param>
        public void subKontostand(String bewBez, double k, int padding = 0)
        {
            _kontostand -= k;
            _letztesUpdate = DateTime.Now;

            string bewegung = "- " + bewBez + ": ";
            for (int i = padding; i > 0; i--)
                bewegung += " ";
            bewegung += k;

            _kontobewegungen.Add(bewegung);
        }

        /// <summary>
        /// Gibt den dispositiven Kontostand für eine spezifizierte Anzahl Monate zurück
        /// </summary>
        /// <param name="monate">Anzahl der Monate, für die die dispositive Betrachtung durchgeführt werden soll</param>
        /// <returns>Dispositiven Kontostand</returns>
        public double KontostandDispositiv(int monate = 1)
        {
            return KontostandDispositiv(Kontostand, monate);
        }

        /// <summary>
        /// Gibt den dispositiven Kontostand für eine spezifizierte Anzahl Monate zurück
        /// </summary>
        /// <param name="ausgangsKontostand">Der Kontostand für den die dispositive Betrachtung durchgeführt werden soll</param>
        /// <param name="monate">Anzahl der Monate, für die die dispositive Betrachtung durchgeführt werden soll</param>
        /// <returns>Dispositiven Kontostand</returns>
        public double KontostandDispositiv(double ausgangsKontostand, int monate = 1)
        {
            double zielKontostand = ausgangsKontostand;

            for (int i = 0; i < monate; i++)
            {
                zielKontostand += MntlZinssatz(zielKontostand, (double)WeitereDaten[WeitereDatenIndexer.DATEN_KEY_ZINSEN]);

                foreach (KeyValuePair<String, double> kvp in _einkommen)
                {
                    zielKontostand += kvp.Value;
                }

                foreach (KeyValuePair<String, double> kvp in _ausgaben)
                {
                    zielKontostand -= kvp.Value;
                }
            }

            return zielKontostand;
        }

        /// <summary>
        /// Aktualisiert den Kontostand (führt ausstehende monatliche Verbuchungen durch)
        /// </summary>
        public void UpdateKontostand()
        {
            int letztesJahr = _letztesUpdate.Year;
            int letzterMonat = _letztesUpdate.Month;
            int aktuellesJahr = DateTime.Now.Year;
            int aktuellerMonat = DateTime.Now.Month;
            bool buchungAusgefuehrt = false;
            bool verbuchungStartMsg = false;

            int maxLaengeKey = 0;
            int maxLaengeValue = 0;
            double zinsen = 0;

            // Größten String suchen
            foreach (KeyValuePair<String, double> kvp in _ausgaben)
            {
                if (kvp.Key.Length > maxLaengeKey)
                    maxLaengeKey = kvp.Key.Length;

                if (((int)kvp.Value).GetIntDigitCount() > maxLaengeValue)
                    maxLaengeValue = ((int)kvp.Value).GetIntDigitCount();
            }
            foreach (KeyValuePair<String, double> kvp in _einkommen)
            {
                if (kvp.Key.Length > maxLaengeKey)
                    maxLaengeKey = kvp.Key.Length;

                if (((int)kvp.Value).GetIntDigitCount() > maxLaengeValue)
                    maxLaengeValue = ((int)kvp.Value).GetIntDigitCount();
            }

            if (letztesJahr < aktuellesJahr)
            {
                for (; (letztesJahr < aktuellesJahr);)
                {
                    //In diesem Fall kein ganzes Jahr ergänzen, da z.B. nur 8 Monate weiter sind
                    if ((letztesJahr == (aktuellesJahr - 1)) && letzterMonat > aktuellerMonat)
                    {
                        for (int i = aktuellerMonat + 12 - letzterMonat; i > 0; i--)
                        {
                            if (!verbuchungStartMsg)
                            {
                                _kontobewegungen.Add("------------------------------------------------------------");
                                _kontobewegungen.Add("Auflistung monatlicher Verbuchungen");

                                verbuchungStartMsg = true;
                            }

                            if (WeitereDaten.Keys.Contains(WeitereDatenIndexer.DATEN_KEY_ZINSEN))
                            {
                                zinsen = MntlZinssatz(Kontostand, (double)WeitereDaten[WeitereDatenIndexer.DATEN_KEY_ZINSEN]);
                                addKontostand("Verbuchung " + DatumZuText(letzterMonat, letztesJahr) + " Zinsen   ", zinsen, maxLaengeKey + maxLaengeValue - ((int)zinsen).GetIntDigitCount() + 2);
                            }

                            foreach (KeyValuePair<String, double> kvp in _einkommen)
                                addKontostand("Verbuchung " + DatumZuText(letzterMonat, letztesJahr) + " Einkommen: " + kvp.Key + new string(' ', maxLaengeKey - kvp.Key.Length), kvp.Value, maxLaengeValue - ((int)kvp.Value).GetIntDigitCount());

                            foreach (KeyValuePair<String, double> kvp in _ausgaben)
                                subKontostand("Verbuchung " + DatumZuText(letzterMonat, letztesJahr) + " Ausgaben : " + kvp.Key + new string(' ', maxLaengeKey - kvp.Key.Length), kvp.Value, maxLaengeValue - ((int)kvp.Value).GetIntDigitCount());

                            if (++letzterMonat == 13)
                            {
                                letzterMonat = 1;
                                letztesJahr++;
                            }
                        }

                        break; // Schleife kann hier unterbrochen werden, um noch mehr Spaghetti Code zu erzeugen ;-) -> Die Verbuchung soll für weniger als ein Jahr ausgeführt werden, das ist hiermit passiert.
                    }
                    else
                    {
                        //Ein Jahr dispositives Einkommen hinzufügen
                        for (int i = 0; i < 12; i++)
                        {
                            if (!verbuchungStartMsg)
                            {
                                _kontobewegungen.Add("------------------------------------------------------------");
                                _kontobewegungen.Add("Auflistung monatlicher Verbuchungen");

                                verbuchungStartMsg = true;
                            }

                            if (WeitereDaten.Keys.Contains(WeitereDatenIndexer.DATEN_KEY_ZINSEN))
                            {
                                zinsen = MntlZinssatz(Kontostand, (double)WeitereDaten[WeitereDatenIndexer.DATEN_KEY_ZINSEN]);
                                addKontostand("Verbuchung " + DatumZuText(letzterMonat, letztesJahr) + " Zinsen   ", zinsen, maxLaengeKey + maxLaengeValue - ((int)zinsen).GetIntDigitCount() + 2);
                            }

                            foreach (KeyValuePair<String, double> kvp in _einkommen)
                                addKontostand("Verbuchung " + DatumZuText(letzterMonat, letztesJahr) + " Einkommen: " + kvp.Key + new string(' ', maxLaengeKey - kvp.Key.Length), kvp.Value, maxLaengeValue - ((int)kvp.Value).GetIntDigitCount());

                            foreach (KeyValuePair<String, double> kvp in _ausgaben)
                                subKontostand("Verbuchung " + DatumZuText(letzterMonat, letztesJahr) + " Ausgaben : " + kvp.Key + new string(' ', maxLaengeKey - kvp.Key.Length), kvp.Value, maxLaengeValue - ((int)kvp.Value).GetIntDigitCount());

                            letzterMonat++;
                            if (letzterMonat == 13)
                            {
                                letzterMonat = 1;
                                letztesJahr++;
                            }
                        }
                    }
                }

                buchungAusgefuehrt = true;
            }

            if (letzterMonat < aktuellerMonat)
            {
                for (; letzterMonat < aktuellerMonat; letzterMonat++)
                {
                    if (!verbuchungStartMsg)
                    {
                        _kontobewegungen.Add("------------------------------------------------------------");
                        _kontobewegungen.Add("Auflistung monatlicher Verbuchungen");

                        verbuchungStartMsg = true;
                    }

                    if (WeitereDaten.Keys.Contains(WeitereDatenIndexer.DATEN_KEY_ZINSEN))
                    {
                        zinsen = MntlZinssatz(Kontostand, (double)WeitereDaten[WeitereDatenIndexer.DATEN_KEY_ZINSEN]);
                        addKontostand("Verbuchung " + DatumZuText(letzterMonat, letztesJahr) + " Zinsen   ", zinsen, maxLaengeKey + maxLaengeValue - ((int)zinsen).GetIntDigitCount() + 2);
                    }

                    foreach (KeyValuePair<String, double> kvp in _einkommen)
                        addKontostand("Verbuchung " + DatumZuText(letzterMonat, letztesJahr) + " Einkommen: " + kvp.Key + new string(' ', maxLaengeKey - kvp.Key.Length), kvp.Value, maxLaengeValue - ((int)kvp.Value).GetIntDigitCount());

                    foreach (KeyValuePair<String, double> kvp in _ausgaben)
                        subKontostand("Verbuchung " + DatumZuText(letzterMonat, letztesJahr) + " Ausgaben : " + kvp.Key + new string(' ', maxLaengeKey - kvp.Key.Length), kvp.Value, maxLaengeValue - ((int)kvp.Value).GetIntDigitCount());
                }

                buchungAusgefuehrt = true;
            }

            _letztesUpdate = DateTime.Now;

            if (buchungAusgefuehrt)
            {
                _kontobewegungen.Add("= Monatliche Verbuchungen für " + _letztesUpdate + " ausgeführt. Neuer Kontostand:\n  " + _kontostand);
                _kontobewegungen.Add("------------------------------------------------------------");
            }
        }

        public string DatumZuText(int monat, int jahr)
        {
            return monat.ToString().PadLeft(2, '0') + "." + jahr;
        }

        /// <summary>
        /// Aktualisiert den Kontostand (führt ausstehende monatliche Verbuchungen durch)
        /// </summary>
        public void UpdateKontostandAlt()
        {
            int letztesJahr = _letztesUpdate.Year;
            int letzterMonat = _letztesUpdate.Month;
            int aktuellesJahr = DateTime.Now.Year;
            int aktuellerMonat = DateTime.Now.Month;
            bool buchungAusgefuehrt = false;

            if (letztesJahr < aktuellesJahr)
            {
                for (; (letztesJahr < aktuellesJahr); letztesJahr++)
                {
                    //In diesem Fall kein ganzes Jahr ergänzen, da z.B. nur 8 Monate weiter sind
                    if ((letztesJahr == (aktuellesJahr - 1)) && letzterMonat > aktuellerMonat)
                    {
                        for (int i = aktuellerMonat + 12; letzterMonat < i; letzterMonat++)
                        {
                            if (letzterMonat == 13)
                            {
                                letztesJahr++;
                            }

                            SetKontostand(KontostandDispositiv(_kontostand, 1), false);
                        }
                    }
                    else
                    {
                        //Ein Jahr dispositives Einkommen hinzufügen
                        for (int i = 0; i < 12; i++)
                        {
                            SetKontostand(KontostandDispositiv(_kontostand, 1), false);
                        }
                    }
                }

                buchungAusgefuehrt = true;
            }

            if (letzterMonat < aktuellerMonat)
            {
                for (; letzterMonat < aktuellerMonat; letzterMonat++)
                {
                    SetKontostand(KontostandDispositiv(_kontostand, 1), false);
                }

                buchungAusgefuehrt = true;
            }

            _letztesUpdate = DateTime.Now;

            if (buchungAusgefuehrt)
            {
                _kontobewegungen.Add("= Monatliche Verbuchungen für " + _letztesUpdate + " ausgeführt. Neuer Kontostand:\n  " + _kontostand);
            }
        }

        /// <summary>
        /// Fügt das angegebene monatliche Einkommen hinzu
        /// </summary>
        /// <param name="name">Bezeichnung des monatlichen Einkommens</param>
        /// <param name="wert">Höhe des monatlichen Einkommens</param>
        /// <returns><c>false</c>, wenn dieses Einkommen bereits existiert</returns>
        public bool EinkommenHinzufuegen(String name, double wert)
        {
            try
            {
                _einkommen.Add(name, wert);
                return true;
            }
#pragma warning disable CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
            catch (Exception e)
#pragma warning restore CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
            {
                return false;
            }
        }

        /// <summary>
        /// Entfernt das angegebene monatliche Einkommen
        /// </summary>
        /// <param name="name">Bezeichnung des monatlichen Einkommens</param>
        /// <returns><c>false</c>, wenn dieses Einkommen nicht gelöscht werden konnte (weil es nicht existiert)</returns>
        public bool EinkommenEntfernen(String name)
        {
            try
            {
                return _einkommen.Remove(name);
            }
#pragma warning disable CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
            catch (Exception e)
#pragma warning restore CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
            {
                return false;
            }
        }

        /// <summary>
        /// Fügt die angegebenen monatlichen Ausgaben hinzu
        /// </summary>
        /// <param name="name">Bezeichnung der monatlichen Ausgaben</param>
        /// <param name="wert">Höhe der monatlichen Ausgaben</param>
        /// <returns><c>false</c>, wenn diese Ausgabe bereits existiert</returns>
        public bool AusgabenHinzufuegen(String name, double wert)
        {
            try
            {
                _ausgaben.Add(name, wert);
                return true;
            }
#pragma warning disable CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
            catch (Exception e)
#pragma warning restore CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
            {
                return false;
            }
        }

        /// <summary>
        /// Entfernt die angegebene monatliche Ausgabe
        /// </summary>
        /// <param name="name">Bezeichnung der monatlichen Ausgaben</param>
        /// <returns><c>false</c>, wenn diese Ausgabe nicht gelöscht werden konnte (weil sie nicht existiert)</returns>
        public bool AusgabenEntfernen(String name)
        {
            try
            {
                return _ausgaben.Remove(name);
            }
#pragma warning disable CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
            catch (Exception e)
#pragma warning restore CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
            {
                return false;
            }
        }

        /// <summary>
        /// Storniert die angegebene Buchung
        /// </summary>
        /// <param name="buchungsTyp">Zu stornierende Buchungsart (Nur Addition/Subtraktion)</param>
        /// <param name="bezeichnung">Komplette Bezeichnung der Buchung (z.B. "+ 16.2.2019 - Katzencaffee Aachen : 17")</param>
        /// <returns>
        ///  1 = Buchung erfolgreich storniert
        ///  0 = Fehler beim Erstatten (Erstattungsbetrag konnte nicht ermittelt werden)
        /// -1 = Buchung mit angegebener Bezeichnung nicht vorhanden
        /// -2 = Angegebener Buchungstyp kann nicht storniert werden (nur Addition und Subtraktion!)
        /// </returns>
        public int Stornieren(BuchungsTyp buchungsTyp, String bezeichnung)
        {
            if (buchungsTyp != BuchungsTyp.Addition && buchungsTyp != BuchungsTyp.Subtraktion)
                return -2;

            if (_kontobewegungen.Any(s => s.Contains(bezeichnung))) //Ist die gesuchte Buchung überhaupt vorhanden?
            {
                // Merken der Bewegung zum herausfinden des Betrages
                String bewegung = _kontobewegungen.Last(s => s.Contains(bezeichnung));

                double erstattungsBetrag = 0;
                String[] bewegungTeil = bewegung.Split(' ');
                if (double.TryParse(bewegungTeil[bewegungTeil.Length - 1], out erstattungsBetrag)) // Holen des Geldbetrags (steht ganz hinten im String)
                {
                    // Löschen der Bewegung
                    _kontobewegungen.RemoveAt(_kontobewegungen.IndexOf(_kontobewegungen.Last(s => s.Contains(bezeichnung))));

                    if (buchungsTyp == BuchungsTyp.Addition)
                    {
                        _kontostand -= erstattungsBetrag;
                    }
                    else if (buchungsTyp == BuchungsTyp.Subtraktion)
                    {
                        _kontostand += erstattungsBetrag;
                    }

                    Console.WriteLine("Buchung storniert.");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Beim Erstatten ist ein Fehler aufgetreten!");
                    return 0;
                }
            }
            else
            {
                Console.WriteLine("Es ist keine " + (buchungsTyp == BuchungsTyp.Addition ? "Eingangs" : "Ausgangs") + "buchung mit der Identität \"" + bezeichnung + "\" vorhanden");
                return -1;
            }
        }

        /// <summary>
        /// Errechnet den monatlichen Zinswert: ACHTUNG! Gibt den tatsächlichen Zinswert zurück, NICHT den Kontostand nach Verrechnung mit Zinsen!
        /// </summary>
        /// <param name="ausgangswert">Ausgangskontostand</param>
        /// <param name="zinsen">Zinsen</param>
        /// <returns>Zinswert</returns>
        public double MntlZinssatz(double ausgangswert, double zinsen)
        {
            double ergebnis = 0;

            ergebnis = ausgangswert * zinsen * ((double)1 / (double)12);
            ergebnis = Math.Round(ergebnis, 2);

            return ergebnis;
        }

        #endregion Methods
    }
}
