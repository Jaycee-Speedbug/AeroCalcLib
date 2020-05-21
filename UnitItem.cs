using System;



namespace AeroCalcCore {



    public class UnitItem {

        /*
         * PROPRIETES
         */

        public String dimension { get; private set; }

        public String name { get; private set; }

        public String alias { get; private set; }

        public int convertions { get; private set; }

        public bool isRef { get; private set; }

        public double factor { get; private set; }

        public double constant { get; private set; }



        /*
         * CONSTRUCTEURS
         */

        /// <summary>
        /// Construit un objet représentant une unité numérique
        /// </summary>
        /// <param name="unitDimension">Dimension de l'unité (nature de la dimension), doit être l'une des
        /// dimensions codées en dur</param>
        /// <param name="unitName">Nom complet de l'unité</param>
        /// <param name="unitAlias">Alias pouvant être utilisé dans les commandes en ligne</param>
        /// <param name="unitConstant"></param>
        /// <param name="unitFactor"></param>
        /// <param name="unitIsRef"></param>
        /// 
        public UnitItem(String unitDimension, String unitName, String unitAlias, bool unitIsRef, double unitFactor, double unitConstant) {
            dimension = unitDimension;
            name = unitName;
            alias = unitAlias;
            isRef = unitIsRef;
            if (unitIsRef) {
                factor = 1;
                constant = 0;
            }
            else {
                factor = unitFactor;
                constant = unitConstant;
            }
        }



        /*
         * SERVICES
         */

        /// <summary>
        /// Retourne une chaîne descriptive des champs de l'unité
        /// </summary>
        /// <returns>String, contenant la dimension, le nom étendu et l'alias de l'unité</returns>
        /// 
        public override string ToString() {
            String msg;
            msg = "Dimension: " + dimension + "   Name: " + name + "   Alias: " + alias;
            if (isRef) {
                msg += "   is reference for that dimension.";
            }
            else {
                msg += String.Format("   Conversion Factor: {0:f8}   and Constant: {1:f8}",factor,constant);
            }
            return msg;
        }



        /*
         * INTERFACE
         */
        
        /*
        public int Compare(UnitItem a, UnitItem b) {
        }
        */


        
        /*
         * METHODES
         */
        
        /// <summary>
        /// Test les conditions d'égalité entre l'unité et l'unité fournie en argument
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private bool equals(UnitItem unit) {
            if (unit.dimension== this.dimension &&
                unit.name == this.name) {
                return true;
            }
            return false;
        }

    }

}