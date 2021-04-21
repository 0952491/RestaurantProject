using System;
using resourceMethods;
using LoginPage; // namespace van login.cs
using MenuPage; // namespace van Menu.cs
using TablePage; // namespace van Tafels.cs
namespace Main_Restaurant
{
    class Main_Menu  // hoofdmenu van de applicatie
    {
        static void Main()
        {
            /*
            VOORBEELD JSON
            Table Tafel1 = new Table(1, 5);
            string output = JsonConvert.SerializeObject(Tafel1);
            Console.WriteLine(output);
            File.WriteAllText("C:/Users/J3roe/Source/Repos/RestaurantProject/NewJson.json", output);
            string json = File.ReadAllText("NewJson.json");
            dynamic Obj = JsonConvert.DeserializeObject(json);
            Console.WriteLine(Obj.Occupied);
            return;*/
            string optie = "";
            while (optie != "quit") {
                Console.Clear();
                // de optie om beschikbare tafels te bekijken moet weg op een gegeven moment
                string[] userActions = {"Registreren", "Inloggen", "Reserveren", "Menu bekijken", "Contact", "Bekijk beschikbare tafels", "Sluit applicatie"};
                Resources.orderOptions("Welkom bij ons restaurant!", userActions);
                optie = Resources.inputCheck("Voert u alstublieft een nummer in: ", Resources.makeRangeArr(1, userActions.Length));
                if (optie == "1") { Login.Registreren(); }
                else if (optie == "2") { Login.Inloggen(); }
                else if (optie == "3") { Login.ReserveerHome(); }
                else if (optie == "4") { Menu.MenuChoices(); }
                else if (optie == "5") { ContactPage(); }
                else if (optie == "6") { TableInit.MainTable(); }
                else {  // de gebruiker koos de laatste optie dus sluit de applicatie
                    Resources.succesMessage("Dankjewel voor het gebruiken van onze app, tot volgende keer ; )");
                    optie = "quit";
                    Console.ReadLine();
                    return;
                }
            }
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