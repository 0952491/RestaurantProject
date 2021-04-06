using System;
using resourceMethods;

namespace TablePage
{
    public class TableInit
    {   
        ///<summary>Maakt nieuwe Table arrays afhankelijk van het aantal stoelen per tafel</summary>
        private static Table[] createTables(int howManyTables, int seatsPerTable, int tableHeight, int tableWidth=6) {
            Table[] tableArr = new Table[howManyTables];
            for (int i = 0; i < howManyTables; i++) {
                tableArr[i] = new Table(i + 1, seatsPerTable, tableHeight, tableWidth);
            }
            return tableArr;
        }

        ///<summary>Vult de references van de jagged arrays met string arrays</summary>
       private static Func<Table[], string[][]> createJagged =  t =>
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
            Table[] forTwo = createTables(8, 2, 3);
            Table[] forFour = createTables(5, 4, 5);
            Table[] forSix = createTables(2, 6, 5, 12);
            Table[][] dinnerRoom = new Table[][] {forTwo, forFour, forSix};
            
            drawMap(forTwo, forFour, forSix, reserveren);
            if (reserveren) {
                string message = "Typt u alstublieft het tafelnummer waar u voor wilt reserveren: ";
                string errormessage = "Helaas is die optie niet beschikbaar, Kiest u alstublieft uit de groen gekleurde tafels.";
                string chose_num = Resources.inputCheck(message, getAvailableTables(dinnerRoom), errormessage);
                occupyTable(chose_num, dinnerRoom);
                // hier moet alle info over bezette tafels naar json worden overgezet
                Console.Clear();
                Console.WriteLine("Nieuwe Beschikbare Tafels");
                drawMap(forTwo, forFour, forSix);
            }
            Resources.input("druk op enter om verder te gaan\n");
            return; // ga terug naar de parent class
        }
    }
    

    public class Table 
    {
        private string table_num;  // het tafelnummer
        private int seats;  // het aantal stoelen van de tafel
        private int width;  // de breedte van de tafel
        private int height;  // de hoogte van de tafel
        private bool occupied;  // een bool die noteert of de tafel bezet is of niet.
       
        // de Table constructor
        public Table(int t_num, int chairs, int newHeight=3, int newWidth=6) {
            seats = chairs;
            width = newWidth;
            Height = newHeight;
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

        // getter en setter method voor height attribute
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        // getter en setter method voor het tafelnummer
        public string Number
        {
            get { return table_num; }
            set { table_num = value; }
        }

        ///<summary>Maakt een string van de tafel</string>
        public string[] DrawTable() {
            if (seats == 6) { // de tafel voor 6 personen ligt op zijn kant dus heeft een ander uiterlijk
                string line0 = " " + Resources.drawString(2, "_") + Resources.drawString(2, "O___") + "O_ ";
                string line1 = "|" + Resources.drawString(12, " ") + "|";
                string line2 = "|" + Resources.drawString(5, " ") + table_num + Resources.drawString(5, " ") + "|";
                string line3 = "|" + Resources.drawString(12, "_") + "|";
                string line4 = Resources.drawString(3, "   O") + "  ";
                return new string[] {line0, line1, line2, line3, line4};
                }
            int currentIndex = 0;
            string[] tableStr = new string[height + 1];
            tableStr[currentIndex] += Resources.drawString(4, " ");
            string widthSide = Resources.drawString(width, "_");
            string chair = " O ";
            tableStr[currentIndex++] += widthSide + Resources.drawString(4, " ");  // na deze line is de eerste kant van de tafel af.
            
            // Maakt zijdes totdat de line met het tafelnummer in de string moet.
            string vertical = Resources.drawString(3, " ") + "|" + Resources.drawString(width, " ") + "|" + Resources.drawString(3, " ");
            string verticalSeats = chair + "|" + Resources.drawString(width, " ") + "|" + chair;
            for (int i = 0; i < height / 2; i++) {
                if (i % 2 == 0) {
                    tableStr[currentIndex++] += vertical;
                } else {
                    tableStr[currentIndex++] += verticalSeats;
                }
            }
            // de line met het tafelnummer (heeft ook stoelen als het aantal stoelen geen 4 is)
            if (seats == 4) {
                tableStr[currentIndex++] += Resources.drawString(3, " ") + "|" + Resources.drawString(width / 2 - 1, " ") + table_num +  Resources.drawString(width / 2 - 1, " ") + "|" + Resources.drawString(3, " ");
            } else {
                tableStr[currentIndex++] += " O " + "|" + Resources.drawString(width / 2 - 1, " ") + table_num +  Resources.drawString(width / 2 - 1, " ") + "|" + " O ";
            }
            // begin de loop na de line met het tafelnummer (word geskipped als dit de laatste line van de tafel is)
            for (int i = height / 2 + 1; i < height - 1; i++) {
                if (i % 2 == 0)
                    tableStr[currentIndex++] += vertical;
                else
                    tableStr[currentIndex++] += verticalSeats;
            }
            // de laatste line van de tafel
            tableStr[currentIndex++] += Resources.drawString(3, " ") + "|" + widthSide + "|" + Resources.drawString(3, " ");
            return tableStr;
        }
    }
}
