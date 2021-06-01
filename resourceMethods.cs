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
        public static string inputCheck(string prompt, string[] options, string errorprompt="Helaas is die optie niet beschikbaar. Kiest u alstublieft opnieuw.", int maxTries=0, bool caseSens=true) {
            string answer = input(prompt);
            int totalTries = 0;
            if (!caseSens) {
                answer = answer.ToLower();
                int index = 0;
                foreach (string s in options)
                    options[index++] = s.ToLower();
            }
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
                if (!caseSens) 
                   answer = answer.ToLower();
            }
            return answer;
        }

        /// <summary>Vergelijkt de ingevoerde mail (ToLower) met alle strings uit de array (ook ToLower)</summary>
        public static string inputMail(string prompt, string[] mails, string errorprompt="De ingevoerde mail is niet in gebruik", int maxTries=0) {
            string answer = input(prompt);
            int totalTries = 0;
            while (!MailCheck(answer, mails)) {
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

        /// <summary>Checked of de mail in de array van mails zit en returned een boolean</summary>
        public static bool MailCheck(string mail, string[] mails) { 
            foreach (string m in mails) {
                if (m.ToLower() == mail.ToLower())
                    return true;
            }
            return false;
        }

        /// <summary>Maakt de gegeven string zijn achtergrond donkerblauw</summary>
        public static void printBlue(string mess) {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(mess);
            Console.BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>Helpt om de dagen om te zetten naar nederlands</summary>
        public static string DayConvert(string day) {
            if (day == "Monday")
                return "Maandag";
            else if (day == "Tuesday")
                return "Dinsdag";
            else if (day == "Wednesday")
                return "Woensdag";
            else if (day == "Thursday")
                return "Donderdag";
            else if (day == "Friday")
                return "Vrijdag";
            else if (day == "Saturday")
                return "Zaterdag";
            else
                return "Zondag";
        }

        /// <summary>Laat een menu zien met datums en laat de gebruiker uit een datum kiezen</summary>
        public static DateTime inputDate(string prompt, DateTime[] dates, string errorprompt= "Die datum is helaas niet beschikbaar. Kiest u alstublieft opnieuw.", int maxtries=0) {
            string[] options = new string[dates.Length];  
            for (int i = 0; i < dates.Length; i++)
                options[i] = dates[i].ToString("dd/MM/yyyy") + " " + DayConvert(dates[i].DayOfWeek.ToString());
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
        public static string InputRegex(string prompt, string regexStr, string errorprompt="Input onjuist, probeer het opnieuw", int maxtries=0) {
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

        /// <summary>Controleert wachtwoord input</summary>
        public static string InputWachtwoord(string prompt, string checkpass="", string errorprompt="Input onjuist, probeer het opnieuw", int maxtries=0) {
            string answer = HiddenInput(prompt);
            bool check;
            Regex newre = new Regex(@"");
            if (checkpass == "") {
                newre = new Regex(@"\w{8,}");
                check = newre.IsMatch(answer);
            }
            else {
                check = answer == checkpass;
            }
            int totaltries = 0;
            while (!check) {  // de check was niet succesvol
                errorMessage(errorprompt);
                totaltries++;
                if (maxtries != 0 && totaltries >= maxtries) {
                    answer = "";
                    break;
                }
                answer = HiddenInput(prompt);
                if (checkpass == "") {
                    newre = new Regex(@"\w{8,}");
                    check = newre.IsMatch(answer);
                }
                else {
                    check = answer == checkpass;
                }
            }
            return answer;
        }

        /// <summary>Zorgt ervoor dat de input niet zichtbaar is</summary>
        private static string HiddenInput(string prompt) {
            string password = "";
            Console.Write(prompt);
            while (true) {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
                else if (key.Key == ConsoleKey.Enter)
                    break;
                else if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.Write("\n");
            return password;
        }


        /// <summary>Een method die ja of nee krijgt als input en een bool returned (ja = true, nee = false)</summary>
        public static bool YesOrNo(string prompt) {
            string[] yesoptions = new string[] { "ja", "j", "y", "yes"};
            string[] options = new string[] { "ja", "j", "y", "yes", "nee", "n", "no"};
            string choice = inputCheck(prompt, options, "Dat is geen ja of nee, typ alsjeblieft 'ja' of 'nee'", caseSens: false);
            
            return yesoptions.Contains(choice);
        }

        /// <summary>Vraagt om een enter input van de user voordat het programma verder gaat</summary>
        public static void EnterMessage() => input("Typ 'b'om terug te gaan: ");
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
