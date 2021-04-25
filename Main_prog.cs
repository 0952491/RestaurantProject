using System;
using resourceMethods;
using LoginPage; // namespace van login.cs
using MenuPage; // namespace van Menu.cs
using TablePage; // namespace van Tafels.cs
namespace Main_Restaurant
{
    class Main_Menu  // hoofdmenu van de applicatie
    {
        public static MenuKaart Menu = new MenuKaart();
        public static UserAdministration UserAdmin = new UserAdministration();
        public static DinnerRoom dinnerroom = new DinnerRoom();
        // TODO: zie hieronder
        /* public static ReserveerAdministration ReserveerAdmin;*/


        /// <summary>Het menu dat je te zien krijgt wanneer je de applicatie opstart</summary>
        public static void BeginMenu() {
            dinnerroom.Occupy("4B"); // voorbeeldfunctie om te laten zien hoe de plattegrond eruit ziet met tafels die bezet zijn
            dinnerroom.Occupy("1C");
            while (true) {
                Console.Clear();
                // de optie om beschikbare tafels te bekijken moet weg op een gegeven moment
                string[] userActions = {"Registreren", "Inloggen", "Reserveren", "Menu bekijken", "Contact", "Bekijk beschikbare tafels", "Sluit applicatie"};
                Resources.errorMessage("ALLE ACTIES DIE MET RESERVEREN TE MAKEN HEBBEN WERKEN NOG NIET\nHIER MOET NOG EEN APARTE CLASS VOOR GEBOUWD WORDEN\n");
                string optie = Resources.makeMenuInput("Welkom bij ons restaurant!", "Voert u alstublieft een nummer in: ", userActions);
                if (optie == "1") 
                    UserAdmin.Registreren(false);
                else if (optie == "2") {
                    var loginUser = UserAdmin.Login();
                    if (loginUser != null)
                        LoggedInMenu(loginUser);
                    else
                        Resources.errorMessage("Inloggen mislukt"); }
                else if (optie == "3")
                    Login.ReserveerHome();
                else if (optie == "4")
                    Menu.ShowGerechten();
                else if (optie == "5")
                    ContactPage();
                else if (optie == "6") {
                    dinnerroom.DrawMap();
                    Resources.input("Druk op enter om verder te gaan");
                }
                else {  // de gebruiker koos de laatste optie dus sluit de applicatie
                    Resources.succesMessage("Dankjewel voor het gebruiken van onze app, tot volgende keer ; )");
                    Console.ReadLine();
                    break;
                }
            }
        }


        /// <summary>Een menu voor wanneer een user is ingelogd</summary>
        public static void LoggedInMenu(Person user) {
            while (true) {
                string message;
                if (user.Tussenvoegsel == "")
                    message = $"Ingelogd als {user.Voornaam} {user.Achternaam}";
                else
                    message = $"Ingelogd als {user.Voornaam} {user.Tussenvoegsel} {user.Achternaam}";
                if (user.IsAdmin()) { // geef het menu weer van een admin
                    string[] opties = new string[] {"Reserveringen", "Menu", "Gebruikers", "Uitloggen"};
                    string choice = Resources.makeMenuInput(message, "Kies een van de bovenstaande opties: ", opties);
                    // TODO: zie hieronder
                    /*if (choice == "1")
                        ReserveerAdmin.AdminMenu();*/
                    if (choice == "2")
                        Menu.AdminMenu();
                    else if (choice == "3")
                        UserAdmin.AdminMenu(user);
                    else
                        break;
                }
                else {
                    string[] opties = new string[] {"Reserveringen", "Menu", "Profiel", "Uitloggen"};
                    string choice = Resources.makeMenuInput(message, "Kies een van de bovenstaande opties: ", opties, backbutton: true);
                    // TODO: zie hieronder
                    /*if (choice == "1")
                    ReserveerAdmin.DefaultMenu();  */
                    if (choice == "2")
                        Menu.ShowGerechten();
                    else if (choice == "3")
                        UserAdmin.DefaultMenu(user);
                    else
                        break;
                }

            }

        }

        public static void Main()
        {
            BeginMenu();
        }

        public static void ContactPage()
        {
            Console.Clear();
            string contact = $"Adres: \t \t Wijnhaven 107 \n" +
                "\t\t 3011 WN Rotterdam\n" +
                "Email: \t \t info@restaurantTeam4.nl \n" +
                "Telefoonnummer:\t 0612345678";
            Console.WriteLine(contact);
            string choice = Resources.inputCheck("Typ 'b' om terug te gaan\n", new string[] {"b", "B"}, maxTries: 1);
            if (choice == "") {
                ContactPage();
            } else {  // gebruiker typte 'b' of 'B' om terug te gaan
                return;
            }
        }

        // op dit moment is overzicht onzichtbaar voor gebruikers, overzicht moet worden gekoppeld aan alle invoer na de reservering [ #FransZijnDing :-) ]
        public static void Overzicht()
        {
            Console.Clear();
            Console.WriteLine("Reservering Overzicht \n" +
                $"Naam: \n" +
                $"Email: \n" +
                $"Tel.nr: \n" +
                $"Datum \n" +
                $"Tijd: \n" +
                $"Tafel: \n" +
                $"vooraf besteld: \n" +
                $"Totale Prijs: \n");
            string choice = Resources.inputCheck("Typ 'b' om terug te gaan\n", new string[] {"b", "B"}, maxTries: 1);
            if (choice == "") {
                Overzicht();
            } else {  // gebruiker typte 'b' of 'B' om terug te gaan
                return; // dit moet later worden vervangen door reserveren als de reserveermethod goed werkt
            }
        }
    }
}