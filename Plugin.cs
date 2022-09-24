using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UMM;

namespace UltraRandomizer
{
    //[UKPlugin("UltraRandomizer", "Enemy Randomizer for ULTRAKILL", "1.0.0",false,false)]
    [BepInPlugin("radsi.ultrarandomizer", "UltraRandomizer", "1.0.0")]
    public class UltraRandomizer : BaseUnityPlugin
    {
        GameObject player;

        SpawnableObjectsDatabase objectsDatabase;

        ConfigEntry<int> difficulty;

        SpawnableObject newEnemy;
        public List<GameObject> ToDestroyThisFrame = new List<GameObject>();

        DifficultiesHandler difficultyHandler;

        private void Start()
        {
            difficultyHandler = new DifficultiesHandler();

            difficultyHandler.New(new int[] { 0, 1, 2, 3, 21 });
            difficultyHandler.New(new int[] { 0, 1, 2, 3, 4, 9, 14, 21 });
            difficultyHandler.New(new int[] { 0, 1, 2, 3, 4, 9, 14, 15, 21 });
            difficultyHandler.New(new int[] { 0, 1, 2, 3, 4, 9, 14, 15, 16, 19, 21, 22 });
            difficultyHandler.New(new int[] { 0, 1, 2, 3, 4, 5, 6, 9, 14, 15, 16, 18, 19, 21, 22 });
            difficultyHandler.New(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 22, 23, 24, 25 });

            Logger.LogMessage("Plugin UltraRandomizer loaded");
            difficulty = Config.Bind("Enemys Randomizer", "Difficulty", 1, new ConfigDescription("The difficulty of the enemies that can appear (1-6)", new AcceptableValueRange<int>(1, 6)));
        }

        public static object GetPrivate<T>(T instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo field = typeof(T).GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        private void Update()
        {
            for (int i=0;i<ToDestroyThisFrame.Count;i++)
            {
                GameObject enemy = ToDestroyThisFrame[i];
                if (enemy)
                {
                    Destroy(enemy);
                }
            }

            if (player == null)
            {
                player = GameObject.Find("Player");
            }
            else if (player != null && objectsDatabase == null)
            {
                objectsDatabase = (SpawnableObjectsDatabase)GetInstanceField(typeof(SpawnMenu), player.transform.GetChild(10).GetChild(21).gameObject.GetComponent<SpawnMenu>(), "objects");
                foreach (var x in objectsDatabase.enemies)
                {
                    x.objectName += " mod";
                    x.name += " mod";
                    x.gameObject.name += " mod";
                }
            }

            if (NewMovement.Instance)
            {
                GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

                for (int i = 0; i < enemys.Length; i++)
                {
                    if (enemys[i].transform.childCount > 3 && !enemys[i].name.Contains("mod"))
                    {
                        System.Random r = new System.Random();
                        int[] arr = difficultyHandler.GetDifficulty(difficulty.Value-1).enemies;
                        int rInt = arr[r.Next(arr.Length)];

                        newEnemy = objectsDatabase.enemies[rInt];

                        GameObject ne = Instantiate(newEnemy.gameObject);

                        ne.transform.position = enemys[i].transform.position;
                        ne.transform.SetParent(enemys[i].transform.parent);

                        GameObject enemy = enemys[i];
                        enemy.name += "mod";
                        ToDestroyThisFrame.Add(enemy);

                        if (enemy.TryGetComponent(out EventOnDestroy eod))
                        {
                            MethodInfo dynMethod = eod.GetType().GetMethod("OnDestroy",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            dynMethod.Invoke(eod, null);
                        }
                    }
                }
            }
        }


        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
    }
}
