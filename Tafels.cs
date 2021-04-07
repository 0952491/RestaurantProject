using System;
using resourceMethods;

namespace TablePage
{
    public class TableInit
    {   
        ///<summary>Maakt nieuwe Table arrays afhankelijk van het aantal stoelen per tafel</summary>
        private static Table[] createTables(int howManyTables, int seatsPerTable) {
            Table[] tableArr = new Table[howManyTables];
            for (int i = 0; i < howManyTables; i++) {
                tableArr[i] = new Table(i + 1, seatsPerTable);
            }
            return tableArr;
        }

        ///<summary>Vult de references van de jagged arrays met string arrays</summary>
       private static Func<Table[], string[][]> createJagged = t =>
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
                        Console.ForegroundColor = t1[col].Occupied ? ConsoleColor.Red : ConsoleColor.Green; 
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
                            Console.ForegroundColor = t2[index].Occupied ? ConsoleColor.Red : ConsoleColor.Green;
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
                        Console.ForegroundColor = t2[index].Occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        Console.Write(t2Tables[index++][row + t2Tables[0].Length / 2]);
                    } else {
                        if (colored) {
                            Console.ForegroundColor = t2[index].Occupied ? ConsoleColor.Red : ConsoleColor.Green;
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
                            Console.ForegroundColor = t3[indexT3].Occupied ? ConsoleColor.Red : ConsoleColor.Green;
                        }
                        string line = row != 0 && row != 1 ? t3Tables[indexT3][row - 2] : Resources.drawString(14, " ");
                        Console.Write(line); indexT3++;
                    } else if (col % 2 == 0){
                        if (colored) {
                            Console.ForegroundColor = t2[indexT2].Occupied ? ConsoleColor.Red : ConsoleColor.Green;
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
                            Console.ForegroundColor = t3[index].Occupied ? ConsoleColor.Red : ConsoleColor.Green;
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
                    if (!table.Occupied) { totalAvailable++; }
                }
            }
            string[] allAvailables = new string[totalAvailable];
            int index = 0;
            foreach (Table[] tablegroup in tableRoom) {
                foreach (Table table in tablegroup) {
                    if (!table.Occupied) { allAvailables[index++] = table.Number; }
                }
            }
            return allAvailables;
        }


        ///<summary>Zorgt ervoor dat in een van de gegeven Table objecten een tafel bezet wordt</summary>
        private static void occupyTable(string tableNumber, Table[][] tableRoom) {
            foreach (Table[] tableGroup in tableRoom) {
                foreach(Table table in tableGroup) {
                    if (table.Number == tableNumber) {
                        table.Occupied = true;
                        return;
                    }
                }
            }
        }
       
        /////////////////////////////
        ///     MAIN FUNCTION     ///
        /////////////////////////////
        public static void MainTable(bool reserveren=false) {  // als de parameter false is dan zie je alleen de plattegrond zonder een tafel te kiezen
            Console.Clear();
            Table[] forTwo = createTables(8, 2);
            Table[] forFour = createTables(5, 4);
            Table[] forSix = createTables(2, 6);
            Table[][] dinnerRoom = new Table[][] {forTwo, forFour, forSix};
            
            if (reserveren) {
                string message = "Typt u alstublieft het tafelnummer waar u voor wilt reserveren: ";
                string errormessage = "Helaas is die optie niet beschikbaar, Kiest u alstublieft uit de groen gekleurde tafels.";
                string chose_num = Resources.inputCheck(message, getAvailableTables(dinnerRoom), errormessage);
                occupyTable(chose_num, dinnerRoom);
                // hier moet alle info over bezette tafels naar json worden overgezet
                Console.Clear();
                Console.WriteLine("Nieuwe Beschikbare Tafels");
                drawMap(forTwo, forFour, forSix);
            } else
            {
                drawMap(forTwo, forFour, forSix, reserveren);
            }
            Resources.input("druk op enter om verder te gaan\n");
            return; // ga terug naar de parent class
        }
    }
    
    public class Table 
    {
        private string table_num;  // het tafelnummer
        private int seats;  // het aantal stoelen van de tafel
        private bool occupied;  // een bool die noteert of de tafel bezet is of niet.
       
        // de Table constructor
        public Table(int t_num, int chairs) {
            Seats = chairs;
            Number = t_num.ToString();
            Number += chairs == 2 ? "A" : chairs == 4 ? "B" : "C";
            Occupied = false;
        }

        // getter en setter method voor occupied attribute
        public bool Occupied 
        {
            get { return occupied;}
            set { occupied = value; }
        }

        // getter en setter method voor het tafelnummer
        public string Number
        {
            get { return table_num; }
            set { table_num = value; }
        }

        // getter and setter method voor seats attribuut
        public int Seats
        {
            get { return seats;  }
            set { seats = value == 2 ? 2 : value == 4 ? 4 : value == 6 ? 6 : 2; }
        }

        ///<summary>Maakt een string van de tafel</string>
        public string[] DrawTable() {
            string[] tableString;
            if (Seats == 6)
            { // de tafel voor 6 personen
                tableString = new string[5];
                tableString[0] = " " + Resources.drawString(2, "_") + Resources.drawString(2, "O___") + "O_ ";
                tableString[1] = "|" + Resources.drawString(12, " ") + "|";
                tableString[2] = "|" + Resources.drawString(5, " ") + Number + Resources.drawString(5, " ") + "|";
                tableString[3] = "|" + Resources.drawString(12, "_") + "|";
                tableString[4] = Resources.drawString(3, "   O") + "  ";
            } else
            { // de tafel is voor 2 of 4 personen
                string tableEnd = Resources.drawString(6, "_");
                string tableMiddle = "|" + Resources.drawString(6, " ") + "|";
                string chairSide = " O ";
                string tableSide = Resources.drawString(3, " ");
                string firstLine = Resources.drawString(4, " ") + tableEnd + Resources.drawString(4, " ");
                string lastLine = tableSide + "|" + tableEnd + "|" + tableSide;
                if (Seats == 4)
                {
                    tableString = new string[6];
                    tableString[0] = firstLine;
                    tableString[1] = tableSide + tableMiddle + tableSide;
                    tableString[2] = chairSide + tableMiddle + chairSide;
                    tableString[3] = tableSide + $"|  {table_num}  |" + tableSide;
                    tableString[4] = tableString[2];
                    tableString[5] = lastLine;
                } else
                {
                    tableString = new string[4];
                    tableString[0] = firstLine;
                    tableString[1] = tableSide + tableMiddle + tableSide;
                    tableString[2] = chairSide + $"|  {table_num}  |" + chairSide;
                    tableString[3] = tableSide + "|" + tableEnd + "|" + tableSide;
                }                
            }
            return tableString;
        }
    }
}
