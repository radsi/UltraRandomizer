using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace UltraRandomizer
{
    public class EnemyBalanceHandler
    {
        public List<EnemyBalance> enemiesToBalance = new List<EnemyBalance>();
        int balanceHealth = 15;

        public void BalanceEnemies()
        {
            foreach (EnemyBalance eb in enemiesToBalance)
            {
                if (eb.eid.health > balanceHealth)
                {
                    eb.eid.health = balanceHealth;
                    Debug.Log("oi cuzzz balanced this fucking doozy lookin wanker");
                }
            }
        }
    }
}
