// using System.Globalization;
// using System.Text;
// using CsvHelper;
// using CsvHelper.Configuration;
// using Microsoft.EntityFrameworkCore;
// using MiniMessenger.Api.Data;
// using MiniMessenger.Api.DTOs;
// using MiniMessenger.Api.Models;
// using MiniMessenger.Api.Services.Interfaces;

// namespace MiniMessenger.Api.Services;

// public class ContactService : IContactService
// {
//     private const int BatchSize = 500;
//     private const int MaxReportedErrors = 100;

//     private readonly AppDbContext _db;

//     public ContactService(AppDbContext db)
//     {
//         _db = db;
//     }

//     public async Task<PagedResultDto<ContactDto>> GetPagedAsync(int ownerId, int page, int pageSize)
//     {
//         page = Math.Max(page, 1);
//         pageSize = Math.Clamp(pageSize, 1, 100);

//         var query = _db.Contacts.AsNoTracking().Where(c => c.OwnerId == ownerId);

//         var total = await query.CountAsync();

//         var items = await query
//             .OrderBy(c => c.FirstName).ThenBy(c => c.Id)
//             .Skip((page - 1) * pageSize)
//             .Take(pageSize)
//             .Select(c => new ContactDto
//             {
//                 Id = c.Id,
//                 FirstName = c.FirstName,
//                 LastName = c.LastName,
//                 PhoneNumber = c.PhoneNumber,
//                 Email = c.Email,
//                 IsOptedOut = c.IsOptedOut
//             })
//             .ToListAsync();

//         return new PagedResultDto<ContactDto>
//         {
//             Items = items,
//             Total = total,
//             Page = page,
//             PageSize = pageSize
//         };
//     }

//     public async Task<ImportReportDto> ImportCsvAsync(int ownerId, Stream csvStream)
//     {
//         var report = new ImportReportDto();

//         var config = new CsvConfiguration(CultureInfo.InvariantCulture)
//         {
//             HasHeaderRecord = true,
//             DetectDelimiter = true,          // gère "," et ";"
//             MissingFieldFound = null,
//             HeaderValidated = null,
//             BadDataFound = null,
//             PrepareHeaderForMatch = args => NormalizeHeader(args.Header)
//         };

//         using var reader = new StreamReader(csvStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
//         using var csv = new CsvReader(reader, config);

//         if (!await csv.ReadAsync() || !csv.ReadHeader())
//             throw new ArgumentException("Le fichier est vide.");

//         var headers = (csv.HeaderRecord ?? Array.Empty<string>())
//             .Select(NormalizeHeader)
//             .ToHashSet();

//         if (!headers.Contains("prenom") || (!headers.Contains("telephone") && !headers.Contains("email")))
//             throw new ArgumentException(
//                 "Colonnes attendues : prenom (obligatoire), nom, telephone, email (au moins l'un des deux).");

//         var batch = new List<Contact>();
//         var seenPhones = new HashSet<string>();
//         var seenEmails = new HashSet<string>();

//         while (await csv.ReadAsync())
//         {
//             report.TotalRows++;
//             var line = csv.Parser.Row;   // numéro de ligne dans le fichier (l'en-tête est la ligne 1)

//             csv.TryGetField<string>("prenom", out var firstName);
//             csv.TryGetField<string>("nom", out var lastName);
//             csv.TryGetField<string>("telephone", out var phoneRaw);
//             csv.TryGetField<string>("email", out var emailRaw);

//             firstName = firstName?.Trim();
//             lastName = lastName?.Trim();

//             if (string.IsNullOrWhiteSpace(firstName))
//             {
//                 AddError(report, line, "Prénom manquant.");
//                 continue;
//             }
//             if (firstName.Length > 100 || (lastName?.Length ?? 0) > 100)
//             {
//                 AddError(report, line, "Nom ou prénom trop long (100 caractères maximum).");
//                 continue;
//             }
//             if (!ContactNormalizer.TryNormalizePhone(phoneRaw, out var phone))
//             {
//                 AddError(report, line, "Numéro de téléphone invalide.");
//                 continue;
//             }
//             if (!ContactNormalizer.TryNormalizeEmail(emailRaw, out var email))
//             {
//                 AddError(report, line, "Adresse e-mail invalide.");
//                 continue;
//             }
//             if (phone is null && email is null)
//             {
//                 AddError(report, line, "Téléphone ou e-mail requis.");
//                 continue;
//             }

//             // Doublon à l'intérieur du fichier
//             if ((phone is not null && !seenPhones.Add(phone)) ||
//                 (email is not null && !seenEmails.Add(email)))
//             {
//                 report.Duplicates++;
//                 continue;
//             }

//             batch.Add(new Contact
//             {
//                 OwnerId = ownerId,
//                 FirstName = firstName,
//                 LastName = string.IsNullOrWhiteSpace(lastName) ? null : lastName,
//                 PhoneNumber = phone,
//                 Email = email
//             });

//             if (batch.Count >= BatchSize)
//                 await FlushAsync(ownerId, batch, report);
//         }

//         await FlushAsync(ownerId, batch, report);
//         return report;
//     }

//     // Insère un paquet après avoir écarté les contacts déjà présents en base
//     private async Task FlushAsync(int ownerId, List<Contact> batch, ImportReportDto report)
//     {
//         if (batch.Count == 0) return;

//         var phones = batch.Where(c => c.PhoneNumber != null).Select(c => c.PhoneNumber!).ToList();
//         var emails = batch.Where(c => c.Email != null).Select(c => c.Email!).ToList();

//         // Une seule requête pour tout le paquet (et non une par contact)
//         var existing = await _db.Contacts.AsNoTracking()
//             .Where(c => c.OwnerId == ownerId &&
//                         ((c.PhoneNumber != null && phones.Contains(c.PhoneNumber)) ||
//                          (c.Email != null && emails.Contains(c.Email))))
//             .Select(c => new { c.PhoneNumber, c.Email })
//             .ToListAsync();

//         var existingPhones = existing.Where(e => e.PhoneNumber != null).Select(e => e.PhoneNumber!).ToHashSet();
//         var existingEmails = existing.Where(e => e.Email != null).Select(e => e.Email!).ToHashSet();

//         var toInsert = new List<Contact>();
//         foreach (var contact in batch)
//         {
//             var isDuplicate =
//                 (contact.PhoneNumber != null && existingPhones.Contains(contact.PhoneNumber)) ||
//                 (contact.Email != null && existingEmails.Contains(contact.Email));

//             if (isDuplicate) report.Duplicates++;
//             else toInsert.Add(contact);
//         }

//         _db.Contacts.AddRange(toInsert);
//         await _db.SaveChangesAsync();
//         _db.ChangeTracker.Clear();   // évite que le suivi EF grossisse à chaque paquet

//         report.Imported += toInsert.Count;
//         batch.Clear();
//     }

//     private static void AddError(ImportReportDto report, int line, string message)
//     {
//         report.Invalid++;
//         if (report.Errors.Count < MaxReportedErrors)
//             report.Errors.Add(new ImportErrorDto { Line = line, Message = message });
//     }

//     // "Prénom", "PRENOM", " téléphone " -> "prenom", "telephone"
//     private static string NormalizeHeader(string header)
//     {
//         var decomposed = header.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
//         var sb = new StringBuilder();
//         foreach (var c in decomposed)
//         {
//             if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
//                 sb.Append(c);
//         }
//         return sb.ToString().Normalize(NormalizationForm.FormC);
//     }
// }



using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MongoDB.Driver;
using MiniMessenger.Api.DTOs;
using MiniMessenger.Api.Models;
using MiniMessenger.Api.Services.Interfaces;

namespace MiniMessenger.Api.Services;

public class ContactService : IContactService
{
    private const int BatchSize = 500;
    private const int MaxReportedErrors = 100;

    private readonly IMongoCollection<Contact> _contactsCollection;

    public ContactService(IMongoDatabase database)
    {
        _contactsCollection = database.GetCollection<Contact>("Contacts");
    }

    public async Task<PagedResultDto<ContactDto>> GetPagedAsync(string ownerId, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var filter = Builders<Contact>.Filter.Eq(c => c.OwnerId, ownerId);

        var total = await _contactsCollection.CountDocumentsAsync(filter);

        var items = await _contactsCollection
            .Find(filter)
            .Sort(Builders<Contact>.Sort.Ascending(c => c.FirstName).Ascending(c => c.Id))
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        var contactDtos = items.Select(c => new ContactDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            PhoneNumber = c.PhoneNumber,
            Email = c.Email,
            IsOptedOut = c.IsOptedOut
        }).ToList();

        return new PagedResultDto<ContactDto>
        {
            Items = contactDtos,
            Total = (int)total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ImportReportDto> ImportCsvAsync(string ownerId, Stream csvStream)
    {
        var report = new ImportReportDto();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            DetectDelimiter = true,            // gère "," et ";"
            MissingFieldFound = null,
            HeaderValidated = null,
            BadDataFound = null,
            PrepareHeaderForMatch = args => NormalizeHeader(args.Header)
        };

        using var reader = new StreamReader(csvStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        using var csv = new CsvReader(reader, config);

        if (!await csv.ReadAsync() || !csv.ReadHeader())
            throw new ArgumentException("Le fichier est vide.");

        var headers = (csv.HeaderRecord ?? Array.Empty<string>())
            .Select(NormalizeHeader)
            .ToHashSet();

        if (!headers.Contains("prenom") || (!headers.Contains("telephone") && !headers.Contains("email")))
            throw new ArgumentException(
                "Colonnes attendues : prenom (obligatoire), nom, telephone, email (au moins l'un des deux).");

        var batch = new List<Contact>();
        var seenPhones = new HashSet<string>();
        var seenEmails = new HashSet<string>();

        while (await csv.ReadAsync())
        {
            report.TotalRows++;
            var line = csv.Parser.Row;   // numéro de ligne dans le fichier

            csv.TryGetField<string>("prenom", out var firstName);
            csv.TryGetField<string>("nom", out var lastName);
            csv.TryGetField<string>("telephone", out var phoneRaw);
            csv.TryGetField<string>("email", out var emailRaw);

            firstName = firstName?.Trim();
            lastName = lastName?.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                AddError(report, line, "Prénom manquant.");
                continue;
            }
            if (firstName.Length > 100 || (lastName?.Length ?? 0) > 100)
            {
                AddError(report, line, "Nom ou prénom trop long (100 caractères maximum).");
                continue;
            }
            if (!ContactNormalizer.TryNormalizePhone(phoneRaw, out var phone))
            {
                AddError(report, line, "Numéro de téléphone invalide.");
                continue;
            }
            if (!ContactNormalizer.TryNormalizeEmail(emailRaw, out var email))
            {
                AddError(report, line, "Adresse e-mail invalide.");
                continue;
            }
            if (phone is null && email is null)
            {
                AddError(report, line, "Téléphone ou e-mail requis.");
                continue;
            }

            // Doublon à l'intérieur du fichier
            if ((phone is not null && !seenPhones.Add(phone)) ||
                (email is not null && !seenEmails.Add(email)))
            {
                report.Duplicates++;
                continue;
            }

            batch.Add(new Contact
            {
                OwnerId = ownerId,
                FirstName = firstName,
                LastName = string.IsNullOrWhiteSpace(lastName) ? null : lastName,
                PhoneNumber = phone,
                Email = email
            });

            if (batch.Count >= BatchSize)
                await FlushAsync(ownerId, batch, report);
        }

        await FlushAsync(ownerId, batch, report);
        return report;
    }

    // Insère un paquet après avoir écarté les contacts déjà présents en base
    private async Task FlushAsync(string ownerId, List<Contact> batch, ImportReportDto report)
    {
        if (batch.Count == 0) return;

        var phones = batch.Where(c => c.PhoneNumber != null).Select(c => c.PhoneNumber!).ToList();
        var emails = batch.Where(c => c.Email != null).Select(c => c.Email!).ToList();

        var filterBuilder = Builders<Contact>.Filter;
        var ownerFilter = filterBuilder.Eq(c => c.OwnerId, ownerId);

        var conditions = new List<FilterDefinition<Contact>>();
        if (phones.Count > 0)
            conditions.Add(filterBuilder.In(c => c.PhoneNumber, phones));
        if (emails.Count > 0)
            conditions.Add(filterBuilder.In(c => c.Email, emails));

        var existing = new List<Contact>();
        if (conditions.Count > 0)
        {
            var combinedConditions = filterBuilder.Or(conditions);
            var finalFilter = filterBuilder.And(ownerFilter, combinedConditions);
            existing = await _contactsCollection.Find(finalFilter).ToListAsync();
        }

        var existingPhones = existing.Where(e => e.PhoneNumber != null).Select(e => e.PhoneNumber!).ToHashSet();
        var existingEmails = existing.Where(e => e.Email != null).Select(e => e.Email!).ToHashSet();

        var toInsert = new List<Contact>();
        foreach (var contact in batch)
        {
            var isDuplicate =
                (contact.PhoneNumber != null && existingPhones.Contains(contact.PhoneNumber)) ||
                (contact.Email != null && existingEmails.Contains(contact.Email));

            if (isDuplicate) report.Duplicates++;
            else toInsert.Add(contact);
        }

        if (toInsert.Count > 0)
        {
            await _contactsCollection.InsertManyAsync(toInsert);
        }

        report.Imported += toInsert.Count;
        batch.Clear();
    }

    private static void AddError(ImportReportDto report, int line, string message)
    {
        report.Invalid++;
        if (report.Errors.Count < MaxReportedErrors)
            report.Errors.Add(new ImportErrorDto { Line = line, Message = message });
    }

    private static string NormalizeHeader(string header)
    {
        var decomposed = header.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}