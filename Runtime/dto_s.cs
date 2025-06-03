namespace DataDrivenAbilitySystem
{
    public class AbilityDTO
    {
        public string Name { get; set; } = "";
        public int? Power { get; set; }
        public List<EffectDTO> Effects { get; set; } = new();
    }

    public class EffectDTO
    {
        public string Type { get; set; } = "";
        public string Target { get; set; } = "";
    }
}