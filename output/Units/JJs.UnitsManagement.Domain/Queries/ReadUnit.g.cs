namespace JJs.UnitsManagement;

/// <summary>
/// Retrieves a specific unit by its unique identifier
/// </summary>
public sealed record ReadUnit
{
    internal ReadUnit(
        Guid publicId,
        Guid unitId) =>
        (PublicId, UnitId) =
        (publicId, unitId);

    public Guid PublicId { get; }
    public Guid UnitId { get; }
}

/// <summary>
/// Extension methods for ReadUnit
/// </summary>
public static class ReadUnitExtensions
{
    extension(ReadUnit)
    {
        public static ReadUnit? New(
            Guid unitId) =>
            ReadUnit.CreateValid(Guid.CreateVersion7(), unitId);

        public static ReadUnit? CreateValid(
            Guid publicId,
            Guid unitId) =>
            publicId.Version != 7 ? null
            : new ReadUnit(publicId, unitId);
    }
}

