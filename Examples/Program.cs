using System.Threading.Tasks;

public class Example
{
    public static async Task Main(string[] args)
    {
        // Create units
        Unit hero = new() { Name = "Hero", Health = 100, Attack = 20, Defense = 5 };
        Unit monster = new() { Name = "Monster", Health = 80, Attack = 15, Defense = 3 };

        // Check monster's health
        Console.WriteLine($"{monster.Name} has {monster.Health} health left.");

        var jsonPath = "C:/Users/vince/Nextcloud/Documents/Projects/CardGame/test/db_mock.json";
        var abilities = await AbilityService.LoadAbilitiesFromJsonAsync(jsonPath);
        abilities[0].Execute(hero, monster);
    }
}