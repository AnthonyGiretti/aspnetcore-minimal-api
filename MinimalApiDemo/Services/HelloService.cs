namespace MinimalApiDemo.Services;

public interface IHelloService
{
    string Hello(ClaimsPrincipal user, bool isHappy);
}

public class HelloService : IHelloService
{
    public string Hello(ClaimsPrincipal user, bool isHappy)
    {
        var hello = $"Hello {user.Identity.Name}";

        if (isHappy)
            return $"{hello}, you seem to be happy today";
        return hello;
    }
}