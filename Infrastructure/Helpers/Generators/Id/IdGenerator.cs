namespace Infrastructure.Helpers.Generators.Id;

public class IdGenerator
{
    public static string GUIDGenerator() => Guid.NewGuid().ToString();
    
    public static string Product(string prefix = "pr-") => prefix + GUIDGenerator();
    
}
