using System;
using Newtonsoft.Json;
using resourceMethods;

namespace TablePage
{
    class Week {
        public Day[] reserveringsWeek = new Day[9]; // een array van days die bestaat uit de komende 7 dagen, vandaag en gister
        public const string FILENAME = "Week.json";

        public Week() {
            if (!DataHandler.FileExists(FILENAME))
                MakeDays();
            else {
                reserveringsWeek = LoadWeek(DataHandler.LoadJson(FILENAME).reserveringsWeek);
            }
        }
        public Week(Day[] days) => reserveringsWeek = days;

        /// <summary>Vormt een json object naar een Day[] Array</summary>
        public Day[] LoadWeek(dynamic Dagen) => Dagen.ToObject<Day[]>();

        /// <summary>slaat de week op in "Week.json" file in Data folder</summary>
        public void Save() => DataHandler.WriteJson(FILENAME, this);

        /// <summary>Maakt alle dagen van de reserveringsWeek array, alle dagen in deze array zijn nieuw en hebben nog geen initiele values</summary>
        public void MakeDays() {  
            TimeSpan oneday = new TimeSpan(1, 0, 0, 0); // represents one day
            DateTime today = DateTime.Now;
            DateTime yesterday = today.Subtract(oneday);
            reserveringsWeek[0] = new Day(yesterday);
            reserveringsWeek[1] = new Day(today);
            for (int i = 0; i < 7; i++) {
                TimeSpan newspan = oneday * (i + 1);
                reserveringsWeek[i + 2] = new Day(today + newspan);
            }
        }

        /// <summary>Een update methode die checkt of alle dagen nog steeds kloppen en zo niet update die deze en checkt het opnieuw</summary>
        public void UpdateWeek() { 
            if (reserveringsWeek[1].Datum.Date != DateTime.Today) {  // if the first element in the array is not today's date
                Day[] newWeek = new Day[9];
                for (int i = 0; i < newWeek.Length - 1; i++) {
                    newWeek[i] = reserveringsWeek[i + 1];  // copy all to newWeek except one
                }
                newWeek[8] = new Day(newWeek[1].Datum.AddDays(7));
                reserveringsWeek = newWeek;
                UpdateWeek();
            }
        }

        /// <summary>Returned alle datums van alle day objecten in de week</summary>
        public DateTime[] GetDates() {
            DateTime[] newDates = new DateTime[reserveringsWeek.Length];
            for (int i = 0; i < newDates.Length; i++) {
                newDates[i] = reserveringsWeek[i].Datum;
            }
            return newDates;
        }

        /// <summary>Returned alle datums behalve gister</summary>
        public DateTime[] GetRelevantDates() { 
            DateTime[] newDates = new DateTime[reserveringsWeek.Length - 1];
            for (int i = 0, j = 1; i < newDates.Length; i++) {
                newDates[i] = reserveringsWeek[j++].Datum;
            }
            return newDates;
        }

        /// <summary>Een methode die een bepaalde dag van de week returned</summary>
        public Day GetDay(bool all) {
            Day returnDag = null;
            DateTime selectedDate = new DateTime();
            if (all) {
                selectedDate = Resources.inputDate("Kies een van de bovenstaande data: ", GetDates()); 
            }
            else {
                selectedDate = Resources.inputDate("Kies een van de bovenstaande data: ", GetRelevantDates());
            }
            foreach (Day dag in reserveringsWeek) {
                if (dag.Datum.Date == selectedDate.Date)
                    returnDag = dag;
            }
            return returnDag;  // returned null als gebruiker op "b" voor back heeft geklikt
        }
    }

    class Day {
        public DateTime Datum; // de datum van de bezettingsdag
        public DinnerRoom VoorVijfTotZes; // voor 5 - 6.45
        public DinnerRoom VoorZesTotAcht; // voor 6.45 tot 8.30
        public DinnerRoom VoorAchtTotTien; // voor 8.30 tot 10.15


        public Day(DateTime date) : this(date, new DinnerRoom("17:00 - 18:45"), new DinnerRoom("18:45 - 20:30"), new DinnerRoom("20:30 - 22:15")) { }
        public Day(Day day) : this(day.Datum, day.VoorVijfTotZes, day.VoorZesTotAcht, day.VoorAchtTotTien) { }
        [JsonConstructor]
        public Day(DateTime date, DinnerRoom voorvijf, DinnerRoom voorzes, DinnerRoom vooracht) {
            Datum = date;
            VoorVijfTotZes = voorvijf;
            VoorZesTotAcht = voorzes;
            VoorAchtTotTien = vooracht;
        }

        /// <summary>Geeft een menu van tijden weer en returned een DinnerRoom object gebaseerd op de keuze</summary>
        public DinnerRoom GetRoom() {
            string[] options = new string[] { "17:00 - 18:45", "18:45 - 20:30", "20:30 - 22:15" };
            string choice = Resources.makeMenuInput("Alle beschikbare tijden", "Kies uit een van de bovenstaande tijden: ", options, backbutton: true);
            if (choice == "1")
                return VoorVijfTotZes;
            else if (choice == "2")
                return VoorZesTotAcht;
            else if (choice == "3")
                return VoorAchtTotTien;
            else  // gebruiker wil terug
                return null;
        }
    }

    class DinnerRoom
    {
        public Table[] VoorTwee; // tafelnr eindigt op A
        public Table[] VoorVier; // eindigt op B
        public Table[] VoorZes; // eindigt op C
        public string Tijdvak;

        [JsonConstructor]
        public DinnerRoom(string tijd, Table[] voortwee, Table[] voorvier, Table[] voorzes) {
            Tijdvak = tijd;
            VoorTwee = voortwee;
            VoorVier = voorvier;
            VoorZes = voorzes;
        }

        public DinnerRoom(string tijd)
        {
            VoorTwee = new Table[8];
            VoorVier = new Table[5];
            VoorZes = new Table[2];
            MakeNew();
            Tijdvak = tijd;
        }

        /// <summary>Hulp voor de constructor, vult alle Table[] fields van de DinnerRoom Object met Table Objects</summary>
        public void MakeNew()
        {
            for (int i = 0; i < VoorTwee.Length; i++)
                VoorTwee[i] = new Table(i + 1, 2);
            for (int i = 0; i < VoorVier.Length; i++)
                VoorVier[i] = new Table(i + 1, 4);
            for (int i = 0; i < VoorZes.Length; i++)
                VoorZes[i] = new Table(i + 1, 6);
        }

        /// <summary>Returned een Table Object gebaseerd op het gegeven tafel nummer</summary>
        public Table GetTafel(string tafel_no) {
            Table[] tafelArr = tafel_no.EndsWith("A") ? VoorTwee : tafel_no.EndsWith("B") ? VoorVier : VoorZes;
            Table newTable = null;
            foreach (Table tafel in tafelArr) {
                if (tafel.table_num == tafel_no)
                    newTable = tafel;
            }
            return newTable;
        }

        /// <summary>Returned een Table Object gebaseerd op de code van de reservering gekoppelt aan die tafel</summary>
        public Table GetGereserveerdeTafel(string code) {
            Table newTable = null;
            foreach (Table tafel in AllTafels()) {
                if (tafel.reserveerCode == code)
                    newTable = tafel;
            }
            return newTable;
        }

        /// <summary>Yield een Table Object voor elke tafel</summary>
        public System.Collections.Generic.IEnumerable<Table> AllTafels() { 
            foreach (Table tafel in VoorTwee)
                yield return tafel;
            foreach (Table tafel in VoorVier)
                yield return tafel;
            foreach (Table tafel in VoorZes)
                yield return tafel;
        }

        /// <summary>Returned een string array met alle tafelnummers</summary>
        public string[] GetTafels() {
            string[] table_numbers = new string[VoorTwee.Length + VoorVier.Length + VoorZes.Length];
            int index = 0;
            foreach (Table tafel in AllTafels())
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
                    if (index < VoorVier.Length)
                        Console.ForegroundColor = VoorVier[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                    if (col % 2 != 0)
                        Console.Write(Resources.drawString(7, " "));
                    else if (col % 4 == 0)                   
                        Console.Write(VoorVier[index].TableArr()[row + VoorVier[index++].TableArr().Length / 2]);
                    else
                        Console.Write(VoorVier[index++].TableArr()[row]);
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
                        Console.Write(VoorVier[indexT2].TableArr()[row + VoorVier[indexT2].TableArr().Length / 2]);
                        indexT2 += 2;
                    }
                }
                Console.Write("\n");
            }

            // Maakt het laatste deel van de zespersonentafels
            for (int row = 1; row < VoorZes[0].TableArr().Length; row++) {
                for (int col = 1, index = 0; col < 6; col++) {
                    if (index < VoorZes.Length)
                        Console.ForegroundColor = VoorZes[index].occupied ? ConsoleColor.Red : ConsoleColor.Green;
                    if (col % 2 != 0)
                        Console.Write(Resources.drawString(28, " "));
                    else
                        Console.Write(VoorZes[index++].TableArr()[row]);
                }
                Console.Write("\n");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
        
   class Table 
   {
        public string table_num;  // het tafelnummer
        public int seats;  // het aantal stoelen van de tafel
        public bool occupied;  // een bool die noteert of de tafel bezet is of niet.
        public string reserveerCode; // de code gekoppelt aan de eventuele reservering
       
        // de Table constructor
        public Table(int t_num, int chairs) {
            seats = chairs;
            table_num = t_num.ToString();
            table_num += chairs == 2 ? "A" : chairs == 4 ? "B" : "C";
            occupied = false;
        }

        /// <summary>Returned het tegenovergestelde van occupied, gemaakt voor makkelijker lezen</summary>
        public bool isAvailable() => !occupied;

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

        /// <summary>Zet een reservering voor de tafel en geeft de code mee die gekoppelt is aan de reservering</summary>
        public void SetReservering(string code) { occupied = true; reserveerCode = code;  }

        /// <summary>Verwijderd alle reserveringsgegevens van een tafel zodat deze weer vrij is</summary>
        public void DelReservering() { occupied = false; reserveerCode = null; }
   }
}
