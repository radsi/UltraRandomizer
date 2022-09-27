using System;
using System.Collections.Generic;
using System.Text;

namespace UltraRandomizer.MenuSystem
{
    public class EnemySetting
    {
        public string displayname = "oi cuzz";
        public bool enabled = false;
        public int spawnarmindex;
        public string id;

        public EnemySetting(string displayname, string id,int spawnarmindex)
        {
            this.id = id;
            this.displayname = displayname;
            this.spawnarmindex = spawnarmindex;
        }
    }
}
