using System;
using System.IO;
using System.Collections.Generic;
namespace Logger
{
    public static class Log
    {
        public static string fn = (@"LOGS/" + DateTimeOffset.Now.ToUnixTimeSeconds() + ".log");
        public static void Success(string ToBeLogged)
        {
            string tolog = "[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]-[SUCCESS]-" + ToBeLogged;
            File.AppendAllText(fn, tolog + Environment.NewLine);
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write("[SUCCESS]");
            Console.ResetColor();
            Console.Write("[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]");
            Console.WriteLine(" " + ToBeLogged);

        }
        public static void Info(string ToBeLogged)
        {
            string tolog = "[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]-[INFO]-" + ToBeLogged;
            File.AppendAllText(fn, tolog + Environment.NewLine);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.Write("[INFO]");
            Console.ResetColor();
            Console.Write("[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]");
            Console.WriteLine(" " + ToBeLogged);
        }
        public static void Warning(string ToBeLogged)
        {
            string tolog = "[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]-[WARNING]-" + ToBeLogged;
            File.AppendAllText(fn, tolog + Environment.NewLine);
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.Write("[WARNING]");
            Console.ResetColor();
            Console.Write("[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]");
            Console.WriteLine(" " + ToBeLogged);
        }
        public static void Error(string ToBeLogged)
        {
            string tolog = "[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]-[ERROR]-" + ToBeLogged;
            File.AppendAllText(fn, tolog + Environment.NewLine);
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write("[ERROR]");
            Console.ResetColor();
            Console.Write("[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]");
            Console.WriteLine(" " + ToBeLogged);
        }
        public static void Network(string ToBeLogged)
        {
            string tolog = "[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]-[NETWORK]-" + ToBeLogged;
            File.AppendAllText(fn, tolog + Environment.NewLine);
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.Write("[NETWORK]");
            Console.ResetColor();
            Console.Write("[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]");
            Console.WriteLine(" " + ToBeLogged);
        }
        public static void Clean(int count)
        {
            //make logs folder if it does not exists
            if (!Directory.Exists(@"LOGS"))
            {
                Directory.CreateDirectory(@"LOGS");
            }
            string[] FileList = Directory.GetFiles(@"LOGS");
            List<int> UpdatedList = new List<int>(); ;
            if (FileList.Length > count - 1)
            {
                //get all the log filenames, remove the .log at the end and make them into an int, then add it to the UpdatedList[] list
                foreach (var file in FileList)
                {
                    
                    string tmp = file;
                    tmp = tmp.Substring(0, tmp.Length - 4);
                    tmp = tmp.Remove(0, 5);
                    UpdatedList.Add(Convert.ToInt32(tmp));
                }
            }

            //delete extra log files, this is usually only 1
            UpdatedList.Sort();
            // repeat until log count is to desired amount
            while (UpdatedList.Count > count - 1)
            {
                // delete the oldest log
                File.Delete(@"LOGS/" + UpdatedList[0] + ".log");
                UpdatedList.RemoveAt(0);
            }
            Log.Success("Cleared old log files");
        }
    }
}