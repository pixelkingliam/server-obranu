using Config;
using Logger;
using System;
using SOHash;
using System.IO;
using System.Linq;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Server;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Accounts
{
    public class PrivateKey
    {
        public string Accountname = null;
    }

    public class Account
    {
        public string username;
        public string password;
    }
    public class CreateAccount : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            Send(File.ReadAllText(@"TEMP/ProfilesList.json"));
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
            JArray jsonaccounts = JArray.Parse(File.ReadAllText(@"USER/accounts.json"));
            JObject jsonprofile = JObject.Parse(File.ReadAllText(@"TEMP/Profiles.json"));
            bool AccountExist = ((jsonaccounts.ToString().Contains("\"" + username + "\"")));
            var jsonresponse = new JObject();
            var jsonresults = new JObject();
            bool issafe;
            // i'll be stacking if statements here just to make the logic a tad bit easier to read
            // This config option decides if the server will verify compatibility with windows file/folder name
            if (!Conf.allowillegalfilepath)
            {
                if ( // check if the username contains a forbidden character
                    !username.ToLower().Contains("<") &
                    !username.ToLower().Contains(">") &
                    !username.ToLower().Contains(":") &
                    !username.ToLower().Contains("/") &
                    !username.ToLower().Contains("\\") &
                    !username.ToLower().Contains("|") &
                    !username.ToLower().Contains("?") &
                    !username.ToLower().Contains("*") 
                    
                )
                {
                    string[] forbiddenwords = {"CON","PRN","AUX","NUL","COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9","LPT1","LPT2","LPT3","LPT4","LPT5","LPT6","LPT7","LPT8","LPT9"};
                    if ( // check if the username contains forbidden file/folder name
                        !forbiddenwords.Contains(username)
                    )
                    {
                        if (!username.EndsWith(" ") & !username.EndsWith(".")) // Windows doesn't like it when filename ends with these
                        {
                            issafe = true;
                        }else
                        {
                            issafe = false;
                        }
                    }else
                    {
                        issafe = false;
                    }
                }else
                {
                    issafe = false;
                }
            }else
            {
                if (!username.ToLower().Contains("/"))
                {
                issafe = true;
                }else
                {
                    issafe = false;
                }
            }
            if (!AccountExist & issafe)
            {
                
                Account NewAccount = new Account();
                NewAccount.username = username;
                NewAccount.password = Hash.HashString(password);
                Log.Success(NewAccount.username + " Has made a new account");

                JObject NewaccJson = (JObject)JToken.FromObject(NewAccount);

                jsonaccounts.Add(new JObject { ["username"] = NewAccount.username, ["password"] = NewAccount.password });

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
            else if (!issafe)
            {
                jsonresults["result"] = 2;
                Log.Warning(username + "Tried to make an account with illegal characters!");
            }else if (issafe)
            {
                jsonresults["result"] = 1;
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
        public static List<Account> LoadedAccs;
        Random rnd = new Random();
        //static public List<string> pkeys = new List<string>();
        public static IDictionary<string, string> pkeys = new Dictionary<string, string>();
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data;
            //JObject jsonpacket = JObject.Parse(e.Data);
            JObject jsonpacket = JObject.Parse(msg);
            string username = (string)jsonpacket.SelectToken("connection.username");
            string password = (string)jsonpacket.SelectToken("connection.password");
            LoadedAccs = new List<Account>();
            JArray jsonaccounts = JArray.Parse(System.IO.File.ReadAllText(@"USER/accounts.json"));
            JObject jsonresponse = new JObject();
            JObject jsonresults = new JObject();
            //Log.Info
            foreach (var item in jsonaccounts)
            {
                LoadedAccs.Add(new Account{ username = item["username"].ToString(), password = item["password"].ToString() });
            }
            int index = 0;
            foreach (var item in LoadedAccs)
            {
                if (item.username == username)
                {
                    break;
                }else
                {
                    index++;
                }
            }
        if ((Hash.HashString(password) == LoadedAccs[index].password))
            {
                string pkey = Hash.HashString(Convert.ToString(rnd.Next()));
                // private key is used for the game client so users dont have to give password/username all the time
                pkeys.Add(pkey, username);
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
            
            if (Login.pkeys.ContainsKey(key)){
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