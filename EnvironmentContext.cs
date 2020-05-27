using System;
using System.IO;

///
/// Classe fournissant un environnement au processeur de commande
///




namespace AeroCalcCore {


    public class EnvironmentContext {


        /*
         * CONSTANTES
         */



        /*
         * MEMBRES 
         */



        /*
         * PROPRIETES
         */
        
        public string publicAppVersion {get; private set;}
        public string publicAppName { get; private set;}

        public int configFileVersion {get; private set;}

        public string appDirectory {get; private set;}
        public string configurationDirectory {get;private set;}
        public string modelsDirectory {get;private set;}
        public string scriptsDirectory {get; private set;}

        public string configFilePath {get; private set;}

        public string unitsFileName {get; private set;}

        public bool unitsEnabled {get; private set; }

        public bool verbose {get; private set;}
        public bool verboseAllowed {get; private set;}
        public bool logger {get; private set;}
        public int activeLanguageRef {get; private set;}


        public int status {get; private set;}
        

        /*
         * CONSTRUCTEUR
         */


        public EnvironmentContext() {
            status = 0;
            appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public EnvironmentContext(string configurationFileRelativePath) {
            
            status = 0;
            appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (setConfigFileDir(configurationFileRelativePath)) {
                loadConfiguration();
            }
            //configFilePath = appDirectory + configurationFileRelativePath;
            //configurationDirectory = Path.GetFullPath(configFilePath).Substring(0,configFilePath.LastIndexOf(Path.DirectorySeparatorChar));

            // Lecture et exploitation du fichier XML de configuration

        }



        /*
         * SERVICES
         */

        public int loadConfigFile(string configurationFileRelativePath) {
            if (setConfigFileDir(configurationFileRelativePath))
            {
                loadConfiguration();
            }
            return status;
        }



        /// Set VERBOSE according to VerboseAllowed
        public void setVerbose(bool v) {
            if (verboseAllowed) {
                verbose = v;
            }
            else {
                verbose = false;
            }
        }



        public override string ToString() {
            string msg;

            msg = "EnvironmentContext";
            msg += "\n appDirectory          : " + appDirectory;
            msg += "\n configurationDirectory: " + configurationDirectory;
            msg += "\n configurationFileName : " + configFilePath;
            msg += "\n unitsFileName         : " + unitsFileName;
            msg += "\n modelsDirectory       : " + modelsDirectory;
            msg += "\n scriptsDirectory      : " + scriptsDirectory;
            msg += "\n verboseAllowed        : " + verboseAllowed;
            msg += "\n verbose               : " + verbose;
            msg += "\n unitsEnabled          : " + unitsEnabled;
            msg += "\n logger                : " + logger;

            return msg;
        }



        /*
         * METHODES
         */

        bool setConfigFileDir(string configurationFileRelativePath) {
            configFilePath = appDirectory + configurationFileRelativePath;
            try {
                configurationDirectory = Path.GetFullPath(configFilePath).Substring(0, configFilePath.LastIndexOf(Path.DirectorySeparatorChar));
            } catch (ArgumentException) {
                return false;
            }
            return true;
        }



        /// Lecture du fichier de configuration
        /// TODO: Doit assurer le traitement de toutes les exceptions pour retourner un status exploitable
        bool loadConfiguration() {
            
            // Lecture et exploitation du fichier XML de configuration
            XMLFile configFile = new XMLFile("", configFilePath);
            if (configFile.IOStatus == FileIO.FILEOP_SUCCESSFUL) {
                // Fichier des unités de calcul
                unitsFileName = configurationDirectory +
                                System.IO.Path.DirectorySeparatorChar +
                                configFile.getValue(XMLFile.NODE_FILE, XMLFile.ATTRIB_NAME, XMLFile.UNITS);
                // Dossier des modèles de calcul
                modelsDirectory = appDirectory +
                                  configFile.getValue(XMLFile.NODE_DIR, XMLFile.ATTRIB_NAME, XMLFile.MODELS);
                // Dossier des scripts
                scriptsDirectory = appDirectory +
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
            else {
                status = configFile.IOStatus;
                return false;
            }
            
            // NO ERROR
            return true;
        }



        /// <summary>
        /// Return the boolean value of the designated XML field (a node name with an attribute), otherwise the default value
        /// </summary>
        bool getBoolValue(XMLFile configFile, string nodeName, string attributeName, string attributeValue, bool defaultVal) {

            bool b;

            if (!Boolean.TryParse(configFile.getValue(nodeName, attributeName, attributeValue), out b)) {
                return defaultVal;
            }
            else {
                return b;
            }

        }

    }

}