using Logger;
using System;
using SOHash;
using System.IO;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Server;
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
    protected override void OnMessage (MessageEventArgs e)
    {
        var msg = e.Data;
        try
        {
            JObject.Parse(System.IO.File.ReadAllText(msg));
        }   
        catch (Exception ex)
        {

        }
        JObject jsonpacket = JObject.Parse(msg);
        string username = (string)jsonpacket.SelectToken("connection.username");
        string password = (string)jsonpacket.SelectToken("connection.password");
        string profile = (string)jsonpacket.SelectToken("connection.profile");
        JObject jsonaccounts = JObject.Parse(System.IO.File.ReadAllText(@"USER/accounts.json"));
        JObject jsonprofile = JObject.Parse(System.IO.File.ReadAllText(@"TEMP/Profiles.json"));
        bool AccountExist = ((jsonaccounts.ToString().Contains("\"" + username + "\"")));
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
            Directory.CreateDirectory(@"USER/" + username);
            File.WriteAllText(@"USER/" + username + "/stash.json", jsonstash.ToString());
            File.WriteAllText(@"USER/" + username + "/gear.json", jsongear.ToString());
            File.WriteAllText(@"USER/" + username + "/trading.json", jsontrading.ToString());
            Log.Success("Created profile data for " + username);

        }else
        {
            Log.Warning(username + " Tried to make an existing account!");
        }
    }
}
public class Login : WebSocketBehavior
{
    Random rnd = new Random();

    protected override void OnMessage (MessageEventArgs e)
    {
        JObject jsonpacket = JObject.Parse(e.Data);

        string username = (string)jsonpacket.SelectToken("connection.username");
        string password = (string)jsonpacket.SelectToken("connection.password");

        JObject jsonaccounts = JObject.Parse(System.IO.File.ReadAllText(@"USER/accounts.json"));
        
        if  ((username == (string)jsonaccounts.SelectToken(username + ".username")) &(Hash.HashString(password) == (string)jsonaccounts.SelectToken(username + ".password")))
        {
            Send("Successfully logged in!");
            Send("This is your private key! : " + Hash.HashString(Convert.ToString(rnd.Next())));
        //    Log.Success("Test Passed for existing");
        }else
        {
            Log.Warning("Client failed to login");
            Send ("Invalid Password/Username");
        }
    }
}
}