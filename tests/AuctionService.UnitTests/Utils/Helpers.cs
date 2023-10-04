using System.Security.Claims;

namespace AuctionService.UnitTests;

public class Helpers
{
    //This claims will be using for mock the User Identity in pur
    //Controller test
    //The endpoints that needs Authorize
    public static ClaimsPrincipal GetClaimsPrincipal()
    {
        var claims = new List<Claim>{new Claim(ClaimTypes.Name, "test")};
        var identity = new ClaimsIdentity(claims, "testing");
        return new ClaimsPrincipal(identity);
    }
}