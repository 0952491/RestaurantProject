using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
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
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(prompt);
            Console.ForegroundColor = ConsoleColor.White;
        }

        ///<summary>Geeft een string weer in succes kleuren</summary>
        public static void succesMessage(string prompt) {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(prompt);
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
                    errorMessage($"Maximaal aantal pogingen {maxTries} bereikt, druk op enter om verder te gaan");
                    input("");
                    answer = "";
                    break;
                }
                answer = input(prompt);
            }
            return answer;
        }

        /// <summary>Laat een menu zien met datums en laat de gebruiker uit een datum kiezen</summary>
        public static DateTime inputDate(string prompt, DateTime[] dates, string errorprompt= "Die datum is helaas niet beschikbaar. Kiest u alstublieft opnieuw.", int maxtries=0) {
            string[] options = new string[dates.Length];
            for (int i = 0; i < dates.Length; i++)
                options[i] = dates[i].ToString("dd/MM/yyyy");
            string returned = makeMenuInput("De volgende dagen zijn beschikbaar", prompt, options, errorprompt, true, maxtries);
            if (returned == "b")
                return DateTime.MinValue;
            return dates[Convert.ToInt32(returned) - 1];
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
        public static string[] makeRangeArr(int min, int max, bool backbutton=false) {
            string[] returnArr;
            int original_min = min;
            if (backbutton)
                returnArr = new string[max - min + 2];
            else
                returnArr = new string[max - min + 1];
            for (int i = 0; min <= max; min++) {
                returnArr[i++] = $"{min}";
            }
            if (backbutton)
                returnArr[max - original_min + 1] = "b";
            return returnArr;
        }

        /// <summary>Maakt een menu van de gegeven opties en controleert de gegeven input tegen de beschikbare input</summary>
        public static string makeMenuInput(string title, string prompt, string[] options, string errorprompt="Helaas is die optie niet beschikbaar. Kiest u alstublieft opnieuw.",bool backbutton=false, int maxtries=0){
            orderOptions(title, options, backbutton);
            return inputCheck(prompt, makeRangeArr(1, options.Length, backbutton), errorprompt, maxtries);
        }


        /// <summary>Controleert de gegeven input tegen de gegeven regexStr</summary>
        public static string inputRegex(string prompt, string regexStr, string errorprompt="Input onjuist, probeer het opnieuw", int maxtries=0) {
            string answer = input(prompt);
            Regex newre = new Regex(regexStr);
            bool check = newre.IsMatch(answer);
            int totaltries = 0;
            while (!check) {  // de check was niet succesvol
                errorMessage(errorprompt);
                totaltries++;
                if (maxtries != 0 && totaltries >= maxtries) {
                    answer = "";
                    break;
                }
                answer = input(prompt);
                check = newre.IsMatch(answer);
            }
            return answer;
        }

        /// <summary>Een method die ja of nee krijgt als input en een bool returned (ja = true, nee = false)</summary>
        public static bool YesOrNo(string prompt) {
            string[] yesoptions = new string[] { "ja", "j", "y", "yes" };
            string[] options = new string[] { "ja", "j", "y", "yes", "nee", "n", "no" };
            string choice = inputCheck(prompt, options, "Dat is geen ja of nee, typ alsjeblieft 'ja' of 'nee'");
            if (yesoptions.Contains(choice))
                return true;
            else
                return false;
        }

        /// <summary>Vraagt om een enter input van de user voordat het programma verder gaat</summary>
        public static void EnterMessage() => input("Druk op enter om verder te gaan");
    }

    class DataHandler{
        ///<summary>Laad alle objects van een json file in een object</summary>
        public static dynamic LoadJson(string filename) {
            SetRightCwd();
            string json = File.ReadAllText($"Data/{filename}");
            dynamic Obj = JsonConvert.DeserializeObject(json);
            return Obj;
        }

        ///<summary>Returns een bool afhankelijk van of de file bestaat in de data folder of niet</summary>
        public static bool FileExists(string file) {
            SetRightCwd();
            return File.Exists($"Data/{file}");
        }

        ///<summary>Backup alle data van een object naar een json file</summary>
        public static void WriteJson(string filename, dynamic Obj){
            SetRightCwd();  
            string output = JsonConvert.SerializeObject(Obj, Formatting.Indented);
            File.WriteAllText($"Data/{filename}", output);
        }

        ///<summary>Zet de current working directory naar de juiste hoofdfolder</summary>
        public static void SetRightCwd() {
            string curdir = Directory.GetCurrentDirectory();
            while (!Directory.Exists($"{curdir}/Data")) {
                Directory.SetCurrentDirectory("..");  // .. staat voor de parent folder
                curdir = Directory.GetCurrentDirectory();
            }
        }
    }
}
