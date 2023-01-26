using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UMM;
using UltraRandomizer.HarmonyPatches;
using UltraRandomizer.MenuSystem;

namespace UltraRandomizer
{
    [UKPlugin("UltraRandomizer", "Enemy Randomizer for ULTRAKILL", "1.4.0", false, false)]

    public class UltraRandomizer : UKMod
    {
        GameObject player;

        SpawnableObjectsDatabase objectsDatabase;

        public List<GameObject> ToDestroyThisFrame = new List<GameObject>();

        public override void OnModLoaded()
        {
            new Harmony("UltraRandomizer").PatchAll();
        }

        private void Update()
        {
            EnemiesEnabled ee = EnemiesEnabled.Instance;

            DestroyOldEnemies();
            Database();

            DontDestroyOnLoad(ee);
            DontDestroyOnLoad(EnemySettingHandler.Instance);

            if (IsCheatActive.Instance.EnemyEnabled == true && ee.enemiesEnabled.Count > 0)
            {
                GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

                if (enemys.Length < 0)
                    return;
                
                for (int i = 0; i < enemys.Length; i++)
                {
                    if (enemys[i].transform.childCount > 3 && !enemys[i].name.Contains("mod"))
                    {
                        System.Random r = new System.Random();
                        int rInt = ee.enemiesEnabled[r.Next(ee.enemiesEnabled.Count)].spawnarmindex;
                        SpawnableObject newEnemy = objectsDatabase.enemies[rInt];

                        GameObject enemy = enemys[i];
                        GameObject ne = Instantiate(newEnemy.gameObject);

                        checkEnemiesStuff(ne, enemy);

                        ne.transform.position = enemys[i].transform.position;
                        ne.transform.SetParent(enemys[i].transform.parent);
                        ne.gameObject.name += " mod";

                        enemy.name += "mod";
                        ToDestroyThisFrame.Add(enemy);

                        EnemyPositionFix(ne,enemy);
                    }
                }
            }
        }

        void Database()
        {
            if (player == null)
            {
                player = GameObject.Find("Player");
            }
            else if (objectsDatabase == null)
            {
                objectsDatabase = (SpawnableObjectsDatabase)GetInstanceField(typeof(SpawnMenu), player.transform.GetChild(10).GetChild(21).gameObject.GetComponent<SpawnMenu>(), "objects");
            }
        }

        void DestroyOldEnemies()
        {
            for (int i = 0; i < ToDestroyThisFrame.Count; i++)
            {
                GameObject enemy = ToDestroyThisFrame[i];
                if (enemy)
                {
                    Destroy(enemy);
                    ToDestroyThisFrame.RemoveAt(i);
                }
                else
                    ToDestroyThisFrame.RemoveAt(i);
            }
        }
        
        void checkEnemiesStuff(GameObject ne, GameObject enemy)
        {
            if (ne.TryGetComponent(out EnemyIdentifier ei) && enemy.TryGetComponent(out EventOnDestroy eod))
            {
                ei.onDeath.AddListener(eod.stuff.Invoke);
            }

            if (ne.TryGetComponent(out KeepInBounds kib))
            {
                Destroy(kib);
            }

            if (enemy.TryGetComponent(out LeviathanController lc))
            {
                lc.DeathEnd();
            }
        }

        void EnemyPositionFix(GameObject ne, GameObject enemy)
        {
            if (enemy.TryGetComponent(out V2 v) && SceneManager.GetActiveScene().name == "Level 1-4")
            {
                ne.transform.position = new Vector3(0, -19.5f, 627);
            }
        }

        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
        
        internal static object CallInstanceVoid(Type type, object instance, string voidName)
        {
            MethodInfo dynMethod = type.GetType().GetMethod(voidName,
            BindingFlags.NonPublic | BindingFlags.Instance);
            return dynMethod.Invoke(instance, null);
        }
    }
}
