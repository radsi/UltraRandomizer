using Newtonsoft.Json.Linq;
using System.IO;
using UltraRandomizer.HarmonyPatches;
using UnityEngine;
using System;

namespace UltraRandomizer.MenuSystem
{
    public class SaveSystem
    {
        string file = Application.dataPath.Replace("ULTRAKILL_Data", "BepInEx/UMM Mods/UltraRandomizer/UltraRandomizerSettings.json");

        public void CheckFiles()
        {
            if (!Directory.Exists(Application.dataPath.Replace("ULTRAKILL_Data", "BepInEx/UMM Mods/UltraRandomizer")) || !File.Exists(Application.dataPath.Replace("ULTRAKILL_Data", "BepInEx/UMM Mods/UltraRandomizer/UltraRandomizerSettings.json")))
            {
                Directory.CreateDirectory(Application.dataPath.Replace("ULTRAKILL_Data", "BepInEx/UMM Mods/UltraRandomizer"));
                File.Create(Application.dataPath.Replace("ULTRAKILL_Data", "BepInEx/UMM Mods/UltraRandomizer/UltraRandomizerSettings.json"));
            }
        }

        public void Save()
        {
            JObject jsonExistingData = new();

            string fullData = "";
            string existingData = "";

            using (StreamReader r = new StreamReader(file))
            {
                existingData = r.ReadToEnd();
                r.Close();
            }

            if (existingData != "")
            {
                jsonExistingData = JObject.Parse(existingData);
            }

            foreach(var enemy in EnemiesEnabled.Instance.enemiesEnabled)
            {
                Debug.Log(enemy.displayname);
                if (!jsonExistingData.ContainsKey(enemy.displayname))
                {

                    JObject data = new JObject(
                        new JProperty("enabled", enemy.enabled.ToString())
                    );

                    JObject dataToWrite = new JObject(
                        new JProperty(enemy.displayname, data)
                    );

                    if (existingData.Length > 5)
                    {
                        fullData = "{" + existingData.TrimStart('{').TrimEnd('}') + ", " + dataToWrite.ToString().TrimStart('{').TrimEnd('}') + "}";
                    }
                    else
                    {
                        fullData = dataToWrite.ToString();
                    }
                }
                else
                {
                    jsonExistingData[enemy.displayname]["enabled"] = enemy.enabled.ToString();
                    fullData = jsonExistingData.ToString();
                }
            }

            File.WriteAllText(file, fullData);
        }

        public JObject Get()
        {
            return JObject.Parse(File.ReadAllText(file));
        }

        public void Apply()
        {
            JObject enemiesSaved = Get();

            if(enemiesSaved.Count != 0)
            {
                foreach (var enemy in enemiesSaved)
                {
                    foreach (var x in EnemySettingHandler.Instance.shitstuff)
                    {
                        if (x.displayname == enemy.Key)
                        {
                            EnemiesEnabled.Instance.enemiesEnabled.Add(x);
                        }
                    }
                }
            }
        }
    }
}
