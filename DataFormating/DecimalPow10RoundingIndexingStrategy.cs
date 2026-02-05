using System;
using System.Collections.Generic;

namespace ThrustMetrics.FlightPerformanceData.AxisIndexing
{
    /// <summary>
    /// Decimal base-10 quantization strategy:
    /// key(x) = round(x * 10^p) with MidpointRounding.AwayFromZero
    /// </summary>
    public sealed class DecimalPow10RoundingIndexingStrategy : IAxisIndexingStrategy
    {
        
        /// <summary>
        /// Gets the unique identifier for the rounding strategy used by this instance.
        /// </summary>
        public string StrategyId => "dec10-round-away";

        /// <summary>
        /// 
        /// </summary>
        private const int StrategyVersionConst = 1;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public AxisIndexMeta BuildMeta(IEnumerable<double> inputs, AxisIndexingBuildOptions? options = null)
        {
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));
            options ??= new AxisIndexingBuildOptions();

            if (options.SignificantDigits < 1) throw new ArgumentOutOfRangeException(nameof(options.SignificantDigits));
            if (options.MinExponentP < 0) throw new ArgumentOutOfRangeException(nameof(options.MinExponentP));
            if (options.MaxExponentP < options.MinExponentP)
                throw new ArgumentOutOfRangeException(nameof(options.MaxExponentP),
                    "MaxExponentP must be >= MinExponentP.");

            if (options.MarginRatio < 0.0) throw new ArgumentOutOfRangeException(nameof(options.MarginRatio));
            if (options.TrimTopFraction < 0.0 || options.TrimTopFraction >= 1.0)
                throw new ArgumentOutOfRangeException(nameof(options.TrimTopFraction));

            double maxAbs = 0.0;
            int countFinite = 0;

            // Collect for trimming only if requested
            List<double>? absValues = options.TrimTopFraction > 0.0
                ? new List<double>(4096)
                : null;

            foreach (var x in inputs)
            {
                if (double.IsNaN(x) || double.IsInfinity(x))
                {
                    if (options.StrictFiniteInputs)
                        throw new ArgumentException("NaN or Infinity encountered in axis inputs.", nameof(inputs));
                    else
                        continue;
                }

                double ax = Math.Abs(x);
                countFinite++;

                if (absValues != null) absValues.Add(ax);
                else if (ax > maxAbs) maxAbs = ax;
            }

            if (countFinite == 0)
            {
                // No usable data → stable fallback
                return BuildMeta(0.0, options);
            }

            if (absValues != null)
            {
                absValues.Sort();
                int n = absValues.Count;
                int keep = (int)Math.Floor(n * (1.0 - options.TrimTopFraction));
                if (keep < 1) keep = 1;
                maxAbs = absValues[keep - 1];
            }

            // Apply safety margin so the chosen p doesn't flip for tiny domain extensions
            double maxAbsDesign = maxAbs * (1.0 + options.MarginRatio);

            return BuildMeta(maxAbsDesign, options);
        }



        /// <summary>
        /// Builds an axis indexing metadata object based on the specified maximum absolute design value and optional
        /// build options.
        /// </summary>
        /// <remarks>If maxAbsDesign is zero, the returned metadata will use the minimal exponent as
        /// specified in the options. The method ensures that the resulting exponent does not exceed the range supported
        /// by long integer values.</remarks>
        /// <param name="maxAbsDesign">The maximum absolute value to be represented by the axis. Must be a finite, non-negative number.</param>
        /// <param name="options">Optional build options that control significant digits and minimum exponent. If null, default options are
        /// used.</param>
        /// <returns>An AxisIndexMeta instance containing metadata for axis indexing, configured according to the specified
        /// design value and options.</returns>
        /// <exception cref="ArgumentException">Thrown if maxAbsDesign is not a finite number.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if options.SignificantDigits is less than 1, or if options.MinExponentP is negative.</exception>
        public AxisIndexMeta BuildMeta(double maxAbsDesign, AxisIndexingBuildOptions? options = null)
        {
            options ??= new AxisIndexingBuildOptions();

            if (double.IsNaN(maxAbsDesign) || double.IsInfinity(maxAbsDesign))
                throw new ArgumentException("maxAbsDesign must be finite.", nameof(maxAbsDesign));

            if (options.SignificantDigits < 1) throw new ArgumentOutOfRangeException(nameof(options.SignificantDigits));

            if (options.MinExponentP < 0) throw new ArgumentOutOfRangeException(nameof(options.MinExponentP));

            double A = Math.Abs(maxAbsDesign);

            // Degenerate axis (constant 0) → keep minimal exponent
            if (A == 0.0)
            {
                return new AxisIndexMeta(
                    StrategyId,
                    StrategyVersionConst,
                    options.MinExponentP,
                    0.0);
            }

            // Significant digits rule:
            // k = floor(log10(A)), p_sig = (N - 1) - k
            const int DoubleSigDigits = 15;
            int k = (int)Math.Floor(Math.Log10(A));
            int pSig = (options.SignificantDigits - 1) - k;
            int pMaxDouble = DoubleSigDigits - 1 - k; // Max p to avoid double precision issues

            int p = Math.Max(options.MinExponentP, pSig);

            // --- long overflow guard ---
            int pMaxLong = (int)Math.Floor(Math.Log10(long.MaxValue / A));
            if (p > pMaxLong) p = pMaxLong;
            if (p < 0) p = 0;

            // --- discipline clamp (only if feasible with long) ---
            int pCeiling = Math.Min(options.MaxExponentP, pMaxLong);
            pCeiling = Math.Min(pCeiling, pMaxDouble);
            int pFloor = options.MinExponentP;

            if (pCeiling < pFloor)
            {
                // Cannot satisfy MinExponentP without overflow; keep best safe p (pCeiling), caller may log.
                p = Math.Max(0, pCeiling);
            }
            else
            {
                // Normal clamp
                if (p < pFloor) p = pFloor;
                if (p > pCeiling) p = pCeiling;
            }


            return new AxisIndexMeta(
                StrategyId,
                StrategyVersionConst,
                p,
                A);
        }



        /// <summary>
        /// Converts a double-precision value to a 64-bit integer key using the specified axis index metadata.
        /// </summary>
        /// <remarks>The conversion multiplies the input value by the multiplier specified in <paramref
        /// name="meta"/> and rounds the result to the nearest integer using midpoint rounding away from zero. This
        /// method ensures that the resulting key is within the valid range for a 64-bit signed integer.</remarks>
        /// <param name="value">The value to convert. Must be a finite double-precision number.</param>
        /// <param name="meta">The axis index metadata that provides the scaling multiplier and strategy identifier for the conversion.</param>
        /// <returns>A 64-bit integer key representing the scaled and rounded value.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not a finite number.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the strategy identifier in <paramref name="meta"/> does not match the expected strategy.</exception>
        /// <exception cref="OverflowException">Thrown if the scaled value exceeds the range of a 64-bit signed integer.</exception>
        public long ToKey(double value, in AxisIndexMeta meta)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("value must be finite.", nameof(value));

            if (!string.Equals(meta.StrategyId, StrategyId, StringComparison.Ordinal))
                throw new InvalidOperationException($"AxisIndexMeta strategy mismatch: {meta.StrategyId} ≠ {StrategyId}");

            double scaled = value * meta.Multiplier;

            // Conservative range check (floating precision safety)
            const double margin = 0.999999999999;
            if (scaled > long.MaxValue * margin || scaled < long.MinValue * margin)
                throw new OverflowException("Scaled value exceeds Int64 range.");

            return checked((long)Math.Round(scaled, 0, MidpointRounding.AwayFromZero));
        }



        /// <summary>
        /// Converts a key value to its corresponding axis value using the specified axis metadata.
        /// </summary>
        /// <param name="key">The key value to convert to an axis value.</param>
        /// <param name="meta">The axis metadata that provides the conversion parameters, including the multiplier and strategy identifier.</param>
        /// <returns>The axis value corresponding to the specified key, calculated using the provided metadata.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the strategy identifier in <paramref name="meta"/> does not match the current strategy.</exception>
        public double FromKey(long key, in AxisIndexMeta meta)
        {
            if (!string.Equals(meta.StrategyId, StrategyId, StringComparison.Ordinal))
                throw new InvalidOperationException($"AxisIndexMeta strategy mismatch: {meta.StrategyId} ≠ {StrategyId}");

            return key / meta.Multiplier;
        }



        /// <summary>
        /// Gets the epsilon value associated with the specified axis metadata.
        /// </summary>
        /// <param name="meta">The axis metadata from which to retrieve the epsilon value.</param>
        /// <returns>The epsilon value defined in the specified <paramref name="meta"/>.</returns>
        public double GetEpsilon(in AxisIndexMeta meta) => meta.Epsilon;

    }
}
