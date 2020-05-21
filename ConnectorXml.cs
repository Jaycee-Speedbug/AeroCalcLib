using System.Xml.Linq;



namespace AeroCalcCore
{



    /// <summary>
    /// Classe de conversion d'un fichier .csv en fichier de performances de vol XML
    /// </summary>
    public class ConnectorXML : FileConnector {

        /*
         * CONSTANTES
         */

        // Constantes des balises XML du fichier de configuration
        public const string XML_APP = "Application";
        public const string XML_NAME = "Name";
        public const string XML_VERSION = "Version";
        public const string XML_ABSOLUTE_PATH = "AbsolutePath";
        public const string XML_RELATIVE_PATH = "RelativePath";
        public const string XML_CONFIG = "Configuration";
        public const string XML_DATE = "Date";
        public const string XML_MODELSDIR = "ModelsDirectory";
        public const string XML_UNITS_FILE = "UnitsFile";
        public const string XML_FILENAME = "FileName";
        public const string XML_UNITS = "Units";
        public const string XML_ENABLE = "Enabled";
        public const string XML_ALLOWED = "Allowed";
        public const string XML_VERBOSE = "Verbose";
        public const string XML_UNIT_DICTIONARY = "Dictionary";
        public const string XML_UNIT_ITEM = "Unit";
        public const string XML_UNIT_NAME = "Name";
        public const string XML_UNIT_DIMENSION = "Dimension";
        public const string XML_UNIT_ALIAS = "Alias";
        public const string XML_UNIT_ISREF = "IsRef";
        public const string XML_UNIT_FACTOR = "Factor";
        public const string XML_UNIT_CONSTANT = "Constant";


        /*
         * PROPRIETES
         */

        /// <summary>
        /// Recoit le code de traitement du fichier XML
        /// </summary>
        public int lastOperationResultCode { get; private set; }



        /*
         * MEMBRES
         */

        /// <summary>
        /// Document XML
        /// </summary>
        XDocument xDoc;



        /*
         * CONSTRUCTEURS
         */

        public ConnectorXML() : base() {
            xDoc = new XDocument();
        }

        public ConnectorXML(string workDirectoryPath) : base(workDirectoryPath) {
            xDoc = new XDocument();
        }

        public ConnectorXML(string workDirectoryPath, 
                                string fileAbsolutePath) : base(workDirectoryPath, fileAbsolutePath) {
            readXmlFile();
        }

        public ConnectorXML(string workDirectoryPath,
                                string inputFileAbsolutePath,
                                string outputFileAbsolutePath) : base(workDirectoryPath, 
                                                                      inputFileAbsolutePath, 
                                                                      outputFileAbsolutePath) {
            readXmlFile();
        }



        /*
         *  SERVICES 
         */

        /// <summary>
        /// Lit le fichier XML dont le chemin absolu est passé en argument
        /// </summary>
        /// <param name="absolutePath">Chemin absolu vers le fichier XML</param>
        /// <returns>True en cas de succès, false sinon</returns>
        /// 
        public bool readXmlFile(string absolutePath) {
            if (setInputFileAbsolutePath(absolutePath)) {
                xDoc = XDocument.Load(inputFileAbsolutePath);
                if (xDoc != null) return true;
            }
            return false;
        }


        
        /// <summary>
        /// Lit le fichier XML dont le chemin absolu est passé en argument
        /// </summary>
        /// <param name="absolutePath">Chemin absolu vers le fichier XML</param>
        /// <returns>True en cas de succès, false sinon</returns>
        /// 
        public bool readXmlFile() {
            if (inputFileExists()) {
                xDoc = XDocument.Load(inputFileAbsolutePath);
                if (xDoc != null) return true;
            }
            return false;
        }


        /// <summary>
        /// Renvoie la chaine de caratère correspondant à un attribut situé dans une ligée de noeuds
        /// </summary>
        /// <param name="attributeName">Nom XML de l'attribut à chercher</param>
        /// <param name="nodeNames">
        /// Tableau de chaines de caractères contenant les noms XML des noeuds
        /// à examiner.
        /// </param>
        /// <returns>
        /// Chaine contenant l'attribut, ou une chaine vide si l'attribut n'a pas été trouvé
        /// </returns>
        /// 
        public string getAttribute(string attributeName, string[] nodeNames) {
            if (xDoc != null && nodeNames != null) {
                XElement xe = xDoc.Element(nodeNames[0]);
                for (int index = 1; index < nodeNames.Length; index++) {
                    xe = xe.Element(nodeNames[index]);
                }
                return xe.Attribute(attributeName).Value;
            }
            else return "";
        }



        /// <summary>
        /// Sauvegarde un objet PerfPile dans un fichier XML
        /// </summary>
        /// <param name="pp">Objet PerfPile à sauvegarder</param>
        /// <param name="xmlFileAbsolutePath">Chemin complet vers le fichier XML à créer</param>
        /// <returns>Entier signalant la réussite ou l'échec de l'opération
        ///     XMLConnector.FILEOP_SUCCESSFUL : réussite
        ///     XMLConnector.FILEOP_UNABLE_TO_CREATE_OUTPUT_FILE : Impossible de créer le fichier XML
        ///     XMLConnector.FILEOP_UNKNOWN_ERROR : Erreur non identifiable
        /// </returns>
        /// <remarks>
        /// DEBUG: A DEVELOPPER
        /// </remarks>
        /// 
        public int savePerfPileToXML(PerfPile pp, string xmlFileAbsolutePath) {
            XDocument xmlPerfDoc = new XDocument();
            xmlPerfDoc.AddFirst(xmlPerfPile(pp));
            xmlPerfDoc.Save(xmlFileAbsolutePath);
            return ConnectorXML.FILEOP_SUCCESSFUL;
        }


        /// <summary>
        /// Sauvegarde un objet UnitDictionary dans un fichier XML
        /// </summary>
        /// <param name="ud">Objet UnitDictionary à sauvegarder</param>
        /// <param name="xmlFileAbsolutePath">Chemin absolu vers le fichier XML à créer</param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// DEBUG: A DEVELOPPER
        /// </remarks>
        /// 
        public int saveUnitDictionaryToXML(UnitDictionary ud, string xmlFileAbsolutePath) {
            XDocument xmlUnitsDoc = new XDocument();
            xmlUnitsDoc.AddFirst(xmlUnitDictionary(ud));
            xmlUnitsDoc.Save(xmlFileAbsolutePath);
            return ConnectorXML.FILEOP_SUCCESSFUL;
        }



        /*
         *  METHODES 
         */

        /// <summary>
        /// Retourne un XElement contenant la définition XML d'une unité de mesure
        /// </summary>
        /// <param name="u">Unité de mesure</param>
        /// <returns></returns>
        /// 
        private XElement xmlUnit(UnitItem u) {

            string isref;

            if (u.isRef) {
                isref = "1";
            }
            else {
                isref = "0";
            }
            return new XElement(XML_UNIT_ITEM, new XAttribute(XML_UNIT_DIMENSION, u.dimension),
                                               new XAttribute(XML_UNIT_NAME, u.name),
                                               new XAttribute(XML_UNIT_ISREF,isref),
                                               new XAttribute(XML_UNIT_ALIAS,u.alias),
                                               new XAttribute(XML_UNIT_FACTOR,u.factor),
                                               new XAttribute(XML_UNIT_CONSTANT,u.constant));
        }



        /// <summary>
        /// Retourne un XElement contenant l'ensemble des défintions d'unités du dictionnaire
        /// </summary>
        /// <param name="dico">Dictionnaire des unités</param>
        /// <returns></returns>
        /// 
        private XElement xmlUnitDictionary(UnitDictionary dico) {

            XElement x = new XElement(XML_UNIT_DICTIONARY);

            foreach (UnitItem u in dico.units) {
                x.Add(xmlUnit(u));
            }
            return x;
        }



        /// <summary>
        /// Formate un élément XML pour refléter un objet PerfPoint
        /// </summary>
        /// <param name="pp">Objet PerfPoint</param>
        /// <returns>Un élément XML</returns>
        /// 
        private XElement xmlPerfPoint(PerfPoint pp) {
            XElement xElem = new XElement("Point", new XAttribute("In", pp.factorValue),
                                                   new XAttribute("Out", pp.output),
                                                   new XAttribute("Break", pp.isBreak));
            return xElem;
        }



        /// <summary>
        /// Formate un élément XML pour refléter un objet PerfSerie
        /// </summary>
        /// <param name="pp">Objet PerfSerie</param>
        /// <returns>Un élément XML</returns>
        /// 
        private XElement xmlPerfSerie(PerfSerie pf) {
            XElement xElem = new XElement("Serie", new XAttribute("Key", pf.dataBaseKey),
                                                   new XAttribute("Factor", pf.factorValue));
            XElement xColElem = new XElement("Points");
            // Ajout de chaque point de la série
            for (int count = 0; count < pf.count; count++) {
                xColElem.Add(xmlPerfPoint(pf.pointAt(count)));
            }
            xElem.Add(xColElem);
            return xElem;
        }



        /// <summary>
        /// Formate un élément XML pour refléter un objet PerfLayer
        /// </summary>
        /// <param name="pp">Objet PerfLayer</param>
        /// <returns>Un élément XML</returns>
        /// 
        private XElement xmlPerfLayer(PerfLayer pl) {
            XElement xElem = new XElement("Layer", new XAttribute("Name", pl.outputName),
                                                   new XAttribute("Key", pl.dataBaseKey),
                                                   new XAttribute("Factor", pl.factorValue),
                                                   new XAttribute("PointFactorName", pl.pointFactorName),
                                                   new XAttribute("PointFactorUnitCode", pl.pointFactorUnitCode),
                                                   new XAttribute("SerieFactorName", pl.serieFactorName),
                                                   new XAttribute("SerieFactorUnitCode", pl.serieFactorUnitCode));
            XElement xColElem = new XElement("Series");
            // Ajout de chaque série de la layer
            for (int count = 0; count < pl.count; count++) {
                xColElem.Add(xmlPerfSerie(pl.SerieAt(count)));
            }
            xElem.Add(xColElem);
            return xElem;
        }



        /// <summary>
        /// Formate un élément XML pour refléter un objet PerfPile
        /// </summary>
        /// <param name="pp">Objet PerfPile</param>
        /// <returns>Un élément XML</returns>
        /// 
        private XElement xmlPerfPile(PerfPile pp) {
            XElement xElem = new XElement("Pile", new XAttribute("Name", pp.outputName),
                                                  new XAttribute("Key", pp.dataBaseKey),
                                                  new XAttribute("Hidden", (pp.hidden == true ? "1" : "0")),
                                                  new XAttribute("Factor", pp.factorValue),
                                                  new XAttribute("DiscretName", pp.discretName),
                                                  new XAttribute("DiscretValue", pp.discretValue),
                                                  new XAttribute("PointFactorName", pp.pointFactorName),
                                                  new XAttribute("PointFactorUnitCode", pp.pointFactorUnitCode),
                                                  new XAttribute("SerieFactorName", pp.serieFactorName),
                                                  new XAttribute("SerieFactorUnitCode", pp.serieFactorUnitCode),
                                                  new XAttribute("LayerFactorName", pp.layerFactorName),
                                                  new XAttribute("LayerFactorUnitCode", pp.layerFactorUnitCode));
            XElement xColElem = new XElement("Layers");
            // Ajout de chaque layer de la pile
            for (int count = 0; count < pp.count; count++) {
                xColElem.Add(xmlPerfLayer(pp.layerAt(count)));
            }
            xElem.Add(xColElem);
            return xElem;
        }

    }

}