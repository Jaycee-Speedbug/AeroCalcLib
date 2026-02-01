using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using AeroCalcCore.FlightPerformanceEngine;



namespace AeroCalcCore.FlightPerformanceEngine
{


    public static class AeroCalc
    {

        // Constantes utilisées dans la manipulation des modèles de performances
        public const int MODEL_RANGE_INCLUDE_LIMIT = 1;
        public const int MODEL_RANGE_EXCLUDE_LIMIT = 0;
        public const double MODEL_DIMENSION_DEFAULT_VALUE = 1.0;


        // Constantes et messages d'erreurs propres au calculateur
        /*
        public const int ERROR_CALC_INPUT_VALUE_OUT_OF_RANGE = 10001;
        public const String ERROR_CALC_MSG_INPUT_VALUE_OUT_OF_RANGE = "Un facteur de la commande de calcul est en dehors du domaine autorisé";
        public const int ERROR_CALC_OUTPUT_OUT_OF_RANGE = 10002;
        public const String ERROR_CALC_MSG_OUTPUT_OUT_OF_RANGE = "Le résultat de la commande de calcul est en dehors du domaine autorisé";
        public const int ERROR_CALC_VOID_SYSTEM = 10003;
        public const String ERROR_CALC_MSG_VOID_SYSTEM = "Impossible de résoudre la commande numériquement";
        */


        // Constantes du dictionnaire des unités
        public const int UNIT_UNDETERMINED = -1;
        public const int UNIT_NUMBER = 1;
        public const int UNIT_PERCENT = 2;
        public const int UNIT_TEMPERATURE_DEG_KELVIN = 1001;
        public const int UNIT_TEMPERATURE_DEG_CELSIUS = 1002;
        public const int UNIT_TEMPERATURE_DEG_FARENHEIT = 1003;

        public const int UNIT_SPEED_KT = 2101;
        public const int UNIT_SPEED_KMH = 2001;
        public const int UNIT_SPEED_MPS = 2002;

        public const int UNIT_ALTITUDE_M = 3001;
        public const int UNIT_ALTITUDE_FT = 3002;




        /// <summary>
        /// Compares two double-precision floating-point values for equality within a specified tolerance.
        /// </summary>
        /// <remarks>This method is useful for comparing floating-point values where exact equality is
        /// unreliable due to precision limitations. If the absolute difference between the values is less than or equal
        /// to the specified tolerance, they are considered equal.</remarks>
        /// <param name="a">The first double value to compare.</param>
        /// <param name="b">The second double value to compare.</param>
        /// <param name="tolerance">The maximum allowed difference between the two values for them to be considered equal. Must be non-negative.</param>
        /// <returns>0 if the values are equal within the specified tolerance; -1 if the first value is less than the second; 1
        /// if the first value is greater than the second.</returns>
        /// <exception cref="ArgumentException">Throws exception if one the parameters is NaN.</exception>
        public static int CompareAxisValues(double a, double b, double tolerance)
        {
            // Check for NaN values
            if (double.IsNaN(a) || double.IsNaN(b))
                throw new ArgumentException("CompareAxisValues does not support NaN values.");

            if (Math.Abs(a - b) <= tolerance)
            {
                return 0;
            }
            else if (a < b)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }



}