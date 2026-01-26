


using AeroCalcCore.FileServices;
using AeroCalcCore.FlightPerformanceEngine;

namespace AeroCalcCore
{



    /// <summary>
    /// Classe assurant l'interface avec des fichiers textes, au format CSV, contenant des modèles
    /// de calcul de performances de vol.
    /// 
    /// Principe:
    /// Reçoit le chemin du dossier à traiter et le filtre à appliquer sur les noms de fichiers.
    /// Renvoie une liste des fichiers répondant au critère du filtre
    /// Renvoie un modèle de performance de vol sur requète d'un nom de fichier de cette liste
    /// </summary>
    /// 
    public class ModelCSVFile : CSVFile
    {


        /*
         * CONSTANTES
         */

        protected const string KWD_PILE_KEY = "PileKey";
        protected const string KWD_PILE_FACTOR_VALUE = "PileFactorValue";
        protected const string KWD_PILE_HIDDEN = "Hidden";
        protected const string KWD_DISCRET_NAME = "DiscretName";
        protected const string KWD_DISCRET_VALUE = "DiscretValue";
        protected const string KWD_INDEPENDENT_LAYERS = "IndependentLayers";

        protected const string KWD_OUTPUT_NAME = "OutputName";
        protected const string KWD_OUTPUT_UNIT = "OutputUnit";
        protected const string KWD_OUTPUT_VALUE = "OutputValue";
        protected const string KWD_BREAK_VALUE = "BreakValue";

        protected const string KWD_LAYER_KEY = "LayerKey";
        protected const string KWD_LAYER_FACTOR_NAME = "LayerFactorName";
        protected const string KWD_LAYER_FACTOR_UNIT = "LayerFactorUnit";
        protected const string KWD_LAYER_FACTOR_VALUE = "LayerFactorValue";

        protected const string KWD_SERIE_FACTOR_NAME = "SerieFactorName";
        protected const string KWD_SERIE_FACTOR_UNIT = "SerieFactorUnit";
        protected const string KWD_SERIE_FACTOR_VALUE = "SerieFactorValue";

        protected const string KWD_POINT_FACTOR_NAME = "PointFactorName";
        protected const string KWD_POINT_FACTOR_UNIT = "PointFactorUnit";
        protected const string KWD_POINT_FACTOR_VALUE = "PointFactorValue";



        /*
         * CONSTRUCTEURS
         */

        /// <summary>
        /// Constructeur de la classe, simple appel au constructeur de la classe mère
        /// </summary>
        /// 
        public ModelCSVFile() : base()
        {

        }



        /*
         * SERVICES
         */

        /// <summary>
        /// Renvoie un objet PerfPile contenant toutes les données extraites depuis un fichier texte au format csv
        /// contenant un modèle de performances de vol.
        /// </summary>
        /// <param name="fileAbsolutePath">Chemin complet du fichier CSV à analyser</param>
        /// <returns>Objet PerfPile</returns>
        /// 
        public PerfPile readFile(string fileAbsolutePath)
        {
            PerfPile pp = null;

            if (readTextFile(fileAbsolutePath, true) == FILEOP_SUCCESSFUL)
            {

                // Locals
                bool hidden;
                double pileFactorValue;
                long discretValue;
                int pointFactorUnit;
                int serieFactorUnit;
                int layerFactorUnit;
                int outputUnit;
                bool independent;
                string discretName;
                string pointFactorName;
                string serieFactorName;
                string layerFactorName;
                string outputName;


                // Lecture des paramètres numériques de la Pile
                // Une 
                if (!parseADouble(StrRightOf(KWD_PILE_FACTOR_VALUE), out pileFactorValue))
                {
                    pileFactorValue = 1;
                }
                if (!long.TryParse(StrRightOf(KWD_DISCRET_VALUE), out discretValue))
                {
                    discretValue = 1;
                }
                if (!parseABoolean(StrRightOf(KWD_PILE_HIDDEN), out hidden))
                {
                    hidden = false;
                }
                if (!parseABoolean(StrRightOf(KWD_INDEPENDENT_LAYERS), out independent))
                {
                    independent = false;
                }

                // Lecture des paramètres textuels de la Pile
                outputName = StrRightOf(KWD_OUTPUT_NAME);

                discretName = StrRightOf(KWD_DISCRET_NAME);
                if (discretName == null)
                {
                    discretName = "";
                }

                layerFactorName = StrRightOf(KWD_LAYER_FACTOR_NAME);
                if (layerFactorName == null)
                {
                    layerFactorName = "";
                }

                serieFactorName = StrRightOf(KWD_SERIE_FACTOR_NAME);
                if (serieFactorName == null)
                {
                    serieFactorName = "";
                }

                pointFactorName = StrRightOf(KWD_POINT_FACTOR_NAME);
                if (pointFactorName == null)
                {
                    pointFactorName = "";
                }

                // TODO UnitCode as int should be replaced by string
                // Lecture des unités non gérées pour l'instant
                /*
                pointFactorUnit = parseUnitCode(KWD_POINT_FACTOR_UNIT);
                serieFactorUnit = parseUnitCode(KWD_SERIE_FACTOR_UNIT);
                layerFactorUnit = parseUnitCode(KWD_LAYER_FACTOR_UNIT);
                outputUnit = parseUnitCode(KWD_OUTPUT_UNIT);
                */
                pointFactorUnit = -1;
                serieFactorUnit = -1;
                layerFactorUnit = -1;
                outputUnit = -1;

                // Constitution de la Pile
                pp = new PerfPile(pileFactorValue, discretName, discretValue,
                                  pointFactorName, pointFactorUnit,
                                  serieFactorName, serieFactorUnit,
                                  layerFactorName, layerFactorUnit, independent,
                                  outputName, outputUnit,
                                  hidden);

                // Analyse du tableau des layers de performance
                //int startLine = getLineIndex(KWD_OUTPUT_NAME) + 1;
                int startLine = GetLineIndex(KWD_START_TABLE) + 1;
                int endLine = GetLineIndex(KWD_END_TABLE);

                int layerFactorColumn = GetColumnIndex(KWD_LAYER_FACTOR_VALUE);
                int serieFactorColumn = GetColumnIndex(KWD_SERIE_FACTOR_VALUE);
                int pointFactorColumn = GetColumnIndex(KWD_POINT_FACTOR_VALUE);
                int outColumn = GetColumnIndex(KWD_OUTPUT_VALUE);
                int breakColumn = GetColumnIndex(KWD_BREAK_VALUE);

                if (layerFactorColumn > -1 &&
                    serieFactorColumn > -1 &&
                    pointFactorColumn > -1 &&
                    outColumn > -1 &&
                    breakColumn > -1 &&
                    startLine > -1 && endLine > startLine)
                {
                    // Les colonnes sont identifiées, lecture possible

                    double layerFactor;
                    double serieFactor;
                    double pointFactor;
                    double outputFactor;
                    bool breakValue;

                    for (int count = startLine; count < endLine; count++)
                    {

                        layerFactor = double.NaN;
                        serieFactor = double.NaN;
                        pointFactor = double.NaN;
                        outputFactor = double.NaN;
                        breakValue = false;

                        if (parseADouble(ValueAtPosition(count, layerFactorColumn), out layerFactor) &&
                            parseADouble(ValueAtPosition(count, serieFactorColumn), out serieFactor) &&
                            parseADouble(ValueAtPosition(count, pointFactorColumn), out pointFactor) &&
                            parseADouble(ValueAtPosition(count, outColumn), out outputFactor) &&
                            parseABoolean(ValueAtPosition(count, breakColumn), out breakValue))
                        {
                            // Données valides, un Point de performances peut être ajouté à la Pile
                            pp.add(new PerfPoint(pointFactor, outputFactor, breakValue), serieFactor, layerFactor);
                        }
                    }
                }
                else
                {
                    // Problème de structure du fichier, les colonnes ne peuvent être correctement identifiées
                    pp = null;
                }
            }
            return pp;
        }
    }
}