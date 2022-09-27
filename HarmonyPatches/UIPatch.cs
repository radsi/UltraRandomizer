using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using UnityEngine.EventSystems;
using UltraRandomizer.MenuSystem;
using System.Reflection;

namespace UltraRandomizer.HarmonyPatches
{
    [HarmonyPatch(typeof(OptionsMenuToManager), "Start", new Type[] { })]
    public class UIPatch
    {
        public static void Prefix(OptionsMenuToManager __instance)
        {
            if (__instance.pauseMenu.name == "Main Menu (1)") // check to see that we're patching out the main menu's menu, not like an in game menu one
            {
                bool wasOn = MonoSingleton<PrefsManager>.Instance.GetBool("variationMemory", false);
                __instance.pauseMenu.transform.Find("Panel").localPosition = new Vector3(0, 325, 0);

                void FullSize(Transform tf)
                {
                    bool wasActive = tf.gameObject.activeSelf;
                    tf.gameObject.SetActive(false);
                    //tf.localScale = new Vector3(.5f, 1f, 1f);
                    tf.GetComponent<RectTransform>().sizeDelta = new Vector2(240*2, 80);
                    //tf.Find("Text").localScale = new Vector3(2f, 1f, 1f);
                    Traverse hudEffect = Traverse.Create(tf.gameObject.GetComponent<HudOpenEffect>());
                    hudEffect.Field("originalWidth").SetValue(1f);
                    hudEffect.Field("originalHeight").SetValue(1f);
                    tf.gameObject.SetActive(wasActive);
                }

                Transform options = __instance.pauseMenu.transform.Find("Options");

                GameObject newModsButton = GameObject.Instantiate(__instance.pauseMenu.transform.Find("Continue").gameObject, __instance.pauseMenu.transform, true);
                newModsButton.SetActive(false);
                newModsButton.transform.localPosition = new Vector3(0, options.localPosition.y-240, 0);
                FullSize(newModsButton.transform);
                newModsButton.GetComponentInChildren<Text>(true).text = "RANDOMIZER";
                newModsButton.name = "RANDOMIZER";

                GameObject modsMenu = GameObject.Instantiate(__instance.optionsMenu, __instance.transform);
                modsMenu.SetActive(false);
                for (int i = 0; i < modsMenu.transform.childCount; i++)
                    modsMenu.transform.GetChild(i).gameObject.SetActive(false);

                Transform colorBlind = modsMenu.transform.Find("ColorBlindness Options");
                colorBlind.gameObject.SetActive(true);
                GameObject.Destroy(colorBlind.GetComponent<GamepadObjectSelector>()); // sorry gamepad players, but without this the mod manager breaks
                Text modHeaderText = colorBlind.transform.Find("Text (1)").GetComponent<Text>();
                modHeaderText.text = "--RANDOMIZER--";

                Transform content = colorBlind.transform.Find("Scroll Rect").Find("Contents");
                RectTransform cRect = content.GetComponent<RectTransform>();
                content.Find("Enemies").gameObject.SetActive(false);
                content.gameObject.SetActive(true);
                GameObject template = content.Find("HUD").Find("Health").gameObject;
                content.Find("HUD").gameObject.SetActive(false);
                template.SetActive(false);
                __instance.variationMemory.gameObject.SetActive(false);

                GameObject hoverText = content.Find("Default").gameObject;
                hoverText.transform.parent = modsMenu.transform;
                hoverText.GetComponentInChildren<Text>().text = "Enables the enemy in the enemy randomizer";
                hoverText.transform.localPosition -= new Vector3(0f, 520f, 0f);
                GameObject.Destroy(hoverText.GetComponent<BackSelectOverride>());
                GameObject.Destroy(hoverText.GetComponent<Button>());
                hoverText.SetActive(false);

                GameObject player = GameObject.Find("Player");
                SpawnableObjectsDatabase objectsDatabase = null;

                static object GetInstanceField(Type type, object instance, string fieldName)
                {
                    BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                    FieldInfo field = type.GetField(fieldName, bindFlags);
                    return field.GetValue(instance);
                }


                bool doneThing = false;
                void dothing()
                {
                    EnemySettingHandler esh = EnemySettingHandler.Instance;
                    if (doneThing == false)
                        doneThing = true;
                    else
                        return;

                    if (player == null)
                    {
                        player = GameObject.Find("Player");
                    }
                    else if (objectsDatabase == null)
                    {
                        objectsDatabase = (SpawnableObjectsDatabase)GetInstanceField(typeof(SpawnMenu), player.transform.GetChild(10).GetChild(21).gameObject.GetComponent<SpawnMenu>(), "objects");
                        
                        for (int i=0;i<objectsDatabase.enemies.Length;i++)
                        {
                            var x = objectsDatabase.enemies[i];
                            EnemySetting es = new EnemySetting(x.objectName, x.objectName+"_id", i);
                            esh.shitstuff.Add(es);
                        }
                    }

                    List<EnemySetting> information = esh.shitstuff;
                    if (information.Count > 0)
                    {
                        for (int i = 0; i < information.Count; i++)
                        {
                            EnemySetting info = information[i];
                            GameObject newInformation = GameObject.Instantiate(template, content);
                            GameObject.Destroy(newInformation.GetComponent<ColorBlindSetter>());

                            Button newButton = newInformation.AddComponent<Button>();
                            newButton.transition = Selectable.Transition.ColorTint;
                            newButton.targetGraphic = newInformation.GetComponent<Image>();
                            newButton.targetGraphic.color = info.enabled ? Color.green : Color.red;

                            newInformation.transform.Find("Red").gameObject.SetActive(false);
                            newInformation.transform.Find("Green").gameObject.SetActive(false);
                            newInformation.transform.Find("Blue").gameObject.SetActive(false);
                            newInformation.transform.Find("Image").gameObject.SetActive(false);
                            newInformation.transform.localScale = new Vector3(1.64415f, 1.64415f, 1.64415f);
                            newInformation.transform.localPosition = new Vector3(0f, -60f * i, 0f);

                            RectTransform erection = newInformation.GetComponent<RectTransform>();
                            erection.sizeDelta = new Vector2(erection.sizeDelta.x, 30);

                            var id = info.id;
                            Text modText = newInformation.transform.Find("Text").GetComponent<Text>();
                            modText.text = info.displayname;
                            modText.alignment = TextAnchor.UpperLeft;
                            modText.transform.localPosition = new Vector3(-49.2f, -7.5f, 0f);
                            modText.transform.localScale = new Vector3(0.66764f, 0.66764f, 0.66764f);

                            GameObject toggleObj = GameObject.Instantiate(__instance.variationMemory.gameObject, newInformation.transform);
                            toggleObj.transform.localPosition = new Vector3(247f, -15f, 0f);
                            Toggle toggle = toggleObj.GetComponent<Toggle>();
                            toggle.isOn = info.enabled;
                            toggle.onValueChanged = new Toggle.ToggleEvent();
                            toggle.onValueChanged.AddListener(delegate
                            {
                                info.enabled = !info.enabled;
                                toggle.isOn = info.enabled;
                                newButton.targetGraphic.color = info.enabled ? Color.green : Color.red;
                                if (info.enabled == true)
                                {
                                    for (int i = 0; i < esh.shitstuff.Count; i++)
                                    {
                                        if (esh.shitstuff[i].id == id)
                                        {
                                            Debug.Log("added " + id + " to enabled enemies");
                                            EnemiesEnabled.Instance.enemiesEnabled.Add(esh.shitstuff[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < esh.shitstuff.Count; i++)
                                    {
                                        if (esh.shitstuff[i].id == id)
                                        {
                                            Debug.Log("removed " + id + " from enabled enemies");
                                            EnemiesEnabled.Instance.enemiesEnabled.Remove(esh.shitstuff[i]);
                                        }
                                    }
                                }
                            });
                            EventTrigger trigger = toggle.gameObject.AddComponent<EventTrigger>();
                            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
                            hoverEntry.eventID = EventTriggerType.PointerEnter;
                            hoverEntry.callback.AddListener(delegate
                            {
                                hoverText.SetActive(true);
                            });
                            EventTrigger.Entry unHoverEntry = new EventTrigger.Entry();
                            unHoverEntry.eventID = EventTriggerType.PointerExit;
                            unHoverEntry.callback.AddListener(delegate
                            {
                                hoverText.SetActive(false);
                            });
                            trigger.triggers.Add(hoverEntry);
                            trigger.triggers.Add(unHoverEntry);

                            toggleObj.SetActive(true);
                            newInformation.SetActive(true);
                        }

                        cRect.sizeDelta = new Vector2(600f, information.Count * 60); // setting the scrollbar fit all of the mods
                    }
                    else
                    {
                        Debug.Log("NOTHING FOUND!!!!!!");
                    }
                }        

                __instance.variationMemory.gameObject.SetActive(true);
                MonoSingleton<PrefsManager>.Instance.SetBool("variationMemory", wasOn); // there's a bug where this patch sets variation memory to always be on once you get to the menu fo romse reason, this fixes that

                Button.ButtonClickedEvent modsButton = newModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                modsButton.AddListener(delegate
                {
                    __instance.CheckIfTutorialBeaten();
                    __instance.pauseMenu.SetActive(false);
                    modsMenu.SetActive(true);
                    dothing();
                });
                newModsButton.SetActive(true);
            }
        }
    }
}
