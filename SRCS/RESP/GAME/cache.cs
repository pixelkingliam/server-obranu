using System;
using Logger;
using System.IO;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Server;
namespace Game
{
    public class GameCache : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var jsonpacket = JObject.Parse(e.Data);
            string cachedfile = jsonpacket.SelectToken("connection.file").ToString();
            switch (cachedfile)
            {
                case "Items":
                    Send(File.ReadAllText("TEMP/Items.json"));
                    break;
            }
        }
        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Log.Network(e.Message + " This is most likely a client-related issue, the server will keep running.");
            Log.Error(e.Exception.Message);
        }
    }
}