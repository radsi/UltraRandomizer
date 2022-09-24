using System;
using System.Collections.Generic;
using System.Text;

namespace UltraRandomizer
{
    public class DifficultiesHandler
    {
        private List<Difficulty> difficulties = new List<Difficulty>();

        public void New(int[] enemies)
        {
            var newdiff = new Difficulty();
            newdiff.enemies = enemies;

            difficulties.Add(newdiff);
        }

        public Difficulty GetDifficulty(int index)
        {
            return difficulties[index];
        }
    }
}
