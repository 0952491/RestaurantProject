using System;
using resourceMethods;
using LoginPage; // namespace van login.cs
using MenuPage; // namespace van Menu.cs
using TablePage; // namespace van Tafels.cs
using ReserveringPage; // namespace van Reserveren.cs
namespace Main_Restaurant
{
    class Restaurant  // hoofdmenu van de applicatie
    {
        public static MenuKaart Menu = new MenuKaart();
        public static UserAdministration UserAdmin = new UserAdministration();
        public static DinnerRoom dinnerroom = new DinnerRoom(""); // word gemaakt om een plattegrond van de tafels te laten zien
        public static ReserveringsAdministration ReserveerAdmin = new ReserveringsAdministration();
        public static Tuple<string, string, string, string, string> Contact = Tuple.Create("Wijnhaven 107", "3011 WN", "Rotterdam", "info@restaurantTeam4.nl", "0612345678");
        
        /// <summary> START METHOD VAN HET PROGRAMMA!!!! </summary>
        public static void Main() => BeginMenu();

        /// <summary>Het menu dat je te zien krijgt wanneer je de applicatie opstart</summary>
        public static void BeginMenu() {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            while (true) {
                Console.Clear();
                Logo();
                // de optie om beschikbare tafels te bekijken moet weg op een gegeven moment
                string[] userActions = {"Registreren", "Inloggen", "Reserveren", "Menu bekijken", "Contact", "Bekijk beschikbare tafels", "Sluit applicatie"};
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
                    ReserveerAdmin.ReserveringMenu(false, Menu, UserAdmin);
                else if (optie == "4")
                    Menu.ShowGerechten();
                else if (optie == "5")
                    ContactPage();
                else if (optie == "6") {
                    dinnerroom.DrawMap();
                    Resources.EnterMessage();
                }
                else {  // de gebruiker koos de laatste optie dus sluit de applicatie
                    Resources.succesMessage("Dankjewel voor het gebruiken van onze app, tot volgende keer ; )");
                    Console.ReadLine();
                    break;
                }
                Menu.Save();
                UserAdmin.Save();
            }
        }

        /// <summary>Een menu voor wanneer een user is ingelogd</summary>
        public static void LoggedInMenu(User user) {
            while (true) {
                Console.Clear();
                string message;
                if (user.Tussenvoegsel == "")
                    message = $"Ingelogd als {user.Voornaam} {user.Achternaam}";
                else
                    message = $"Ingelogd als {user.Voornaam} {user.Tussenvoegsel} {user.Achternaam}";
                if (user.IsAdmin()) { // geef het menu weer van een admin
                    string[] opties = new string[] {"Reserveringen", "Menu", "Gebruikers", "Uitloggen"};
                    string choice = Resources.makeMenuInput(message, "Kies een van de bovenstaande opties: ", opties);
                    if (choice == "1")
                        ReserveerAdmin.Menu(user, Menu, UserAdmin);
                    else if (choice == "2")
                        Menu.AdminMenu();
                    else if (choice == "3")
                        UserAdmin.AdminMenu(user, ReserveerAdmin);
                    else
                        break;
                }
                else {
                    string[] opties = new string[] {"Reserveringen", "Menu", "Profiel", "Uitloggen"};
                    string choice = Resources.makeMenuInput(message, "Kies een van de bovenstaande opties: ", opties);
                    if (choice == "1")
                        ReserveerAdmin.Menu(user, Menu, UserAdmin);
                    else if (choice == "2")
                        Menu.ShowGerechten();
                    else if (choice == "3")
                        UserAdmin.DefaultMenu(user, ReserveerAdmin);
                    else
                        break;
                }
                Menu.Save();
                UserAdmin.Save();
            }
        }

        /// <summary>Geeft een menu om de contactinfo te veranderen</summary>
        public static void ChangeContacts() { // TODO: deze functie moet nog een plek krijgen voor de admin
            while (true) {
                ContactPage();
                string[] options = new string[] { "Adres", "Postcode", "Plaats", "Telefoonnummer", "Email" };
                string choice = Resources.makeMenuInput("Welke contactinformatie wilt u wijzigen?", "Kies een van de bovenstaande opties: ", options, backbutton: true);
                if (choice == "b")
                    return;
                // TODO: hier moet de code komen met een aantal inputregex functies die checken of de nieuwe input correct is
            }
        }

        /// <summary>Laat het logo zien</summary>
        public static void Logo()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@ (@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&.@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@     @@@@@@@@@@@@@");
            Console.WriteLine("@@@@@       (@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%    %@@@( .@@@@@@@@@");
            Console.WriteLine("@@@@@/         @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@     @@@@    .@@@@@@@@@");
            Console.WriteLine("@@@@@@           (@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%    %@@@(    @@@@*  .@@@@");
            Console.WriteLine("@@@@@@@             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@     @@@@    .@@@@    @@@@@@");
            Console.WriteLine("@@@@@@@@/             (@@@@@@@@@@@@@@@@@@@@@@@@@@#        .    @@@@*   .@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@               @@@@@@@@@@@@@@@@@@@@@@,             &@@@    &@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@               (@@@@@@@@@@@@@@@@@@%                   .@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@                @@@@@@@@@@@@@@@@@                 @@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@@@(               (@@@@@@@@@@@@@.              .@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@/               @@@@@@@@(               &@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@██@@@@@@@@@.             (@@@#        @@@@@@@@@@@@@██@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@██@@@@@@@@@@@@@#.           @@@@   /@@@@@@@@@@@@@@@██@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@██@@@@@@@@@@@@@@@@@@@@@.      /@@@@@@@@@@@@@@@@@@@@██@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@██@@@██@@@@@@@@@@@@@@@@@@@#@@@,       @@@@@@@@@@@@@@@@@@██@@@██@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@█████@@@@@@@@@@@@@@@@@/    /@@@*       /@@@@@@@@@@@@@@@@█████@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@&         ,@@@/        @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@.         ,@@@@@@@(        /@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@(          %@@@@@@@@@@@#         @@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@           @@@@@@@@@@@@@@@@%         /@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@@@@,          *@@@@@@@███████@@@@@@&          @@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@%           &@@@@@@@@@██@@@@@@@@@@@@@&          /@@@@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@            @@@@@@@@@@@@█████@@@@@@@@@@@@@           @@@@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@@@/           (@@@@@@@@@@@@@@██@@@@@@@@@@@@@@@@@@           /@@@@@@@@@@@@");
            Console.WriteLine("@@@@@@@            @@@@@@@@@@@@@@@@@██@@@@@@@@@@@@@@@@@@@@            @@@@@@@@@@");
            Console.WriteLine("@@@@             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@            @@@@@@@@");
            Console.WriteLine("@@@@          #@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@          /@@@@@@@");
            Console.WriteLine("@@@@#       @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@        @@@@@@@@");
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>Geeft de contactgegevens weer van het restaurant</summary>
        public static void ContactPage() {
            Console.Clear();
            Console.WriteLine($"Adres : {Contact.Item1}  {Contact.Item2}");
            Console.WriteLine($"Plaats: {Contact.Item3}");
            Console.WriteLine($"Tel.no: {Contact.Item4}");
            Console.WriteLine($"Email : {Contact.Item5}");
            string choice = Resources.inputCheck("Typ 'b' om terug te gaan\n", new string[] {"b", "B"}, maxTries: 1);
            if (choice == "") {
                ContactPage();
            } else {  // gebruiker typte 'b' of 'B' om terug te gaan
                return;
            }
        }
    }
}