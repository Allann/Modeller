namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Types of trucks used in garbage collection and recycling operations
/// </summary>
public enum TruckType
{
    /// <summary>
    /// Standard garbage collection truck
    /// </summary>
    Garbage = 1,

    /// <summary>
    /// Recycling collection truck
    /// </summary>
    Recycling = 2,

    /// <summary>
    /// Bulk waste collection truck
    /// </summary>
    BulkWaste = 3,

    /// <summary>
    /// Green waste collection truck
    /// </summary>
    GreenWaste = 4,

    /// <summary>
    /// Commercial waste truck
    /// </summary>
    Commercial = 5,

    /// <summary>
    /// Compactor truck
    /// </summary>
    Compactor = 6,

    /// <summary>
    /// Hook lift truck
    /// </summary>
    HookLift = 7,

    /// <summary>
    /// Side loader truck
    /// </summary>
    SideLoader = 8,

    /// <summary>
    /// Rear loader truck
    /// </summary>
    RearLoader = 9,

    /// <summary>
    /// Front loader truck
    /// </summary>
    FrontLoader = 10,

    /// <summary>
    /// Multi-purpose utility truck
    /// </summary>
    Utility = 11,

    /// <summary>
    /// Other specialized truck type
    /// </summary>
    Other = 99
}
