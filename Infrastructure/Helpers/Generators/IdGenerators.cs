namespace Infrastructure.Helpers.Generators;

public class IdGenerators
{
    public static string GUIDGenerator() => Guid.NewGuid().ToString();
    
    public static string Product(string prefix = "pr-") => prefix + GUIDGenerator();
    
}
