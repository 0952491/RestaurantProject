using System;
using System.Linq;
using resourceMethods;

namespace LoginPage

{
    class Login
    {
        public static UserAdministration UserAdmin = new UserAdministration();

        public static void ReserveerHome() {
        // ik heb dit een reserveringsmenu gemaakt want we hadden al deze opties voor inloggen al op de homepagina (p.s. Jeroen)
            Console.Clear();
            string[] options = new string[] {"Inloggen", "Registreren", "Reserveer als Gast", "Backup gebruikers"};
            string input = Resources.makeMenuInput("Hoe wilt u reserveren?", "Kies een nummer: ", options, backbutton: true);
            if (input == "1") { Inloggen(true); }
            else if (input == "2") { Registreren(true); }
            else if (input == "3") { Gast(); }
            else if (input == "4") { UserAdmin.Save(); } // deze method hoort hier niet maar dit is tijdelijk
        }

        public static void Inloggen(bool reserveren=false)
        {
            Console.Clear();
            string[] registeredEmails = UserAdmin.GetMails();  // laad later alle mails uit de json file naar deze variable (mss met een for loop)
            Console.WriteLine("Login Pagina\n");
            int tries = 3;
            string loginMail = Resources.inputCheck("Email: ", registeredEmails, "Email incorrect", tries);
            string password = UserAdmin.GetPass(loginMail);
            string loginPass = loginMail != "" ? Resources.inputCheck("Wachtwoord: ", new string[] {password}, "Wachtwoord incorrect", tries) : "Geen wachtwoord";
    
            if (loginMail == "") {  // de gebruiker heeft 3 keer een verkeerde mail opgegeven  
                Resources.errorMessage($"U heeft {tries} keer een incorrecte mail opgegeven");
                Resources.errorMessage("U wordt teruggebracht naar de vorige pagina");
                ReserveerHome();
            } else if (loginPass == "") {  // de gebruiker heeft 3 keer een incorrect wachtwoord opgegeven
                Resources.errorMessage($"U heeft {tries} keer een incorrect wachtwoord voor email {loginMail} opgegeven");
                Resources.errorMessage("U wordt teruggebracht naar de vorige pagina");
                ReserveerHome();
            } else if (reserveren) {  // als de parameter reserveren true is ga je meteen door naar reserveren
                Resources.succesMessage("U bent succesvol ingelogd!");  
                Reserveren();
            } else { // als je succesvol inlogt ga je naar je eigen pagina waar je een overzicht hebt van je reserveringen en meer
                Resources.succesMessage("U bent succesvol ingelogd!");
                ReserveerHome(); 
            }

        }
        public static void Registreren(bool sub=true)
        {
            Console.Clear();
            var subs = UserAdmin;
            Console.WriteLine("Registreren\n");
            // veel van deze inputs kunnen later gechecked worden met een regex (https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=net-5.0)
            string voornaam = Resources.inputRegex("Voornaam: ", @"\w+");
            string tussenvoegsel = Resources.input("Tussenvoegsel: ");
            string achternaam = Resources.inputRegex("Achternaam: ", @"\w+");
            string email = Resources.inputRegex("E-mail Adres: ", @"^\w+@\w+\.\w{2,3}$");
            string telefoonnummer = Resources.inputRegex("Telefoonnr: ", @"^(06|\+316)\d{8}$"); 
            string geboortedatum = Resources.inputRegex("Geboortedatum(dd-mm-yyyy): ", @"\d{2}\-\d{2}\-\d{4}");
            string wachtwoord = Resources.inputRegex("Wachtwoord: ", @"\w{8}");
            string inputHerhaal = Resources.inputCheck("Herhaal Wachtwoord: ", new string[] {wachtwoord}, "Wachtwoorden komen niet overeen", 3);
            if (inputHerhaal == "") {  // het herhaalde wachtwoord is {int maxtries} te vaak ingevoerd
                Resources.errorMessage("Te vaak een verkeerd wachtwoord uitgeprobeerd");
                Resources.errorMessage("U wordt teruggebracht naar de vorige pagina");
                ReserveerHome();
            } else {  // de gebruiker registreert alle gegevens TODO: hier moeten alle gegevens van de gebruiker in json worden opgeslagen
                if (sub) {
                    Person newuser = new Person(voornaam, achternaam, email, telefoonnummer, geboortedatum, wachtwoord, 0, tussenvoegsel);
                    UserAdmin.AddSub(newuser);
                    Resources.succesMessage("U bent succesvol geregistreerd!");
                    ReserveerHome();
                } else {
                    Person newadmin = new Person(voornaam, achternaam, email, telefoonnummer, geboortedatum, wachtwoord, 1, tussenvoegsel);
                    UserAdmin.AddAdmin(newadmin);
                    Resources.succesMessage("U bent succesvol geregistreerd!");
                    ReserveerHome();
                }
            }
        }
        public static void Gast()
        {
            Console.Clear();
            Console.WriteLine("Gast\n");
            // hier geldt hetzelfde als voor de vorige method wat betreft de regex
            string Email = Resources.input("Email: ");
            string telefoonnummer = Resources.inputRegex("Telefoonnr: ", @"^06\d{8}$"); 
            // hier moet later ook weer een line komen die de gegevens (email en tel.nr.) opslaat in json
            Reserveren();
        }
        public static void Reserveren()
        {
            Console.Clear();
            Console.WriteLine("Welkom bij restaurant Wiqui");
            Console.ReadLine();
        }
    }
    class Person { 
        public string Voornaam;
        public string Tussenvoegsel = "";
        public string Achternaam;
        public string Email;
        public string Tel_no;
        public string Leeftijd;
        public string Wachtwoord;
        public int ModLevel; // modlevel 0 = subscriber, 1 = admin (mss nog een voor personeel?)
        
        public Person(string voornaam, string achternaam, string email, string tel, string geboortedatum, string wachtwoord, int modLevel, string tussenvoegsel = ""){
            Voornaam = voornaam;
            Tussenvoegsel = tussenvoegsel;
            Achternaam = achternaam;
            Email = email;
            Tel_no = tel;
            Leeftijd = geboortedatum;
            Wachtwoord = wachtwoord;
            ModLevel = modLevel;
        }

        public void Present() {
            if (Tussenvoegsel != "")
                Console.WriteLine($"Naam: {Voornaam} {Tussenvoegsel} {Achternaam}");
            else
                Console.WriteLine($"Naam: {Voornaam} {Achternaam}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Telefoon nr.: {Tel_no}");
            Console.WriteLine($"Leeftijd: {Leeftijd}");
        }

        /// <summary>Returned true if modLevel = 1, anders returns false</summary>
        public bool IsAdmin() { 
            return ModLevel == 1;
        }

        /// <summary>Met deze method kan je de gegevens van een persoon veranderen</summary>
        public void ChangePerson() {
            Present();
            string[] opties = new string[] {"Voornaam", "Tussenvoegsel", "Achternaam", "Email", "Telefoon nr", "Leeftijd"};
            string choice = Resources.makeMenuInput("Verander je gegevens", "Kies een van de bovenstaande opties: ", opties, backbutton: true);
            if (choice == "1") // verander je voornaam
                Voornaam = Resources.input("Vul je voornaam in: ");
            else if (choice == "2") // verander je tussenvoegsel
                Tussenvoegsel = Resources.input("Vul je tussenvoegsel in: ");
            else if (choice == "3") // verander je achternaam
                Achternaam = Resources.input("Vul je achternaam in: ");
            else if (choice == "4") // verander je email
                Email =   Resources.inputRegex("Vul je email in: ", @"^\w+@\w+\.\w{2,3}$");
            else if (choice == "5") // verander je tel nr
                Tel_no = Resources.inputRegex("Vul je Telefoonnr in: ", @"^{06}\d{8}$");
            else if (choice == "6")
                Leeftijd = Resources.inputCheck("Vul je leeftijd in: ", Resources.makeRangeArr(18, 125), "Het ingevoerde getal is helaas onjuist, wees ervan bewust dat wij alleen gebruikers aannemen boven de 18");
        }
    }

    class UserAdministration { // De adminstratie waar alle admins en users worden opgeslagen
        public Person[] Subscribers;
        public Person[] Admins;
        private const string FILENAME = "Users.json";

        public UserAdministration() {
            Subscribers = LoadUsers(DataHandler.loadJson(FILENAME).Subscribers);
            Admins = LoadUsers(DataHandler.loadJson(FILENAME).Admins);
        }

        /// <summary>Zet de gegevens om in Person arrays</summary>
        public Person[] LoadUsers(dynamic Users)
        {
            return Users.ToObject<Person[]>();
        }

        /// <summary>slaat de UserAdministration op in "Users.json" file in Data folder</summary>
        public void Save() {
            DataHandler.writeJson(FILENAME, this);
        }

        /// <summary>Voegt een Subscriber toe aan Subscribers array</summary>
        public void AddSub(Person sub) {
            if (!Exists(sub) && !sub.IsAdmin()) {
                Person[] newSubscribers = new Person[Subscribers.Length + 1];
                for (int i = 0; i < Subscribers.Length; i++) {
                    newSubscribers[i] = Subscribers[i];
                }
                newSubscribers[Subscribers.Length] = sub;
                Subscribers = newSubscribers;
            }
        }

        ///<summary>Voegt een Admin toe aan Admins array</summary>
        public void AddAdmin(Person admin) {
            if (!Exists(admin) && admin.IsAdmin()) { 
                Person[] newAdmins = new Person[Admins.Length + 1];
                for (int i = 0; i < Admins.Length; i++) {
                    newAdmins[i] = Admins[i];
                }
                newAdmins[Admins.Length] = admin;
                Admins = newAdmins;
            } else {
                Resources.errorMessage("");
            }
        }

        /// <summary>Verwijderd een Subscriber uit de Subscribers array</summary>
        public void RemoveSub(Person user) {
            if (Exists(user) && !user.IsAdmin()) {
                Person[] newSubscribers = new Person[Subscribers.Length - 1];
                for (int i = 0; i < Subscribers.Length; i++) { 
                    if (Subscribers[i] == user) { 
                        continue;
                    }
                    newSubscribers[i] = Subscribers[i];
                }
                Subscribers = newSubscribers;
            } else { 
                Resources.errorMessage("That subscriber doesn't exist");
                Resources.input("Press enter to continue");
            }
        }

        /// <summary>Checked of de passed user bestaat gebaseerd op alle mails (van Admins en Subscribers)</summary>
        public bool Exists(Person user) { 
            return GetMails().Contains(user.Email);
        }

        /// <summary>Verzamelt alle mails van alle gebruikers in een array en returned die array</summary>
        public string[] GetMails() {
            string[] mails;
            mails = new string[Subscribers.Length + Admins.Length];
            int totalIndex = 0;
            for (int i = 0; i < Subscribers.Length; i++) {
                mails[totalIndex++] = Subscribers[i].Email;
            }
            for (int i = 0; i < Admins.Length; i++) {
                mails[totalIndex++] = Admins[i].Email;
            }
            return mails;
        }

        /// <summary>Returned het wachtwoord die bij de gegeven mail hoort</summary>
        public string GetPass(string mail) {
            string pass = "";
            foreach (Person user in Subscribers) {
                if (user.Email == mail)
                    pass = user.Wachtwoord;
            }
            foreach (Person user in Admins) {
                if (user.Email == mail)
                    pass = user.Wachtwoord;
            }
            return pass;
        }
    }  
}