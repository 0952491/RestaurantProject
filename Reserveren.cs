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
        public DinnerRoom Tijdsvak;
        public Table Tafel;
        public Gerecht[] Bestelling;
        public User Gebruiker;
        public Person Guest;
        
        public Reservering(string code, Day dag, DinnerRoom tijdsvak, Table tafel, Gerecht[] bestelling, User user=null, Person guest=null) {
            Dag = dag;
            Tijdsvak = tijdsvak;
            Tafel = tafel;
            ToegangsCode = code;
            Bestelling = bestelling;
            Gebruiker = user;
            Guest = guest;
        }
        public Reservering() => new Reservering("", null, null, null, null, null);


        /// <summary>Returned true als de reservering van een gast is, anders returned false</summary>
        public bool IsGuest() => Guest == null;

        /// <summary>Een curried functie om het verschil in tijd uit te rekenen tussen nu en de reserveringstijd</summary>
        public Func<DateTime, Func<DateTime, Func<TimeSpan, bool>>> CompareTimeBuilder = ResTijd => CurTijd => VergelijkTS => (ResTijd - CurTijd) > VergelijkTS;

        /// <summary>Laat een bericht zien voor een geannuleerde verandering van de reservering</summary>
        public void CancelChange() {
            Console.WriteLine("Verandering van reservering is geannuleerd");
            Resources.EnterMessage();
        }

        /// <summary>Returned een bool kijkend of de eerste datum en tijd nog ver weg genoeg zijn om een reservering te doen</summary>
        public bool CompareTime(DateTime restijd) {
            var firstFun = CompareTimeBuilder(restijd)(DateTime.Now);
            return firstFun(new TimeSpan(1, 0, 0));  // voor nu staat de vergelijkingstijd op 1 uur van de reserveringstijd, dit kan later veranderd worden
        }

        /// <summary>veranderd de reservering gebaseerd op de input van de gebruiker</summary>
        public void Change(Week week, User changingUser, MenuKaart menu) {  // TODO: DEZE METHOD UITVOERIG TESTEN
            if (!CompareTime(Dag.GetCorrectTime(Tijdsvak.Tijdvak)) && !changingUser.IsAdmin()) {
                Console.WriteLine("Het is niet meer mogelijk deze reservering te veranderen");
                Console.WriteLine("Als u toch nog een verandering wilt doorvoeren kunt u bellen naar het nummer van het restaurant onder 'Contact'");
                Resources.EnterMessage();
                return;
            }
            string[] options = new string[] { "Datum en Tijd", "Tafel", "Bestelling" };
            while (true) {
                OneLine();
                string choice = Resources.makeMenuInput("Verander je reservering", "Kies een van de bovenstaande opties: ", options, backbutton: true);
                if (choice == "1") {
                    string TimeChoice = Resources.makeMenuInput("Wat wilt u veranderen?", "Kies een van bovenstaande opties: ", new string[] { "Datum", "Tijd" }, backbutton: true);
                    if (TimeChoice == "1") {
                        Day chosenDay = ChangeDate(week);
                        if (chosenDay == null) { CancelChange(); continue; }
                        Console.WriteLine("Nadat de dag is veranderd moet u opnieuw een tijdsvak kiezen");
                        DinnerRoom chosenRoom = ChangeTime(chosenDay);
                        if (chosenRoom == null) { CancelChange(); continue; }
                        Console.WriteLine("Nadat het tijdsvak is veranderd van de reservering moet u opnieuw een tafel kiezen");
                        Table chosenTable = ChangeTable(chosenRoom);
                        if (chosenTable == null) { CancelChange(); continue; }
                        Tafel.DelReservering(); // verwijder de oude reservering van de tafel
                        Dag = chosenDay;
                        Tijdsvak = chosenRoom;
                        Tafel = chosenTable;
                        Tafel.SetReservering(ToegangsCode); // sla de nieuwe reservering op
                    }
                    else if (TimeChoice == "2") {
                        DinnerRoom chosenRoom = ChangeTime(Dag);
                        if (chosenRoom == null) { CancelChange(); continue; }
                        Console.WriteLine("Nadat het tijdsvak is veranderd van de reservering moet u opnieuw een tafel kiezen");
                        Table chosenTable = ChangeTable(chosenRoom);
                        if (chosenTable == null) { CancelChange(); continue; }
                        Tafel.DelReservering(); // verwijder de oude reservering van de tafel
                        Tijdsvak = chosenRoom;
                        Tafel = chosenTable;
                        Tafel.SetReservering(ToegangsCode); // sla de nieuwe reservering op
                    }
                    else
                        continue;
                }
                else if (choice == "2") {
                    Table chosenTable = ChangeTable(Tijdsvak);
                    if (chosenTable == null) { CancelChange(); continue; }
                    Tafel.DelReservering(); // verwijder de oude reservering van de tafel
                    Tafel = chosenTable;
                    Tafel.SetReservering(ToegangsCode); // sla de nieuwe reservering op
                }
                else if (choice == "3") {
                    ChangeOrder(menu);
                }
                else
                    break;
            }
        }

        public Day ChangeDate(Week week) {
            Day chosenDay = week.GetDay(false);
            return chosenDay;   
        }

        public DinnerRoom ChangeTime(Day dag) {
            DinnerRoom chosenRoom = dag.GetRoom();
            return chosenRoom;
        }

        public Table ChangeTable(DinnerRoom room) {
            Table chosenTable = room.GetTafel();
            return chosenTable;
        }

        public void ChangeOrder(MenuKaart menu) {
            while (true) {
                Console.WriteLine(StringBestelling());
                string[] options = new string[] { "Gerecht toevoegen", "Gerecht verwijderen" };
                string choice = Resources.makeMenuInput("Verander je bestelling", "Kies een van de bovenstaande opties: ", options, backbutton: true);
                if (choice == "b")
                    break;
                Gerecht gerecht = menu.ChooseGerechten();
                if (gerecht == null)
                    continue;
                if (choice == "1")
                    ReserveringsAdministration.AddGerechtToArr(gerecht, Bestelling);
                else 
                    ReserveringsAdministration.RemoveGerechtFromArr(gerecht, Bestelling);
            }
        }

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
            Console.WriteLine($"Datum      : {Dag.Datum:dd/MM/yyyy}       Tijdsvak: {Tijdsvak.Tijdvak}");
            Console.WriteLine($"Tafel      : {Tafel.table_num}");
            Console.WriteLine("\n\nBestelling:");
            Console.WriteLine(StringBestelling());
        }

        /// <summary>Laat in een line alle info van de reservering zien</summary>
        public string OneLine() {
            string returnLine = "";
            returnLine += ToegangsCode + Resources.drawString(10 - ToegangsCode.Length, " ");
            returnLine += Dag.Datum.ToString("dd/MM/yyyy") + ":";
            returnLine += Tijdsvak.Tijdvak + Resources.drawString(10 - (Dag.Datum.ToString("dd/MM/yyyy") + ":").Length, " ");
            returnLine += Tafel.table_num + Resources.drawString(10 - Tafel.table_num.Length, " ");
            returnLine += Gebruiker.Email;
            return returnLine;
        }
    }

    class ReserveringsAdministration {
        public Reservering[] Reserveringen;
        public Reservering[] GastReservering;
        public Week DezeWeek;
        private const string FILENAME = "Reserveringen.json";

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

        /// <summary>Verwijderd een reservering van alle reserveringen gebaseerd op een reservering object</summary>
        public void RemoveReservering(Reservering reservering) => RemoveReservering(reservering.ToegangsCode);

        /// <summary>Verwijderd alle reserveringen van een bepaalde gebruiker</summary>
        public void RemoveReservering(User user) { 
            foreach (Reservering res in Reserveringen) {
                if (user != null && (res.Gebruiker == user || res.Guest == user))
                    RemoveReservering(res.ToegangsCode);
            }
        }

        /// <summary>Laat reserveringen zien afhankelijk van wie de method callt</summary>
        public void SeeReserveringen(User user) { 
            foreach (Reservering res in GetReserveringen(user)) {
                if (user.ModLevel == 1)
                    res.OneLine();
                else if (res.Gebruiker == user)
                    res.ShowReservering();
            }
        }

        /// <summary>Returned een array met reserveringen afhankelijk van de gegeven gebruiker</summary>
        public Reservering[] GetReserveringen(User user) {
            int totalLength = 0;
            Reservering[] returnReserveringen;
            if (user.ModLevel == 1) {  // voor admins
                returnReserveringen = new Reservering[Reserveringen.Length + GastReservering.Length];
                for (int i = 0; i < Reserveringen.Length; i++)
                    returnReserveringen[i] = Reserveringen[i];
                for (int i = 0; i < GastReservering.Length; i++)
                    returnReserveringen[Reserveringen.Length + i] = GastReservering[i];
            }
            else {  // voor normale gebruikers
                foreach (Reservering res in Reserveringen) {
                    if (res.Gebruiker == user)
                        totalLength++;
                }
                returnReserveringen = new Reservering[totalLength];
                for (int i = 0, j = 0; i < Reserveringen.Length; i++) {
                    if (Reserveringen[i].Gebruiker == user)
                        returnReserveringen[j++] = Reserveringen[i];
                } 
            }
            return returnReserveringen;
        }

        /// <summary>Zet een array met reserveringen om naar een array met beschrijvingen (strings) van de reserveringen</summary>
        public string[] ToStrArray(Reservering[] reserveringen) {
            string[] strArr = new string[reserveringen.Length];
            for (int i = 0; i < strArr.Length; i++) {
                strArr[i] = reserveringen[i].OneLine();
            }
            return strArr;
        }

        // TODO: zorg ervoor dat reserveringen worden opgeslagen.
        /// <summary>Een method die het menu voor reserveringen weergeeft voor admins</summary>
        public void Menu(User user, MenuKaart menu, UserAdministration useradmin) {
            string[] options;
            Resources.errorMessage("Reserveringen opslaan werkt helaas nog niet naar behoren, dit wordt zsm gefixt");
            if (user.IsAdmin())
                options = new string[] { "Zie reserveringen", "Maak een nieuwe reservering aan", "Pas een reservering aan", "Verwijder een reservering" };
            else
                options = new string[] { "Zie eigen reserveringen", "Maak een nieuwe reservering aan", "Pas een reservering aan", "Verwijder een reservering" };
            while (true) {
                Save();
                string choice = Resources.makeMenuInput($"Welkom bij de reservering opties {user.Voornaam}", "Kies een van bovenstaande opties: ", options, backbutton: true);
                if (choice == "1") { // zie reserveringen
                    SeeReserveringen(user);
                    Resources.EnterMessage();
                }
                else if (choice == "2" && user.IsAdmin()) // maak een nieuwe reservering als admin
                    ReserveringMenu(true, menu, useradmin);
                else if (choice == "2") // maak een nieuwe reservering als gebruiker
                    MakeReservering(user, menu);
                else if (choice == "3") { // pas een reservering aan
                    Reservering[] UserReserveringen = GetReserveringen(user);
                    string changechoice = Resources.makeMenuInput("Reserveringen om aan te passen", "Kies een van de bovenstaande opties", ToStrArray(UserReserveringen), backbutton: true);
                    if (changechoice == "b")
                        continue;
                    UserReserveringen[Convert.ToInt32(changechoice) - 1].Change(DezeWeek, user, menu);
                }
                else if (choice == "4") { // verwijder een reservering
                    Reservering[] UserReserveringen = GetReserveringen(user);
                    string changechoice = Resources.makeMenuInput("Reserveringen om aan te passen", "Kies een van de bovenstaande opties", ToStrArray(UserReserveringen), backbutton: true);
                    if (changechoice == "b")
                        continue;
                    RemoveReservering(UserReserveringen[Convert.ToInt32(changechoice) - 1]);
                }
                else // ga terug naar het vorige menu
                    break;
            }
        }   

        /// <summary>Laat een algemeen menu zien voor het reserveren en returned een reservering object (null als de reservering niet is geslaagd)</summary>
        public void ReserveringMenu(bool isadmin, MenuKaart menu, UserAdministration useradmin) {
            while (true) {
                Console.Clear();
                Resources.errorMessage("Reserveringen opslaan werkt helaas nog niet naar behoren, dit wordt zsm gefixt");
                string[] reserveOptions = new string[] { "Reserveer als gast", "Reserveer als gebruiker" };
                string reserveChoice = Resources.makeMenuInput("Hoe wilt u een reservering maken?", "Kies een van de bovenstaande opties", reserveOptions, backbutton: true);
                if (reserveChoice == "1") {
                    Person guest = MakeGuest();
                    guest.Present();
                    if (Resources.YesOrNo("Is deze informatie correct?: "))
                        MakeReservering(new User(guest.Voornaam, guest.Achternaam, guest.Email, guest.Tel_no, guest.Leeftijd, null, -1, guest.Tussenvoegsel), menu);
                    else
                        continue;
                }
                else  if (reserveChoice == "2") { // reserveer als gebruiker
                    if (!isadmin) {
                        User loggedIn = useradmin.Login();
                        if (loggedIn == null || loggedIn.IsAdmin()) {
                            Resources.errorMessage("De ingelogde gebruiker kan geen reservering doen op zijn/haar naam");
                        }
                        MakeReservering(loggedIn, menu);
                        return;
                    }
                    string mail = Resources.inputCheck("Voer de mail van de gebruiker in die een reservering doet", useradmin.GetMails(), "Die email is helaas niet beschikbaar", maxTries: 3);
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
                else
                    break;
            }
        }

        /// <summary>Maakt een person object, dit is een gast dus word niet opgeslagen in de useradministration, alleen in de reservering</summary>
        public Person MakeGuest() {
            string voornaam = Resources.InputRegex("Voornaam: ", @"\w+");
            string tussenvoegsel = Resources.input("Tussenvoegsel: ");
            string achternaam = Resources.InputRegex("Achternaam: ", @"\w+");
            string email = Resources.InputRegex("E-mail Adres: ", @"^\w+@\w+\.\w{2,3}$");
            string telefoonnummer = Resources.InputRegex("Telefoonnr: ", @"^(06|\+316)\d{8}$");
            string leeftijd = Resources.inputCheck("Leeftijd: ", Resources.makeRangeArr(18, 125), "Het ingevoerde getal is helaas onjuist, wees ervan bewust dat wij alleen gebruikers aannemen boven de 18");
            return new Person(voornaam, achternaam, email, telefoonnummer, leeftijd, tussenvoegsel);
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
                    chosenTable = chosenRoom.GetTafel();
                    if (chosenTable == null)
                        step--;
                    else
                        step++;
                }
                else if (step == 4) { // doe een bestelling voor de reservering
                    while (true) {
                        string choice = Resources.makeMenuInput("Weet je al wat je wilt? Dan kan je alvast een bestelling doen!", "Kies een van bovenstaande opties: ", new string[] { "Voeg een gerecht toe aan de bestelling", "Verwijder een gerecht van bestelling", "Ga verder" }, backbutton: true);
                        if (choice == "1") {
                            Gerecht addgerecht = menu.ChooseGerechten();
                            bestelling = AddGerechtToArr(addgerecht, bestelling);
                        }
                        else if (choice == "2") {
                            Gerecht removegerecht = menu.ChooseGerechten();
                            bestelling = RemoveGerechtFromArr(removegerecht, bestelling);
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

        /// <summary>Voegt een gerecht toe aan een gerecht[] array</summary>
        public static Gerecht[] AddGerechtToArr(Gerecht gerecht, Gerecht[] Bestelling) {
            Gerecht[] nieuwBestelling = new Gerecht[Bestelling.Length + 1];
            for (int i = 0; i < Bestelling.Length; i++) {
                nieuwBestelling[i] = Bestelling[i];
            }
            nieuwBestelling[Bestelling.Length] = gerecht;
            return nieuwBestelling;
        }

        /// <summary>Verwijdert een gerecht van een gerecht[] array</summary>
        public static Gerecht[] RemoveGerechtFromArr(Gerecht gerecht, Gerecht[] Bestelling) {
            Gerecht[] nieuwBestelling = new Gerecht[Bestelling.Length - 1];
            for (int i = 0; i < Bestelling.Length; i++) {
                if (gerecht != Bestelling[i])
                    nieuwBestelling[i] = Bestelling[i];  // TODO: verbeter dit zodat je maar een kopie van een gerecht verwijdert per keer en niet meteen alle gerechten met dezelfde naam
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
