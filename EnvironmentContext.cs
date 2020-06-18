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



        public int status { get; private set; }


        /*
         * CONSTRUCTEUR
         */
        /// <summary>
        /// Construit un objet EnvironmentContext, en charge de tous les paramètres de configuration
        /// utiles lors de l'exécution
        /// </summary>
        public EnvironmentContext()
        {
            status = 0;
            appDirPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Construit un objet EnvironmentContext, en charge de tous les paramètres de configuration
        /// utiles lors de l'exécution
        /// </summary>
        public EnvironmentContext(string configFileRelPath)
        {
            status = 0;
            appDirPath = AppDomain.CurrentDomain.BaseDirectory;
            loadConfigFile(configFileRelPath);
        }



        /*
         * SERVICES
         */
        /// <summary>
        /// Réalise la lecture de tous les paramètres disponibles dans le fichier de configuration
        /// </summary>
        /// <param name="configFileRelPath"></param>
        /// <returns></returns>
        public int loadConfigFile(string configFileRelPath)
        {
            // Path absolu du fichier de configuration
            configFilePath = appDirPath + configFileRelPath;
            // Dossier de configuration
            configDirPath = configFilePath.Substring(0, configFilePath.LastIndexOf(Path.DirectorySeparatorChar));
            // Lecture et exploitation du fichier XML de configuration
            ConfigXMLFile configFile = new ConfigXMLFile(configFilePath);

            if (configFile.IOStatus == FileIO.FILEOP_SUCCESSFUL)
            {
                // Langue active
                activeLangIndex = 0;
                activeLang = configFile.getValue(XMLFile.NODE_SETTING, XMLFile.ATTRIB_NAME, XMLFile.LANGUAGE);
                // Version du fichier de configuration (non utilisé)
                // TODO implémenter l'utilisation du numéro de version du fichier de configuration
                //string[] nodes = { XMLFile.NODE_CONFIG };
                configFileVersion = configFile.getAttribute(new string[] { XMLFile.NODE_APP, XMLFile.NODE_CONFIG},
                                                            XMLFile.ATTRIB_VERSION);
                // Packs de langue
                Langs = configFile.GetLanguagesFromXML(configDirPath);
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



        /// <summary>
        /// Set VERBOSE according to VerboseAllowed
        /// </summary>
        /// <param name="v"></param>
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



        public void setActiveLanguage(int libIndex) {
            if (libIndex>=0 && libIndex < Langs.Count) {
                activeLangIndex = libIndex;
                activeLang = Langs.Library[activeLangIndex].isoCode;
            }
        }



        public Language getActiveLanguage() {
            return Langs.Find(activeLang);
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
            foreach (Language l in Langs.Library)
            {
                msg += "\n     " + l.isoCode + " " + l.name + " " + l.fileAbsolutePath;
            }
            return msg;
        }



        /*
         * METHODES
         */

    }

}