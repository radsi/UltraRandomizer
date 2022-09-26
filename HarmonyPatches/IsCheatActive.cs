using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UltraRandomizer.HarmonyPatches
{
    public class IsCheatActive : MonoSingleton<IsCheatActive>
    {
        public bool EnemyEnabled;

        public bool WeaponEnabled;
    }
}
