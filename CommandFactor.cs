using System;



namespace AeroCalcCore {



    public class CommandFactor : IEquatable<CommandFactor> {

        // Properties
        public string name {
            get;
            private set;
        }

        public double value {
            get;
            private set;
        }

        public int unitCode {
            get;
            private set;
        }

        /*
         * Constructors
         */

        public CommandFactor(string factorName, double factorValue, int factorUnitCode) {
            this.name = factorName.ToUpper();
            this.value = factorValue;
            this.unitCode = factorUnitCode;
        }

        public CommandFactor(string factor) {

            //string name = "";
            //int unitDictionaryIndex = 0;
            double val;
            this.name = "";

            // DEBUG les séparateurs ne doivent pas être locaux
            char[] separators = { '=', ':' };
            string[] subStrings = factor.Split(separators);

            if (subStrings.Length == 2) {
                // Facteur sans unité
                if (double.TryParse(subStrings[1], out val)) {
                    this.name = subStrings[0];
                    this.value = val;
                    unitCode = AeroCalc.UNIT_UNDETERMINED;
                }
                else {
                    value = double.NaN;
                    unitCode = AeroCalc.UNIT_UNDETERMINED;
                }
            }
            else if (subStrings.Length == 3) {
                // Facteur avec unité
                if (double.TryParse(subStrings[1], out val)) {
                    this.name = subStrings[0];
                    this.value = val;
                    /// <remarks>
                    /// DEBUG, à modifier une fois la gestion des unités implémentée
                    /// </remarks>
                    unitCode = AeroCalc.UNIT_UNDETERMINED;
                    /*
                    unitDictionaryIndex = container.unitDictionary().getIndexByAlias(subStrings[2]);
                    if (unitDictionaryIndex != AeroCalc.UNIT_UNDETERMINED) {
                    }
                    */
                }
                else {
                    value = double.NaN;
                    unitCode = AeroCalc.UNIT_UNDETERMINED;
                }
            }
            else {
                // Le facteur ne répond pas à un format connu, on 
                this.value = double.NaN;
                this.unitCode = AeroCalc.UNIT_UNDETERMINED;
            }
            // Les noms des facteurs doivent être exprimés en majuscules
            this.name = this.name.ToUpper();
        }



        /// <summary>
        /// Implémente l'interface IEquatable
        /// </summary>
        /// <param name="cmdFactor">Autre commandFactor à comparer</param>
        /// <returns>True si les deux commandFactor ont le même nom</returns>
        public bool Equals(CommandFactor cmdFactor) {
            if (cmdFactor == null) {
                return false;
            }
            return this.name.Equals(cmdFactor.name);
        }
    }
}