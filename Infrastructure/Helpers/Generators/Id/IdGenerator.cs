namespace Infrastructure.Helpers.Generators.Id;

public class IdGenerator
{
    public static string GUIDGenerator()
    {
        return Guid.NewGuid().ToString();
    }
}
