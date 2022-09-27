using System;
using System.Collections.Generic;
using System.Text;

namespace UltraRandomizer.MenuSystem
{
    public class EnemySetting
    {
        public int id;
        public string displayName = "oi cuzz";
        public bool enabled = false;

        public EnemySetting(string displayname, int id)
        {
            this.id = id;
            this.displayName = displayname;
        }
    }
}
