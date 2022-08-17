using System;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Server;
namespace MiscResponses 
{
    public class Ping : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            JObject jsonserverconf = JObject.Parse(System.IO.File.ReadAllText(@"CONF/server.json"));
            JObject serverinfo = new JObject();
            serverinfo["server-info"] = jsonserverconf["server-info"];
            Send(serverinfo.ToString());

            this.Sessions.CloseSession(this.ID);
        }
    }
}