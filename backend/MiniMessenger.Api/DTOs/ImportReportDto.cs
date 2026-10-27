namespace MiniMessenger.Api.DTOs;

public class ImportReportDto
{
    public int TotalRows { get; set; }
    public int Imported { get; set; }
    public int Duplicates { get; set; }
    public int Invalid { get; set; }
    public List<ImportErrorDto> Errors { get; set; } = new();   // limité aux 100 premières
}

public class ImportErrorDto
{
    public int Line { get; set; }
    public string Message { get; set; } = string.Empty;
}