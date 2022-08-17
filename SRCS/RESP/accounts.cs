using Logger;
using System;
using SOHash;
using System.IO;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Server;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Accounts
{

    public class Account
    {
        public string username = null;
        public string password = null;
    }
    public class CreateAccount : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            Send(System.IO.File.ReadAllText(@"TEMP/ProfilesList.json"));
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data;
            try
            {
                JObject.Parse((msg));
            }
            catch { }
            JObject jsonpacket = JObject.Parse(msg);
            string username = (string)jsonpacket.SelectToken("connection.username");
            string password = (string)jsonpacket.SelectToken("connection.password");
            string profile = (string)jsonpacket.SelectToken("connection.profile");
            JObject jsonaccounts = JObject.Parse(System.IO.File.ReadAllText(@"USER/accounts.json"));
            JObject jsonprofile = JObject.Parse(System.IO.File.ReadAllText(@"TEMP/Profiles.json"));
            bool AccountExist = ((jsonaccounts.ToString().Contains("\"" + username + "\"")));
            var jsonresponse = new JObject();
            var jsonresults = new JObject();
            if (!AccountExist)
            {
                
                Account NewAccount = new Account();
                NewAccount.username = username;
                NewAccount.password = Hash.HashString(password);
                Log.Success(NewAccount.username + " Has made a new account");

                JObject NewaccJson = (JObject)JToken.FromObject(NewAccount);

                jsonaccounts[username] = new JObject { ["username"] = NewAccount.username, ["password"] = NewAccount.password };

                File.WriteAllText(@"USER/accounts.json", jsonaccounts.ToString());
                JObject jsonstash = jsonprofile.SelectToken(profile + ".stash").ToObject<JObject>();
                JObject jsongear = jsonprofile.SelectToken(profile + ".gear").ToObject<JObject>();
                JObject jsontrading = jsonprofile.SelectToken(profile + ".trading").ToObject<JObject>();
                // creates all the folders for the newly registered accounts with data from a profile preset
                Directory.CreateDirectory(@"USER/" + username);
                File.WriteAllText(@"USER/" + username + "/stash.json", jsonstash.ToString());
                File.WriteAllText(@"USER/" + username + "/gear.json", jsongear.ToString());
                File.WriteAllText(@"USER/" + username + "/trading.json", jsontrading.ToString());
                Log.Success("Created profile data for " + username);
                jsonresults["result"] = 0;
            }
            else
            {
                jsonresults["result"] = 0;
                Log.Warning(username + " Tried to make an existing account!");
            }
            // this sends a response to the client with a variable which says if the operation requested was successful or with a variable that gives requested data
            jsonresponse["result"] = jsonresults;
            Send(jsonresponse.ToString());
        }
        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Log.Network(e.Message + " This is most likely a client-related issue, the server will keep running.");
            Log.Error(e.Exception.Message);
        }
    }
    public class Login : WebSocketBehavior
    {
        Random rnd = new Random();
        static public List<string> pkeys = new List<string>();
        protected override void OnMessage(MessageEventArgs e)
        {
            JObject jsonpacket = JObject.Parse(e.Data);

            string username = (string)jsonpacket.SelectToken("connection.username");
            string password = (string)jsonpacket.SelectToken("connection.password");

            JObject jsonaccounts = JObject.Parse(System.IO.File.ReadAllText(@"USER/accounts.json"));
            JObject jsonresponse = new JObject();
            JObject jsonresults = new JObject();
            if ((username == (string)jsonaccounts.SelectToken(username + ".username")) & (Hash.HashString(password) == (string)jsonaccounts.SelectToken(username + ".password")))
            {
                string pkey = Hash.HashString(Convert.ToString(rnd.Next()));
                // private key is used for the game client so users dont have to give password/username all the time
                pkeys.Add(pkey);
                jsonresults["key"] = pkey;
                //    Log.Success("Test Passed for existing");
            }
            else
            {
                jsonresults["key"] = "false";
                Log.Warning("Client failed to login");
            }
            jsonresponse["result"] = jsonresults;
            Send(jsonresponse.ToString());
        }
        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Log.Network(e.Message + " This is most likely a client-related issue, the server will keep running.");
            Log.Error(e.Exception.Message);
        }
    }
    public class VerifyAccount : WebSocketBehavior
    {
        /*private readonly Login login;

        public VerifyAccount(Login login)
        {
        this.login = login;
        }*/
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data;
            try
            {
                JObject.Parse((msg));
            }
            catch { }
            JObject jsonpacket = JObject.Parse(msg);
            string key = (string)jsonpacket.SelectToken("connection.key");
            var jsonresponse = new JObject();
            var jsonresults = new JObject();
            
            if (Login.pkeys.Contains(key)){
                jsonresults["result"] = 0;
            }else
            {
                jsonresults["result"] = 1;
            }
            jsonresponse["result"] = jsonresults;
            Send(jsonresponse.ToString());
        }
        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Log.Network(e.Message + " This is most likely a client-related issue, the server will keep running.");
            Log.Error(e.Exception.Message);
        }
    }
}