using System;
using SOHash;
using Logger;
using Accounts;
using System.IO;
using System.Net;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Server;
using System.Collections.Generic;
using System.Security.Cryptography;
//fancy using namespace 
public class Launch
{
    public static double Time = ((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())+5) / (1000));
    public static void CheckData()
    {
        if (!Directory.Exists(@"DATA"))
            {
                Log.Error("Data Folder does not exist!");
                Log.Warning("A blank Data folder will be created but the server will not have any content and new accounts will not be able to be made!");
                Directory.CreateDirectory(@"DATA");
            }
    }
    public static void CheckConf()
    { 
        if (!Directory.Exists(@"CONF"))
            {
                Log.Error("Configuration Folder does not exist!");
                Console.Read();
                Environment.Exit(1);
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
                        Log.Success("Profiles Cached.");

                    }
                    if  ((bool)JObject.Parse(System.IO.File.ReadAllText(item + "/mod.init.json")).SelectToken("Content.Hasitems"))
                    {
                        LoadItems(item);
                        Log.Success("Items Cached.");
                    }
               }
                catch (Exception ex) //some other exception
                {
                    Log.Error(ex.Message);
                    Console.Read();
                    Environment.Exit(1);
                    
                }
            }else
            {
                Log.Warning(item + " is a folder in DATA/ but does not contain a mod.init.json.");
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
        Log.Success("Cleaned temporary files folder");
    }
        
}
class Server
{
    public static void Main(string[] args)
    { 
        try {
            Launch.CheckConf();
            Launch.CheckData();
            JObject jsonserverconf = JObject.Parse(System.IO.File.ReadAllText(@"CONF/server.json"));
            string ip = (string)jsonserverconf.SelectToken("server-configs.ip");
            ushort port = (ushort)jsonserverconf.SelectToken("server-configs.port");
            Log.Clean((int)jsonserverconf.SelectToken("server-configs.logcount"));
            Cache.Clean();
            Cache.LoadMods();

            Log.Info(@"     /\ \                                     ");                                     
            Log.Info(@"  ___\ \ \____  _ __    __      ___   __  __  ");
            Log.Info(@" / __`\ \ '__`\/\`'__\/'__`\  /' _ `\/\ \/\ \ ");
            Log.Info(@"/\ \L\ \ \ \L\ \ \ \//\ \L\.\_/\ \/\ \ \ \_\ \");
            Log.Info(@"\ \____/\ \_,__/\ \_\\ \__/.\_\ \_\ \_\ \____/");
            Log.Info(@" \/___/  \/___/  \/_/ \/__/\/_/\/_/\/_/\/___/ ");
        

            var wssv = new WebSocketServer (System.Net.IPAddress.Any, port);
            wssv.AddWebSocketService<CreateAccount> ("/CrtAcc");
            wssv.AddWebSocketService<Login> ("/LogIn");

            wssv.Start ();
            Log.Success("Server is  running on " + ip + ":" + port);
            Console.ReadKey (true);
            Log.Info("shittingthebed");
            //wssv.Stop ();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            Console.Read();
            Environment.Exit(1);
        }
    }
}