using System;
using System.Collections.Generic;
using System.Text;
using UltraRandomizer.MenuSystem;

namespace UltraRandomizer.HarmonyPatches
{
    public class EnemiesEnabled : MonoSingleton<EnemiesEnabled>
    {
        public List<EnemySetting> enemiesEnabled = new List<EnemySetting>();
    }
}
