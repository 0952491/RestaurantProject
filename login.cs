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
            Resources.errorMessage("Dit menu voor reserveren werkt nog niet");
            Resources.input("Druk op enter om verder te gaan");

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

        /// <summary>Presenteert alle zichtbare attributen van de Person</summary>
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
        public bool IsAdmin() => ModLevel == 1;

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
        public Person[] LoadUsers(dynamic Users) => Users.ToObject<Person[]>();

        /// <summary>slaat de UserAdministration op in "Users.json" file in Data folder</summary>
        public void Save() => DataHandler.writeJson(FILENAME, this);

        /// <summary>Het menu voor de administrator</summary>
        public void AdminMenu(Person admin) {
            string[] options = new string[] { "Registreer Gebruiker", "Registreer Admin", "Zie alle geregistreerde mails", "Wijzig profiel", "Verwijder Gebruiker", "Verwijder Admin", "Verwijder eigen account"};
            while (true) {
                Save();
                string choice = Resources.makeMenuInput("Beheer Gebruikers", "Kies een van de bovenstaande opties: ", options, backbutton: true);
                if (choice == "1")
                    Registreren(false);
                else if (choice == "2")
                    Registreren(true);
                else if (choice == "3") {
                    foreach (string mail in GetMails())
                        Console.WriteLine(mail);
                }
                else if (choice == "4")
                    admin.ChangePerson();
                else if (choice == "5")
                    RemoveUser(false, admin);
                else if (choice == "6")
                    RemoveUser(true, admin);
                else if (choice == "7") {
                    RemoveAdmin(admin);
                    Save();
                    break;
                }
                else
                    break;
            }
        }
        
        /// <summary>Een menu voor normale gebruikers (geen admins)</summary>
        public void DefaultMenu(Person user) {
            string[] options = new string[] { "Zie account informatie", "Wijzig profiel", "Verwijder eigen account" };
            while (true) {
                Save();
                string choice = Resources.makeMenuInput("Gebruiker menu", "Kies een van bovenstaande opties", options, backbutton: true);
                if (choice == "1")
                    user.Present();
                else if (choice == "2")
                    user.ChangePerson();
                else if (choice == "3") { // als de method 'RemoveSub' later word verplaatst naar Main menu mag deze clause weg
                    string passcheck = Resources.inputCheck("Voer je wachtwoord in om je account te verwijderen: ", new string[] { user.Wachtwoord }, "Wachtwoord incorrect", 3);
                    if (passcheck == "")
                        continue;
                    string confirm = Resources.inputCheck("Weet je zeker dat je je account wil verwijderen? (j/n): ", new string[] { "j", "n", "ja", "nee" }, "Kies alsjeblieft uit ja of nee");
                    if (new string[] { "j", "ja" }.Contains(confirm)) {
                        RemoveSub(user);
                        Save();
                        break;
                    }
                }
                else
                    break;
            }
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

        /// <summary>Geeft een prompt en verwijderd gebruiker gebaseerd op email, mag alleen worden gebruikt door admins</summary>
        public void RemoveUser(bool adminRemove, Person user) {
            string removeMail = Resources.inputCheck("Email van te verwijderen gebruiker: ", GetMails(), "Email incorrect", 3);
            if (removeMail == "")
                return;
            if (removeMail == user.Email) {
                Resources.errorMessage("If you want to delete your own account please pick option 7 from the administrator menu");
                return;
            }
            if (!adminRemove) { // if no admin will be removed he/she doesn't need to have the password of that user
                RemoveSub(GetUser(removeMail));
                return;
            }
            string removePass = Resources.inputCheck("Wachtwoord van de te verwijderen gebruiker: ", new string[] { GetPass(removeMail) }, "Wachtwoord incorrect", 3);
            if (removePass == "") // wachtwoord was 3 keer verkeerd ingevoerd
                return;
            if (adminRemove)
                RemoveAdmin(GetUser(removeMail));
            else
                RemoveSub(GetUser(removeMail));
        }

        /// <summary>Verwijderd een Subscriber uit de Subscribers array</summary>
        public void RemoveSub(Person user) {
            // TODO: later in deze method ook de user verwijderen uit de reserveeradministratie of deze method naar main menu verplaatsen
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
                Resources.errorMessage("Die gebruiker bestaat niet");
                Resources.input("Druk op enter om verder te gaan");
            }
        }

        /// <summary>Verwijderd een Admin uit de Admin array</summary>
        public void RemoveAdmin(Person user) {
            if (Exists(user) && user.IsAdmin()) {
                Person[] newAdmins = new Person[Admins.Length - 1];
                for (int i = 0; i < Admins.Length; i++) {
                    if (Admins[i] == user) {
                        continue;
                    }
                    newAdmins[i] = Admins[i];
                }
                Admins = newAdmins;
            } else {
                Resources.errorMessage("Die admin bestaat niet");
                Resources.input("Druk op enter om verder te gaan");
            }
        }

        /// <summary>Checked of de passed user bestaat gebaseerd op alle mails (van Admins en Subscribers)</summary>
        public bool Exists(Person user) => GetMails().Contains(user.Email);

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

        /// <summary>Returneed een Person object uit een van de arrays gebaseerd op de gegeven mail</summary>
        public Person GetUser(string mail) {
            Person returnUser = null;
            foreach (Person user in Subscribers) {
                if (user.Email == mail)
                    returnUser = user;
            }
            foreach (Person user in Admins) {
                if (user.Email == mail)
                    returnUser = user;
            }
            return returnUser;
        }

        /// <summary>Registreer een nieuwe gebruiker</summary>
        public void Registreren(bool admin) { // als admin == true dan registreer een admin
            Person newUser;
            Console.Clear();
            string voornaam = Resources.inputRegex("Voornaam: ", @"\w+");
            string tussenvoegsel = Resources.input("Tussenvoegsel: ");
            string achternaam = Resources.inputRegex("Achternaam: ", @"\w+");
            string email = Resources.inputRegex("E-mail Adres: ", @"^\w+@\w+\.\w{2,3}$");
            while (GetMails().Contains(email)) {
                Resources.errorMessage($"{email} is al geregistreerd, probeer alstublieft opnieuw");
                Resources.inputRegex("E-mail Adres: ", @"^\w+@\w+\.\w{2,3}$");
            }
            string telefoonnummer = Resources.inputRegex("Telefoonnr: ", @"^(06|\+316)\d{8}$");
            string leeftijd = Resources.inputCheck("Leeftijd: ", Resources.makeRangeArr(18, 125), "Het ingevoerde getal is helaas onjuist, wees ervan bewust dat wij alleen gebruikers aannemen boven de 18");
            string wachtwoord = Resources.inputRegex("Wachtwoord: ", @"\w{8}");
            string inputHerhaal = Resources.inputCheck("Herhaal Wachtwoord: ", new string[] { wachtwoord }, "Wachtwoorden komen niet overeen", 3);
            if (admin && inputHerhaal != "")
            {
                newUser = new Person(voornaam, achternaam, email, telefoonnummer, leeftijd, wachtwoord, 1, tussenvoegsel);
                AddAdmin(newUser);
                Resources.succesMessage("Succesvol Geregistreerd!");
                Resources.input("Druk op enter om verder te gaan");
            }
            else if (!admin && inputHerhaal != "")
            {
                newUser = new Person(voornaam, achternaam, email, telefoonnummer, leeftijd, wachtwoord, 0, tussenvoegsel);
                AddSub(newUser);
                Resources.succesMessage("Succesvol Geregistreerd!");
                Resources.input("Druk op enter om verder te gaan");
            }
            else
                Resources.errorMessage("3 keer een verkeerd wachtwoord ingevoerd voor herhaling, Registratie mislukt");
                Resources.input("Druk op enter om verder te gaan");
        }

        /// <summary>Laat gebruiker inloggen en returned ingelogde gebruiker</summary>
        public Person Login()
        {
            string loginMail = Resources.inputCheck("Email: ", GetMails(), "Email incorrect", 3);
            string loginPass = loginMail != "" ? Resources.inputCheck("Wachtwoord: ", new string[] { GetPass(loginMail) }, "Wachtwoord incorrect", 3) : "Geen wachtwoord";
            if (loginMail != "" && loginPass != "Geen wachtwoord")
                return GetUser(loginMail);
            else
                return null;
        }
    }  
}