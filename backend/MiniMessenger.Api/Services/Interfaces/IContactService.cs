// using MiniMessenger.Api.DTOs;

// namespace MiniMessenger.Api.Services.Interfaces;

// public interface IContactService
// {
//     Task<PagedResultDto<ContactDto>> GetPagedAsync(int ownerId, int page, int pageSize);

//     // Lève ArgumentException si le fichier est inutilisable (vide, colonnes manquantes)
//     Task<ImportReportDto> ImportCsvAsync(int ownerId, Stream csvStream);
// }


using MiniMessenger.Api.DTOs;

namespace MiniMessenger.Api.Services.Interfaces;

public interface IContactService
{
    Task<PagedResultDto<ContactDto>> GetPagedAsync(string ownerId, int page, int pageSize);

    // Lève ArgumentException si le fichier est inutilisable (vide, colonnes manquantes)
    Task<ImportReportDto> ImportCsvAsync(string ownerId, Stream csvStream);
}