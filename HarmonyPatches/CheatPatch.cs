using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraRandomizer.HarmonyPatches
{
	[HarmonyPatch(typeof(CheatsManager), "Start", new Type[] { })]
	public class CheatPatch
    {
		public static void Prefix(CheatsManager __instance)
		{
			__instance.RebuildIcons();
			__instance.RegisterCheat(new TheCheat(), "mods");
		}
	}
}
