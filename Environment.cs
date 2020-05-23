using System;
using System.IO;

///
/// Classe fournissant un environnement au processeur de commande
///




namespace AeroCalcCore {


    public class Environment {


        /*
         * CONSTANTES
         */



        /*
         * MEMBRES 
         */


        /*
         * PROPRIETES
         */
        public string configurationDirectory {get;private set;}

        public string modelsDirectory {get;private set;}

        public string scriptsDirectory {get; private set;}

        public string configurationFileName {get; private set;}

        

        /*
         * CONSTRUCTEUR
         */

        public Environment(string configurationFilePath) {
            
            configurationFileName = Path.GetFileName(configurationFilePath);
            configurationDirectory = Path.GetFullPath(configurationFilePath);







        }


        /*
         * SERVICES
         */



        /*
         * METHODES
         */
        bool loadConfiguration() {
            
            // Lecture et exploitation du fichier XML de configuration
            ConnectorXML configFile = new ConnectorXML("", configFileFullPath());
            return true;
        }

        string configFileFullPath() {
            return configurationDirectory + Path.DirectorySeparatorChar + configurationFileName;

        }

    }

}