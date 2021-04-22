using System;
using resourceMethods;

namespace TablePage
{
    class TableInit
    {   
        ///<summary>Vult de references van de jagged arrays met string arrays</summary>
       /*private static Func<Table[], string[][]> createJagged = t =>
        {
            string[][] jagged = new string[t.Length][];
            for (int i = 0; i < jagged.Length; i++) {
                jagged[i] = t[i].DrawTable();
            }
            return jagged;
        };

        ///<summary>Maakt een plattegrond van de gegeven tafelarrays</summary>
        public static void drawMap(Table[] t1, Table[] t2, Table[] t3, bool colored=true) {
            string[][] t1Tables = createJagged(t1);
            string[][] t2Tables = createJagged(t2);
            string[][] t3Tables = createJagged(t3);
            // de eerste 8 tweepersoonstafels
            for (int row = 0; row < t1Tables[0].Length; row++) {
                for (int col = 0; col < t1.Length; col++) {
                    if (colored) {
                        Console.ForegroundColor = t1[col].occupied ? ConsoleColor.Red : ConsoleColor.Green; 
                    }
                    Console.Write(t1Tables[col][row]);
                }
                Console.Write("\n");
            }

            Console.Write("\n");  // een klein beetje extra ruimte voor de tweepersoonstafels

            // maakt de bovenste helft van de eerste 2 vierpersoonstafels aan de hand van de tweepersoonstafels
            for (int row = 0; row < t2Tables[0].Length / 2; row++) {
                for (int col = 1, index = 1; col < t1.Length + 1; col++) {
                    if (col % 3 == 0) {
                        if (colored) {
                            Console.ForegroundColor = t2[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        Console.Write(t2Tables[index][row]);
                        index += 2;
                    } else {
                        string fillerLine = Resources.drawString(14, " ");
                        Console.Write(fillerLine);
                    }
                }
                Console.Write("\n");
            }

            // Maakt de bovenste helft van de overgebleven vierpersoonstafels en de onderste helft van voorgaande tafels
            for (int row = 0; row < t2Tables[0].Length / 2; row++) {
                for (int col = 1, index = 0; col < 12; col++) {
                    if (col % 2 != 0) {
                        Console.Write(Resources.drawString(7, " "));
                    } else if (col % 4 == 0) {
                        if (colored) {
                        Console.ForegroundColor = t2[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        Console.Write(t2Tables[index++][row + t2Tables[0].Length / 2]);
                    } else {
                        if (colored) {
                            Console.ForegroundColor = t2[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        Console.Write(t2Tables[index++][row]);
                    }
                }
                Console.Write("\n");
            }

            // Maakt de onderste helft van de laatste vierpersoonstafels en de bovenste helft van de 2 zespersoonstafels
            for (int row = 0; row < t2Tables[0].Length / 2; row++) {
                for (int col = 1, indexT2 = 0, indexT3 = 0; col < 12; col++) {
                    if (col % 2 != 0) {
                        Console.Write(Resources.drawString(7, " "));
                    } else if (col % 4 == 0) {
                        if (colored) {
                            Console.ForegroundColor = t3[indexT3].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        string line = row != 0 && row != 1 ? t3Tables[indexT3][row - 2] : Resources.drawString(14, " ");
                        Console.Write(line); indexT3++;
                    } else if (col % 2 == 0){
                        if (colored) {
                            Console.ForegroundColor = t2[indexT2].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        Console.Write(t2Tables[indexT2][row + t2Tables[0].Length / 2]);
                        indexT2 += 2;
                    }
                }
                Console.Write("\n");
            }

            // Maakt het laatste deel van de zespersonentafels
            for (int row = 1; row < t3Tables[0].Length; row++) {
                for (int col = 1, index = 0; col < 6; col++) {
                    if (col % 2 != 0) {
                        Console.Write(Resources.drawString(28, " "));
                    } else {
                        if (colored) {
                            Console.ForegroundColor = t3[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        Console.Write(t3Tables[index++][row]);
                    }
                }
                Console.Write("\n");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }    

        
        ///<summary>Maakt een string array van alle beschikbare tafelnummers</summary>
        private static string[] getAvailableTables(Table[][] tableRoom) {
            int totalAvailable = 0;
            foreach (Table[] tablegroup in tableRoom) {
                foreach (Table table in tablegroup) {
                    if (!table.occupied) { totalAvailable++; }
                }
            }
            string[] allAvailables = new string[totalAvailable];
            int index = 0;
            foreach (Table[] tablegroup in tableRoom) {
                foreach (Table table in tablegroup) {
                    if (!table.occupied) { allAvailables[index++] = table.table_num; }
                }
            }
            return allAvailables;
        }


        ///<summary>Zorgt ervoor dat in een van de gegeven Table objecten een tafel bezet wordt</summary>
        private static void occupyTable(string tabletable_num, Table[][] tableRoom) {
            foreach (Table[] tableGroup in tableRoom) {
                foreach(Table table in tableGroup) {
                    if (table.table_num == tabletable_num) {
                        table.occupied = true;
                        return;
                    }
                }
            }
        }*/

        public static void MainTable() {
            DinnerRoom dinnerroom = new DinnerRoom();
            Console.WriteLine(dinnerroom.GetBeschikbareTafels());
            dinnerroom.DrawMap();
            Resources.input("Druk op enter om verder te gaan");
        }
    }


    class DinnerRoom
    {
        public string Tijdvak;
        public string Datum;
        public Table[] VoorTwee; // tafelnr eindigt op 
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

        public Table GetTafel(string tafel_no) {
            Table[] tafelArr = tafel_no.EndsWith("A") ? VoorTwee : tafel_no.EndsWith("B") ? VoorVier : VoorZes;
            Table newTable = null;
            for (int i = 0; i < tafelArr.Length; i++) {
                if (tafelArr[i].table_num == tafel_no)
                    newTable = tafelArr[i];
            }
            return newTable;
        }


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

        /*
         * for (int row = 0; row < t1Tables[0].Length; row++) {
                for (int col = 0; col < t1.Length; col++) {
                    if (colored) {
                        Console.ForegroundColor = t1[col].occupied ? ConsoleColor.Red : ConsoleColor.Green; 
                    }
                    Console.Write(t1Tables[col][row]);
                }
                Console.Write("\n");
            }
        // maakt de bovenste helft van de eerste 2 vierpersoonstafels aan de hand van de tweepersoonstafels
            for (int row = 0; row < t2Tables[0].Length / 2; row++) {
                for (int col = 1, index = 1; col < t1.Length + 1; col++) {
                    if (col % 3 == 0) {
                        if (colored) {
                            Console.ForegroundColor = t2[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        Console.Write(t2Tables[index][row]);
                        index += 2;
                    } else {
                        string fillerLine = Resources.drawString(14, " ");
                        Console.Write(fillerLine);
                    }
                }
                Console.Write("\n");
            }

            // Maakt de bovenste helft van de overgebleven vierpersoonstafels en de onderste helft van voorgaande tafels
            for (int row = 0; row < t2Tables[0].Length / 2; row++) {
                for (int col = 1, index = 0; col < 12; col++) {
                    if (col % 2 != 0) {
                        Console.Write(Resources.drawString(7, " "));
                    } else if (col % 4 == 0) {
                        if (colored) {
                        Console.ForegroundColor = t2[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        Console.Write(t2Tables[index++][row + t2Tables[0].Length / 2]);
                    } else {
                        if (colored) {
                            Console.ForegroundColor = t2[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        Console.Write(t2Tables[index++][row]);
                    }
                }
                Console.Write("\n");
            }*/

        public void DrawMap() {
            //t1Tables == VoorTwee
            //t2Tables == VoorVier
            //t3Tables == VoorZes
            // alle tafeltjes voor twee worden hier geprint
            for (int col = 0; col < VoorTwee[0].TableArr().Length; col++)
            {
                for (int row = 0; row < VoorTwee.Length; row++)
                {
                    Console.ForegroundColor = VoorTwee[row].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.Write(VoorTwee[row].TableArr()[col]);
                }
                Console.Write("\n");
            }
            Console.WriteLine("");

            // maakt de bovenste helft van de eerste 2 vierpersoonstafels aan de hand van de tweepersoonstafels
            for (int row = 0; row < t2Tables[0].Length / 2; row++)
            {
                for (int col = 1, index = 1; col < t1.Length + 1; col++)
                {
                    if (col % 3 == 0)
                    {

                        Console.ForegroundColor = t2[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        Console.Write(t2Tables[index][row]);
                        index += 2;
                    }
                    else
                    {
                        string fillerLine = Resources.drawString(14, " ");
                        Console.Write(fillerLine);
                    }
                }
                Console.Write("\n");
            }
        }
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

        ///<summary>Maakt een string van de tafel</string>
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
