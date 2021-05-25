using System;
using System.Linq;
using Newtonsoft.Json;
using ReserveringPage;
using resourceMethods;

namespace LoginPage
{
    class Person { 
        public string Voornaam;
        public string Tussenvoegsel = "";
        public string Achternaam;
        public string Email;
        public string Tel_no;
        public string Leeftijd;
        
        
        public Person(string voornaam, string achternaam, string email, string tel, string leeftijd, string tussenvoegsel = "") {
            Voornaam = voornaam;
            Tussenvoegsel = tussenvoegsel;
            Achternaam = achternaam;
            Email = email;
            Tel_no = tel;
            Leeftijd = leeftijd;
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

        /// <summary>Met deze method kan je de gegevens van een persoon veranderen</summary>
        public void ChangePerson()
        {
            while (true) {
                Console.Clear();
                Present();
                string[] opties = new string[] { "Voornaam", "Tussenvoegsel", "Achternaam", "Email", "Telefoon nr", "Leeftijd" };
                string choice = Resources.makeMenuInput("Verander je gegevens", "Kies een van de bovenstaande opties: ", opties, backbutton: true);
                if (choice == "1") // verander je voornaam
                    Voornaam = Resources.input("Vul je voornaam in: ");
                else if (choice == "2") // verander je tussenvoegsel
                    Tussenvoegsel = Resources.input("Vul je tussenvoegsel in: ");
                else if (choice == "3") // verander je achternaam
                    Achternaam = Resources.input("Vul je achternaam in: ");
                else if (choice == "4") // verander je email
                    Email = Resources.InputRegex("Vul je email in: ", @"^(\w|\.)+@\w+\.\w{2,3}$");
                else if (choice == "5") // verander je tel nr
                    Tel_no = Resources.InputRegex("Vul je Telefoonnr in: ", @"^\d{10}$");
                else if (choice == "6")
                    Leeftijd = Resources.inputCheck("Vul je leeftijd in: ", Resources.makeRangeArr(18, 125), "Het ingevoerde getal is helaas onjuist, wees ervan bewust dat wij alleen gebruikers aannemen boven de 18");
                else
                    break;
            }
        }
    }


    class User : Person {
        public string Wachtwoord;
        public int ModLevel; // modlevel 0 = subscriber, 1 = admin (mss nog een voor personeel?)
        
        [JsonConstructor]
        public User(string voornaam, string achternaam, string email, string tel, string leeftijd, string wachtwoord, int modlevel, string tussenvoegsel = "") 
            : base(voornaam, achternaam, email, tel, leeftijd, tussenvoegsel) {
            Wachtwoord = wachtwoord;
            ModLevel = modlevel;
        }
        public User(User copy) : this(copy.Voornaam, copy.Achternaam, copy.Email, copy.Tel_no, copy.Leeftijd, copy.Wachtwoord, copy.ModLevel, copy.Tussenvoegsel) {}

        /// <summary>Returned true if modLevel = 1, anders returns false</summary>
        public bool IsAdmin() => ModLevel == 1;

        public static bool operator ==(User p1, User p2) {
            if (ReferenceEquals(p1, p2))
                return true;
            else if (ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
                return false;
            return p1.ModLevel == p2.ModLevel && p1.Wachtwoord == p2.Wachtwoord && p1.Email == p2.Email && p1.Voornaam == p2.Voornaam && p1.Achternaam == p2.Achternaam && p1.Leeftijd == p2.Leeftijd && p1.Tel_no == p2.Tel_no && p1.Tussenvoegsel == p2.Tussenvoegsel;
        }

        public static bool operator !=(User p1, User p2) {
            return !(p1 == p2);
        }
    }


    class UserAdministration { // De adminstratie waar alle admins en users worden opgeslagen
        public User[] Subscribers;
        public User[] Admins;
        private const string FILENAME = "Users.json";

        public UserAdministration() {
            if (!DataHandler.FileExists(FILENAME)) {
                Subscribers = new User[0];
                Admins = new User[0];
            }
            else {
                Subscribers = LoadUsers(DataHandler.LoadJson(FILENAME).Subscribers);
                Admins = LoadUsers(DataHandler.LoadJson(FILENAME).Admins);
            }
        }

        /// <summary>Zet de gegevens om in Person arrays</summary>
        public User[] LoadUsers(dynamic Users) => Users.ToObject<User[]>();

        /// <summary>slaat de UserAdministration op in "Users.json" file in Data folder</summary>
        public void Save() => DataHandler.WriteJson(FILENAME, this);

        /// <summary>Het menu voor de administrator</summary>
        public void AdminMenu(User admin, ReserveringsAdministration resAdmin) {
            string[] options = new string[] { "Registreer Gebruiker", "Registreer Admin", "Zie alle geregistreerde mails", "Wijzig profiel", "Verwijder Gebruiker", "Verwijder Admin", "Verwijder eigen account"};
            while (true) {
                Console.Clear();
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
                    RemoveUser(false, admin, resAdmin);
                else if (choice == "6")
                    RemoveUser(true, admin, resAdmin);
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
        public void DefaultMenu(User user, ReserveringsAdministration resAdmin) {
            string[] options = new string[] { "Zie account informatie", "Wijzig profiel", "Verwijder eigen account" };
            while (true) {
                Console.Clear();
                Save();
                string choice = Resources.makeMenuInput("Gebruiker menu", "Voer hier een van de bovenstaande opties in: ", options, backbutton: true);
                if (choice == "1") {
                    Console.Clear();
                    user.Present();
                    Resources.EnterMessage();
                }
                else if (choice == "2") {
                    User oldPerson = new User(user);
                    user.ChangePerson();
                    resAdmin.ResetPerson(oldPerson, user);
                }
                else if (choice == "3") {
                    string passcheck = Resources.InputWachtwoord("Voer je wachtwoord in om je account te verwijderen: ", user.Wachtwoord, "Wachtwoord incorrect", 3);
                    if (passcheck == "")
                        continue;
                    bool confirm = Resources.YesOrNo("Weet je zeker dat je je account wil verwijderen? (j/n): ");
                    if (confirm) {
                        RemoveSub(user);
                        resAdmin.RemoveReservering(user);
                        break;
                    }
                }
                else
                    break;
            }
        }

        /// <summary>Voegt een Subscriber toe aan Subscribers array</summary>
        public void AddSub(User sub) {
            if (!Exists(sub) && !sub.IsAdmin()) {
                User[] newSubscribers = new User[Subscribers.Length + 1];
                for (int i = 0; i < Subscribers.Length; i++) {
                    newSubscribers[i] = Subscribers[i];
                }
                newSubscribers[Subscribers.Length] = sub;
                Subscribers = newSubscribers;
            }
        }

        ///<summary>Voegt een Admin toe aan Admins array</summary>
        public void AddAdmin(User admin) {
            if (!Exists(admin) && admin.IsAdmin()) { 
                User[] newAdmins = new User[Admins.Length + 1];
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
        public void RemoveUser(bool adminRemove, User user, ReserveringsAdministration resAdmin) {
            string removeMail = Resources.inputCheck("Email van te verwijderen gebruiker: ", GetMails(), "Email incorrect", 3);
            if (removeMail == "")
                return;
            if (removeMail.ToLower() == user.Email.ToLower()) {
                Resources.errorMessage("Als je je eigen account wil verwijderen kies dan optie 7 van het admin menu");
                return;
            }
            if (!adminRemove) { // als er geen admin word verwijderd dan heeft diegene geen wachtwoord nodig om het account te verwijderen
                RemoveSub(GetUser(removeMail));
                resAdmin.RemoveReservering(user);
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
        public void RemoveSub(User user) {
            if (Exists(user) && !user.IsAdmin()) {
                User[] newSubscribers = new User[Subscribers.Length - 1];
                for (int i = 0; i < Subscribers.Length; i++) { 
                    if (Subscribers[i] == user) { 
                        continue;
                    }
                    newSubscribers[i] = Subscribers[i];
                }
                Subscribers = newSubscribers;
                Resources.succesMessage($"Account onder email {user.Email} succesvol verwijderd!");
                Resources.EnterMessage();
            } else { 
                Resources.errorMessage("Die gebruiker bestaat niet");
                Resources.EnterMessage();
            }
        }

        /// <summary>Verwijderd een Admin uit de Admin array</summary>
        public void RemoveAdmin(User user) {
            if (Exists(user) && user.IsAdmin()) {
                User[] newAdmins = new User[Admins.Length - 1];
                for (int i = 0; i < Admins.Length; i++) {
                    if (Admins[i] == user) {
                        continue;
                    }
                    newAdmins[i] = Admins[i];
                }
                Admins = newAdmins;
                Resources.succesMessage($"Account onder email {user.Email} succesvol verwijderd!");
                Resources.EnterMessage();
            } else {
                Resources.errorMessage("Die admin bestaat niet");
                Resources.EnterMessage();
            }
        }

        /// <summary>Checked of de passed user bestaat gebaseerd op alle mails (van Admins en Subscribers)</summary>
        public bool Exists(User user) => Resources.MailCheck(user.Email, GetMails());

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
            foreach (User user in Subscribers) {
                if (user.Email.ToLower() == mail.ToLower())
                    pass = user.Wachtwoord;
            }
            foreach (User user in Admins) {
                if (user.Email.ToLower() == mail.ToLower())
                    pass = user.Wachtwoord;
            }
            return pass;
        }

        /// <summary>Returned een User object uit een van de arrays gebaseerd op de gegeven mail</summary>
        public User GetUser(string mail) {
            User returnUser = null;
            foreach (User user in Subscribers) {
                if (user.Email.ToLower() == mail.ToLower())
                    returnUser = user;
            }
            foreach (User user in Admins) {
                if (user.Email.ToLower() == mail.ToLower())
                    returnUser = user;
            }
            return returnUser;
        }

        /// <summary>Registreer een nieuwe gebruiker</summary>
        public void Registreren(bool admin) { // als admin == true dan registreer een admin
            User newUser;
            Console.Clear();
            string voornaam = Resources.InputRegex("Voornaam: ", @"\w+");
            string tussenvoegsel = Resources.input("Tussenvoegsel: ");
            string achternaam = Resources.InputRegex("Achternaam: ", @"\w+");
            string email = Resources.InputRegex("E-mail Adres: ", @"^(\w|\.)+@\w+\.\w{2,3}$");
            while (GetMails().Contains(email)) {
                Resources.errorMessage($"{email} is al geregistreerd, probeer alstublieft opnieuw");
                email = Resources.InputRegex("E-mail Adres: ", @"^(\w|\.)+@\w+\.\w{2,3}$");
            }
            string telefoonnummer = Resources.InputRegex("Telefoonnr: ", @"^\d{10}$", "Voert u alstublieft 10 cijfers in als telefoonnummer");
            string leeftijd = Resources.inputCheck("Leeftijd: ", Resources.makeRangeArr(18, 125), "Het ingevoerde getal is helaas onjuist, wees ervan bewust dat wij alleen gebruikers aannemen boven de 18");
            string wachtwoord = Resources.InputWachtwoord("Wachtwoord: ");
            string inputHerhaal = Resources.InputWachtwoord("Herhaal wachtwoord: ", wachtwoord, maxtries: 3);
            if (admin && inputHerhaal != "") {
                newUser = new User(voornaam, achternaam, email, telefoonnummer, leeftijd, wachtwoord, 1, tussenvoegsel);
                AddAdmin(newUser);
                Resources.succesMessage("Succesvol Geregistreerd!");
                Resources.EnterMessage();
            }
            else if (!admin && inputHerhaal != "") {
                newUser = new User(voornaam, achternaam, email, telefoonnummer, leeftijd, wachtwoord, 0, tussenvoegsel);
                AddSub(newUser);
                Resources.succesMessage("Succesvol Geregistreerd!");
                Resources.EnterMessage();
            }
            else {
                Resources.errorMessage("3 keer een verkeerd wachtwoord ingevoerd voor herhaling, Registratie mislukt");
                Resources.EnterMessage();
            }
        }

        /// <summary>Laat gebruiker inloggen en returned ingelogde gebruiker</summary>
        public User Login() {
            Console.Clear();
            string loginMail = Resources.inputMail("Email: ", GetMails(), "Email incorrect", 3);
            string loginPass = loginMail != "" ? Resources.InputWachtwoord("Wachtwoord: ", GetPass(loginMail), "Wachtwoord incorrect", 3) : "";
            if (loginMail != "" && loginPass != "")
                return GetUser(loginMail);
            else
                return null;
        }
    }  
}