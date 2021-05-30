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
        public static Tuple<string, string, string, string, string> Contact = Tuple.Create("Wijnhaven 107", "3011 WN", "Rotterdam", "0612345678", "info@restaurantTeam4.nl");
        // TODO: de Contact tuple moet nog worden opgeslagen en geladen vanuit json, nu slaat de app de veranderingen niet op en start deze elke keer met dezelfde contactgegevens
        
        /// <summary>START METHOD VAN HET PROGRAMMA!!!!</summary>
        public static void Main() => BeginMenu();

        /// <summary>Het menu dat je te zien krijgt wanneer je de applicatie opstart</summary>
        public static void BeginMenu() {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            while (true) {
                Console.Clear();
                Logo();
                // de optie om beschikbare tafels te bekijken moet weg op een gegeven moment
                string[] userActions = {"Registreren", "Inloggen", "Reserveren", "Menu bekijken", "Contact", "Bekijk plattegrond", "Sluit applicatie"};
                string optie = Resources.makeMenuInput("Welkom bij ons restaurant!", "Voer hier een van de bovenstaande opties in: ", userActions);
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
                string message = "Ingelogd als ";
                if (user.IsAdmin())
                    message += "Admin: ";
                if (user.Tussenvoegsel == "")
                    message += $"{user.Voornaam} {user.Achternaam}";
                else
                    message += $"{user.Voornaam} {user.Tussenvoegsel} {user.Achternaam}";
                if (user.IsAdmin()) { // geef het menu weer van een admin
                    string[] opties = new string[] {"Reserveringen", "Menu", "Gebruikers", "Verander contactgegevens van het restaurant", "Uitloggen"};
                    string choice = Resources.makeMenuInput(message, "Voer hier een van de bovenstaande opties in: ", opties);
                    if (choice == "1")
                        ReserveerAdmin.Menu(user, Menu, UserAdmin);
                    else if (choice == "2")
                        Menu.AdminMenu();
                    else if (choice == "3")
                        UserAdmin.AdminMenu(user, ReserveerAdmin);
                    else if (choice == "4")
                        ChangeContacts();
                    else
                        break;
                }
                else {
                    string[] opties = new string[] {"Reserveringen", "Menu", "Profiel", "Uitloggen"};
                    string choice = Resources.makeMenuInput(message, "Voer hier een van de bovenstaande opties in: ", opties);
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
        public static void ChangeContacts() {
            string tempAddr = Contact.Item1;
            string tempZipCode = Contact.Item2;
            string tempPlace = Contact.Item3;
            string tempPhone = Contact.Item4;
            string tempEmail = Contact.Item5;
            while (true) {
                ContactPage(true);
                string[] options = new string[] { "Adres", "Postcode", "Plaats", "Telefoonnummer", "Email" };
                string choice = Resources.makeMenuInput("Welke contactinformatie wilt u wijzigen?", "Kies een van de bovenstaande opties: ", options, backbutton: true);
                Console.Clear();
                if (choice == "b")
                    return;
                else if (choice == "1") {  // wijzig adres
                    Console.WriteLine($"Vorige adres: {tempAddr}");
                    tempAddr = Resources.InputRegex("Voer hier het nieuwe adres in: ", @"^[A-Za-z0-9'\.\-\s\,]+$");
                }
                else if (choice == "2") {
                    Console.WriteLine($"Vorige postcode: {tempZipCode}");
                    tempZipCode = Resources.InputRegex("Voer hier de nieuwe postcode in: ", @"^\d{4}[A-Z][A-Z]$");
                }
                else if (choice == "3") {
                    Console.WriteLine($"Vorige plaats: {tempPlace}");
                    tempPlace = Resources.input("Voer hier de nieuwe plaats in: ");
                }
                else if (choice == "4") {
                    Console.WriteLine($"Vorige telefoonnummer: {tempPhone}");
                    tempPhone = Resources.InputRegex("Voer hier het nieuwe telefoonnummer in: ", @"^\d{10}$");
                }
                else {
                    Console.WriteLine($"Vorige email: {tempEmail}");
                    tempEmail = Resources.InputRegex("Voer hier de nieuwe email in: ", @"^(\w|\.)+@\w+\.\w{2,3}$");
                }
            }
            Contact = Tuple.Create(tempAddr, tempZipCode, tempPlace, tempPhone, tempEmail);
        }

        /// <summary>Laat het logo zien</summary>
        public static void Logo()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
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
        public static void ContactPage(bool ForAdmin=false) {
            Console.Clear();
            Console.WriteLine($"Adres : {Contact.Item1}  {Contact.Item2}");
            Console.WriteLine($"Plaats: {Contact.Item3}");
            Console.WriteLine($"Tel.no: {Contact.Item4}");
            Console.WriteLine($"Email : {Contact.Item5}");
            string choice;
            if (ForAdmin)
                choice = "b";
            else
                choice = Resources.inputCheck("Typ 'b' om terug te gaan\n", new string[] {"b", "B"}, maxTries: 1);
            if (choice == "") {
                ContactPage();
            } else {  // gebruiker typte 'b' of 'B' om terug te gaan
                return;
            }
        }
    }
}