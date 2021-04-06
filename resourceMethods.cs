using System;
using System.Linq;
namespace resourceMethods{
    class Resources{
        ///<summary>Maakt een stringline afhankelijk van de lengte en de gegeven character(string geen char)</summary>
        public static string drawString(int Length, string character) {
            string side = "";
            for (int i = 0; i < Length; i++) { side += character; }
            return side; 
        }

        ///<summary>Geeft een string weer in error kleuren</summary>
        public static void errorMessage(string prompt) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(prompt);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        ///<summary>Geeft een string weer in succes kleuren</summary>
        public static void succesMessage(string prompt) {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(prompt);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        ///<summary>Een method die een prompt laat zien en vervolgens de input van de user returned</summary>
        public static string input(string promptString) {
            Console.Write(promptString);
            string ans = Console.ReadLine();
            return ans;
        }

        ///<summary>Checked of de input van de gebruiker voldoet aan een van de meegegeven opties (zo niet wordt de prompt herhaald)</summary>
        public static string inputCheck(string prompt, string[] options, string errorprompt="Helaas is die optie niet beschikbaar. Kiest u alstublieft opnieuw.", int maxTries=0) {
            string answer = input(prompt);
            int totalTries = 0;
            while (!options.Contains(answer)) {
                errorMessage(errorprompt);
                totalTries++;
                if (maxTries != 0 && totalTries >= maxTries) {
                    answer = "";
                    break;
                }
                Console.Write(prompt);
                answer = Console.ReadLine();
            }
            return answer;
        }

        ///<summary>Een method die een array van strings organiseert in genummerde opties en print</summary>
        public static void orderOptions(string prompt, string[] options, bool backbutton=false) {
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Length; i++) {
                Console.WriteLine($"[{i+1}] {options[i]}");
            }
            if (backbutton) { Console.WriteLine("[b] Ga een stap terug"); }
        }

        ///<summary>Een method die een string[] maakt van alle nummers tussen de gegeven min en max (incl. max)</summary>
        public static string[] makeRangeArr(int min, int max) {
            string[] returnArr = new string[max - min + 1];
            for (int i = 0; min <= max; min++) {
                returnArr[i++] = $"{min}";
            }
            return returnArr;
        }
    }
}