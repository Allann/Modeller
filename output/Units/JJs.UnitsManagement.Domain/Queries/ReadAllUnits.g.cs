namespace JJs.UnitsManagement;

/// <summary>
/// Retrieves all units with optional pagination and filtering
/// </summary>
public sealed record ReadAllUnits
{
    internal ReadAllUnits(
        Guid publicId,
        int? page = null,
        int? pageSize = null,
        bool? activeOnly = null,
        string? searchTerm = null,
        TruckType? truckType = null,
        string? company = null,
        string? state = null) =>
        (PublicId, Page, PageSize, ActiveOnly, SearchTerm, TruckType, Company, State) =
        (publicId, page, pageSize, activeOnly, searchTerm, truckType, company, state);

    public Guid PublicId { get; }
    public int? Page { get; }
    public int? PageSize { get; }
    public bool? ActiveOnly { get; }
    public string? SearchTerm { get; }
    public TruckType? TruckType { get; }
    public string? Company { get; }
    public string? State { get; }
}

/// <summary>
/// Extension methods for ReadAllUnits
/// </summary>
public static class ReadAllUnitsExtensions
{
    extension(ReadAllUnits)
    {
        public static ReadAllUnits? New(
            int? page = null,
            int? pageSize = null,
            bool? activeOnly = null,
            string? searchTerm = null,
            TruckType? truckType = null,
            string? company = null,
            string? state = null) =>
            ReadAllUnits.CreateValid(Guid.CreateVersion7(), page, pageSize, activeOnly, searchTerm, truckType, company, state);

        public static ReadAllUnits? CreateValid(
            Guid publicId,
            int? page = null,
            int? pageSize = null,
            bool? activeOnly = null,
            string? searchTerm = null,
            TruckType? truckType = null,
            string? company = null,
            string? state = null) =>
            publicId.Version != 7 ? null
            : new ReadAllUnits(publicId, page, pageSize, activeOnly, searchTerm, truckType, company, state);
    }
}

