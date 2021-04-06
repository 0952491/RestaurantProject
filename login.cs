using System;
using System.Linq;
using resourceMethods;

namespace LoginPage
{
    class Login
    {
        public static void ReserveerHome()
        // ik heb dit een reserveringsmenu gemaakt want we hadden al deze opties voor inloggen al op de homepagina (p.s. Jeroen)
        {
            Console.Clear();
            Resources.orderOptions("Hoe wilt u reserveren?", new string[] {"Inloggen", "Registreren", "Reserveer als Gast"}, true);
            string input = Resources.inputCheck("Kies een nummer: ", new string[] {"1", "2", "3", "b", "B"});
            if (input == "1") { Inloggen(true); }
            else if (input == "2") { Registreren(); }
            else if (input == "3") { Gast(); }
        }

        public static void Inloggen(bool reserveren=false)
        {
            Console.Clear();
            string[] registeredEmails = new string[] {"test@gmail.com"};  // laad later alle mails uit de json file naar deze variable (mss met een for loop)
            Console.WriteLine("Login Pagina\n");
            int tries = 3;
            string loginMail = Resources.inputCheck("Email: ", registeredEmails, "Email incorrect", tries);
            string password = "12345";  // dit moet uit een json file komen en is afhankelijk van de mail
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
        public static void Registreren()
        {
            Console.Clear();
            Console.WriteLine("Registreren\n");
            // veel van deze inputs kunnen later gechecked worden met een regex (https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=net-5.0)
            string voornaam = Resources.input("Voornaam: ");
            string tussenvoegsel = Resources.input("Tussenvoegsel: ");
            string achternaam = Resources.input("Achternaam: ");
            string email = Resources.input("E-mail Adres: ");
            string telefoonnummer = Resources.input("Telefoonnummer: ");
            string geboortedatum = Resources.input("Geboortedatum: ");
            string wachtwoord = Resources.input("Wachtwoord: ");
            string inputHerhaal = Resources.inputCheck("Herhaal Wachtwoord: ", new string[] {wachtwoord}, "Wachtwoorden komen niet overeen", 3);
            if (inputHerhaal == "") {  // het herhaalde wachtwoord is {int maxtries} te vaak ingevoerd
                Resources.errorMessage("Te vaak een verkeerd wachtwoord uitgeprobeerd");
                Resources.errorMessage("U wordt teruggebracht naar de vorige pagina");
                ReserveerHome();
            } else {  // de gebruiker registreert alle gegevens TODO: hier moeten alle gegevens van de gebruiker in json worden opgeslagen
                Resources.succesMessage("U bent succesvol geregistreerd!");
                ReserveerHome();
            }
        }
        public static void Gast()
        {
            Console.Clear();
            Console.WriteLine("Gast\n");
            // hier geldt hetzelfde als voor de vorige method wat betreft de regex
            string Email = Resources.input("Email: ");
            string Teli = Resources.input("Telefoonnummer: ");
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
}