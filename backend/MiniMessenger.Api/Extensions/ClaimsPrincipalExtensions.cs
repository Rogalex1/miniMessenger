using System.Security.Claims;

namespace MiniMessenger.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        // Selon la configuration, le nom du claim varie : on teste les trois possibles
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue("nameid")
                    ?? user.FindFirstValue("sub");

        return int.TryParse(value, out var id)
            ? id
            : throw new UnauthorizedAccessException("Jeton invalide : identifiant manquant.");
    }
}