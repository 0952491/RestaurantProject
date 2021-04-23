using System;
using resourceMethods;

namespace TablePage
{
    class TableInit
    {   

        public static void MainTable() {
            DinnerRoom dinnerroom = new DinnerRoom();
            dinnerroom.DrawMap();
            Resources.input("Druk op enter om verder te gaan");
        }
    }


    class DinnerRoom
    {
        public string Tijdvak;
        public string Datum;
        public Table[] VoorTwee; // tafelnr eindigt op A
        public Table[] VoorVier; // eindigt op B
        public Table[] VoorZes; // eindigt op C

        public DinnerRoom()
        {
            Tijdvak = "";
            Datum = "";
            VoorTwee = new Table[8];
            VoorVier = new Table[5];
            VoorZes = new Table[2];
            MakeNew();
        }

        /// <summary>Hulp voor de constructor, maakt een alle Table[] fields van het DinnerRoom Object</summary>
        public void MakeNew()
        {
            for (int i = 0; i < VoorTwee.Length; i++) {
                VoorTwee[i] = new Table(i + 1, 2);
            }
            for (int i = 0; i < VoorVier.Length; i++) { 
                VoorVier[i] = new Table(i + 1, 4);
            }
            for (int i = 0; i < VoorZes.Length; i++) {
                VoorZes[i] = new Table(i + 1, 6);
            }
        }

        /// <summary>Returned een Table Object gebaseerd op het gegeven tafel nummer</summary>
        public Table GetTafel(string tafel_no) {
            Table[] tafelArr = tafel_no.EndsWith("A") ? VoorTwee : tafel_no.EndsWith("B") ? VoorVier : VoorZes;
            Table newTable = null;
            for (int i = 0; i < tafelArr.Length; i++) {
                if (tafelArr[i].table_num == tafel_no)
                    newTable = tafelArr[i];
            }
            return newTable;
        }

        /// <summary>Yield een Table Object voor elke beschikbare tafel</summary>
        public System.Collections.Generic.IEnumerable<Table> AvailableTafels() { 
            foreach (Table tafel in VoorTwee) {
                if (!tafel.occupied)
                    yield return tafel;
            }
            foreach (Table tafel in VoorVier)
            {
                if (!tafel.occupied)
                    yield return tafel;
            }
            foreach (Table tafel in VoorZes)
            {
                if (!tafel.occupied)
                    yield return tafel;
            }
        }

        /// <summary>Returned een array met alle beschikbare tafelnummers</summary>
        public string[] GetBeschikbareTafels() {
            int newlen = 0;
            foreach (Table tafel in AvailableTafels())
                newlen++;
            string[] table_numbers = new string[newlen];
            int index = 0;
            foreach (Table tafel in AvailableTafels())
                table_numbers[index++] = tafel.table_num;
            return table_numbers;
        }

        /// <summary>Print een overzicht van alle beschikbare tafels van de DinnerRoom Object</summary>
        public void DrawMap() {
            // alle tafeltjes voor twee worden hier geprint
            for (int col = 0; col < VoorTwee[0].TableArr().Length; col++) {
                for (int row = 0; row < VoorTwee.Length; row++) {
                    Console.ForegroundColor = VoorTwee[row].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.Write(VoorTwee[row].TableArr()[col]);
                }
                Console.Write("\n");
            }
            Console.WriteLine("");

            // maakt de bovenste helft van de eerste 2 vierpersoonstafels aan de hand van de tweepersoonstafels
            for (int row = 0; row < VoorVier[0].TableArr().Length / 2; row++) {
                for (int col = 0, index = 1; col < VoorVier.Length + 1; col++) {
                    if (col == 2 || col == 5) {

                        Console.ForegroundColor = VoorVier[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        Console.Write(VoorVier[index].TableArr()[row]);
                        index += 2;
                    } else {
                        string fillerLine = Resources.drawString(14, " ");
                        Console.Write(fillerLine);
                    }
                }
                Console.Write("\n");
            }

            // Maakt de bovenste helft van de overgebleven vierpersoonstafels en de onderste helft van voorgaande tafels
            for (int row = 0; row < VoorVier[0].TableArr().Length / 2; row++){
                for (int col = 1, index = 0; col < 12; col++){
                    if (col % 2 != 0){
                        Console.Write(Resources.drawString(7, " "));
                    } else if (col % 4 == 0) {
                        Console.ForegroundColor = VoorVier[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        Console.Write(VoorVier[index++].TableArr()[row + VoorVier[0].TableArr().Length / 2]);
                    } else {
                        Console.ForegroundColor = VoorVier[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        Console.Write(VoorVier[index++].TableArr()[row]);
                    }
                }
                Console.Write("\n");
            }

            // Maakt de onderste helft van de laatste vierpersoonstafels en de bovenste helft van de 2 zespersoonstafels
            for (int row = 0; row < VoorVier[0].TableArr().Length / 2; row++) {
                for (int col = 1, indexT2 = 0, indexT3 = 0; col < 12; col++) {
                    if (col % 2 != 0) {
                        Console.Write(Resources.drawString(7, " "));
                    } else if (col % 4 == 0) {
                        Console.ForegroundColor = VoorZes[indexT3].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        string line = row != 0 && row != 1 ? VoorZes[indexT3].TableArr()[row - 2] : Resources.drawString(14, " ");
                        Console.Write(line); indexT3++;
                    } else if (col % 2 == 0) {
                        Console.ForegroundColor = VoorVier[indexT2].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        Console.Write(VoorVier[indexT2].TableArr()[row + VoorVier[0].TableArr().Length / 2]);
                        indexT2 += 2;
                    }
                }
                Console.Write("\n");
            }

            // Maakt het laatste deel van de zespersonentafels
            for (int row = 1; row < VoorZes[0].TableArr().Length; row++) {
                for (int col = 1, index = 0; col < 6; col++) {
                    if (col % 2 != 0) {
                        Console.Write(Resources.drawString(28, " "));
                    } else {
                        Console.ForegroundColor = VoorZes[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        Console.Write(VoorZes[index++].TableArr()[row]);
                    }
                }
                Console.Write("\n");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        // TODO: Maak een method die met 1 string parameter (het tafelnummer) die het gegeven tafelnr op occupied zet (tip: gebruik de GetTafel method) 
    }
        
   class Table 
   {
        public string table_num;  // het tafelnummer
        public int seats;  // het aantal stoelen van de tafel
        public bool occupied;  // een bool die noteert of de tafel bezet is of niet.
       
        // de Table constructor
        public Table(int t_num, int chairs) {
            seats = chairs;
            table_num = t_num.ToString();
            table_num += chairs == 2 ? "A" : chairs == 4 ? "B" : "C";
            occupied = false;
        }

        /// <summary>Maakt een string van de tafel</summary>
        public string[] TableArr() {
            string n = table_num;
            string[] tafel;
            if (seats == 2) {
                tafel = new string[] {
                    "    ______    ",
                    "   |      |   ",
                   $" O |  {n}  | O ",
                    "   |______|   "};
            }
            else if (seats == 4) {
                tafel = new string[] {
                     "    ______    ",
                     "   |      |   ",
                     " O |      | O ",
                    $"   |  {n}  |   ",
                     " O |      | O ",
                     "   |______|   "};
            }
            else
            {
                tafel = new string[] {
                      " __O___O___O_ " ,
                      "|            |" ,
                     $"|     {n}     |" ,
                      "|____________|" ,
                      "   O   O   O  " };   
            }
            return tafel;
        }
    }
}
