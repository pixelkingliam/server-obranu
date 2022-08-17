using Config;
using CCache;
using System;
using SOHash;
using Logger;
using Accounts;
using System.IO;
using System.Net;
using MiscResponses;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Server;
using System.Collections.Generic;
using System.Security.Cryptography;
//fancy using namespace 
public class Launch
{
    public static double Time = ((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds()) + 5) / (1000));
    public static void CheckData()
    {
        if (!Directory.Exists(@"DATA"))
        {
            Log.Error("Data Folder does not exist!");
            Log.Warning("A blank Data folder will be created but the server will not have any content and new accounts will not be able to be made!");
            Directory.CreateDirectory(@"DATA");
        }
    }
    public static void CheckUser()
    {
        if (!Directory.Exists(@"USER"))
        {
            Log.Error("USER Folder does not exist!");
            Log.Warning("A USER folder containing an accounts.json will be created ");
            Directory.CreateDirectory(@"USER");
        }
        if (!File.Exists(@"USER/accounts.json"))
        {
            Log.Error("accounts.json File does not exist!");
            Log.Warning("USER/ Folder is lacking a accounts.json, a new one will be made");
            File.WriteAllText(@"USER/accounts.json", "{}");
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

class Server
{
    public static void Main(string[] args)
    {

        try
        {
            Conf.Check();
            Conf.Init();
            
            Log.Clean(Conf.logcount);
            Log.Info("Server Name: " + Conf.name);
            Log.Info("Server Version: " + Conf.version);
            Launch.CheckConf();
            Launch.CheckUser();
            Launch.CheckData();
            Cache.Clean();
            Cache.LoadMods();
            Log.Info(@"     /\ \                                     ");
            Log.Info(@"  ___\ \ \____  _ __    __      ___   __  __  ");
            Log.Info(@" / __`\ \ '__`\/\`'__\/'__`\  /' _ `\/\ \/\ \ ");
            Log.Info(@"/\ \L\ \ \ \L\ \ \ \//\ \L\.\_/\ \/\ \ \ \_\ \");
            Log.Info(@"\ \____/\ \_,__/\ \_\\ \__/.\_\ \_\ \_\ \____/");
            Log.Info(@" \/___/  \/___/  \/_/ \/__/\/_/\/_/\/_/\/___/ ");
            var wssv = new WebSocketServer(System.Net.IPAddress.Any, Conf.port);
            wssv.AddWebSocketService<CreateAccount>("/accounts/crtacc");
            wssv.AddWebSocketService<Login>("/accounts/login");
            wssv.AddWebSocketService<VerifyAccount>("/accounts/verify");
            wssv.AddWebSocketService<Ping>("/pong");
            wssv.Start();
            Log.Success("Server is  running on " + Conf.ip + ":" + Conf.port);
            Console.ReadKey(true);
            Log.Info("Server is exiting");
            wssv.Stop();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            Console.Read();
            Environment.Exit(1);
        }
    }
}