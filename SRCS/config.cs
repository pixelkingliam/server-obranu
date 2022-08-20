using Logger;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Config 
{
    public class Conf
    {
        public static string ip = "127.0.0.1";
        public static ushort port = 1337;
        public static int logcount = 3;
        //-==-
        public static string version = "Obranu Dev Build";
        public static string name = "Obranu Server";
        public static void Check()
        {   //check to see if CONF/ and CONF/server.json exists, 
            if(!Directory.Exists(@"CONF"))
            {
                Directory.CreateDirectory(@"CONF");
                Create();
            }else 
            if(!File.Exists(@"CONF/server.json"))
            {
                Create();
            }else
            {
                try {
                    JObject.Parse(System.IO.File.ReadAllText(@"CONF/server.json"));
                    
                }
                catch (JsonException ex) //some other exception
                    {
                        File.Delete(@"CONF/server.json");
                        Create();
                    }
                var jsonserverconf = JObject.Parse(System.IO.File.ReadAllText(@"CONF/server.json"));
                if (
                    // i have no idea how adding the ! operator to all the checks makes this work properly, time to never touch it again!
                        !jsonserverconf.ContainsKey("server-configs.ip") &
                        !jsonserverconf.ContainsKey("server-configs.sport") &
                        !jsonserverconf.ContainsKey("server-configs.logcount") &
                        !jsonserverconf.ContainsKey("server-info.version") &
                        !jsonserverconf.ContainsKey("server-info.name")
                        
                    )
                    {}else
                    {
                        File.Delete(@"CONF/server.json");
                        Create();
                    }
            }
        }
        public static void Create()
        {
            var jsonserverconf = new JObject();
            var serverconfigs = new JObject();
            var serverinfo = new JObject();
            serverconfigs["ip"] = ip;
            serverconfigs["port"] = port;
            serverconfigs["logcount"] = logcount;
            
            serverinfo["version"] = version;
            serverinfo["name"] = name;

            jsonserverconf["server-configs"] = serverconfigs;
            jsonserverconf["server-info"] = serverinfo;
            File.WriteAllText(@"CONF/server.json", jsonserverconf.ToString());
        }
        public static void Init()
        {
            JObject jsonserverconf = JObject.Parse(System.IO.File.ReadAllText("CONF/server.json"));
            ip = (string)jsonserverconf.SelectToken("server-configs.ip");
            port = (ushort)jsonserverconf.SelectToken("server-configs.port");
            logcount = (int)jsonserverconf.SelectToken("server-configs.logcount");

            version = (string)jsonserverconf.SelectToken("server-info.version");
            name = (string)jsonserverconf.SelectToken("server-info.name");
        }
    }
}