using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace DataDrivenAbilitySystem
{
    public interface IAbilityEffect
    {
        public void Apply(IEffectContext context);
    }

    public class Ability
    {
        public string Name { get; set; } = "";
        public List<(IEffectContext Context, IAbilityEffect Effect)> Effects { get; set; } = new();
        public int Power { get; set; }
        public void Execute(Unit caster, Unit target)
        {
            Effects.ForEach(item =>
            {
                item.Context.Apply(caster, target, Power);
                item.Effect.Apply(item.Context);
            });
        }
    }
}