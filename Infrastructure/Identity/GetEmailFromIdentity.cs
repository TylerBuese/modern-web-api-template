using System.Security.Claims;
using System.Security.Principal;

public static class GetEmailFromIdentity
{
    public static string Email(this IIdentity identity)
    {
        if (identity == null)
            return "noemail";

        var email = (identity as ClaimsIdentity).Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault();
        if (email == null)
            return "noemail";

        return email.Value;
    }
}