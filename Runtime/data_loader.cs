using System.Text.Json;
using System.Text.Json.Serialization;
namespace DataDrivenAbilitySystem
{
    public class AbilityService
    {
        public async static Task<List<Ability>> LoadAbilitiesFromJsonAsync(string jsonPath)
        {
            try
            {
                using var fileStream = File.OpenRead(jsonPath);
                var abilitiesDTO = await JsonSerializer.DeserializeAsync<List<AbilityDTO>>(
                    fileStream,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    }
                ) ?? throw new Exception("No abilities found in the JSON file.");

                return abilitiesDTO.Select(ConvertToAbility).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading abilities: {ex.Message}");
                return new List<Ability>();
            }
        }

        private static (IEffectContext, IAbilityEffect) ConvertToContextEffect(EffectDTO effectDTO)
        {
            IEffectContext context;
            Console.WriteLine($"Converting effect: {effectDTO.Type} with target: {effectDTO.Target}");
            switch (effectDTO.Target.ToLower())
            {
                case "self":
                    context = new SelfContext();
                    break;
                case "target":
                    context = new TargetContext();
                    break;
                // Add more cases for other contexts as needed
                default:
                    throw new ArgumentException($"Unknown target context: {effectDTO.Target}");
            }
            IAbilityEffect effect;
            switch (effectDTO.Type.ToLower())
            {
                case "damage":
                    effect = new DamageEffect();
                    break;
                // Add more cases for other effects as needed
                default:
                    throw new ArgumentException($"Unknown effect: {effectDTO.Type}");
            }
            return (context, effect);
        }

        private static Ability ConvertToAbility(AbilityDTO dto)
        {
            var effects = dto.Effects.Select(ConvertToContextEffect).ToList();
            return new Ability()
            {
                Name = dto.Name,
                Power = dto.Power ?? 0,
                Effects = effects
            };
        }
    }
}