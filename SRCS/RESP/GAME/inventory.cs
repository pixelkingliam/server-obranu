using System;
using Logger;
using Accounts;
using System.IO;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Server;
namespace Game
{
    public class GameInv : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var jsonpacket = JObject.Parse(e.Data);
            string pkey = jsonpacket.SelectToken("connection.key").ToString();
            if (Login.pkeys.ContainsKey(pkey))
            {
                Send(File.ReadAllText("USER/" + Login.pkeys[pkey] + "/stash.json"));
            }
        }
        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Log.Network(e.Message + " This is most likely a client-related issue, the server will keep running.");
            Log.Error(e.Exception.Message);
        }
    }
}