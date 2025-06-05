using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace DataDrivenAbilitySystem
{
    public class SelfContext : IEffectContext
    {
        public Unit Caster { get; private set; } = null!;
        public Unit Target { get; private set; } = null!;
        public int Power { get; private set; }
        public void Apply(Unit caster, Unit target, int power)
        {
            Caster = caster;
            Target = caster;
            Power = power;
        }
    }

    public class TargetContext : IEffectContext
    {
        public Unit Caster { get; private set; } = null!;
        public Unit Target { get; private set; } = null!;
        public int Power { get; private set; }
        public void Apply(Unit caster, Unit target, int power)
        {
            Caster = caster;
            Target = target;
            Power = power;
        }
    }
    public class DamageEffect : IAbilityEffect
    {

        public void Apply(IEffectContext context)
        {
            Unit caster = context.Caster;
            Unit target = context.Target;
            int damage = context.Power;

            target.TakeDamage(damage);
            Console.WriteLine($"{caster.Name} uses {nameof(DamageEffect)} on {target.Name}, dealing {damage} damage.");
        }
    }
}