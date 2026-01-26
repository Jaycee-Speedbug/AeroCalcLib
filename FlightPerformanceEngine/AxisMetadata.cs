
using AeroCalcCore.FlightPerformanceEngine;

/// <summary>
/// Metadata describing how the axis behaves (discrete vs continuous, snapping, breakpoints).
/// This belongs to the MODEL.
/// </summary>
public sealed record AxisMetadata(AxisNature Nature, 
                                  BreakpointsPolicy BreakpointsPolicy = BreakpointsPolicy.HardStop,
                                  DiscreteSelectionMode DiscreteMode = DiscreteSelectionMode.ExactOnly,
                                  double? SnapMaxDistance = null)
{
    public bool SnappingAllowed =>
        Nature == AxisNature.Discrete && DiscreteMode != DiscreteSelectionMode.ExactOnly;
}


// ------------------------------------------------------------------------
// 1) MODEL METADATA (what the model ALLOWS)
// ------------------------------------------------------------------------

public enum AxisNature
{
    /// <summary>Continuous factor axis (interpolation allowed).</summary>
    Continuous,

    /// <summary>Discrete factor axis (exact value required unless snapping is allowed).</summary>
    Discrete
}

public enum DiscreteSelectionMode
{
    /// <summary>Exact match required (within epsilon). Otherwise: no selection.</summary>
    ExactOnly,

    /// <summary>Snap to the nearest value (ties resolved by a deterministic rule).</summary>
    SnapToNearest,

    /// <summary>Snap to the nearest lower-or-equal value (within max distance if set).</summary>
    SnapDown,

    /// <summary>Snap to the nearest greater-or-equal value (within max distance if set).</summary>
    SnapUp
}

public enum BreakpointsPolicy
{
    /// <summary>Never cross a breakpoint when selecting around x.</summary>
    HardStop,

    /// <summary>
    /// Breakpoints are "hints" (rare). The selector may cross if needed.
    /// Keep for future; you will likely use HardStop everywhere.
    /// </summary>
    SoftPreference
}

