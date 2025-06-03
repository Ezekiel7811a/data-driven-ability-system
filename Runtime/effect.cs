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