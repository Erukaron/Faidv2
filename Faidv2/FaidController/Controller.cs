using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Faidv2.FaidModel;
using Faidv2.FaidView.M1;
using Faidv2.FaidView.M2;
using System.Windows.Forms;
using Faidv2.Properties;
using System.ComponentModel;

namespace Faidv2.FaidController
{
    public class Controller
    {
        #region Delegates und Events 
        public delegate void UebernehmeM2ErfassungHandler(Model konto, M2ModusTypen modus, DauerBuchungsTyp dauerbuchung, BuchungsTyp bewegung, DateTime datum, decimal wert, string kommentar);
        public delegate void UebernehmeM2KorrekturHandler(EintragBase eintrag, M2ModusTypen modus, DauerBuchungsTyp dauerbuchung, BuchungsTyp bewegung, DateTime datum, decimal wert, string kommentar);

        public delegate void SpeichernFehlerEventHandler(object sender, Exception e, string datei);
        public delegate void LadenFehlerEventHandler(object sender, Exception e, string datei);

        public event SpeichernFehlerEventHandler SpeichernFehler;
        public event LadenFehlerEventHandler LadenFehler;
        #endregion Delegates und Events

        #region Felder
        /// <summary>
        /// Zugriff auf die Haupt-View-Komponente
        /// </summary>
        private Maske1 _m1;
        #endregion Felder

        #region Konstruktor
        /// <summary>
        /// Initialisiert eine neue Instanz des Controllers
        /// </summary>
        public Controller()
        {
        }
        #endregion Konstruktor

        #region Eigenschaften
        /// <summary>
        /// Zugriff auf Haupt-View-Komponente
        /// </summary>
        private Maske1 M1 { get => _m1; }
        #endregion Eigenschaften

        /// <summary>
        /// Haupteinstiegspunkt
        /// </summary>
        /// <param name="args">Parameter</param>
        [STAThread]
        public static void Main(string[] args)
        {
            Controller c = new Controller();
            
            Model konto = null;
            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    konto = c.KontoLaden(args[0]);
                }
            }

            c.StartM1(konto);

            // ToDo: Drag&Drop Prüfe Datei bei DragEnter
            // ToDo: Legende
        }

        #region Maske 1 (Haupt)
        /// <summary>
        /// Startet Maske 1
        /// </summary>
        private void StartM1()
        {
            StartM1(null);
        }

        /// <summary>
        /// Startet Maske 1
        /// </summary>
        /// <param name="konto">Kontodatei, die beim Aufruf mit übergeben werden kann</param>
        private void StartM1(Model konto)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (konto == null)
                _m1 = new Maske1(this);
            else
                _m1 = new Maske1(this, konto);

            Application.Run(M1);
        }
        #endregion Maske 1

        #region Maske 2 (Erfassung/Korrektur)
        /// <summary>
        /// Startet Maske 2 zur Erfassung einer Dauerbewegung
        /// </summary>
        /// <param name="konto">Konto, für das Erfassung durchgeführt wird</param>
        /// <param name="modusTyp">Erfassungsmodus</param>
        /// <param name="dauerBuchungsTyp">Dauerbuchungstyp</param>
        public void Erfassung(Model konto, M2ModusTypen modusTyp, DauerBuchungsTyp dauerBuchungsTyp)
        {
            Erfassung(konto, modusTyp, dauerBuchungsTyp, BuchungsTyp.KeinBuchungstyp);
        }

        /// <summary>
        /// Startet Maske 2 zur Erfassung einer Bewegung
        /// </summary>
        /// <param name="konto">Konto, für das Erfassung durchgeführt wird</param>
        /// <param name="bewegungTyp">Bewegungstyp bei Modus Bewegung</param>
        public void Erfassung(Model konto, BuchungsTyp bewegungTyp)
        {
            Erfassung(konto, M2ModusTypen.Bewegung, DauerBuchungsTyp.Dummy, bewegungTyp);
        }

        /// <summary>
        /// Startet Maske 2 zur Erfassung
        /// </summary>
        /// <param name="konto">Konto, für das Erfassung durchgeführt wird</param>
        /// <param name="modusTyp">Erfassungsmodus</param>
        /// <param name="dauerBuchungsTyp">Dauerbuchungstyp</param>
        /// <param name="bewegungTyp">Bewegungstyp bei Modus Bewegung</param>
        private void Erfassung(Model konto, M2ModusTypen modusTyp, DauerBuchungsTyp dauerBuchungsTyp, BuchungsTyp bewegungTyp)
        {
            UebernehmeM2ErfassungHandler uebernahmeHandler = UebernehmeM2Erfassung;

            Maske2 m2;
            if (modusTyp == M2ModusTypen.Bewegung)
                m2 = new Maske2(this, konto, bewegungTyp, uebernahmeHandler);
            else
                m2 = new Maske2(this, konto, modusTyp, dauerBuchungsTyp, uebernahmeHandler);

            m2.Show(M1);
        }

        /// <summary>
        /// Von M2 aufgerufen bei Datenübernahme im Modus Erfassung
        /// </summary>
        /// <param name="konto">Konto</param>
        /// <param name="modus">Art der Übernahme</param>
        /// <param name="dauerbuchung">Typ der Dauerbuchung</param>
        /// <param name="bewegung">Art der Bewegung</param>
        /// <param name="datum">Bewegungsdatum</param>
        /// <param name="wert">Wert</param>
        /// <param name="kommentar">Kommentar</param>
        private void UebernehmeM2Erfassung(Model konto, M2ModusTypen modus, DauerBuchungsTyp dauerbuchung, BuchungsTyp bewegung, DateTime datum, decimal wert, string kommentar)
        {
            switch (modus)
            {
                case M2ModusTypen.Bewegung:
                    konto.Verbuchen(new Eintrag(bewegung, datum, wert, kommentar, false));
                    break;
                case M2ModusTypen.Einkommen:
                    konto.Einkommen.Add(new DauerEintrag(dauerbuchung, wert, kommentar));
                    break;
                case M2ModusTypen.Ausgaben:
                    konto.Ausgaben.Add(new DauerEintrag(dauerbuchung, wert, kommentar));
                    break;
                case M2ModusTypen.Zinsen:
                    konto.Zinsen.Add(new DauerEintrag(dauerbuchung, wert, kommentar));
                    break;
            }
        }

        /// <summary>
        /// Startet Maske 2 zur Korrektur einer Bewegung
        /// </summary>
        /// <param name="eintrag">Bewegung</param>
        public void Korrektur(Eintrag eintrag)
        {
            Korrektur(eintrag, null, M2ModusTypen.Bewegung);
        }

        /// <summary>
        /// Startet Maske 2 zur Korrektur einer Dauerbewegung
        /// </summary>
        /// <param name="eintrag">Dauereintrag</param>
        /// <param name="modus">Art der Dauerbuchung</param>
        public void Korrektur(DauerEintrag eintrag, M2ModusTypen modus)
        {
            Korrektur(null, eintrag, modus);
        }

        /// <summary>
        /// Startet Maske 2 zur Korrektur
        /// </summary>
        /// <param name="eintrag">Eintrag, falls im Bewegungsmodus zu starten, sonst null</param>
        /// <param name="dauerEintrag">Dauereintrag, falls im Dauereintragsmodus zu starten, sonst null</param>
        /// <param name="modus">Art der Dauerbuchung</param>
        private void Korrektur(Eintrag eintrag, DauerEintrag dauerEintrag, M2ModusTypen modus)
        {
            UebernehmeM2KorrekturHandler uebernahmeHandler = UebernehmeM2Korrektur;

            Maske2 m2;
            if (modus == M2ModusTypen.Bewegung)
                m2 = new Maske2(this, eintrag, uebernahmeHandler);
            else
                m2 = new Maske2(this, dauerEintrag, modus, uebernahmeHandler);

            m2.Show(M1);
        }

        /// <summary>
        /// Von M2 aufgerufen bei Datenübernahme im Modus Korrektur
        /// </summary>
        /// <param name="konto">Konto</param>
        /// <param name="eintragBase">Zu korrigierender Eintrag</param>
        /// <param name="modus">Art der Übernahme</param>
        /// <param name="dauerbuchung">Typ der Dauerbuchung</param>
        /// <param name="bewegung">Art der Bewegung</param>
        /// <param name="datum">Bewegungsdatum</param>
        /// <param name="wert">Wert</param>
        /// <param name="kommentar">Kommentar</param>
        private void UebernehmeM2Korrektur(EintragBase eintragBase, M2ModusTypen modus, DauerBuchungsTyp dauerbuchung, BuchungsTyp bewegung, DateTime datum, decimal wert, string kommentar)
        {
            if (eintragBase is Eintrag)
            {
                Eintrag eintrag = eintragBase as Eintrag;
                eintrag.Datum = datum;
                eintrag.Typ = bewegung;
                eintrag.Wert = wert;
                eintrag.Kommentar = kommentar;

                M1.DGVBewegungAktualisieren();
            }
            else
            {
                DauerEintrag dauerEintrag = eintragBase as DauerEintrag;
                dauerEintrag.Typ = dauerbuchung;
                dauerEintrag.Wert = wert;
                dauerEintrag.Kommentar = kommentar;

                M1.DGVEinkommenAktualisieren();
                M1.DGVAusgabenAktualisieren();
                M1.DGVZinsenAktualisieren();
            }
        }
        #endregion Maske 2

        #region Anlage, Speichern, Laden
        /// <summary>
        /// Erstellt ein neues Konto
        /// </summary>
        /// <param name="kontoname">Kontoname</param>
        /// <returns>Neues Konto</returns>
        public static Model NeuesKonto(string kontoname)
        {
            Model m = new Model(kontoname);
            return m;
        }

        /// <summary>
        /// Serialisiert die Kontodatei
        /// </summary>
        /// <param name="konto">Zu speicherndes Konto</param>
        /// <param name="pfad">Dateipfad für zu speichernde Datei</param>
        /// <param name="passwort">Passwort zur Verschlüsselung (leer lassen für keine Verschlüsselung)</param>
        /// <returns>
        /// <c>true</c>, wenn Speichern erfolgreich war
        /// <c>false</c>, falls ein Fehler aufgetreten ist
        /// </returns>
        public bool KontoSpeichern(Model konto, string pfad, string passwort = "")
        {
            bool erfolg = true;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = null;

            try
            {
                // Serialisiert wird eine DeepCopy des Objekts, da das Objekt selbst höchst wahrscheinlich an mindestens eine Maske gebunden ist und entsprechend nicht serialisiert werden kann.
                Model zuSpeichern = konto.DeepCopy();

                //Überschreibt die Datei jedes Mal!
                stream = new FileStream(pfad, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, zuSpeichern);
            }
            catch (Exception e)
            {
                erfolg = false;
                SpeichernFehler?.Invoke(this, e, pfad);
            }
            finally
            {
                stream?.Close();
            }

            return erfolg;
        }

        /// <summary>
        /// Deserialisiert die Kontodatei
        /// </summary>
        /// <param name="pfad">Dateipfad für zu ladende Datei</param>
        /// <param name="passwort">Passwort zur Enschlüsselung (leer lassen, falls kein Schlüssel erforderlich)</param>
        /// <returns>
        /// <c>null</c>, falls Datei nicht geladen werden konnte
        /// <c>else</c>, Kontodatei
        /// </returns>
        public Model KontoLaden(string pfad, string passwort = "")
        {
            Model m = null;

            if (!File.Exists(pfad))
            {
                if (!File.Exists(pfad + ".fa"))
                    return m;
                else
                    pfad += ".fa";
            }

            //Versuchen Datei zu laden
            IFormatter formatter = new BinaryFormatter();
            Stream stream = null;
            try
            {
                stream = new FileStream(pfad, FileMode.Open, FileAccess.Read, FileShare.Read);
                m = ((Model)formatter.Deserialize(stream));
            }
            catch (Exception e)
            {
                LadenFehler?.Invoke(this, e, pfad);
            }
            finally
            {
                stream?.Close();
            }

            return m;
        }
        #endregion Anlage, Speichern, Laden

        #region Periodische Verbuchung
        /// <summary>
        /// Verbucht alle noch ausstehenden periodischen Buchungen
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        public void PeriodischeVerbuchungenAusfuehren(Model konto)
        {
            PeriodischeVerbuchungenAusfuehren(konto, DateTime.Now);
        }

        /// <summary>
        /// Verbucht alle noch ausstehenden periodischen Buchungen
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="bisDatum">Datum, bis zu dem die Verbuchungen durchgeführt werden</param>
        public void PeriodischeVerbuchungenAusfuehren(Model konto, DateTime bisDatum)
        {
            // Alle anstehenden Verbuchungen seit dem letzten periodischen Ausführen ausführen
            DateTime letzte = konto.LetztePeriodischeAusfuehrung;

            /*
            List<DauerEintrag> wochentagEinkommen = konto.Einkommen.Where(eintrag => eintrag.Typ >= DauerBuchungsTyp.Montags && eintrag.Typ <= DauerBuchungsTyp.Sonntags)?.ToList();
            List<DauerEintrag> wochentagAusgaben = konto.Ausgaben.Where(eintrag => eintrag.Typ >= DauerBuchungsTyp.Montags && eintrag.Typ <= DauerBuchungsTyp.Sonntags)?.ToList();
            List<DauerEintrag> wochentagZinsen = konto.Zinsen.Where(eintrag => eintrag.Typ >= DauerBuchungsTyp.Montags && eintrag.Typ <= DauerBuchungsTyp.Sonntags)?.ToList();

            List<DauerEintrag> woechentlicheEinkommen = konto.Einkommen.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Woechentlich)?.ToList();
            List<DauerEintrag> woechentlicheAusgaben = konto.Ausgaben.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Woechentlich)?.ToList();
            List<DauerEintrag> woechentlicheZinsen = konto.Zinsen.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Woechentlich)?.ToList();
            */

            List<DauerEintrag> monatlicheEinkommen = konto.Einkommen.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Monatlich)?.ToList();
            List<DauerEintrag> monatlicheAusgaben = konto.Ausgaben.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Monatlich)?.ToList();
            List<DauerEintrag> monatlicheZinsen = konto.Zinsen.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Monatlich)?.ToList();

            /*
            List<DauerEintrag> jaehrlicheEinkommen = konto.Einkommen.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Jaehrlich)?.ToList();
            List<DauerEintrag> jaehrlicheAusgaben = konto.Ausgaben.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Jaehrlich)?.ToList();
            List<DauerEintrag> jaehrlicheZinsen = konto.Zinsen.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Jaehrlich)?.ToList();

            List<DauerEintrag> benutzerEinkommen = konto.Einkommen.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Benutzerdefiniert)?.ToList();
            List<DauerEintrag> benutzerAusgaben = konto.Ausgaben.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Benutzerdefiniert)?.ToList();
            List<DauerEintrag> benutzerZinsen = konto.Zinsen.Where(eintrag => eintrag.Typ == DauerBuchungsTyp.Benutzerdefiniert)?.ToList();
            */

            DateTime aktuell = letzte;
            while (letzte < bisDatum)
            {
                aktuell.AddDays(1);

                /*
                List<DauerEintrag> tEinkommen = wochentagEinkommen.Where(eintrag => (int)eintrag.Typ == (int)letzte.DayOfWeek)?.ToList();
                List<DauerEintrag> tAusgaben = wochentagAusgaben.Where(eintrag => (int)eintrag.Typ == (int)letzte.DayOfWeek)?.ToList();
                List<DauerEintrag> tZinsen = wochentagZinsen.Where(eintrag => (int)eintrag.Typ == (int)letzte.DayOfWeek)?.ToList();
                // Prüfe ob tägliche Verbuchungen vorhanden
                if (tEinkommen.Count > 0)
                    VerbuchePeriodisch(konto, tEinkommen, BuchungsTyp.Addition, letzte);
                if (tAusgaben.Count > 0)
                    VerbuchePeriodisch(konto, tAusgaben, BuchungsTyp.Subtraktion, letzte);
                if (tEinkommen.Count > 0)
                    VerbuchePeriodischZinsen(konto, tZinsen, letzte);
                */

                // Prüfe ob wöchentliche Verbuchungen vorhanden

                // Prüfe ob monatliche Verbuchungen vorhanden
                if (letzte.Month < aktuell.Month)
                {
                    if (monatlicheEinkommen.Count > 0)
                        VerbuchePeriodisch(konto, monatlicheEinkommen, BuchungsTyp.Addition, letzte);
                    if (monatlicheAusgaben.Count > 0)
                        VerbuchePeriodisch(konto, monatlicheAusgaben, BuchungsTyp.Addition, letzte);
                    if (monatlicheZinsen.Count > 0)
                        VerbuchePeriodischZinsen(konto, monatlicheZinsen, letzte);
                }

                // Prüfe ob jährliche Verbuchungen vorhanden

                // Prüfe ob Benutzerdefinierte Verbuchungen vorhanden

                letzte = aktuell;
            }

            konto.LetztePeriodischeAusfuehrung = letzte;
        }

        /// <summary>
        /// Führt die Verbuchung eines periodischen Datensatzes aus
        /// </summary>
        /// <param name="konto">Konto für zu verbuchende Daten</param>
        /// <param name="eintraege">Zu verbuchende Einträge</param>
        /// <param name="typ">Zu verbuchender Typ</param>
        /// <param name="datum">Verbuchungsdatum</param>
        private void VerbuchePeriodisch(Model konto, List<DauerEintrag> eintraege, BuchungsTyp typ, DateTime datum)
        {
            foreach (DauerEintrag eintrag in eintraege)
                konto.Verbuchen(new Eintrag(typ, datum, eintrag.Wert, eintrag.Kommentar, true));
        }

        /// <summary>
        /// Führt die Verbuchung eines periodischen Zins Datensatzes aus
        /// </summary>
        /// <param name="konto">Konto für zu verbuchende Daten</param>
        /// <param name="eintraege">Zu verbuchende Einträge</param>
        /// <param name="datum">Verbuchungsdatum</param>
        private void VerbuchePeriodischZinsen(Model konto, List<DauerEintrag> eintraege, DateTime datum)
        {
            foreach (DauerEintrag eintrag in eintraege)
                konto.Verbuchen(new Eintrag(eintrag.Wert < 0 ? BuchungsTyp.Subtraktion : BuchungsTyp.Addition, datum, konto.Kontostand * eintrag.Wert, eintrag.Kommentar, true));
        }
        #endregion Std. Verbuchung

        #region Dauerbuchungen
        #region Einkommen
        /// <summary>
        /// Fügt den Datensatz zur Liste des Kontos hinzu
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="eintrag">Hinzuzufügender Eintrag</param>
        public void EinkommenHinzufuegen(Model konto, DauerEintrag eintrag)
        {
            PeriodischHinzufuegen(konto.Einkommen, eintrag);
        }

        /// <summary>
        /// Entfer´nt den Datensatz aus der Liste des Kontos
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="eintrag">Zu entfernender Eintrag</param>
        /// <throws><c>InvalidDataException</c>, wenn das zu entfernende Objekt nicht existiert</throws>
        public void EinkommenEntfernen(Model konto, DauerEintrag eintrag)
        {
            PeriodischEntfernen(konto.Einkommen, eintrag);
        }
        #endregion Einkommen

        #region Ausgabe
        /// <summary>
        /// Fügt den Datensatz zur Liste des Kontos hinzu
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="eintrag">Hinzuzufügender Eintrag</param>
        public void AusgabenHinzufuegen(Model konto, DauerEintrag eintrag)
        {
            PeriodischHinzufuegen(konto.Ausgaben, eintrag);
        }

        /// <summary>
        /// Entfer´nt den Datensatz aus der Liste des Kontos
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="eintrag">Zu entfernender Eintrag</param>
        /// <throws><c>InvalidDataException</c>, wenn das zu entfernende Objekt nicht existiert</throws>
        public void AusgabenEntfernen(Model konto, DauerEintrag eintrag)
        {
            PeriodischEntfernen(konto.Ausgaben, eintrag);
        }
        #endregion Ausgaben

        #region Zinsen
        /// <summary>
        /// Fügt den Datensatz zur Liste des Kontos hinzu
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="eintrag">Hinzuzufügender Eintrag</param>
        public void ZinsenHinzufuegen(Model konto, DauerEintrag eintrag)
        {
            PeriodischHinzufuegen(konto.Zinsen, eintrag);
        }

        /// <summary>
        /// Entfer´nt den Datensatz aus der Liste des Kontos
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="eintrag">Zu entfernender Eintrag</param>
        /// <throws><c>InvalidDataException</c>, wenn das zu entfernende Objekt nicht existiert</throws>
        public void ZinsenEntfernen(Model konto, DauerEintrag eintrag)
        {
            PeriodischEntfernen(konto.Zinsen, eintrag);
        }
        #endregion Zinsen

        /// <summary>
        /// Fügt den Datensatz zur Liste des Kontos hinzu
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="eintrag">Hinzuzufügender Eintrag</param>
        private void PeriodischHinzufuegen(BindingList<DauerEintrag> liste, DauerEintrag eintrag)
        {
            liste.Add(eintrag);
        }

        /// <summary>
        /// Entfernt den Datensatz aus der Liste des Kontos
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="eintrag">Zu entfernender Eintrag</param>
        /// <throws><c>InvalidDataException</c>, wenn das zu entfernende Objekt nicht existiert</throws>
        private void PeriodischEntfernen(BindingList<DauerEintrag> liste, DauerEintrag eintrag)
        {
            if (!liste.Remove(eintrag))
                throw new InvalidDataException(String.Format("Der angegebene Eintrag {0} kann nicht entfernt werden!", eintrag.Kommentar));
        }
        #endregion DauerBuchungen

        #region Zukunftsberechnung
        /// <summary>
        /// Ermittelt den dispositiven Kontostand
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="zeitraum">Zeitraum, für den der dispositive Kontostand berechnet werden soll</param>
        /// <returns>Dispositiver Kontozustand für den angegebenen Zeitraum</returns>
        public Model KontostandDispositiv(Model konto, TimeSpan zeitraum)
        {
            Model dispoKonto = konto.DeepCopy();
            PeriodischeVerbuchungenAusfuehren(dispoKonto, DateTime.Now + zeitraum);
            return dispoKonto;
        }

        /// <summary>
        /// Gibt den Zeitpunkt an, zu dem der angegebene Kontostand erreicht wird
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="kontostand">Zu erreichender Kontostand</param>
        /// <returns>
        /// <c>null</c>, wenn der angegebene Kontostand nicht erreicht werden kann
        /// <c>else</c>, das Datum, an dem der Kontostand erreicht wird
        /// </returns>
        public DateTime? Wann(Model konto, decimal kontostand)
        {
            DateTime? zeitpunkt = null;

            // Prüfen ob angegebener Kontostand erreicht werden kann
            if (konto.Kontostand == kontostand)
                return zeitpunkt; // Der angegebene Kontostand kann nicht erreicht werden, wenn identisch

            decimal mtlVerbuchungsSumme = 0;
            decimal mtlZinsSumme = 0;
            foreach (DauerEintrag e in konto.Einkommen.Where(x => x.Typ == DauerBuchungsTyp.Monatlich))
                mtlVerbuchungsSumme += e.Wert;

            foreach (DauerEintrag e in konto.Ausgaben.Where(x => x.Typ == DauerBuchungsTyp.Monatlich))
                mtlVerbuchungsSumme -= e.Wert;

            foreach (DauerEintrag e in konto.Zinsen.Where(x => x.Typ == DauerBuchungsTyp.Monatlich))
                mtlZinsSumme += e.Wert;

            if (mtlVerbuchungsSumme == 0 && mtlZinsSumme == 0)
                return zeitpunkt; // Bei stagnierendem Kontostand wird sich der Wert nicht ändern

            if (kontostand < konto.Kontostand)
            {
                if (konto.Kontostand * mtlZinsSumme + mtlVerbuchungsSumme > 0)
                    return zeitpunkt; // Der Kontostand nimmt zu, es wird aber nach einem kleineren Kontostand gefragt ==> nicht erreichbar
            }
            else
            {
                if (konto.Kontostand * mtlZinsSumme + mtlVerbuchungsSumme < 0)
                    return zeitpunkt; // Der Kontostand nimmt ab, es wird aber nach einem größeren Kontostand gefragt ==> nicht erreichbar
            }

            // Berechnung des Zeitraums
            TimeSpan benoetigteZeit = new TimeSpan(0);
            Model zwKonto = konto.DeepCopy();
            bool isZielErreicht = false;
            while (!isZielErreicht)
            {
                benoetigteZeit.Add(new TimeSpan(1, 0, 0, 0)); // Dispo je Tag berechnen
                // Die periodische Verbuchung wird für das zwischen Konto ausgeführt. Dabei wird jeder Tag einzeln verbucht.
                // Nach jeder Verbuchung ist das Datum der letzten periodischen Ausführung um eins erhöht. Entsprechend wird jedes mal einen Tag weiter verbucht.
                PeriodischeVerbuchungenAusfuehren(zwKonto, zwKonto.LetztePeriodischeAusfuehrung + new TimeSpan(1, 0, 0, 0));

                if (kontostand < konto.Kontostand)
                {
                    if (zwKonto.Kontostand <= kontostand)
                        isZielErreicht = true;
                }
                else
                {
                    if (zwKonto.Kontostand >= kontostand)
                        isZielErreicht = true;
                }
            }

            zeitpunkt = DateTime.Now + benoetigteZeit;

            return zeitpunkt;
        }

        // ToDo: Differenz einkommen/ausgaben (auf Zeitraum gesehen) -> Kontostand aktuell - Kontostand Dispo => diff
        // ToDo: Zinswert Konto (auf Zeitraum gesehen)
        // ToDo: Bewegungs selektion und sortierung
        #endregion Zukunftsberechnung

        #region Sonstiges
        /// <summary>
        /// Storniert den angegebenen Eintrag
        /// </summary>
        /// <param name="konto">Kontodatei</param>
        /// <param name="eintrag">Zu stornierender Eintrag</param>
        /// <throws>
        /// <c>InvalidDataException</c>, falls der angegebene Eintrag nicht existiert
        /// <c>ArgumentException</c>, falls der Eintrag den falschen Buchungstyp hat
        /// </throws>
        public void Stornieren(Model konto, Eintrag eintrag)
        {
            if (!konto.Kontobewegung.Remove(eintrag))
                throw new InvalidDataException(String.Format("Der angegebene Eintrag {0} kann nicht storniert werden!", eintrag.Kommentar));

            switch (eintrag.Typ)
            {
                case BuchungsTyp.Gleichsetzen:
                case BuchungsTyp.Addition:
                case BuchungsTyp.Subtraktion:
                    konto.NeuberechnungKontostandAusBewegung();
                    break;
                default:
                    throw new ArgumentException(String.Format("Ein Eintrag ohne Buchungstyp kann nicht storniert werden!"), "eintrag");
            }
        }

        /// <summary>
        /// Konvertiert die Kontodatei der Version 1.4 in ein Konto der Version 2
        /// </summary>
        /// <param name="pfad">Pfad zur Kontodatei V1.4</param>
        /// <param name="konto">Konvertiertes Konto</param>
        /// <returns>
        /// <c>string.Empty</c>, falls Konvertierung erfolgreich war
        /// andernfalls Fehlerinformation
        /// </returns>
        public string KonvertierenV1_4ZuV2(string pfad, out Model konto)
        {
            konto = null;
            string fehler = "";
            // Laderoutine Faid v1.4
            //Versuchen angegebene Datei mit EscapeCharactern zu laden: 
            //Versuche Versionsinfos zu erhalten
            Faid.DatenV1_4 datenV14 = null;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = null;
            try
            {
                stream = new FileStream(pfad, FileMode.Open, FileAccess.Read, FileShare.Read);
                datenV14 = ((Faid.DatenV1_4)formatter.Deserialize(stream));

                // Den Kontostand noch einmal aktualisieren
                datenV14.UpdateKontostand();
            }
            catch (Exception e)
            {
                fehler = e.Message;
            }
            finally
            {
                stream?.Close();
            }
            
            if (fehler.Equals(string.Empty))
            {
                konto = new Model(datenV14.KontoName);

                try
                {
                    DateTime datum = DateTime.Now;
                    foreach (string zeile in datenV14.Kontobewegungen)
                    {
                        // Valider Eintrag, falls das zweite Zeichen ein Leerzeichen ist
                        if (zeile.ElementAt(1) == ' ')
                        {
                            BuchungsTyp buchungsTyp = BuchungsTyp.KeinBuchungstyp;
                            string kommentar = "";
                            bool isPeriodisch;
                            double dWert = 0;
                            decimal wert = 0;

                            string[] infos = zeile.Split(' ');

                            if (infos[0] == "+")
                                buchungsTyp = BuchungsTyp.Addition;
                            else if (infos[0] == "-")
                                buchungsTyp = BuchungsTyp.Subtraktion;
                            else if (infos[0] == "=")
                                buchungsTyp = BuchungsTyp.Gleichsetzen;

                            if (infos[1].Equals(Resources.KonvertierungMonatlicheVerbuchungIdent))
                            {
                                if (infos[2].ElementAt(0).ToString().Equals(Resources.KonvertierungMonatlicheVerbuchungAltIdent))
                                {
                                    // Altes Datum verwenden
                                    //datum = new DateTime(0);

                                    if (infos[3].Equals(Resources.KonvertierungMonatlicheVerbuchungAltEinkommenIdent))
                                    {
                                        for (int i = 4; i < infos.Length - 1; i++)
                                            kommentar += infos[i] + " ";
                                        kommentar = new string(kommentar.Take(kommentar.Length - 2).ToArray());
                                    }
                                    else if (infos[3].Equals(Resources.KonvertierungMonatlicheVerbuchungAltZinsenIdent))
                                    {
                                        kommentar = Resources.KonvertierungMonatlicheVerbuchungAltZinsenIdent;
                                    }
                                    else
                                    {
                                        for (int i = 5; i < infos.Length - 1; i++)
                                            kommentar += infos[i] + " ";
                                        kommentar = new string(kommentar.Take(kommentar.Length - 2).ToArray());
                                    }
                                }
                                else
                                { 
                                    string[] datumSplit = infos[2].Split('.');
                                    datum = new DateTime(int.Parse(datumSplit[1]), int.Parse(datumSplit[0]), 1);

                                    if (infos[3].Equals(Resources.KonvertierungMonatlicheVerbuchungAltEinkommenIdent))
                                    {
                                        for (int i = 4; i < infos.Length - 1; i++)
                                            kommentar += infos[i] + " ";
                                        kommentar = new string(kommentar.Take(kommentar.Length - 2).ToArray());
                                    }
                                    else if (infos[3].Equals(Resources.KonvertierungMonatlicheVerbuchungAltZinsenIdent))
                                    {
                                        kommentar = Resources.KonvertierungMonatlicheVerbuchungAltZinsenIdent;
                                    }
                                    else
                                    {
                                        for (int i = 5; i < infos.Length - 1; i++)
                                            kommentar += infos[i] + " ";
                                        kommentar = new string(kommentar.Take(kommentar.Length - 2).ToArray());
                                    }
                                }

                                isPeriodisch = true;
                            }
                            else if (infos[1].Equals(Resources.KonvertierungMonatlicheVerbuchungGleichsetzenIdent))
                                continue; // Einen Durchlauf überspringen
                            // Falls keine mntl. Verbuchung, normale Bewegung
                            else
                            {
                                if (buchungsTyp == BuchungsTyp.Gleichsetzen)
                                {
                                    if (infos.Length > 3)
                                    {
                                        if ((infos[1] + " " + infos[2] + " " + infos[3]).Equals(Resources.KonvertierungKontostandManuell))
                                        {
                                            // Altes Datum verwenden
                                        }
                                    }
                                }
                                else
                                {
                                    string[] datumSplit = infos[1].Split('.');
                                    datum = new DateTime(int.Parse(datumSplit[2]), int.Parse(datumSplit[1]), int.Parse(datumSplit[0]));
                                    for (int i = 3; i < infos.Length - 2; i++)
                                        kommentar += infos[i] + " ";
                                    kommentar = new string(kommentar.Take(kommentar.Length - 1).ToArray());
                                }

                                isPeriodisch = false;
                            }

                            kommentar.Replace(" :", "");
                            kommentar.Replace("  ", "");

                            dWert = double.Parse(infos[infos.Length - 1]);
                            wert = (decimal)dWert;
                            wert = decimal.Truncate(wert * 100) / 100;

                            konto.Kontobewegung.Add(new Eintrag(buchungsTyp, datum, wert, kommentar, isPeriodisch));
                        }
                    }
                }
                catch (Exception e)
                {
                    fehler = string.Format(Resources.KonvertierungFormatFehler, e.Message);
                }
            }

            return fehler;
        }
        #endregion Sonstiges
    }
}
