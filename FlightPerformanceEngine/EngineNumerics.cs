
/// <summary>
/// ENGINE NUMERICS (what the engine USES)
/// Numeric tolerances are ENGINE parameters (adjustable globally).
/// </summary>



/// <summary>
/// EngineNumerics defines the numeric tolerances used by the flight performance engine when comparing Double values.
/// </summary>
public sealed record EngineNumerics(double ValueEqualityEpsilon, double BracketEpsilon)
{
    public static EngineNumerics Default =>
        new EngineNumerics(ValueEqualityEpsilon: 1e-9, BracketEpsilon: 1e-9);
}
