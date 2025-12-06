namespace JJs.UnitsManagement;

/// <summary>
/// Extension methods for <see cref="Guid"/>.
/// </summary>
public static class GuidExtensions
{
    extension(Guid)
    {
        /// <summary>
        /// Determines whether this GUID is a version 7 UUID.
        /// </summary>
        /// <returns>true if this is a version 7 UUID; otherwise, false.</returns>
        public bool IsVersion7() => Version == 7;
    }
}

