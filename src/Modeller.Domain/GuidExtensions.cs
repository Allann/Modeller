namespace Modeller.Domain;

/// <summary>
/// Extension methods for Guid validation
/// </summary>
public static class GuidExtensions
{
    extension(Guid guid)
    {
        /// <summary>
        /// Checks if the Guid is a version 7 (time-ordered) UUID
        /// </summary>
        public bool IsVersion7() => guid.Version == 7;
    }
}
