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

        

        /*
         * CONSTRUCTEUR
         */

        public EnvironmentContext(string configurationFileRelativePath) {
            
            appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            configFilePath = appDirectory + configurationFileRelativePath;
            configurationDirectory = Path.GetFullPath(configFilePath).Substring(0,configFilePath.LastIndexOf(Path.DirectorySeparatorChar));

            // Lecture et exploitation du fichier XML de configuration
            loadConfiguration();

        }



        /*
         * SERVICES
         */

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

        bool loadConfiguration() {
            
            // Lecture et exploitation du fichier XML de configuration
            ConnectorXML configFile = new ConnectorXML("", configFilePath);
            // Fichier des unités de calcul
            unitsFileName = configurationDirectory + 
                            System.IO.Path.DirectorySeparatorChar + 
                            configFile.getValue(ConnectorXML.NODE_FILE, ConnectorXML.ATTRIB_NAME, ConnectorXML.UNITS);
            // Dossier des modèles de calcul
            modelsDirectory = appDirectory +
                              configFile.getValue(ConnectorXML.NODE_DIR, ConnectorXML.ATTRIB_NAME, ConnectorXML.MODELS);
            // Dossier des scripts
            scriptsDirectory = appDirectory +
                               configFile.getValue(ConnectorXML.NODE_DIR, ConnectorXML.ATTRIB_NAME, ConnectorXML.SCRIPTS);
            
            // Mode VERBOSE ALLOWED
            verboseAllowed = getBoolValue(configFile, ConnectorXML.NODE_SETTING, ConnectorXML.ATTRIB_NAME, ConnectorXML.VERBOSE_ALLOWED, true);
            // Mode VERBOSE
            setVerbose(getBoolValue(configFile, ConnectorXML.NODE_SETTING, ConnectorXML.ATTRIB_NAME, ConnectorXML.VERBOSE, false));
            // Mode UnitsEnabled
            unitsEnabled = getBoolValue(configFile, ConnectorXML.NODE_SETTING, ConnectorXML.ATTRIB_NAME, ConnectorXML.UNITS_ENABLED, true);
            // Mode Logger
            logger = getBoolValue(configFile, ConnectorXML.NODE_SETTING, ConnectorXML.ATTRIB_NAME, ConnectorXML.LOGGER, true);
            return true;
        }



        /// <summary>
        /// Return the boolean value of the designated XML field (a node name with an attribute), otherwise the default value
        /// </summary>
        bool getBoolValue(ConnectorXML configFile, string nodeName, string attributeName, string attributeValue, bool defaultVal) {

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