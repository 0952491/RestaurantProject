using System;
using System.Text;
using resourceMethods;


namespace MenuPage
{
    class Gerecht
    {
        public string Naam;
        public double Prijs;

        public Gerecht(string naam, double prijs)
        {
            Naam = naam;
            Prijs = prijs;
        }

        /// <summary>Met deze method kan je de fields/attributes veranderen van je gerecht</summary>
        public void ChangeGerecht() {
            string choice = Resources.makeMenuInput("Verander je gerecht", "Kies een van de bovenstaande opties: ", new string[] { "Naam", "Prijs" });
            if (choice == "1") {
                Console.WriteLine($"Oude naam: {Naam}");
                Naam = Resources.input("Geef de nieuwe naam door van het gerecht: ");
            } else { 
                Console.WriteLine($"Oude prijs: {Prijs}");
                Prijs = Convert.ToDouble(Resources.inputRegex("Geef de nieuwe prijs door van het gerecht: ", @"^\d+$"));
            }
        }
    }

    class MenuKaart {
        public Gerecht[] Voorgerechten;
        public Gerecht[] Hoofdgerechten;
        public Gerecht[] Desserts;
        private const string FILENAME = "Menu.json";

        public MenuKaart() {
            Voorgerechten = LoadGerechten(DataHandler.loadJson(FILENAME).Voorgerechten);
            Hoofdgerechten = LoadGerechten(DataHandler.loadJson(FILENAME).Hoofdgerechten);
            Desserts = LoadGerechten(DataHandler.loadJson(FILENAME).Desserts);
        }

        /// <summary>Een method voor het weergeven van alle opties die de admin heeft voor het menu</summary>
        public void AdminMenu()
        {
            while (true) {
                Console.Clear();
                string input = Resources.makeMenuInput("", "Kies een van de bovenstaande opties: ", new string[] { "Zie gerechten", "Voeg een gerecht toe", "Pas een gerecht aan", "Sla gerechten op" }, backbutton: true);
                if (input == "1") { // zie gerechten
                    ShowGerechten();
                    Resources.input("Klik op enter om verder te gaan");
                }
                else if (input == "2") { // voeg een gerecht toe
                    AddGerecht(MakeGerecht());
                }
                else if (input == "3") { // pas een gerecht aan
                    string[] alle_namen = GetNames();
                    string num = Resources.makeMenuInput("Beschikbare Gerechten", "Kies een van de bovenstaande gerechten: ", alle_namen);
                    string naam = alle_namen[Convert.ToInt32(num) - 1];
                    GetGerecht(naam).ChangeGerecht();
                }
                else if (input == "4") { // sla een gerecht op
                    Save();
                } else {
                    break;
                }
            }
        }

        /// <summary>slaat de MenuKaart op in "Menu.json" file in Data folder</summary>
        public void Save() {
            DataHandler.writeJson(FILENAME, this);
        }

        /// <summary>Laad alle gegeven gerechten naar een Gerecht[] array</summary>
        public Gerecht[] LoadGerechten(dynamic gerechten) {
            return gerechten.ToObject<Gerecht[]>();
        }

        /// <summary>Print een geordend menu uit van een array van specifieke gerechten</summary>
        public void ShowGerechten() {
            while (true) {
                Console.OutputEncoding = Encoding.UTF8;
                Gerecht[] gerechtArr = GetCategorie();
                if (gerechtArr == null)
                    break;
                Console.Clear();
                foreach (Gerecht g in gerechtArr) {
                    string display = $"{g.Naam}{Resources.drawString(50 - g.Naam.Length, " ")}�{g.Prijs}";
                    Console.WriteLine(display);
                }
                Resources.input("Typ enter om terug te gaan");
            }
        }

        /// <summary>Maakt een gerecht object</summary>
        public Gerecht MakeGerecht() {
            string naam = Resources.input("Geef de naam door van het gerecht: ");
            int prijs = Convert.ToInt32(Resources.inputRegex("Geef de prijs door van het gerecht: ", @"^\d+$")); ;
            return new Gerecht(naam, prijs);
        }

        /// <summary>Returned een string array met alle namen van de gekozen categorie gerechten</summary>
        public string[] GetNames() {
            Gerecht[] gerechten = GetCategorie();
            string[] namen = new string[gerechten.Length];
            for (int i = 0; i < namen.Length; i++)
                namen[i] = gerechten[i].Naam;
            return namen;
        }

        /// <summary>Loopt over alle gerechten van de categorie arrays</summary>
        private Gerecht loopGerechten(string naam, Gerecht[] gerechtArr) {
            foreach (Gerecht g in gerechtArr)
            {
                if (g.Naam == naam)
                    return g;
            }
            return null;
        }

        public Gerecht GetGerecht(string naam) {
            Gerecht voor = loopGerechten(naam, Voorgerechten);
            Gerecht hoofd = loopGerechten(naam, Hoofdgerechten);
            Gerecht dessert = loopGerechten(naam, Desserts);
            return voor != null ? voor : hoofd != null ? hoofd : dessert;
        }

        /// <summary>Returned een van de Gerecht[] categorieen gebaseerd op welke keuze er gemaakt word</summary>
        public Gerecht[] GetCategorie() {
            Console.Clear();
            string choice = Resources.makeMenuInput($"Kies een categorie", "Kies een van de bovenstaande opties: ", new string[] { "Voorgerechten", "Hoofdgerechten", "Desserts" }, backbutton: true);
            return choice == "1" ? Voorgerechten : choice == "2" ? Hoofdgerechten : choice == "3" ? Desserts : null;
        }

        /// <summary>Voegt een nieuw gerecht toe aan een bepaalde Menu array</summary>
        public void AddGerecht(Gerecht gerecht)
        {
            Gerecht[] gerechtArr = GetCategorie();
            Gerecht[] nieuwArr = new Gerecht[gerechtArr.Length + 1];
            for (int i = 0; i < gerechtArr.Length; i++)
                nieuwArr[i] = gerechtArr[i];
            nieuwArr[gerechtArr.Length] = gerecht;
            if (gerechtArr == Voorgerechten)
                Voorgerechten = nieuwArr;
            else if (gerechtArr == Hoofdgerechten)
                Hoofdgerechten = nieuwArr;
            else
                Desserts = nieuwArr;
        }

        /// <summary>Verwijdert een gerecht uit een bepaalde Menu array</summary>
        public void RemoveGerecht(Gerecht gerecht) {
            Gerecht[] nieuwArr = new Gerecht[Hoofdgerechten.Length - 1];
            for (int i = 0, j = 0; i < nieuwArr.Length; i++)
                nieuwArr[i] = Hoofdgerechten[j] == gerecht ? Hoofdgerechten[j+=2] : Hoofdgerechten[j++];
            Hoofdgerechten = nieuwArr;
        }
    }
}