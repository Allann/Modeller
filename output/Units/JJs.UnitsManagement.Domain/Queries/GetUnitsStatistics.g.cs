namespace JJs.UnitsManagement;

/// <summary>
/// Retrieves comprehensive statistics about units for dashboard display
/// </summary>
public sealed record GetUnitsStatistics
{
    internal GetUnitsStatistics(
        Guid publicId) =>
        (PublicId) =
        (publicId);

    public Guid PublicId { get; }
}

/// <summary>
/// Extension methods for GetUnitsStatistics
/// </summary>
public static class GetUnitsStatisticsExtensions
{
    extension(GetUnitsStatistics)
    {
        public static GetUnitsStatistics? New() =>
            GetUnitsStatistics.CreateValid(Guid.CreateVersion7());

        public static GetUnitsStatistics? CreateValid(
            Guid publicId) =>
            publicId.Version != 7 ? null
            : new GetUnitsStatistics(publicId);
    }
}

