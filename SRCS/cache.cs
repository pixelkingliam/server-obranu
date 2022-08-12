using System;
using Logger;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CCache
{

    public class Cache
    {

        public static void LoadMods()
        {
            string[] modsfolderslist = Directory.GetDirectories(@"DATA/");
            foreach (string item in modsfolderslist)
            {
                if (File.Exists(item + @"/mod.init.json"))
                {
                    try
                    {
                        JContainer.Parse(System.IO.File.ReadAllText(item + @"/mod.init.json"));

                        if ((bool)JObject.Parse(System.IO.File.ReadAllText(item + "/mod.init.json")).SelectToken("Content.Hasprofiles"))
                        {
                            LoadProfiles(item);
                            Log.Success("Profiles Cached.");

                        }
                        if ((bool)JObject.Parse(System.IO.File.ReadAllText(item + "/mod.init.json")).SelectToken("Content.Hasitems"))
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
                }
                else
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
            JObject profileslist = new JObject();
            JArray profilesarray = new JArray();
            foreach (string item2 in datalist)
            {
                Log.Info(item2);
                profilesarray.Add(item2);
                JObject jsonmod = JObject.Parse(System.IO.File.ReadAllText(item + "/Profiles/" + item2 + ".json"));
                jsonprofiles[item2] = jsonmod;
            }
            profileslist["Profiles"] = profilesarray;
            File.WriteAllText(@"TEMP/ProfilesList.json", profileslist.ToString());
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
                foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
                foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
            }
            Directory.CreateDirectory("TEMP");
            Log.Success("Cleaned temporary files folder");
        }

    }
}
