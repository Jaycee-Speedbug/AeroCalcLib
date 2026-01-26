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


    }



}