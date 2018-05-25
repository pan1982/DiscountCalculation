using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace DiscountCalculation
{
    class Program
    {
        static string PathToFile = String.Concat(Path.GetTempPath(), "Segments.txt");

        static void Main()
        {
            Console.WriteLine("Choose action:");
            Console.WriteLine("0 - Exit");
            Console.WriteLine("1 - Calc discount");
            Console.WriteLine("2 - Add segment");
            if (File.Exists(PathToFile))
            {
                Console.WriteLine("3 - Edit segment");
                Console.WriteLine("4 - Delete segment");
                Console.WriteLine("5 - Delete file");
            }
            string Action = Console.ReadLine();
            Console.Clear();

            switch(Action)
            {
                case "0":
                    Environment.Exit(0);
                    break;
                case "1":
                    CalcDiscount();
                    break;
                case "2":
                    AddSegment();
                    break;
                case "3":
                    EditSegment();
                    break;
                case "4":
                    DelSegment();
                    break;
                case "5":
                    DeleteFile();
                    break;
                default:
                    Main();
                    break;
            }
        }
        static void ReturnToMain()
        {
            Console.Clear();
            Main();
        }
        static void AddSegment()
        {
            Console.WriteLine("Put segment name or Enter for return:");
            string Name = Console.ReadLine();

            if (Name == String.Empty)
            {
                ReturnToMain();
            }

            Console.WriteLine("Put segment discount sum:");
            string DiscSum = Console.ReadLine();
            Console.WriteLine("Put segment discount percent:");
            string DiscPerc = Console.ReadLine();

            File.AppendAllText(PathToFile, String.Concat(Environment.NewLine,Name, ';', DiscSum, ';', DiscPerc));
            ReturnToMain();
        }

        static void DeleteFile()
        {
            if (File.Exists(PathToFile)) { File.Delete(PathToFile); }
            ReturnToMain();
        }
        static int exceptString(string segName)
        {

            string exceptString = "MaxDiscProc;birthdayAmount";
            if (exceptString.IndexOf(segName) > -1) { return 1; }
            else { return 0; }
            
        }
        static void DelSegment()
        {
            if(File.Exists(PathToFile))
            {
                
                string[] Segments = File.ReadAllLines(PathToFile);
                Console.WriteLine("Input segment name:");
                
                string seg = Console.ReadLine();
                if (exceptString(seg) == 1)
                {
                    Console.WriteLine("Can't del this seg");
                    Console.ReadLine();
                }
                else
                {
                    Segments = Segments.Where(x => x != FindSegment(seg,0)).ToArray();
                    File.WriteAllLines(PathToFile, Segments);
                }
            }
            ReturnToMain();        }

        static void CalcDiscount()
        {
            string[] Segments = ReadSegments();

            double sum = 0;
            DateTime birthday;
            double discSum = 0;
            double discPerc = 0;
            double fullDiscSum = 0;
            double totalDiscSum = 0;
            double maxDiscSum;
            double birthdayAmount;
            double maxDiscPerc;

            try
            {
                maxDiscPerc = Convert.ToDouble(Segments.Where(x => x == FindSegment("MaxDiscProc")).ToArray()[0].Split(';')[1]);
            }
            catch
            {
                maxDiscPerc = 0;
            }
            
            try
            {
                birthdayAmount = Convert.ToDouble(Segments.Where(x => x == FindSegment("birthdayAmount")).ToArray()[0].Split(';')[1]);
            }
            catch
            {
                birthdayAmount = 0;
            }
                Console.WriteLine("Input data:");

            string clientstr = Console.ReadLine();

            try
            {
                sum = Convert.ToInt32(clientstr.Substring(clientstr.IndexOf('$') + 1)
                                         .Replace(",", ""));
            }
            catch 
            {
                WriteError("Sum");
            }

            string seg = FindSegment(clientstr);
            if (seg != "")
            {
                string[] arrSeg = seg.Split(';');
                discSum = Convert.ToInt32(arrSeg[1]);
                discPerc = Convert.ToInt32(arrSeg[2]);
            }
            else
            {
                WriteError("Segment");
            }

            birthday = DateTime.ParseExact(clientstr.Substring(clientstr.IndexOf('/') - 2, 10), "d",CultureInfo.InvariantCulture);
            try
            {
                maxDiscSum = sum * (Convert.ToDouble(maxDiscPerc) / 100);
            }
            catch
            {
                maxDiscSum = 0;
            }
            
            fullDiscSum = discSum + sum * (discPerc / 100);

            string birthdayDay = String.Concat(birthday.Day, "/", birthday.Month);

            string today = String.Concat(DateTime.Now.Day, "/", DateTime.Now.Month);

            if (birthdayDay == today)
            {
                totalDiscSum = Math.Min(fullDiscSum * birthdayAmount, maxDiscSum);
            }
            else
            {
                totalDiscSum = Math.Min(fullDiscSum, maxDiscSum);
            }

            Console.WriteLine("Total sum - {0}", sum - totalDiscSum);
            RepeatQuestion("CalcDiscount");
        }

        static void RepeatQuestion(string Value)
        {
            Console.WriteLine("Repeat? y/n");
            string answ = Console.ReadLine();
            if (answ == "y")
            {
                switch (Value)
                {
                    case "CalcDiscount":
                        CalcDiscount();
                        break;
                    case "EditSegment":
                        EditSegment();
                        break;
                }
                  
            }
            else
            {
                ReturnToMain();
            }
        }


        static void WriteError(string Value)
        {
            Console.Clear();
            Console.WriteLine("Can't find {0} in input data", Value);
            Console.WriteLine("Repeat input? y/n");
            string answer = Console.ReadLine();
            if (answer == "y")
            {
                Console.Clear();
                CalcDiscount();
            }
            else
            {
                ReturnToMain();
            }
        }

        static string FindSegment(string Value, int i = 1)
        {
            string[] Segments = ReadSegments();

            foreach (string seg in Segments)
            {
                string[] arrSeg = seg.Split(';');
                if (i == 1)
                {
                    if (Value.IndexOf(arrSeg[0]) > -1 && arrSeg[0]!= String.Empty)
                    {
                        return seg;
                    }
                }
                else
                {
                    if (arrSeg[0].IndexOf(Value) > -1)
                    {
                        return seg;
                    }
                }
            }
            return "";
        }

        static void EditSegment()
        {
            string newSum = String.Empty;
            string newPerc = String.Empty;
            if (File.Exists(PathToFile))
            { 
                Console.WriteLine("Input segment name");
                string segName = Console.ReadLine();
                string segment = FindSegment(segName);

                if (segment.Length > 0)
                {
                    Console.WriteLine("Input discount sum:");
                    newSum = Console.ReadLine();
                    if (exceptString(segName) == 0)
                    {
                        Console.WriteLine("Input discount percent:");
                        newPerc = ";" + Console.ReadLine();
                    }
                    string newSegment = String.Concat(segName, ";", newSum, newPerc);

                    string text = File.ReadAllText(PathToFile);
                    text = text.Replace(segment, newSegment);
                    File.WriteAllText(PathToFile, text);
                    ReturnToMain();
                }
                else
                {
                    Console.WriteLine("Wrong seg name...");
                    RepeatQuestion("EditSegment");
                }
            }
        }

        static string[] ReadSegments()
        {
            string[] Segments = {"Low;50;0", "Medium;50;10","High;50;20", "MaxDiscProc;90", "birthdayAmount;2" };
            if (!File.Exists(PathToFile))
            {
                File.WriteAllLines(PathToFile, Segments);
                return Segments;
            }
            else
            {
                return File.ReadAllLines(PathToFile);
            }
        }
    }
}
