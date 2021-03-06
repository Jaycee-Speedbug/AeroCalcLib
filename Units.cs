using System;
using System.Collections.Generic;
using System.Linq;



namespace AeroCalcCore
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

            // Insertion de la première unité (qui ne répond pas aux critères que doivent satisfaire le autres unités)
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
        public void add(String unitDimension, String unitName, String unitAlias,
                        bool unitIsRef, double unitFactor, double unitConstant)
        {
            Unit newUnit;
            // Vérifications et inscription de l'unité au dictionnaire
            if (unitDimension.Length > 0 && unitName.Length > 0)
            {
                if (unitIsRef)
                {
                    unitFactor = 1;
                    unitConstant = 0;
                }
                newUnit = new Unit(unitDimension, unitName, unitAlias, unitIsRef, unitFactor, unitConstant);
                if (!unitExists(unitName) && isDimensionAccepted(unitDimension))
                {
                    // Le nom complet de l'unité doit être unique et la dimension reconnue
                    units.Add(newUnit);
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
        public int getIndexByAlias(String alias)
        {

            for (int index = 0; index < units.Count; index++)
            {
                if (units.ElementAt<Unit>(index).alias.Equals(alias))
                {
                    return index;
                }
            }
            return -1;
        }



        /*
         * METHODES
         */

        /// <summary>
        /// Retourne l'index de la liste des unités correspondant à l'item ayant le nom transmis en argument
        /// </summary>
        /// <param name="name">Nom complet de l'unité</param>
        /// <returns>int index dans la liste des unités</returns>
        /// 
        private int getIndexByName(String name)
        {

            for (int index = 0; index < units.Count; index++)
            {
                if (units.ElementAt<Unit>(index).name.Equals(name))
                {
                    return index;
                }
            }
            return -1;
        }



        /// <summary>
        /// Renvoie True lorsqu'une unité du dictionnaire disposant du même nom existe déjà
        /// </summary>
        /// <param name="code">int Code d'identification de l'unité</param>
        /// <returns>True si une unité ayant le code fourni en argument, False dans le cas contraire</returns>
        /// 
        private bool unitExists(String name)
        {

            if (getIndexByName(name) >= 0)
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