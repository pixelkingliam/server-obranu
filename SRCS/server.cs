using Debug;
using CCache;
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
            wssv.AddWebSocketService<CreateAccount>("/crtacc");
            wssv.AddWebSocketService<Login>("/login");
            wssv.AddWebSocketService<VerifyAccount>("/verify");
            //ws.Log.Output = (data, s) => { Debug.WriteLine(data); };
            wssv.Start();
            Log.Success("Server is  running on " + ip + ":" + port);
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