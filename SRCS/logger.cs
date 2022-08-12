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
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("[SUCCESS]");
            Console.ResetColor();
            Console.Write("[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]");
            Console.WriteLine(" " + ToBeLogged);

        }
        public static void Info(string ToBeLogged)
        {
            string tolog = "[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]-[INFO]-" + ToBeLogged;
            File.AppendAllText(fn, tolog + Environment.NewLine);
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.Write("[INFO]");
            Console.ResetColor();
            Console.Write("[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]");
            Console.WriteLine(" " + ToBeLogged);
        }
        public static void Warning(string ToBeLogged)
        {
            string tolog = "[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]-[WARNING]-" + ToBeLogged;
            File.AppendAllText(fn, tolog + Environment.NewLine);
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write("[WARNING]");
            Console.ResetColor();
            Console.Write("[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]");
            Console.WriteLine(" " + ToBeLogged);
        }
        public static void Error(string ToBeLogged)
        {
            string tolog = "[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]-[ERROR]-" + ToBeLogged;
            File.AppendAllText(fn, tolog + Environment.NewLine);
            Console.BackgroundColor = ConsoleColor.Red;
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
            if (!Directory.Exists(@"LOGS"))
            {
                Directory.CreateDirectory(@"LOGS");
            }
            string[] FileList = Directory.GetFiles(@"LOGS");
            List<int> UpdatedList = new List<int>(); ;
            if (FileList.Length > count - 1)
            {
                foreach (var file in FileList)
                {
                    string tmp = file;
                    tmp = tmp.Substring(0, tmp.Length - 4);
                    tmp = tmp.Remove(0, 5);
                    UpdatedList.Add(Convert.ToInt32(tmp));
                }
            }


            UpdatedList.Sort();
            while (UpdatedList.Count > count - 1)
            {
                File.Delete(@"LOGS/" + UpdatedList[0] + ".log");
                UpdatedList.RemoveAt(0);
            }
            Log.Success("Cleared old log files");
        }
    }
}