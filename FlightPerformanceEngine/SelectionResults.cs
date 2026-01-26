
namespace AeroCalcCore.FlightPerformanceEngine;

public abstract record SelectionResult
{
    public sealed record Discrete(
        int Index,
        bool IsExactMatch,
        bool IsSnapped,
        double SelectedValue,
        double DeltaToInput
    ) : SelectionResult;

    public sealed record Continuous(
        int[] Indices,
        bool BracketsX,
        bool UsedExtrapolation,
        double DomainMin,
        double DomainMax
    ) : SelectionResult;

    public sealed record None(
        string Reason
    ) : SelectionResult;
}
