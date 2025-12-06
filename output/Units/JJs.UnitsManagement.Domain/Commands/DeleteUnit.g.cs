namespace JJs.UnitsManagement;

/// <summary>
/// Deletes a unit from the system
/// </summary>
public sealed record DeleteUnit
{
    internal DeleteUnit(
        Guid publicId,
        Guid unitId) =>
        (PublicId, UnitId) =
        (publicId, unitId);

    public Guid PublicId { get; }
    public Guid UnitId { get; }
}

/// <summary>
/// Extension methods for DeleteUnit
/// </summary>
public static class DeleteUnitExtensions
{
    extension(DeleteUnit)
    {
        public static DeleteUnit? New(
            Guid unitId) =>
            DeleteUnit.CreateValid(Guid.CreateVersion7(), unitId);

        public static DeleteUnit? CreateValid(
            Guid publicId,
            Guid unitId) =>
            publicId.Version != 7 ? null
            : new DeleteUnit(publicId, unitId);
    }
}

