using System;
using System.Xml.Linq;
using System.Collections.Generic;



namespace AeroCalcCore
{



    /// <summary>
    /// Classe de conversion d'un fichier .csv en fichier de performances de vol XML
    /// </summary>
    public class XMLFile : FileIO
    {
        /*
         * CONSTANTES
         */
        // XML mapping
        public const string NODE_APP = "Application";
        public const string NODE_CONFIG = "Configuration";
        public const string NODE_DIR = "Directory";
        public const string NODE_FILE = "File";
        public const string NODE_SETTING = "Setting";
        public const string NODE_LANG_PACK = "LanguagePack";
        public const string NODE_LANGUAGES = "Languages";
        public const string NODE_LANGUAGE = "Language";
        public const string NODE_MESSAGES = "Messages";
        public const string NODE_MESSAGE = "Message";
        public const string NODE_UNITS = "Units";
        public const string NODE_UNIT = "Unit";
        public const string NODE_DIMENSION = "Dimension";
        //
        public const string ATTRIB_NAME = "name";
        public const string ATTRIB_SHORT_NAME = "shortname";
        public const string ATTRIB_VERSION = "version";
        public const string ATTRIB_ID = "id";
        public const string ATTRIB_ALIAS = "alias";
        public const string ATTRIB_ISREF = "isref";
        public const string ATTRIB_FACTOR = "factor";
        public const string ATTRIB_CONST = "constant";
        //
        public const string MODELS = "Models";
        public const string UNITS = "Units";
        public const string LOGS = "Logs";
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
        public const string LANGUAGE = "Language";
        public const string TRUE = "True";
        public const string FALSE = "False";



        /*
         * PROPRIETES
         */

        /// <summary>
        /// Recoit le code de traitement du fichier XML
        /// </summary>



        /*
         * MEMBRES
         */

        /// <summary>
        /// Document XML
        /// </summary>
        protected XDocument xDoc;



        /*
         * CONSTRUCTEURS
         */

        public XMLFile() : base()
        {
            xDoc = new XDocument();
        }

        public XMLFile(string workDirectoryPath) : base(workDirectoryPath)
        {
            xDoc = new XDocument();
        }

        public XMLFile(string workDirectoryPath, string fileAbsolutePath) : base(workDirectoryPath, fileAbsolutePath)
        {
            readXmlFile();
        }

        public XMLFile(string workDirectoryPath,
                       string inputFileAbsolutePath,
                       string outputFileAbsolutePath) : base(workDirectoryPath, inputFileAbsolutePath, outputFileAbsolutePath)
        {
            readXmlFile();
        }



        /*
         *  SERVICES 
         */

        /// <summary>
        /// Lit le fichier XML dont le chemin absolu est passé en argument
        /// </summary>
        /// <param name="absolutePath">Chemin absolu vers le fichier XML</param>
        /// <returns>Int renseignant sur la réalisation de l'opération sur le fichier. Voir FILEIO.FILEOP_</returns>
        /// 
        protected int readXmlFile()
        {

            IOStatus = checkFile(inputFileAbsolutePath);

            if (IOStatus == FILEOP_SUCCESSFUL)
            {
                xDoc = XDocument.Load(inputFileAbsolutePath);
                if (xDoc != null)
                {
                    return FILEOP_SUCCESSFUL;
                }
                else
                {
                    // TODO: Valeur de retour à améliorer
                    return FILEOP_UNKNOWN_ERROR;
                }
            }
            else
            {
                return IOStatus;
            }
        }



        /// <summary>
        /// Renvoie le contenu d'un attribut du dernier noeud situé dans une lignée hyérarchique de noeuds
        /// </summary>
        /// <param name="attributeName">Nom XML de l'attribut à chercher</param>
        /// <param name="nodeNames">
        /// Tableau string contenant les noms XML des noeuds à examiner, en ordre hyérarchique.
        /// </param>
        /// <returns>
        /// Chaine contenant l'attribut, ou une chaine vide si l'attribut n'a pas été trouvé
        /// </returns>
        /// TODO A revoir complètement ! a peut-etre besoin d'un appel récursif
        public string getAttribute(string[] nodeNames, string attributeName)
        {
            if (xDoc == null || nodeNames == null || string.IsNullOrEmpty(attributeName)) { return ""; }
            List<XElement> collec = new List<XElement>();
            collec.Add((XElement)xDoc.FirstNode);
            for (int index = 0; index < nodeNames.Length; index++)
            {
                List<XElement> loopCollec = new List<XElement>();
                foreach (XElement item in collec)
                {
                    loopCollec.AddRange(item.Descendants(nodeNames[index]));
                }
                collec = loopCollec;
            }
            foreach (XElement xe in collec)
            {
                if (xe.Attribute(attributeName) != null)
                {
                    return xe.Attribute(attributeName).Value;
                }
            }
            return "";
        }



        /// <summary>
        /// Renvoie le contenu du premier noeud situé dans une lignée hyérarchique de noeuds, fournie en argument
        /// </summary>
        /// <param name="nodeNames">
        /// Tableau string contenant les noms XML des noeuds à examiner, en ordre hyérarchique.
        /// </param>
        /// <returns>
        /// Chaine contenant l'attribut, ou une chaine vide si l'attribut n'a pas été trouvé
        /// </returns>
        /// 
        // BUG Corrigé main non testé
        public string getValue(string[] nodeNames)
        {
            if (xDoc == null || nodeNames == null) { return ""; }
            List<XElement> collec = new List<XElement>();
            collec.Add((XElement)xDoc.FirstNode);
            for (int index = 0; index < nodeNames.Length; index++)
            {
                List<XElement> loopCollec = new List<XElement>();
                foreach (XElement item in collec)
                {
                    loopCollec.AddRange(item.Descendants(nodeNames[index]));
                }
                collec = loopCollec;
            }
            foreach (XElement xe in collec)
            {
                return xe.Value;
            }
            return "";
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
        public string getValue(string nodeName, string attributeName, string attributeValue)
        {
            if (nodeName != null && attributeName != null && attributeValue != null)
            {
                foreach (XElement elemt in xDoc.Descendants(nodeName))
                {
                    // Noeud trouvé !
                    foreach (XAttribute attrib in elemt.Attributes())
                    {
                        if (attrib.Name.LocalName.Equals(attributeName) && attrib.Value.Equals(attributeValue))
                        {
                            // Attribut trouvé
                            return elemt.Value;
                        }
                    }
                }
            }
            return "";
        }



        /// <summary>
        /// Renvoie le Boolean d'un noeud, identifié par le nom du noeud et un attribut de ce noeud, sinon la valeur par défaut 
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
        /// <param name="defaultValue">
        /// Valeur par défaut
        /// </param>
        /// <returns>
        /// Boolean contenu dans le loeud désigné, ou la valeur par défaut
        /// </returns>
        /// 
        public bool getBoolean(string nodeName, string attributeName, string attributeValue, bool defaultValue)
        {
            return getBoolOrDefault(getValue(nodeName, attributeName, attributeValue), defaultValue);
        }

        public bool getBoolOrDefault(string fieldString, bool defaultValue)
        {
            bool b;

            if (!Boolean.TryParse(fieldString, out b))
            {
                return defaultValue;
            }
            else
            {
                return b;
            }
        }



        public int getIntOrMinValue(string fieldString)
        {
            int i;
            if (!int.TryParse(fieldString, out i))
            {
                // TODO Vérifier la compatibilité sur les plateforme Int16/Int32/Int64
                //      Eventuellement faire un choix qui permettent de viser Linux et Windows 10...
                return int.MinValue;
            }
            else
            {
                return i;
            }
        }



        public double getDoubleOrNaN(string fieldString)
        {
            double d;
            if (Double.TryParse(fieldString, out d))
            {
                return d;
            }
            return double.NaN;
        }

    }

}