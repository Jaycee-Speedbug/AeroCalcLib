using System;
using System.Collections.Generic;
using static AeroCalcCore.FlightPerformanceEngine.AeroCalc;



namespace AeroCalcCore.FlightPerformanceEngine
{
    /// <summary>
    /// Axis selection utility methods.
    /// </summary>
    /// 
    public static class AxisSelector
    {
        /// <summary>
        /// Selects indices around x following:
        ///  - breakpoints => choose contiguous subdomain around x (HardStop)
        ///  - discrete axis => exact (epsilon) or snapping (if allowed)
        ///  - continuous axis => bracketing set of size k (2 or 3 typically)
        /// </summary>
        public static SelectionResult SelectAround<T>(
            IReadOnlyList<T> items,
            double x,
            Func<T, double> getValue,
            Func<T, bool> isBreak,
            AxisMetadata meta,
            EngineNumerics num,
            SelectionPolicy policy
        )
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (getValue == null) throw new ArgumentNullException(nameof(getValue));
            if (isBreak == null) throw new ArgumentNullException(nameof(isBreak));
            if (items.Count == 0) return new SelectionResult.None("Empty axis.");

            // Build contiguous subdomain indices [iMin..iMax] around x, bounded by breakpoints (HardStop).
            (int iMin, int iMax) = GetSubDomain(items, x, getValue, isBreak, meta, num);

            if (iMin > iMax)
                return new SelectionResult.None("No valid subdomain.");

            double dMin = getValue(items[iMin]);
            double dMax = getValue(items[iMax]);

            // Domain check (with BracketEpsilon)
            bool inDomain = x >= dMin - num.BracketEpsilon && x <= dMax + num.BracketEpsilon;

            if (!inDomain && meta.Nature == AxisNature.Discrete && meta.DiscreteMode == DiscreteSelectionMode.ExactOnly)
            {
                // For strict discrete, outside the domain is still "no exact match".
                // We keep going: exact match search can still succeed if x near boundary.
            }
            else if (!inDomain && policy.OutOfDomainPolicy == OutOfDomainPolicy.Reject)
            {
                return new SelectionResult.None($"x={x} is outside subdomain [{dMin}, {dMax}].");
            }

            if (meta.Nature == AxisNature.Discrete)
            {
                return SelectDiscrete(items, x, iMin, iMax, getValue, meta, num);
            }
            else
            {
                if (policy.ContinuousCount < 2)
                    return new SelectionResult.None("ContinuousCount must be >= 2 for continuous axes.");

                return SelectContinuous(items, x, iMin, iMax, getValue, meta, num, policy);
            }
        }

        // ---------------------------
        // Subdomain (breakpoints)
        // ---------------------------

        private static (int iMin, int iMax) GetSubDomain<T>(
            IReadOnlyList<T> items,
            double x,
            Func<T, double> getValue,
            Func<T, bool> isBreak,
            AxisMetadata meta,
            EngineNumerics num
        )
        {
            if (meta.BreakpointsPolicy != BreakpointsPolicy.HardStop)
            {
                // SoftPreference: treat whole axis as one domain.
                return (0, items.Count - 1);
            }

            // Find nearest index by absolute distance (within full list).
            // We use that as the "anchor" to choose the contiguous block between breakpoints.
            int nearest = 0;
            double best = double.PositiveInfinity;
            for (int i = 0; i < items.Count; i++)
            {
                double v = getValue(items[i]);
                double d = Math.Abs(v - x);
                if (d < best)
                {
                    best = d;
                    nearest = i;
                }
            }

            // Expand left until previous breakpoint (exclusive) and right until next breakpoint (exclusive).
            int iMin = nearest;
            int iMax = nearest;

            // A breakpoint item itself is considered part of the domain on its side;
            // your current PerfSerie.subDomain() behaves similarly: it stops *at* breakpoints and then adjusts.
            // Here we keep it simple: breakpoint "cuts" between items, and the breakpoint item belongs to the domain.
            for (int i = nearest - 1; i >= 0; i--)
            {
                iMin = i;
                if (isBreak(items[i]))
                    break;
            }

            for (int i = nearest + 1; i < items.Count; i++)
            {
                iMax = i;
                if (isBreak(items[i]))
                    break;
            }

            // Optional: if you want the breakpoint to be exclusive on one side,
            // you can adjust here. For now: inclusive.
            return (iMin, iMax);
        }

        // ---------------------------
        // Discrete selection
        // ---------------------------

        private static SelectionResult SelectDiscrete<T>(
            IReadOnlyList<T> items,
            double x,
            int iMin,
            int iMax,
            Func<T, double> getValue,
            AxisMetadata meta,
            EngineNumerics num
        )
        {
            // 1) Try exact match within epsilon (priority)
            int exactIdx = -1;
            for (int i = iMin; i <= iMax; i++)
            {
                double v = getValue(items[i]);
                if (Math.Abs(v - x) <= num.ValueEqualityEpsilon)
                {
                    exactIdx = i;
                    break; // deterministic: first match
                }
            }

            if (exactIdx >= 0)
            {
                double v = getValue(items[exactIdx]);
                return new SelectionResult.Discrete(
                    Index: exactIdx,
                    IsExactMatch: true,
                    IsSnapped: false,
                    SelectedValue: v,
                    DeltaToInput: v - x
                );
            }

            // 2) If snapping not allowed, fail.
            if (!meta.SnappingAllowed)
                return new SelectionResult.None($"No exact match for discrete axis at x={x} (epsilon={num.ValueEqualityEpsilon}).");

            // 3) Snapping according to mode
            int bestIdx = -1;
            double bestDist = double.PositiveInfinity;
            double bestValue = double.NaN;

            for (int i = iMin; i <= iMax; i++)
            {
                double v = getValue(items[i]);
                double delta = v - x;

                bool admissible = meta.DiscreteMode switch
                {
                    DiscreteSelectionMode.SnapToNearest => true,
                    DiscreteSelectionMode.SnapDown => delta <= num.ValueEqualityEpsilon, // v <= x (within eps)
                    DiscreteSelectionMode.SnapUp => delta >= -num.ValueEqualityEpsilon,  // v >= x (within eps)
                    _ => false
                };

                if (!admissible)
                    continue;

                double dist = Math.Abs(delta);

                // Optional snap limit
                if (meta.SnapMaxDistance.HasValue && dist > meta.SnapMaxDistance.Value)
                    continue;

                // Choose closest; tie-breaker: prefer lower then higher (deterministic), or change as needed
                if (dist < bestDist || (Math.Abs(dist - bestDist) <= num.ValueEqualityEpsilon && v < bestValue))
                {
                    bestDist = dist;
                    bestIdx = i;
                    bestValue = v;
                }
            }

            if (bestIdx < 0)
                return new SelectionResult.None($"Snapping enabled but no admissible snap candidate for x={x}.");

            return new SelectionResult.Discrete(
                Index: bestIdx,
                IsExactMatch: false,
                IsSnapped: true,
                SelectedValue: bestValue,
                DeltaToInput: bestValue - x
            );
        }

        // ---------------------------
        // Continuous selection (bracketing)
        // ---------------------------

        private static SelectionResult SelectContinuous<T>(
            IReadOnlyList<T> items,
            double x,
            int iMin,
            int iMax,
            Func<T, double> getValue,
            AxisMetadata meta,
            EngineNumerics num,
            SelectionPolicy policy
        )
        {
            // Determine if x is within domain (with epsilon)
            double minV = getValue(items[iMin]);
            double maxV = getValue(items[iMax]);
            bool inDomain = x >= minV - num.BracketEpsilon && x <= maxV + num.BracketEpsilon;

            bool usedExtrap = false;
            if (!inDomain)
            {
                if (policy.OutOfDomainPolicy == OutOfDomainPolicy.AllowExtrapolation)
                {
                    usedExtrap = true;
                    // we still pick indices at the nearest edge
                }
                else if (policy.OutOfDomainPolicy == OutOfDomainPolicy.ClampToDomain)
                {
                    // clamp x for selection (not necessarily for interpolation; you may keep original x later)
                    x = Math.Min(Math.Max(x, minV), maxV);
                }
                else
                {
                    return new SelectionResult.None($"x is outside domain [{minV},{maxV}] and OutOfDomainPolicy=Reject.");
                }
            }

            // Find bracketing pair indices (below, above) inside [iMin..iMax].
            // Assumption: values are monotonic increasing. If not guaranteed, you should sort indices by value once.
            int below = -1, above = -1;
            for (int i = iMin; i <= iMax; i++)
            {
                double v = getValue(items[i]);
                if (v <= x + num.ValueEqualityEpsilon)
                    below = i;
                if (v >= x - num.ValueEqualityEpsilon)
                {
                    above = i;
                    break;
                }
            }

            // Edge conditions
            if (below < 0) below = iMin;
            if (above < 0) above = iMax;

            bool brackets = below <= above &&
                            getValue(items[below]) <= x + num.BracketEpsilon &&
                            getValue(items[above]) >= x - num.BracketEpsilon;

            // Build selection set of size k around [below, above] staying within domain bounds.
            int k = policy.ContinuousCount;

            // If k==2 => just [below, above] (ensure distinct indices if possible)
            if (k == 2)
            {
                int i0 = below;
                int i1 = above;

                if (i0 == i1)
                {
                    // Try to expand to a neighbor for interpolation stability
                    if (i1 + 1 <= iMax) i1 = i1 + 1;
                    else if (i0 - 1 >= iMin) i0 = i0 - 1;
                }

                if (i0 == i1)
                    return new SelectionResult.None("Not enough distinct points to perform continuous selection (k=2).");

                return new SelectionResult.Continuous(
                    Indices: new[] { i0, i1 },
                    BracketsX: brackets,
                    UsedExtrapolation: usedExtrap && !brackets,
                    DomainMin: minV,
                    DomainMax: maxV
                );
            }

            // If k>=3: center around the bracketing region and expand alternately.
            // Start with unique indices set.
            var chosen = new List<int>(capacity: k);

            void addUnique(int idx)
            {
                if (!chosen.Contains(idx))
                    chosen.Add(idx);
            }

            addUnique(below);
            addUnique(above);

            int left = Math.Min(below, above) - 1;
            int right = Math.Max(below, above) + 1;

            while (chosen.Count < k && (left >= iMin || right <= iMax))
            {
                // Prefer the side that yields the closer value to x (distance heuristic)
                int pick = -1;

                if (left >= iMin && right <= iMax)
                {
                    double dl = Math.Abs(getValue(items[left]) - x);
                    double dr = Math.Abs(getValue(items[right]) - x);
                    pick = (dl <= dr) ? left-- : right++;
                }
                else if (left >= iMin)
                {
                    pick = left--;
                }
                else
                {
                    pick = right++;
                }

                if (pick >= iMin && pick <= iMax)
                    addUnique(pick);
            }

            if (chosen.Count < k)
                return new SelectionResult.None($"Not enough points in subdomain to select k={k} items.");

            // Sort indices by axis value (important for interpolation routines)
            chosen.Sort((a, b) => getValue(items[a]).CompareTo(getValue(items[b])));

            return new SelectionResult.Continuous(
                Indices: chosen.ToArray(),
                BracketsX: brackets,
                UsedExtrapolation: usedExtrap && !brackets,
                DomainMin: minV,
                DomainMax: maxV
            );
        }
    }
}


