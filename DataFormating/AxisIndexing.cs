using System;
using System.Collections.Generic;

namespace ThrustMetrics.FlightPerformanceData.AxisIndexing
{
    /// <summary>
    /// Axis indexing contract.
    ///
    /// Purpose:
    /// - Convert continuous numeric axis values (double) into a stable discrete key (long)
    ///   suitable for persistence, sorting, fast range search and neighbor lookup.
    /// - Provide the metadata required to reproduce the mapping deterministically.
    ///
    /// Notes:
    /// - This interface belongs to the "Data / Acquisition / Modeling" domain, not the engine.
    /// - The engine should treat the key as an opaque, ordered identifier.
    /// </summary>
    public interface IAxisIndexingStrategy
    {
        /// <summary>
        /// A short stable identifier of the strategy (e.g. "dec10-round-away-v1").
        /// Store this in persistence (Axis metadata) to support migrations/versioning.
        /// </summary>
        string StrategyId { get; }

        /// <summary>
        /// Build axis indexing metadata from a set of samples (typically PerfPoint.input).
        ///
        /// Contract:
        /// - Must be deterministic for identical inputs + settings.
        /// - Must guard against overflow when mapping to long.
        /// - May choose a quantization exponent (p) or other parameters.
        /// </summary>
        AxisIndexMeta BuildMeta(IEnumerable<double> inputs, AxisIndexingBuildOptions? options = null);

        /// <summary>
        /// Build axis indexing metadata from an explicit design bound (max |x|).
        /// Useful when you know the axis domain without scanning all points,
        /// or when you want to freeze p even if the dataset evolves.
        /// </summary>
        AxisIndexMeta BuildMeta(double maxAbsDesign, AxisIndexingBuildOptions? options = null);

        /// <summary>
        /// Convert a value to its discrete key using the provided metadata.
        /// Must be deterministic, monotonic (increasing x => non-decreasing key), and stable.
        /// </summary>
        long ToKey(double value, in AxisIndexMeta meta);

        /// <summary>
        /// Inverse mapping (approximate): key -> representative value.
        /// Typically value = key / multiplier for decimal strategies.
        /// </summary>
        double FromKey(long key, in AxisIndexMeta meta);

        /// <summary>
        /// Optional helper: returns the implicit tolerance induced by quantization.
        /// This is typically meta.Epsilon for rounding-based strategies.
        /// </summary>
        double GetEpsilon(in AxisIndexMeta meta);
    }

    /// <summary>
    /// Strategy-agnostic metadata stored alongside the axis.
    ///
    /// IMPORTANT:
    /// - ExponentP (or more generally "parameters") are the source of truth.
    /// - Multipliers are derived for speed.
    /// - Keep this struct small and immutable; it may be copied a lot.
    /// </summary>
    public readonly struct AxisIndexMeta
    {
        // --- Generic identification ---
        public readonly string StrategyId;     // e.g. "dec10-round-away-v1"
        public readonly int StrategyVersion;   // for future-proofing

        // --- Common parameters for decimal quantization strategies ---
        public readonly int ExponentP;         // p such that M=10^p
        public readonly double Multiplier;     // M=10^p
        public readonly double Step;           // 1/M
        public readonly double Epsilon;        // 0.5/M (rounding half-step)
        public readonly double MaxAbsDesign;   // design bound used to compute p (traceability)

        public AxisIndexMeta(
            string strategyId,
            int strategyVersion,
            int exponentP,
            double maxAbsDesign)
        {
            StrategyId = strategyId ?? throw new ArgumentNullException(nameof(strategyId));
            StrategyVersion = strategyVersion;

            ExponentP = exponentP;
            Multiplier = Math.Pow(10.0, exponentP);

            Step = 1.0 / Multiplier;
            Epsilon = 0.5 / Multiplier;

            MaxAbsDesign = maxAbsDesign;
        }

        public override string ToString()
            => $"{StrategyId}/v{StrategyVersion}: p={ExponentP}, M={Multiplier:G}, step={Step:G}, eps={Epsilon:G}, A={MaxAbsDesign:G}";
    }

    /// <summary>
    /// Options passed to BuildMeta().
    /// Keep them strategy-agnostic where possible; strategy-specific options can be added
    /// via derived types or a dictionary later if needed.
    /// </summary>
    public sealed class AxisIndexingBuildOptions
    {
        /// <summary>Target significant digits near the largest magnitude (defaults to 10).</summary>
        public int SignificantDigits { get; init; } = 10;

        /// <summary>Lower bound on exponent p (e.g. 5 means multiplier at least 1e5 when feasible).</summary>
        public int MinExponentP { get; init; } = 5;

        /// <summary>
        /// Upper bound on exponent p (prevents over-quantization, especially for small-magnitude axes like Mach).
        /// Typical safe values: 7 or 8.
        /// </summary>
        public int MaxExponentP { get; init; } = 8;

        /// <summary>Safety margin: A_design = A_observed * (1 + MarginRatio).</summary>
        public double MarginRatio { get; init; } = 0.05;

        /// <summary>
        /// If > 0, ignore the top fraction of |x| values before taking max.
        /// Default 0 = disabled.
        /// </summary>
        public double TrimTopFraction { get; init; } = 0.0;

        /// <summary>If true, throws on NaN/Infinity in inputs. If false, skips them.</summary>
        public bool StrictFiniteInputs { get; init; } = true;
    }
}
