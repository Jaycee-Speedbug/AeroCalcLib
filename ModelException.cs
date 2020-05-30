using System;



namespace AeroCalcCore {



    /// <summary>
    /// Classe Exception du calculateur AeroCalc
    /// </summary>
    public class ModelException : Exception {

        /*
         * CONSTANTES
         */

         /*

        public const int DISCRET_VALUE_OUT_OF_RANGE = 1;
        public const int PILE_VALUE_OUT_OF_RANGE = 2;
        public const int LAYER_VALUE_OUT_OF_RANGE = 3;
        public const int SERIE_VALUE_OUT_OF_RANGE = 4;
        public const int POINT_VALUE_OUT_OF_RANGE = 5;
        public const int VOID_SYSTEM = 100;
        public const string UNKNOWN_FACTOR_VALUE_MSG = " : Valeur inconnue.";
        */



        /*
         * PROPRIETES
         */

        public int nature { get; private set; }

        public string modelName { get; private set; }

        public string factor {
            get { return formatFactor();}
            private set { }
        }

        public string factorName { get; private set; }

        public double factorValue { get; private set; }



        /*
         * CONSTRUCTEURS
         */

        /// <summary>
        /// Construit une exception générée dans le calculateur AirCalc
        /// 
        /// </summary>
        /// <param name="message">Message lié à l'exception
        /// </param>
        /// <remarks>
        /// TODO, non implémenté pour l'instant
        /// </remarks>
        public ModelException(String message)
            : base(message) {
            // Rien d'autre
        }


        /// <summary>
        /// Construit une exception générée dans le calculateur AirCalc
        /// </summary>
        /// <param name="nature"> Nature de l'exception</param>
        /// <param name="modelName">Nom du modèle de performances où survient l'exception</param>
        /// <param name="factorName">Nom du facteur en cause, et sa valeur</param>
        /// <remarks>
        /// </remarks>
        public ModelException(int nature, string modelName, string factorName, double factorValue) {
            this.nature = nature;
            this.modelName = modelName;
            this.factorName = factorName;
            this.factorValue = factorValue;
        }



        /*
         * SERVICES
         */

        public void setModelName(string modelName) { this.modelName = modelName; }

        public void setFactorName(string factorName) { this.factorName = factorName; }

        public void setFactor(string factorName, double factorValue) {
            this.factorName = factorName;
            this.factorValue = factorValue;
        }


        /*
         * METHODES
         */

        private string formatFactor() {
            if (string.IsNullOrEmpty(factorName)) {
                return string.Empty;
            }
            else if (double.IsNaN(factorValue)) {
                return string.Concat(factorName, AeroCalc.E_UNKNOWN_FACTOR_VALUE_MSG);
            }
            else {
                return string.Concat(factorName, string.Format("= {0:F3}", factorValue));
            }

        }

    }

}