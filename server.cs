using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
//fancy using namespace 
public class Launch 
{
    public static double Time = ((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())+5) / (1000));
}
public class Hash
{
    public static string HashString(string text, string salt = "")
    {
        if (String.IsNullOrEmpty(text))
        {
            return String.Empty;
        }
    
        // Uses SHA256 to create the hash
        using (var sha = new System.Security.Cryptography.SHA256Managed())
        {
            // Convert the string to a byte array first, to be processed
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
            byte[] hashBytes = sha.ComputeHash(textBytes);
        
        // Convert back to a string, removing the '-' that BitConverter adds
            string hash = BitConverter
                .ToString(hashBytes)
                .Replace("-", String.Empty);

            return hash;
        }
    }
}
public class Logger
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
        public static void Client(string ToBeLogged)
        {
            string tolog = "[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]-[CLIENT]-" + ToBeLogged;
            File.AppendAllText(fn, tolog + Environment.NewLine);
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.Write("[CLIENT]");
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
            List<int> UpdatedList = new List<int>();; 
            if ( FileList.Length > count - 1)
            {
                foreach (var file in FileList)
                {
                    string tmp = file;
                    tmp = tmp.Substring(0, tmp.Length-4);
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
            Logger.Success("Cleared old log files");
        }
    }
public class Account
    {
        public string username = null;
        public string password = null;
    }
// login class handles all received json packets from a client,
public class Connect
{ 
    public static void HandleConnections()
    {
        JObject jsonpacket = JObject.Parse(System.IO.File.ReadAllText(@"connection_pixel.json"));
        switch ((string)jsonpacket.SelectToken("connection.type"))
        {
            case "login": 
                Connect.Login((string)jsonpacket.SelectToken("connection.username"), (string)jsonpacket.SelectToken("connection.password"));
                break;
            case "makeacc":
                Connect.MakeAcc((string)jsonpacket.SelectToken("connection.username"), (string)jsonpacket.SelectToken("connection.password"), (string)jsonpacket.SelectToken("connection.profile"));
                break;
            default :
                Logger.Warning(" Client \"" + (string)jsonpacket.SelectToken("connection.username") + "\" tried to connected with a unknown connection type!");
                break;
        }

}
    public static void MakeAcc(string username, string password, string profile)
    {
        JObject jsonaccounts = JObject.Parse(System.IO.File.ReadAllText(@"USER/accounts.json"));
        JObject jsonprofile = JObject.Parse(System.IO.File.ReadAllText(@"TEMP/Profiles.json"));
        bool AccountExist = ((jsonaccounts.ToString().Contains("\"" + username + "\"")));
        if (!AccountExist)
        {
            Account NewAccount = new Account();
            NewAccount.username = username;
            NewAccount.password = Hash.HashString(password);
            Logger.Success(NewAccount.username + " Has made a new account");
        
            JObject NewaccJson = (JObject)JToken.FromObject(NewAccount);

            jsonaccounts[username] = new JObject { ["username"] = NewAccount.username, ["password"] = NewAccount.password };

            File.WriteAllText(@"USER/accounts.json", jsonaccounts.ToString());
            JObject jsonstash = jsonprofile.SelectToken(profile + ".stash").ToObject<JObject>();
            JObject jsongear = jsonprofile.SelectToken(profile + ".gear").ToObject<JObject>();
            JObject jsontrading = jsonprofile.SelectToken(profile + ".trading").ToObject<JObject>();
            Directory.CreateDirectory(@"USER/" + username);
            File.WriteAllText(@"USER/" + username + "/stash.json", jsonstash.ToString());
            File.WriteAllText(@"USER/" + username + "/gear.json", jsongear.ToString());
            File.WriteAllText(@"USER/" + username + "/trading.json", jsontrading.ToString());
            Logger.Success("Created profile data for " + username);

        }else
        {
            Logger.Warning(username + " Tried to make an existing account!");
        }
    }
    public static void Login(string username, string password)
    {    
        JObject jsonaccounts = JObject.Parse(System.IO.File.ReadAllText(@"USER/accounts.json"));
        
        if (
            (username == (string)jsonaccounts.SelectToken(username + ".username")) &
            (Hash.HashString(password) == (string)jsonaccounts.SelectToken(username + ".password"))
        )
        {
            Logger.Success("Test Passed for existing");
        }else
        {
            Logger.Warning("Client failed to login");
        }
    }

}
public class Cache
{
    
    public static void LoadMods(){
        List<string> modslist = new List<string>();
        string[] modsfolderslist = Directory.GetDirectories(@"DATA/");
        foreach (string item in modsfolderslist) { 
            if (File.Exists(item + @"/mod.init.json")) { 
            try
               {
                    JContainer.Parse(System.IO.File.ReadAllText(item + @"/mod.init.json"));

                    if  ((bool)JObject.Parse(System.IO.File.ReadAllText(item + "/mod.init.json")).SelectToken("Content.Hasprofiles"))
                    {
                        LoadProfiles(item);
                        Logger.Success("Profiles Cached.");

                    }
                    if  ((bool)JObject.Parse(System.IO.File.ReadAllText(item + "/mod.init.json")).SelectToken("Content.Hasitems"))
                    {
                        LoadItems(item);
                        Logger.Success("Items Cached.");
                    }
               }
                catch (Exception ex) //some other exception
                {
                    Logger.Error(ex.Message);
                    Console.Read();
                    Environment.Exit(1);
                    
                }
            }else
            {
                Logger.Warning(item + " is a folder in DATA/ but does not contain a mod.init.json.");
            }
        }
    }
    // this loads all the profiles to TEMP/, those profiles are used whenever the client makes a new account
    public static void LoadProfiles(string item)
    {
        JObject modinitjson = JObject.Parse(System.IO.File.ReadAllText(item + "/mod.init.json"));
        string fileName = String.Format(@"TEMP/Profiles.json"); 
        string[] datalist = (modinitjson.SelectToken("ProfilesNames")).ToObject<string[]>();
        File.Delete(fileName);
        File.WriteAllText(fileName, "{\n\n}");
        JObject jsonprofiles = JObject.Parse(System.IO.File.ReadAllText(@"TEMP/Profiles.json"));

        foreach (string item2 in datalist)
        {
            JObject jsonmod = JObject.Parse(System.IO.File.ReadAllText(item + "/Profiles/" + item2 + ".json"));
            jsonprofiles[item2] = jsonmod;
        }

        File.WriteAllText(@"TEMP/Profiles.json", jsonprofiles.ToString());
    }
    //this loads all the items to TEMP/, those items will be sent to the client
    public static void LoadItems(string item)
    {
        JObject modinitjson = JObject.Parse(System.IO.File.ReadAllText(item + "/mod.init.json"));
        string fileName = String.Format(@"TEMP/Items.json"); 
        string[] datalist = (modinitjson.SelectToken("ItemsNames")).ToObject<string[]>();
        File.Delete(fileName);
        File.WriteAllText(fileName, "{\n\n}");
        JObject jsonprofiles = JObject.Parse(System.IO.File.ReadAllText(@"TEMP/Items.json"));
        foreach (string item2 in datalist)
        {
            JObject jsonmod = JObject.Parse(System.IO.File.ReadAllText(item + "/Items/" + item2 + ".json"));
            jsonprofiles[item2] = jsonmod;
        }
        File.WriteAllText(@"TEMP/Items.json", jsonprofiles.ToString());
    }
    public static void Clean()
    {
        
        System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(@"TEMP/");
        if (directory.Exists)
        {
            foreach(System.IO.FileInfo file in directory.GetFiles()) file.Delete(); 
            foreach(System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }
        Directory.CreateDirectory("TEMP");
        Logger.Success("Cleaned temporary files folder");
    }
        
}
class Server
{
    public static void Main(string[] args)
    { 
        try {
            JObject jsonserverconf = JObject.Parse(System.IO.File.ReadAllText(@"CONF/server.json"));
            string ip = (string)jsonserverconf.SelectToken("server-configs.ip");
            ushort port = (ushort)jsonserverconf.SelectToken("server-configs.port");
            Logger.Clean((int)jsonserverconf.SelectToken("server-configs.logcount"));
            Cache.Clean();
            Cache.LoadMods();

            Logger.Info(@"     /\ \                                     ");                                     
            Logger.Info(@"  ___\ \ \____  _ __    __      ___   __  __  ");
            Logger.Info(@" / __`\ \ '__`\/\`'__\/'__`\  /' _ `\/\ \/\ \ ");
            Logger.Info(@"/\ \L\ \ \ \L\ \ \ \//\ \L\.\_/\ \/\ \ \ \_\ \");
            Logger.Info(@"\ \____/\ \_,__/\ \_\\ \__/.\_\ \_\ \_\ \____/");
            Logger.Info(@" \/___/  \/___/  \/_/ \/__/\/_/\/_/\/_/\/___/ ");
        
            Logger.Success("Server is hopefully running on " + ip + ":" + port + " in the future");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message);
            Console.Read();
            Environment.Exit(1);
        }
    }
}