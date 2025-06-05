using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace DataDrivenAbilitySystem
{
    public interface IEffectContext
    {
        public Unit Caster { get; }
        public Unit Target { get; }
        public int Power { get; }
        public void Apply(Unit caster, Unit target, int power);
    }
}