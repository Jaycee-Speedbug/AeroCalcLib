using System.Xml.Linq;



namespace AeroCalcCore
{



    /// <summary>
    /// Classe de conversion d'un fichier .csv en fichier de performances de vol XML
    /// </summary>
    public class XMLFile : FileIO {

        /*
         * CONSTANTES
         */

        // Constantes des balises XML du fichier de configuration
        public const string NODE_APP = "Application";
        public const string NODE_CONFIG = "Configuration";
        public const string NODE_DIR = "Directory";
        public const string NODE_FILE = "File";
        public const string NODE_SETTING = "Setting";

        public const string ATTRIB_NAME = "name";
        public const string ATTRIB_VERSION = "version";
        public const string ATTRIB_ID = "id";

        public const string MODELS = "Models";
        public const string UNITS = "Units";

        public const string SCRIPTS = "Scripts";
        public const string ABSOLUTE_PATH = "AbsolutePath";
        public const string RELATIVE_PATH = "RelativePath";
        public const string DATE = "Date";
        public const string ENABLE = "Enabled";
        public const string ALLOWED = "Allowed";
        public const string VERBOSE = "Verbose";
        public const string VERBOSE_ALLOWED = "VerboseAllowed";
        public const string UNITS_ENABLED = "UnitsEnabled";
        public const string LOGGER = "Logger";
        public const string TRUE = "True";
        public const string FALSE = "False";
        

        // Units dictionnary XML mapping
        public const string DICTIONARY = "Dictionary";
        public const string UNIT_ITEM = "Unit";
        public const string UNIT_NAME = "Name";
        public const string DIMENSION = "Dimension";
        public const string ALIAS = "Alias";
        public const string ISREF = "IsRef";
        public const string FACTOR = "Factor";
        public const string CONSTANT = "Constant";



        /*
         * PROPRIETES
         */

        /// <summary>
        /// Recoit le code de traitement du fichier XML
        /// </summary>
        public int IOStatus { get; private set; }



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

        public XMLFile() : base() {
            xDoc = new XDocument();
        }

        public XMLFile(string workDirectoryPath) : base(workDirectoryPath) {
            xDoc = new XDocument();
        }

        public XMLFile(string workDirectoryPath, string fileAbsolutePath) : base(workDirectoryPath, fileAbsolutePath) {
            readXmlFile();
        }

        public XMLFile(string workDirectoryPath, 
                       string inputFileAbsolutePath, 
                       string outputFileAbsolutePath) : base(workDirectoryPath,  inputFileAbsolutePath, outputFileAbsolutePath) {
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
        /*
        public bool readXmlFile(string absolutePath) {
            
            if (setInputFileAbsolutePath(absolutePath)) {
                xDoc = XDocument.Load(inputFileAbsolutePath);
                if (xDoc != null) return true;
            }
            return false;
        } */



        /// <summary>
        /// Lit le fichier XML dont le chemin absolu est passé en argument
        /// </summary>
        /// <param name="absolutePath">Chemin absolu vers le fichier XML</param>
        /// <returns>True en cas de succès, false sinon</returns>
        /// 
        public int readXmlFile() {

            IOStatus = checkFile(inputFileAbsolutePath);

            if (IOStatus == FILEOP_SUCCESSFUL) {
                xDoc = XDocument.Load(inputFileAbsolutePath);
                if (xDoc != null) {
                    return FILEOP_SUCCESSFUL;
                }
                else {
                    // TODO: Valeur de retour à améliorer
                    return FILEOP_UNKNOWN_ERROR;
                }
            }
            else {
                return IOStatus;
            }
        }



        /// <summary>
        /// Renvoie le contenu d'un attribut d'un noeud situé dans une lignée hyérarchique de noeuds
        /// </summary>
        /// <param name="attributeName">Nom XML de l'attribut à chercher</param>
        /// <param name="nodeNames">
        /// Tableau string contenant les noms XML des noeuds à examiner, en ordre hyérarchique.
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
        /// Renvoie le contenu d'un noeud situé dans une lignée hyérarchique de noeuds fournie en argument
        /// </summary>
        /// <param name="nodeNames">
        /// Tableau string contenant les noms XML des noeuds à examiner, en ordre hyérarchique.
        /// </param>
        /// <returns>
        /// Chaine contenant l'attribut, ou une chaine vide si l'attribut n'a pas été trouvé
        /// </returns>
        /// 
        public string getValue(string[] nodeNames) {
            if (xDoc != null && nodeNames != null) {
                XElement xe = xDoc.Element(nodeNames[0]);
                for (int index = 1; index < nodeNames.Length; index++) {
                    xe = xe.Element(nodeNames[index]);
                }
                return xe.Value;
            }
            else return "";
        }



        /// <summary>
        /// Renvoie le contenu d'un noeud, identifié par le nom du noeud et un attribut de ce noeud  
        /// </summary>
        /// <param name="nodeName">
        /// Nom du noeud
        /// </param>
        /// <param name="attributeName">
        /// Nom de l'attribut
        /// </param>
        /// <param name="attributeValue">
        /// Contenu de l'attribut
        /// </param>
        /// <returns>
        /// Chaine contenant l'attribut, ou une chaine vide si l'attribut n'a pas été trouvé
        /// </returns>
        /// 
        public string getValue(string nodeName, string attributeName, string attributeValue) {
            if (nodeName != null && attributeName != null && attributeValue != null) {
                foreach (XElement elemt in xDoc.Descendants())
                {
                    if (elemt.Name.LocalName.Equals(nodeName)) {
                        // Noeud trouvé !
                        foreach (XAttribute attrib in elemt.Attributes())
                        {
                            if (attrib.Name.LocalName.Equals(attributeName) && attrib.Value.Equals(attributeValue)) {
                                // Attribut trouvé
                                return elemt.Value;
                            }
                        }
                    }
                }
            }
            return "";
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
            return XMLFile.FILEOP_SUCCESSFUL;
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
            return XMLFile.FILEOP_SUCCESSFUL;
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
            return new XElement(UNIT_ITEM, new XAttribute(DIMENSION, u.dimension),
                                               new XAttribute(UNIT_NAME, u.name),
                                               new XAttribute(ISREF,isref),
                                               new XAttribute(ALIAS,u.alias),
                                               new XAttribute(FACTOR,u.factor),
                                               new XAttribute(CONSTANT,u.constant));
        }



        /// <summary>
        /// Retourne un XElement contenant l'ensemble des défintions d'unités du dictionnaire
        /// </summary>
        /// <param name="dico">Dictionnaire des unités</param>
        /// <returns></returns>
        /// 
        private XElement xmlUnitDictionary(UnitDictionary dico) {

            XElement x = new XElement(DICTIONARY);

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