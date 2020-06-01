using System;
using System.IO;

///
/// Classe fournissant un environnement au processeur de commande
///




namespace AeroCalcCore
{


    public class EnvironmentContext
    {


        /*
         * CONSTANTES
         */



        /*
         * MEMBRES 
         */



        /*
         * PROPRIETES
         */

        public string publicAppVersion { get; private set; }
        public string publicAppName { get; private set; }

        public string configFileVersion { get; private set; }

        public string appDirPath { get; private set; }
        public string configDirPath { get; private set; }
        public string modelsDirPath { get; private set; }
        public string scriptsDirPath { get; private set; }
        public string logDirPath { get; private set; }

        public string configFilePath { get; private set; }

        public string unitsFileName { get; private set; }

        public bool unitsEnabled { get; private set; }

        public bool verbose { get; private set; }
        public bool verboseAllowed { get; private set; }
        public bool logger { get; private set; }
        public int activeLangIndex { get; private set; }

        public string activeLang { get; private set; }

        public Languages Langs { get; private set; }

        // public string[] languageList { get; private set; }


        public int status { get; private set; }


        /*
         * CONSTRUCTEUR
         */

        public EnvironmentContext()
        {
            status = 0;
            appDirPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public EnvironmentContext(string configFileRelPath)
        {

            status = 0;
            appDirPath = AppDomain.CurrentDomain.BaseDirectory;
            loadConfigFile(configFileRelPath);
        }



        /*
         * SERVICES
         */

        public int loadConfigFile(string configFileRelPath)
        {

            // Path absolu du fichier de configuration
            configFilePath = appDirPath + configFileRelPath;
            // Dossier de configuration
            configDirPath = configFilePath.Substring(0, configFilePath.LastIndexOf(Path.DirectorySeparatorChar));
            // Lecture et exploitation du fichier XML de configuration
            XMLFile configFile = new XMLFile("", configFilePath);

            if (configFile.IOStatus == FileIO.FILEOP_SUCCESSFUL)
            {
                // Langue active
                activeLangIndex = 0;
                activeLang = configFile.getValue(XMLFile.NODE_SETTING, XMLFile.ATTRIB_NAME, XMLFile.LANGUAGE);
                // Dossier de l'application
                appDirPath = appDirPath;
                // Fichier de configuration
                configFilePath = configFilePath;

                // Version du fichier de configuration (non utilisé)
                // TODO implémenter l'utilisation du numéro de version du fichier de configuration
                string[] nodes = { XMLFile.NODE_CONFIG };
                configFileVersion = configFile.getAttribute(nodes, XMLFile.ATTRIB_VERSION);

                // Packs de langue
                Langs = configFile.GetLanguagesFromXML();
                // Dossiers des logs
                logDirPath = appDirPath + configFile.getValue(XMLFile.NODE_DIR, XMLFile.ATTRIB_NAME, XMLFile.LOGS);
                // Mode Logger
                logger = configFile.getBoolean(XMLFile.NODE_SETTING, XMLFile.ATTRIB_NAME, XMLFile.LOGGER, true);
                // Dossier des modèles de calcul
                modelsDirPath = appDirPath +
                                  configFile.getValue(XMLFile.NODE_DIR, XMLFile.ATTRIB_NAME, XMLFile.MODELS);
                // Nom publique de l'app
                publicAppVersion = "";
                // N° publique de version
                publicAppVersion = "";
                // Dossier des scripts
                scriptsDirPath = appDirPath +
                                   configFile.getValue(XMLFile.NODE_DIR, XMLFile.ATTRIB_NAME, XMLFile.SCRIPTS);
                // Mode UnitsEnabled
                unitsEnabled = configFile.getBoolean(XMLFile.NODE_SETTING, XMLFile.ATTRIB_NAME, XMLFile.UNITS_ENABLED, true);
                // Fichier des unités de calcul
                unitsFileName = configDirPath +
                                System.IO.Path.DirectorySeparatorChar +
                                configFile.getValue(XMLFile.NODE_FILE, XMLFile.ATTRIB_NAME, XMLFile.UNITS);
                // Mode VERBOSE ALLOWED
                verboseAllowed = configFile.getBoolean(XMLFile.NODE_SETTING, XMLFile.ATTRIB_NAME, XMLFile.VERBOSE_ALLOWED, true);
                // Mode VERBOSE
                setVerbose(configFile.getBoolean(XMLFile.NODE_SETTING, XMLFile.ATTRIB_NAME, XMLFile.VERBOSE, false));

                status = FileIO.FILEOP_SUCCESSFUL;

                // Recherche de l'index du language sélectionné
                activeLangIndex = Langs.languageIndex(activeLang);
            }
            else
            {
                // Config file unreadable
                status = configFile.IOStatus;
            }
            return status;
        }



        /// Set VERBOSE according to VerboseAllowed
        public void setVerbose(bool v)
        {
            if (verboseAllowed)
            {
                verbose = v;
            }
            else
            {
                verbose = false;
            }
        }



        public override string ToString()
        {
            string msg;

            msg = "EnvironmentContext";
            msg += "\n activeLanguageRef     : " + activeLangIndex;
            msg += "\n activeLang            : " + activeLang;
            msg += "\n appDirectory          : " + appDirPath;
            msg += "\n configurationDirectory: " + configDirPath;
            msg += "\n configurationFileName : " + configFilePath;
            msg += "\n configFileVersion     : " + configFileVersion;
            msg += "\n unitsFileName         : " + unitsFileName;
            msg += "\n modelsDirectory       : " + modelsDirPath;
            msg += "\n scriptsDirectory      : " + scriptsDirPath;
            msg += "\n logDirPath            : " + logDirPath;
            msg += "\n verboseAllowed        : " + verboseAllowed;
            msg += "\n verbose               : " + verbose;
            msg += "\n unitsEnabled          : " + unitsEnabled;
            msg += "\n logger                : " + logger;
            msg += "\n Languages             : ";
            foreach (Language l in Langs.LanguageLibrary)
            {
                msg += "\n     " + l.shortName + " " + l.name + " " + l.fileAbsolutePath;
            }
            return msg;
        }



        /*
         * METHODES
         */

        /*  Supprimé pour éviter les traitements bas niveau
        bool setConfigFileDir(string configurationFileRelativePath)
        {
            configFilePath = appDirPath + configurationFileRelativePath;
            try
            {
                configDirPath = Path.GetFullPath(configFilePath).Substring(0, configFilePath.LastIndexOf(Path.DirectorySeparatorChar));
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }
        */



        /// Lecture du fichier de configuration
        /// SUPPR: remplacé par un traitement direct dans une fonction publique
        /*
        bool loadConfiguration()
        {

            // Lecture et exploitation du fichier XML de configuration
            XMLFile configFile = new XMLFile("", configFilePath);
            if (configFile.IOStatus == FileIO.FILEOP_SUCCESSFUL)
            {
                // Fichier des unités de calcul
                unitsFileName = configDirPath +
                                System.IO.Path.DirectorySeparatorChar +
                                configFile.getValue(XMLFile.NODE_FILE, XMLFile.ATTRIB_NAME, XMLFile.UNITS);
                // Dossier des modèles de calcul
                modelsDirPath = appDirPath +
                                  configFile.getValue(XMLFile.NODE_DIR, XMLFile.ATTRIB_NAME, XMLFile.MODELS);
                // Dossier des scripts
                scriptsDirPath = appDirPath +
                                   configFile.getValue(XMLFile.NODE_DIR, XMLFile.ATTRIB_NAME, XMLFile.SCRIPTS);

                // Mode VERBOSE ALLOWED
                verboseAllowed = getBoolValue(configFile, XMLFile.NODE_SETTING, XMLFile.ATTRIB_NAME, XMLFile.VERBOSE_ALLOWED, true);
                // Mode VERBOSE
                setVerbose(getBoolValue(configFile, XMLFile.NODE_SETTING, XMLFile.ATTRIB_NAME, XMLFile.VERBOSE, false));
                // Mode UnitsEnabled
                unitsEnabled = getBoolValue(configFile, XMLFile.NODE_SETTING, XMLFile.ATTRIB_NAME, XMLFile.UNITS_ENABLED, true);
                // Mode Logger
                logger = getBoolValue(configFile, XMLFile.NODE_SETTING, XMLFile.ATTRIB_NAME, XMLFile.LOGGER, true);

                status = FileIO.FILEOP_SUCCESSFUL;
            }
            else
            {
                status = configFile.IOStatus;
                return false;
            }
            // NO ERROR
            return true;
        }
        */



        /// <summary>
        /// Return the boolean value of the designated XML field (a node name with an attribute), otherwise the default value
        /// </summary>
        /*
        bool getBoolValue(XMLFile configFile, string nodeName, string attributeName, string attributeValue, bool defaultVal)
        {

            bool b;

            if (!Boolean.TryParse(configFile.getValue(nodeName, attributeName, attributeValue), out b))
            {
                return defaultVal;
            }
            else
            {
                return b;
            }

        }
        */

    }

}