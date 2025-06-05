using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace DataDrivenAbilitySystem
{
    public class Unit
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }

        public void TakeDamage(int damage)
        {
            int actualDamage = damage - Defense;
            if (actualDamage < 0) actualDamage = 0;
            Health -= actualDamage;
            if (Health < 0) Health = 0;
        }

        public bool IsAlive()
        {
            return Health > 0;
        }
    }
}