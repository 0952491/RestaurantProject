using System;
using System.Linq;
using resourceMethods;


namespace MenuPage
{
    class Menu
    {
        public static void MenuChoices()
        {
            Console.Clear();
            Resources.orderOptions("", new string[] {"Voorgerecht", "Hoofdgerecht", "Dessert"}, true);
            string input = Resources.inputCheck("Kies een nummer: ", new string[] {"1", "2", "3", "b", "B"});
            if (input == "1") { Voorgerecht(); }
            else if (input == "2") { Hoofdgerecht(); }
            else if (input == "3") { Dessert(); }
        }
        public static void Voorgerecht()
        {
            Console.Clear();
            Console.WriteLine("De voorgerecht:");
            Console.WriteLine();
            Console.WriteLine("Tomatensoep (V)                             3");
            Console.WriteLine("Kruidenbouillon                             4");
            Console.WriteLine("Vissoep                                     3.2");
            Console.WriteLine("Wisselsoep (soep van de dag)                3");
            Console.WriteLine("carpaccio van coquille met pastinaak        12");
            string choice = Resources.inputCheck("Typ 'b' om terug te gaan\n", new string[] {"b", "B"}, maxTries: 1);
            if (choice == "") {
                Voorgerecht();
            } else {  // gebruiker typte 'b' of 'B' om terug te gaan
                MenuChoices();
            }
        }
        public static void Hoofdgerecht()
        {
            Console.Clear();
            Console.WriteLine("Hoofdgerecht:");
            Console.WriteLine();
            Console.WriteLine("BAVETTE 200gr Amerikaans Black Angus       19,50");
            Console.WriteLine("PICANHA 250gr Australisch Angus            21.50");
            Console.WriteLine("TOURNEDOS ROSSINI 200gr                    27.50");
            Console.WriteLine("PATATJE STOOFVLEES (V)                     15.00");
            Console.WriteLine("BURGER                                     17.00");
            Console.WriteLine("DORADE                                     12.00");
            Console.WriteLine("CÔTE DE BOEUF                              10.00");
            Console.WriteLine("TRUFFELFRIET  (V)                          15.00");
            Console.WriteLine("STEAK TARTARE                              22.50");
            Console.WriteLine("MEAT CARPACCIO                             11.75");
            Console.WriteLine("VITELLO TONNATO X TONIJN                   9.75");
            Console.WriteLine("CHIOGGIA BIET  (V)                         10.50");
            Console.WriteLine("PROEVERIJ vanaf 2                          27.90");
            string choice = Resources.inputCheck("Typ 'b' om terug te gaan\n", new string[] {"b", "B"}, maxTries: 1);
            if (choice == "") { 
                Hoofdgerecht(); 
            } else {  // gebruiker typte 'b' of 'B' om terug te gaan
                MenuChoices();
            }
        }
        public static void Dessert()
        {
            Console.Clear();
            Console.WriteLine("hello, welcome to Dessert!!!");
            string choice = Resources.inputCheck("Typ 'b' om terug te gaan\n", new string[] {"b", "B"}, "", maxTries: 1);
            if (choice == "") { 
                Dessert(); 
            } else {  // gebruiker typte 'b' of 'B' om terug te gaan
                MenuChoices();
            }
        }
    }
    class Gerecht
    {
        string Naam;
        int Prijs;

        public Gerecht(string naam, int prijs)
        {
            Naam = naam;
            Prijs = prijs
        }
    }
}