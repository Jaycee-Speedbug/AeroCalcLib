//// <summary>Added to the AeroCalcCore for compatibility with C# 9 record types in .NET Standard 2.0</summary>
namespace AeroCalcCore.FlightPerformanceEngine
{
    /// <summary>
    /// Error codes used in the Flight Performance Engine.
    /// 
    /// </summary>
    public static class EngineErrorCodes
    {
        /// <summary>
        /// 
        /// </summary>
        public const int E_DISCRETE_VALUE_OUT_OF_RANGE = 1;

        /// <summary>
        /// Represents an error code indicating that a PerfPile factor value is outside the allowed range.
        /// </summary>
        /// <remarks>
        /// Use this constant to identify errors where a value assigned to PerfPile.factorValue exceeds its
        /// valid range.
        /// </remarks>
        public const int E_PILE_VALUE_OUT_OF_RANGE = 2;

        /// <summary>
        /// Represents an error code indicating that a PerfLayer factorValue is out of the valid range.
        /// </summary>
        /// <remarks>
        /// Use this constant to identify errors where a value assigned to PerfLayer.factorValue exceeds its
        /// allowed range.
        /// </remarks>
        public const int E_LAYER_VALUE_OUT_OF_RANGE = 3;

        /// <summary>
        /// Represents an error code indicating that a PerfSerie factorValue is out of the allowed range.
        /// </summary>
        /// <remarks>
        /// Use this constant to identify errors where a value assigned to PerfSerie.factorValue exceeds its
        /// allowed range.
        /// </remarks>
        public const int E_SERIES_VALUE_OUT_OF_RANGE = 4;

        /// <summary>
        /// Represents an error code indicating that a PerfPoint input value is outside the permissible range.
        /// </summary>
        /// <remarks>
        /// Use this constant to identify errors where a value assigned to PerfPoint.input exceeds its
        /// allowed range.
        /// </remarks>
        public const int E_POINT_INPUT_OUT_OF_RANGE = 5;

        /// <summary>
        /// Represents the error code indicating that the performance elements system provided is null or void.
        /// </summary>
        /// <remarks>
        /// Use this constant to identify errors where a null or empty performance system is encountered.
        /// </remarks>
        public const int VOID_SYSTEM = 100;

        /// <summary>
        /// Represents the error code indicating that the system size is below the minimum required threshold
        /// of two elements.
        /// </summary>
        /// <remarks>
        /// Use this constant to identify errors where a performance system contains fewer than two elements,
        /// </remarks>
        public const int SYSTEM_BELOW_MIN_SIZE = 101;
    }
}