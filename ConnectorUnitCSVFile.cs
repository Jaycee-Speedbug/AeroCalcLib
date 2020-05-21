using System;



namespace AeroCalcCore
{



    /// <summary>
    /// Classe assurant l'interface entre un container de modèles de performance de vol et les fichiers textes CSV
    /// permettant la persistance des unités de mesures utilisées dans ces modèles.
    /// 
    /// Principe:
    /// Reçoit le chemin du dossier à traiter et le filtre à appliquer sur les noms de fichiers.
    /// Renvoie une liste des fichiers répondant au critère du filtre
    /// Renvoie les données des unités de mesures trouvées dans ces fichiers
    /// </summary>
    /// 
    class ConnectorUnitCSVFile : ConnectorCSVFile {


        /*
         * CONSTANTES
         */

        protected const string KWD_UNIT_DIMENSION = "UnitDimension";
        protected const string KWD_UNIT_IS_REF = "IsRef";
        protected const string KWD_UNIT_NAME = "UnitName";
        protected const string KWD_UNIT_ALIAS = "UnitAlias";
        protected const string KWD_UNIT_FACTOR = "Factor";
        protected const string KWD_UNIT_CONSTANT = "Constant";



        /*
         * CONSTRUCTEURS
         */

        /// <summary>
        /// Constructeur de la classe, simple appel au constructeur de la classe mère
        /// </summary>
        /// 
        public ConnectorUnitCSVFile() : base() {

        }



        /*
         * SERVICES
         */

        /// <summary>
        /// Renvoie un objet UnitDictionary contenant toutes les données extraites depuis un fichier texte
        /// d'unités de mesure, au format CSV.
        /// </summary>
        /// <param name="fileAbsolutePath">Chemin complet du fichier CSV à analyser</param>
        /// <returns>Objet UnitDictionary</returns>
        /// <remarks>Masque la méthode héritée readFile</remarks>
        /// 
        public UnitDictionary readFile(string fileAbsolutePath) {

            String unitDimension, unitName, unitAlias;
            bool unitIsRef;
            double unitFactor, unitConstant;
            int unitDimensionColumn, unitNameColumn, unitAliasColumn, unitIsRefColumn, unitFactorColumn, unitConstantColumn;
            int cursor;
            UnitDictionary dico = new UnitDictionary();
            unitFactor = 1;
            unitConstant = 0;

            if (readTextFile(fileAbsolutePath, true) == FILEOP_SUCCESSFUL) {
                // Analyse de la structure de la table TABLE_1
                unitDimensionColumn = getColumnIndex(KWD_UNIT_DIMENSION);
                unitNameColumn = getColumnIndex(KWD_UNIT_NAME);
                unitAliasColumn = getColumnIndex(KWD_UNIT_ALIAS);
                unitIsRefColumn = getColumnIndex(KWD_UNIT_IS_REF);
                unitFactorColumn = getColumnIndex(KWD_UNIT_FACTOR);
                unitConstantColumn = getColumnIndex(KWD_UNIT_CONSTANT);

                // recherche de la position de départ de la lecture
                cursor = getLineIndex(KWD_START_TABLE_1) + 1;

                // Lecture de la table et insertions des items au dictionnaire
                while (!fileLines[cursor].Contains(KWD_END_TABLE_1)) {
                    string[] subs = fileLines[cursor].Split(cellSeparator);
                    if (subs.Length >= 2) {

                        if (parseABoolean(subs[unitIsRefColumn], out unitIsRef)) {
                            unitDimension = subs[unitDimensionColumn];
                            unitName = subs[unitNameColumn];
                            unitAlias = subs[unitAliasColumn];
                            if (!parseADouble(subs[unitFactorColumn], out unitFactor)) {
                                unitFactor = 1;
                            }
                            if (!parseADouble(subs[unitConstantColumn], out unitConstant)) {
                                unitConstant = 0;
                            }
                            dico.add(unitDimension, unitName, unitAlias, unitIsRef, unitFactor, unitConstant);
                        }
                        
                    }
                    cursor++;
                }

            }
            return dico;
        }

    } // class

} // namespace