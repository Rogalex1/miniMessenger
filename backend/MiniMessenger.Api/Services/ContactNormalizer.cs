using System.Net.Mail;
using PhoneNumbers;

namespace MiniMessenger.Api.Services;

public static class ContactNormalizer
{
    private const string DefaultRegion = "BJ";   // pays par défaut quand le numéro n'a pas d'indicatif
    private static readonly PhoneNumberUtil PhoneUtil = PhoneNumberUtil.GetInstance();

    // Champ vide = valide (le champ est optionnel) ; e164 reste alors null
    public static bool TryNormalizePhone(string? raw, out string? e164)
    {
        e164 = null;
        if (string.IsNullOrWhiteSpace(raw)) return true;

        try
        {
            var number = PhoneUtil.Parse(raw.Trim(), DefaultRegion);
            if (!PhoneUtil.IsValidNumber(number)) return false;

            e164 = PhoneUtil.Format(number, PhoneNumberFormat.E164);
            return true;
        }
        catch (NumberParseException)
        {
            return false;
        }
    }

    public static bool TryNormalizeEmail(string? raw, out string? email)
    {
        email = null;
        if (string.IsNullOrWhiteSpace(raw)) return true;

        var trimmed = raw.Trim();
        if (trimmed.Length > 254) return false;

        // refuse aussi les formes du type "Nom <adresse@domaine.com>"
        if (!MailAddress.TryCreate(trimmed, out var address) || address.Address != trimmed)
            return false;

        email = address.Address.ToLowerInvariant();
        return true;
    }
}