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

        public int activeLanguageRef {get; private set;}

        

        /*
         * CONSTRUCTEUR
         */

        public EnvironmentContext(string configurationFilePath) {
            
            appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            configFilePath = appDirectory + configurationFilePath;
            configurationDirectory = Path.GetFullPath(configurationFilePath);

            loadConfiguration();

            // Lecture et exploitation du fichier XML de configuration
            // ConnectorXML configFile = new ConnectorXML("", configurationFilePath);

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
            msg += "\n appDirectory: " + appDirectory;
            msg += "\n configurationDirectory: " + configurationDirectory;
            msg += "\n configurationFileName: " + configFilePath;
            msg += "\n unitsFileName: " + unitsFileName;
            msg += "\n modelsDirectory: " + modelsDirectory;
            msg += "\n scriptsDirectory: " + scriptsDirectory;
            msg += "\n verboseAllowed: " + verboseAllowed;
            msg += "\n verbose: " + verbose;
            msg += "\n unitsEnabled: " + unitsEnabled;

            return msg;
        }



        /*
         * METHODES
         */

        bool loadConfiguration() {
            
            bool b;

            // Lecture et exploitation du fichier XML de configuration
            ConnectorXML configFile = new ConnectorXML("", configFilePath);
            // Fichier des unités de calcul
            unitsFileName = configurationDirectory + 
                            System.IO.Path.DirectorySeparatorChar + 
                            configFile.getValue(ConnectorXML.XML_NODE_FILE, ConnectorXML.XML_ATTR_NAME, ConnectorXML.XML_UNITS);
            // Dossier des modèles de calcul
            modelsDirectory = appDirectory +
                              System.IO.Path.DirectorySeparatorChar + 
                              configFile.getValue(ConnectorXML.XML_NODE_DIR, ConnectorXML.XML_ATTR_NAME, ConnectorXML.XML_MODELS);
            // Mode VERBOSE
            if (!Boolean.TryParse(configFile.getValue(ConnectorXML.XML_NODE_SETTING, 
                                                      ConnectorXML.XML_ATTR_NAME, 
                                                      ConnectorXML.XML_VERBOSE_ALLOWED), out b)) {
                verboseAllowed = false;
            }
            else {
                verboseAllowed = b ? true : false;
            }
            if (!Boolean.TryParse(configFile.getValue(ConnectorXML.XML_NODE_SETTING, 
                                                      ConnectorXML.XML_ATTR_NAME, 
                                                      ConnectorXML.XML_VERBOSE), out b)) {
                verbose = false;
            }
            else {
                setVerbose(b);
            }
            // Mode UnitsEnabled
            if (!Boolean.TryParse(configFile.getValue(ConnectorXML.XML_NODE_SETTING, 
                                                      ConnectorXML.XML_ATTR_NAME, 
                                                      ConnectorXML.XML_UNITS_ENABLED), out b)) {
                unitsEnabled = false;
            }
            else {
                unitsEnabled = b ? true : false;
            }
            return true;
        }

    }

}