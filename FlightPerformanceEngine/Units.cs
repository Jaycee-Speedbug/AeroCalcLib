using System;
using System.Collections.Generic;
using System.Linq;



namespace AeroCalcCore.FlightPerformanceEngine
{



    public class Units
    {

        /*
         *  CONSTANTES
         */

        public static string UNIT_DIM_LENGHT = "LENGHT";
        public static string UNIT_DIM_TIME = "TIME";
        public static string UNIT_DIM_MASS = "MASS";
        public static string UNIT_DIM_VOLUME = "VOLUME";
        public static string UNIT_DIM_TEMPERATURE = "TEMPERATURE";
        public static string UNIT_DIM_SPEED = "SPEED";
        public static string UNIT_DIM_NUMBER = "NUMBER";
        public static string UNIT_DIM_ANGLE = "ANGLE";



        /*
         *  MEMBRES
         */

        /// <summary>
        /// Tableau des différentes dimensions d'unité acceptées par AirCalc
        /// </summary>
        private string[] unitDimensions = { UNIT_DIM_LENGHT,
                                            UNIT_DIM_TIME,
                                            UNIT_DIM_MASS,
                                            UNIT_DIM_VOLUME,
                                            UNIT_DIM_TEMPERATURE,
                                            UNIT_DIM_SPEED,
                                            UNIT_DIM_NUMBER,
                                            UNIT_DIM_ANGLE };



        /*
         * PROPRIETES
         */

        /// <summary>
        /// Structure du dictionnaire des unités sous forme de List<UnitItem>
        /// </summary>
        public List<Unit> units { get; private set; }



        /*
         * CONSTRUCTEURS
         */

        /// <summary>
        /// Constructeur
        /// Insertion de la première unité dédiée aux nombres sans unité
        /// </summary>
        public Units()
        {

            units = new List<Unit>();

            // Insertion de la première unité (qui ne répond pas aux critères que doivent satisfaire les autres unités)
            units.Add(new Unit("", "NUMBER", "", true, 1, 0));

        }



        /*
         * SERVICES
         */

        /// <summary>
        /// Ajoute une unité au dictionnaire des unités, après vérification du nom de l'unité et de l'existence
        /// de la dimension. Le nom de l'unité et la dimension doivent être fournis. L'alias est optionnel.
        /// Dans ce cas, l'utilisateur devra utiliser le nom complet de l'unité.
        /// </summary>
        /// <param name="unitName">Nom complet de l'unité</param>
        /// <param name="unitDimension">Nom de la dimension de l'unité</param>
        /// <param name="unitIsRef">Indique si cette unité est considérée comme la référence de sa dimension</param>
        /// <param name="unitAlias">Alias de l'unité, pouvant être utilisé dans les commandes en ligne</param>
        /// <param name="unitFactor">Facteur à utiliser pour convertir vers la référence de la dimension</param>
        /// <param name="unitConstant">Constante à utiliser pour convertir vers la référence de la dimension</param>
        /// 
        public void add(String unitDimension, 
                        String unitName, 
                        String unitAlias,
                        bool unitIsRef, 
                        double unitFactor, 
                        double unitConstant)
        {
            // Validity checks before addition to the dictionary
            if (unitDimension.Length > 0 && unitName.Length > 0)
            {
                // If unit is reference, unitFactor and unitConstant are forced to 1 and 0 resp.
                if (unitIsRef)
                {
                    unitFactor = 1;
                    unitConstant = 0;
                }
                // The new unit should be unically identified by its name, and its alias
                // The new unit should belongs to a known dimension
                if (!unitExists(unitName, unitAlias) && isDimensionAccepted(unitDimension))
                {
                    units.Add(new Unit(unitDimension, unitName, unitAlias, unitIsRef, unitFactor, unitConstant));
                }
            }
        }



        public void add(Unit newUnit)
        {
            add(newUnit.dimension, newUnit.name, newUnit.alias, newUnit.isRef, newUnit.factor, newUnit.constant);
        }


        /// <summary>
        /// Ajout d'une table complète d'unités
        /// </summary>
        /// <param name="unitDimensionTable">Table des dimensions</param>
        /// <param name="unitNameTable">Table des noms complets</param>
        /// <param name="unitAliasTable">Table des alias</param>
        /// 
        public void add(String[] unitDimensionTable, String[] unitNameTable, String[] unitAliasTable,
                        bool[] unitIsRef, double[] unitFactor, double[] unitConstant)
        {

            for (int count = 0; count < unitNameTable.Length; count++)
            {
                add(unitDimensionTable[count], unitNameTable[count], unitAliasTable[count],
                    unitIsRef[count], unitFactor[count], unitConstant[count]);
            }
        }



        /// <summary>
        /// Retourne un objet UnitItem portant le nom transmis en argument
        /// </summary>
        /// <param name="name">Nom complet de l'unité</param>
        /// <returns>Objet UnitItem</returns>
        /// 
        public Unit getUnitByName(String name)
        {

            int index = getIndexByName(name);

            if (index >= 0)
            {
                return units.ElementAt(index);
            }
            return null;
        }



        /// <summary>
        /// Retourne un objet UnitItem portant l'alias transmis en argument
        /// </summary>
        /// <param name="alias">Alias de l'unité</param>
        /// <returns>Objet UnitItem</returns>
        /// 
        public Unit getUnitByAlias(String alias)
        {

            int index = getIndexByAlias(alias);

            if (index >= 0)
            {
                return units.ElementAt(index);
            }
            return null;
        }



        /// <summary>
        /// Renvoie la liste des unités
        /// </summary>
        /// <returns></returns>
        public List<Unit> getUnits()
        {
            return units;
        }



        /// <summary>
        /// Retourne l'index dans la liste des unités de l'unité portant l'alias transmis en argument
        /// </summary>
        /// <param name="alias">String, alias de l'unité à chercher dans la liste</param>
        /// <returns>int, valeur de l'index désigant l'unité recherchée</returns>
        /// 



        /*
         * METHODES
         */

        /// <summary>Returns the index of the unit in the list matching the alias provided as argument</summary>
        /// <returns>int index in the unit list, -1 if unit alias not found</returns>
        /// <param name="alias">Alias of the unit to search in the list</param>
        private int getIndexByAlias(String alias)
        {
            return units.FindIndex(Unit => Unit.alias == alias);
            /* Old implementation
            for (int index = 0; index < units.Count; index++)
            {
                if (units.ElementAt<Unit>(index).alias.Equals(alias))
                {
                    return index;
                }
            }
            return -1;
            */
        }



        /// <summary>
        /// Returns the Unit dictionary index of the unit corresponding to the name
        /// </summary>
        /// <param name="name">Unit full name</param>
        /// <returns>index of the unit in the dictionary</returns>
        /// 
        private int getIndexByName(String name)
        {
            return units.FindIndex(Unit => Unit.name == name);

            /* Old implementation
            for (int index = 0; index < units.Count; index++)
            {
                if (units.ElementAt<Unit>(index).name.Equals(name))
                {
                    return index;
                }
            }
            return -1;
            */
        }



        /// <summary>
        /// Check if a unit is already in the dictionary, by its name or its alias
        /// </summary>
        /// <param name="name">Full name of unit</param>
        /// <param name="alias">Alias of unit</param>
        /// <returns>True if a unit holding the same name or alias is already available in the dictionary, False otherwise</returns>
        /// 
        private bool unitExists(String name, String alias)
        {
            if (getIndexByName(name) >= 0 || getIndexByAlias(alias) >= 0)
            {
                return true;
            }
            return false;
        }



        /// <summary>
        /// Vérification de l'existence d'une dimension dont le nom est fourni en argument
        /// </summary>
        /// <param name="dimension">Nom de la dimension proposée à la vérification</param>
        /// <returns>True, si la dimension fait partie de la liste des dimensions acceptées</returns>
        /// 
        private bool isDimensionAccepted(String dimension)
        {

            for (int index = 0; index < unitDimensions.Length; index++)
            {
                if (dimension.Equals(unitDimensions[index]))
                {
                    return true;
                }
            }
            return false;
        }

    }

}