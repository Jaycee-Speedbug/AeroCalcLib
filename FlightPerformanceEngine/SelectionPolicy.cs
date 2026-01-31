/// <summary>
/// Specifies the policy to apply when a value falls outside the defined domain during selection operations.
/// </summary>
/// <remarks>Use this enumeration to control how selection methods handle input values that are not within the
/// valid (sub)domain. The chosen policy affects whether the operation fails, clamps the value to the nearest boundary,
/// or allows extrapolation (if supported).</remarks>
public enum OutOfDomainPolicy
{
    /// <summary>Fail if x is outside the (sub)domain (considering BracketEpsilon).</summary>
    Reject,

    /// <summary>Allow edge-clamping: use closest boundary bracket/point.</summary>
    ClampToDomain,

    /// <summary>Allow extrapolation (for continuous axes only; selection still stays in subdomain).</summary>
    AllowExtrapolation
}



/// <summary>
/// Runtime policy: how many points, and what to do at edges/out-of-domain.
/// </summary>
public sealed record SelectionPolicy(int ContinuousCount,                 // typically 2 (linear) or 3 (Lagrange)
                                     OutOfDomainPolicy OutOfDomainPolicy)  // Reject / Clamp / Extrapolate
{
    /// <summary>
    /// Politique de sélection linéaire avec clamp : utilise 2 points et limite la valeur à la frontière du domaine si elle est hors domaine.
    /// </summary>
    public static SelectionPolicy LinearClamp
    {
        get
        {
            return new SelectionPolicy(ContinuousCount: 2, OutOfDomainPolicy.ClampToDomain);
        }
    }

    /// <summary>
    /// Politique de sélection Lagrange à 3 points avec clamp : utilise 3 points et limite la valeur à la frontière du domaine si elle est hors domaine.
    /// </summary>
    public static SelectionPolicy Lagrange3Clamp => new(ContinuousCount: 3, OutOfDomainPolicy.ClampToDomain);
}
