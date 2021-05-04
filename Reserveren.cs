using System;
using System.Linq;
using System.Text;
using resourceMethods;
using LoginPage; // namespace van login.cs
using MenuPage; // namespace van Menu.cs
using TablePage; // namespace van Tafels.cs

namespace ReserveringPage
{
    class Reservering
    {
        public string ToegangsCode; // de code om bij de reservering te kunnen
        public Day Dag;
        public DinnerRoom Datum;
        public Table Tafel;
        public Gerecht[] Bestelling;
        public User Gebruiker;
        public Person Guest;
        
        public Reservering(string code, Day dag, DinnerRoom datum, Table tafel, Gerecht[] bestelling, User user=null, Person guest=null) {
            Dag = dag;
            Datum = datum;
            Tafel = tafel;
            ToegangsCode = code;
            Bestelling = bestelling;
            Gebruiker = user;
            Guest = guest;
        }
        public Reservering() => new Reservering("", null, null, null, null, null);


        /// <summary>Returned true als de reservering van een gast is, anders returned false</summary>
        public bool IsGuest() => Guest == null;

        /// <summary>veranderd de reservering gebaseerd op de input van de gebruiker</summary>
        public void Change(Week week) {
            string[] options = new string[] { "Datum en Tijd", "Tafel", "Bestelling" };
            while (true) {
                string choice = Resources.makeMenuInput("Verander je reservering", "Kies een van de bovenstaande opties: ", options, backbutton: true);
                if (choice == "1") { 
                
                }
                else if (choice == "2") { 
                
                }
                else if (choice == "3") { 
                
                }
                else
                    break;
            }
        }

        /*public void ChangeDate() { 
            
        }

        public void ChangeTime() { 

        }

        public void ChangeTable() { 

        }*/

        /// <summary>Maakt een soort menu met de volledige bestelling die bij de reservering hoort en returned een string daarvan</summary>
        public string StringBestelling() {  // TODO: Zorg ervoor dat gerechten die vaker in de bestelling staan bij elkaar worden opgeteld en niet vaker worden geprint naar de terminal
            Console.OutputEncoding = Encoding.UTF8;
            string outputStr = "";
            if (Bestelling == null || Bestelling.Length == 0)
                return outputStr;
            else {
                double totaalprijs = 0;
                foreach (Gerecht g in Bestelling) {
                    outputStr += $"{g.Naam}{Resources.drawString(50 - g.Naam.Length, " ")}€{g.Prijs}\n";
                    totaalprijs += g.Prijs;
                }
                outputStr += Resources.drawString(55, "~");
                outputStr += $"Totaalprijs{Resources.drawString(50 - "Totaalprijs".Length, " ")}€{totaalprijs}";
                return outputStr;
            }
        }

        /// <summary>Laat een overzicht zien van alle componenten van de reservering</summary>
        public void ShowReservering() {
            Console.WriteLine($"Op naam van: {Gebruiker.Voornaam} {Gebruiker.Achternaam}");
            Console.WriteLine($"Datum      : {Dag.Datum:dd/MM/yyyy}       Tijdsvak: {Datum.Tijdvak}");
            Console.WriteLine($"Tafel      : {Tafel.table_num}");
            Console.WriteLine("\n\nBestelling:");
            Console.WriteLine(StringBestelling());
        }

        /// <summary>Laat in een line alle info van de reservering zien</summary>
        public string OneLine() {
            string returnLine = "";
            returnLine += ToegangsCode + Resources.drawString(10 - ToegangsCode.Length, " ");
            returnLine += Dag.Datum.ToString("dd/MM/yyyy") + ":";
            returnLine += Datum.Tijdvak + Resources.drawString(10 - (Dag.Datum.ToString("dd/MM/yyyy") + ":").Length, " ");
            returnLine += Tafel.table_num + Resources.drawString(10 - Tafel.table_num.Length, " ");
            returnLine += Gebruiker.Email;
            return returnLine;
        }
    }

    class ReserveringsAdministration {
        public Reservering[] Reserveringen;
        public Reservering[] GastReservering; // TODO: gast reserveringen zijn optioneel
        public Week DezeWeek;
        private const string FILENAME = "Reserveringen.json";
        // TODO: maak alle methods en fields voor de ReserveringsAdministratie

        public ReserveringsAdministration() {
            if (!DataHandler.FileExists(FILENAME)) {
                DezeWeek = new Week();
                Reserveringen = new Reservering[0];
                GastReservering = new Reservering[0];
            }
            else {
                DezeWeek = LoadWeek(DataHandler.LoadJson(FILENAME).DezeWeek);
                DezeWeek.UpdateWeek();
                Reserveringen = Load(DataHandler.LoadJson(FILENAME).Reserveringen);
                GastReservering = Load(DataHandler.LoadJson(FILENAME).GastReservering);
            }
        }

        /// <summary>Vormt een json object tot een Reservering[] Array</summary>
        public Reservering[] Load(dynamic Reserveringen) => Reserveringen.ToObject<Reservering[]>();

        /// <summary>Vormt een json object tot een Week object</summary>
        public Week LoadWeek(dynamic weekJson) => weekJson.ToObject<Week>();

        /// <summary>slaat de reserveringsadministratie op in "Week.json" file in Data folder</summary>
        public void Save() => DataHandler.WriteJson(FILENAME, this);

        /// <summary>Voegt een nieuwe reservering toe aan alle reserveringen</summary>
        public void AddReservering(Reservering res, bool guest=false) {
            res.Tafel.SetReservering(res.ToegangsCode);
            Reservering[] resArr = guest ? GastReservering : Reserveringen;
            int totalRes = 0;
            foreach (Reservering _ in resArr)
                totalRes++;
            Reservering[] nieuweRes = new Reservering[totalRes + 1];
            for (int i = 0; i < resArr.Length; i++) {
                nieuweRes[i] = resArr[i];
            }
            nieuweRes[totalRes] = res;
            if (guest)
                GastReservering = nieuweRes;
            else
                Reserveringen = nieuweRes;
        }

        /// <summary>Verwijderd een reservering van alle reserveringen gebaseerd op de code van de reservering</summary>
        public void RemoveReservering(string code) {
            if (!Exists(code)) {
                Console.WriteLine("De gegeven code bestaat niet in de huidige reserveringen"); 
                return;
            }
            int totalRes = 0;
            foreach (Reservering _ in Reserveringen) {
                totalRes++;
            }
            Reservering[] nieuweRes = new Reservering[totalRes - 1];
            for (int i = 0, j = 0; i < Reserveringen.Length; i++) {
                if (Reserveringen[i].ToegangsCode == code)
                    continue;
                nieuweRes[j++] = Reserveringen[i];
            }
        }

        /// <summary>Laat reserveringen zien afhankelijk van wie de method callt</summary>
        public void SeeReserveringen(User user) { 
            foreach (Reservering res in Reserveringen) {
                if (user.ModLevel == 1)
                    res.OneLine();
                else if (res.Gebruiker == user)
                    res.ShowReservering();
            }
        }

        /// <summary>Een method die het menu voor reserveringen weergeeft voor admins</summary>
        public void AdminMenu(User admin, MenuKaart menu, UserAdministration useradmin) {
            string[] options = new string[] { "Zie reserveringen", "Maak een nieuwe reservering aan", "Pas een reservering aan", "Verwijder een reservering" };
            while (true) {
                Save();
                string choice = Resources.makeMenuInput($"Welkom bij de reservering opties {admin.Voornaam}", "Kies een van bovenstaande opties: ", options, backbutton: true);
                if (choice == "1") {
                    SeeReserveringen(admin);
                    Resources.EnterMessage();
                }
                else if (choice == "2")
                    ReserveringMenu(true, menu, useradmin);
                else if (choice == "3")
                { }
                // Pas een reservering aan via admin menu, moet ook eerst de code invoeren van de reservering of de email van de gebruiker om het juiste profiel te pakken te krijgen
                else if (choice == "4")
                { }
                // Verwijder een reservering, hier heeft de admin ook de code voor nodig of kan kiezen uit een menu van reserveringen
                else
                    // ga terug naar het vorige menu
                    break;
            }
        }   

        /// <summary>Een method die het menu voor reserveringen weergeeft voor normale gebruikers</summary>
        public void DefaultMenu(User user, MenuKaart menu) {
            string[] options = new string[] { "Zie eigen reserveringen", "Maak een nieuwe reservering aan", "Pas een reservering aan", "Verwijder een reservering" };
            while (true) {
                Console.Clear();
                Save();
                string choice = Resources.makeMenuInput("Gebruiker Reservering Opties", "Kies een van bovenstaande opties: ", options, backbutton: true);
                if (choice == "1") {
                    SeeReserveringen(user);
                    Resources.EnterMessage();
                }
                else if (choice == "2")
                    MakeReservering(user, menu);
                else if (choice == "3")
                { }
                else if (choice == "4")
                { }
                else
                    break;
            }
        }

        /// <summary>Laat een algemeen menu zien voor het reserveren en returned een reservering object (null als de reservering niet is geslaagd)</summary>
        public void ReserveringMenu(bool isadmin, MenuKaart menu, UserAdministration useradmin) { 
            string[] reserveOptions = new string[] { "Reserveer als gast", "Reserveer als gebruiker" };
            string reserveChoice = Resources.makeMenuInput("Hoe wilt u een reservering maken?", "Kies een van de bovenstaande opties", reserveOptions, backbutton: true);
            if (reserveChoice == "1") {
                Person guest = MakeGuest();
                guest.Present();
                MakeReservering(new User(guest.Voornaam, guest.Achternaam, guest.Email, guest.Tel_no, guest.Leeftijd, null, -1, guest.Tussenvoegsel), menu);
            }
            else { // reserveer als gebruiker
                if (!isadmin) {
                    User loggedIn = useradmin.Login();
                    if (loggedIn == null || loggedIn.IsAdmin()) {
                        Resources.errorMessage("De ingelogde gebruiker kan geen reservering doen op zijn/haar naam");
                    }
                    MakeReservering(loggedIn, menu);
                    return;
                }

                string mail = Resources.inputCheck("Voer de mail van de gebruiker in die een reservering doet", useradmin.GetMails(), "Die email is helaas niet beschikbaar",maxTries: 3);
                if (mail == null) {
                    Resources.EnterMessage();
                    return;
                }     
                User selected = useradmin.GetUser(mail);
                if (selected.IsAdmin() || selected == null) {
                    Resources.errorMessage("De geselecteerde gebruiker kan geen reservering doen op zijn/haar naam");
                    Resources.EnterMessage();
                }
                MakeReservering(selected, menu);
            }

        }

        /// <summary>Maakt een person object, dit is een gast dus word niet opgeslagen in de useradministration, alleen in de reservering</summary>
        public Person MakeGuest() {
            string voornaam = Resources.inputRegex("Voornaam: ", @"\w+");
            string tussenvoegsel = Resources.input("Tussenvoegsel: ");
            string achternaam = Resources.inputRegex("Achternaam: ", @"\w+");
            string email = Resources.inputRegex("E-mail Adres: ", @"^\w+@\w+\.\w{2,3}$");
            string telefoonnummer = Resources.inputRegex("Telefoonnr: ", @"^(06|\+316)\d{8}$");
            string leeftijd = Resources.inputCheck("Leeftijd: ", Resources.makeRangeArr(18, 125), "Het ingevoerde getal is helaas onjuist, wees ervan bewust dat wij alleen gebruikers aannemen boven de 18");
            return new Person(voornaam, achternaam, email, telefoonnummer, leeftijd);
        }

        /// <summary>Maakt een reservering gebaseerd op de gegeven input, hiervoor moet al een gebruiker zijn geregistreerd</summary>
        public void MakeReservering(User user, MenuKaart menu) {
            int step = 1;
            Day chosenDay = null;
            DinnerRoom chosenRoom = null;
            Table chosenTable = null;
            Gerecht[] bestelling = null;
            while (step < 6) {
                Console.Clear();
                Console.WriteLine($"{step}/5");
                if (step == 1) { // selecteer de reserveerdag
                    chosenDay = DezeWeek.GetDay(false);
                    if (chosenDay == null)
                        return;
                    step++;
                }
                else if (step == 2) { // selecteer de datum van de reservering
                    chosenRoom = chosenDay.GetRoom();
                    if (chosenRoom == null)
                        step--;
                    else
                        step++;
                }
                else if (step == 3) { // selecteer de Tafel
                    chosenRoom.DrawMap();
                    int totalTables = 0;
                    foreach (Table tafel in chosenRoom.AllTafels())
                        totalTables += tafel.isAvailable() ? 1 : 0;
                    string[] tableNums = new string[totalTables + 1];
                    int index = 0;
                    foreach (Table tafel in chosenRoom.AllTafels()) {
                        if (tafel.isAvailable())
                            tableNums[index++] = tafel.table_num;
                    }
                    tableNums[index++] = "b";
                    string chosenNumber = Resources.inputCheck("Kies een van de beschikbare tafels (of typ 'b' om terug te gaan): ", chosenRoom.GetTafels(), "Die tafel is helaas niet beschikbaar, probeer het opnieuw");
                    if (chosenNumber == "b")
                        step--;
                    else
                        chosenTable = chosenRoom.GetTafel(chosenNumber);
                        step++;
                }
                else if (step == 4) { // doe een bestelling voor de reservering
                    while (true) {
                        string choice = Resources.makeMenuInput("Weet je al wat je wilt? Dan kan je alvast een bestelling doen!", "Kies een van bovenstaande opties: ", new string[] { "Voeg een gerecht toe aan de bestelling", "Verwijder een gerecht van bestelling", "Ga verder" }, backbutton: true);
                        if (choice == "1") {
                            Gerecht addgerecht = menu.ChooseGerechten();
                            bestelling = AddBestelling(addgerecht, bestelling);
                        }
                        else if (choice == "2") {
                            Gerecht removegerecht = menu.ChooseGerechten();
                            bestelling = RemoveBestelling(removegerecht, bestelling);
                        }
                        else if (choice == "3") {
                            step++;
                            break;
                        }
                        else {
                            step--;
                            break;
                        }
                    }
                }
                else if (step == 5) { // Geef een overzicht van de bestelling 
                    string Code = GenerateCode();
                    Reservering nieuwReservering = new Reservering(Code, chosenDay, chosenRoom, chosenTable, bestelling, user);
                    nieuwReservering.ShowReservering();
                    string choice = Resources.makeMenuInput("", "Kies een van bovenstaande opties: ", new string[] { "Bevestig reservering", "Annuleer reservering" }, backbutton: true);
                    if (choice == "1") {
                        AddReservering(nieuwReservering);
                        Resources.succesMessage("Reservering succesvol opgeslagen");
                        Resources.succesMessage($"Uw persoonlijke toegangscode om u reservering te bekijken: {Code}");
                        Resources.EnterMessage();
                        step++;
                    }
                    else if (choice == "2") {
                        Console.WriteLine("Reservering geannuleerd");
                        Resources.EnterMessage();
                        step++;
                    }
                    else
                        step--;
                }
            }
        }

        /// <summary>Returned een string voor de toegangscode van de reservering, format: 0000AA </summary>
        public string GenerateCode() {
            string strPart = "";
            strPart = $"{Reserveringen.Length % 9999}";
            strPart = Resources.drawString(4 - (strPart.Length % 4), "0") + strPart;
            int char1 = 'A';
            int char2 = 'A';
            char1 += Reserveringen.Length / 9999;
            char2 += char1 / ('A' + 25);
            char first = (char)(char1 % ('A' + 25));
            char second = (char)(char2 % ('A' + 25));
            return strPart + first + second;
        }

        /// <summary>Voegt een gerecht toe aan de Bestelling parameter</summary>
        public Gerecht[] AddBestelling(Gerecht gerecht, Gerecht[] Bestelling) {
            Gerecht[] nieuwBestelling = new Gerecht[Bestelling.Length + 1];
            for (int i = 0; i < Bestelling.Length; i++) {
                nieuwBestelling[i] = Bestelling[i];
            }
            nieuwBestelling[Bestelling.Length] = gerecht;
            return nieuwBestelling;
        }

        /// <summary>Verwijdert een gerecht van de Bestelling parameter</summary>
        public Gerecht[] RemoveBestelling(Gerecht gerecht, Gerecht[] Bestelling) {
            Gerecht[] nieuwBestelling = new Gerecht[Bestelling.Length - 1];
            for (int i = 0; i < Bestelling.Length; i++) {
                if (gerecht != Bestelling[i])
                    nieuwBestelling[i] = Bestelling[i];  // TODO: verbeter dit zodat je maar een kopie van een gerecht verwijdert per keer
            }
            return nieuwBestelling;
        }

        /// <summary>Checked of de reservering bestaat en returned een bool</summary>
        public bool Exists(Reservering res) => GetCodes().Contains(res.ToegangsCode);
        public bool Exists(string code) => GetCodes().Contains(code); 

        /// <summary>Returned een array met alle reserveringscodes</summary>
        public string[] GetCodes() {
            string[] Codes = new string[Reserveringen.Length];
            for (int i = 0; i < Codes.Length; i++) {
                Codes[i] = Reserveringen[i].ToegangsCode;
            }
            return Codes;
        }
    }
}
