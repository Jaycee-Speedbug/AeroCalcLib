﻿using System;



namespace AeroCalcCore {


    /// <summary>
    /// Classe définissant le processeur de commande AeroCalcCommand
    /// Le processeur travaille sur la base d'une commande pré-traitée par un préprocesseur
    /// et n'accepte donc pas de commande en ligne de commande au format texte.
    /// 
    /// Le processeur est principalement en charge des traitements numériques.
    /// 
    /// </summary>
    public class AeroCalcCommandProcessor {

        /*
         * CONSTANTES
         */

        //private const bool verboseAllowed = true; // Selon la version du code, renseigner la valeur à true pour autoriser le mode Verbose
        private const int SUCCESS = 1;
        private const int NO_SUCCESS = 0;



        /*
         * PROPRIETES
         */

        // public bool verbose { get; private set; }

        // public bool verboseAllowed { get; private set; }

        public bool initialized { get; private set; }

        // public string modelsDirectory { get; private set; }

        // public bool unitsEnabled { get; private set; }

        // public string publicAppVersion { get; private set; }

        // public string publicAppName { get; private set; }

        // public string unitsFilePath { get; private set; }

        public EnvironmentContext EnvContext {get; private set;}

        public ConnectorScriptFile ScriptConnect { get; private set; }

        public DataModelContainer Container { get; private set; }

        public UnitDictionary Units { get; private set; }


        /*
         * MEMBRES
         */

        private char[] commandSeparator = { ' ', '²' };


        /*
         * CONSTRUCTEURS
         */

        /// <summary>
        /// Construit un objet de traitement des commandes
        /// </summary>
        /// 
        public AeroCalcCommandProcessor() {

            // Création du container de données de performances
            Container = new DataModelContainer();
            // Création de l'objet de connexion aux fichiers de Script
            ScriptConnect = new ConnectorScriptFile();
            // Reglage initial du mode verbose
            // DEBUG
            // verbose = true;
            // Reglage intial du flag initialized
            initialized = false;
        }



        public AeroCalcCommandProcessor(string configFileRelativePath) {
            
            // Création du container de données de performances
            Container = new DataModelContainer();
            // Création de l'objet de connexion aux fichiers de Script
            ScriptConnect = new ConnectorScriptFile();
            // Création de l'objet d'environnement
            EnvContext = new EnvironmentContext(configFileRelativePath);
        }



        /// <summary>
        /// Fonction principale de traitement
        /// 
        /// </summary>
        /// <param name="txtCommand">Ligne de commande fournie par l'utilisateur, ou tirée d'un fichier
        /// de script</param>
        /// <returns>retourne une commande traitée, sous forme d'un objet AeroCalcCommand</returns>
        /// <remarks>
        /// Fonction assurant le routage des commandes, éviter tout traitement et privilégier un appel à une
        /// fonction privée respectant le nommage suivant:
        /// cmd_XXXX()
        /// </remarks>
        /// 
        public AeroCalcCommand process(string txtCommand) {

            // Construction d'un objet AeroCalcCommand et exécution de la commande simple
            // Une commande complexe (nécessitant l'exécution de plus d'une commande simple) doit
            // d'abord être décomposée



            AeroCalcCommand Cmd = new AeroCalcCommand(txtCommand, Container, EnvContext.verbose);

            
            // Certaines commandes rendent la main pour être traitées ici, dans le processeur
            switch (Cmd.action) {

                case AeroCalcCommand.ACTION_INIT_INTERPRETER:
                // Initialisation
                if (initProcessor()) {
                    Cmd.setEventCode(AeroCalcCommand.EVENTCODE_INIT_SUCCESSFULL);
                    Cmd.setResultText(initMsg());
                }
                else {
                    Cmd.setEventCode(AeroCalcCommand.EVENTCODE_INIT_UNSUCCESSFULL);
                    Cmd.setCommentText(AeroCalcCommand.COMMENT_ERROR_INIT_UNSUCCESSFULL);
                }
                break;

                case AeroCalcCommand.ACTION_SCRIPTFILE:
                readScriptFile(Cmd.subs[1]);
                break;
                
                case AeroCalcCommand.ACTION_HELP:
                Cmd.setResultText(helpMsg());
                Cmd.setEventCode(AeroCalcCommand.EVENTCODE_PROCESS_SUCCESSFULL);
                break;



                
            }
            
            return Cmd;
        }



        /// <summary>
        /// Traitement de la commande de lecture et d'exécution d'un fichier de script
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>
        /// True, si la commande a été traitée sans erreur
        /// </returns>
        private bool readScriptFile(string fileAbsolutePath) {            
            // COMMANDE NON SUPPORTEE
            return true;
        }



        /*
        /// <summary>
        /// Traite la commande d'impression des performances
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns></returns>
        private bool cmd_PRINT(AeroCalcCommand Cmd) {
            // COMMANDE NON SUPPORTEE
            return true;
        }
        */



/*
        /// <summary>
        /// Traite la commande de chargement d'un ou plusieurs modèles de données
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>Code du traitement de l'opération</returns>
        /// <remarks>TO BE DEVELOPPED</remarks>
        /// 
        private bool cmd_LOAD(AeroCalcCommand Cmd) {
            int counter = 0;
            foreach(String filter in Cmd.subs) {
                // subs[0] = "LOAD" est traité, mais un mot réservé ne doit pas être utilisé comme identificateur
                counter += Container.loadDataModels(filter);
            }
            if (counter > 0) {
                Cmd.postProcess(AeroCalcCommand.EVENTCODE_LOAD_MODELS_SUCCESSFULL);
                // Ajout du nombre de modèles chargés
                Cmd.setResultText(String.Format("{0} modèle(s) ont été chargés en mémoire.", counter));
            }
            else {
                Cmd.postProcess(AeroCalcCommand.EVENTCODE_NO_MODEL_LOADED);
            }
            return true;
        }
*/



        /// <summary>
        /// Traite la commande d'initialisation de l'interpreteur
        /// Première (et unique) commande à pouvoir être lançée
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>Etat de réussite de la commande</returns>
        /// 
        private bool initProcessor() {

            if (!initialized) {
                //
                // TODO: Ici, coder l'initialisation
                //

                //bool initSuccessfull = false;

                // TODO: Remplacer le code de chargement de la config par l'utilisation d'un objet 
                
                // DEBUG
                Console.WriteLine(EnvContext.ToString());
                
                /*
                bool verboseSetting;
                string[] xmlNames = { ConnectorXML.XML_NODE_APP, ConnectorXML.XML_NODE_CONFIG, "" };
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string configFilePath = baseDir + AeroCalc.CONFIG_FILE_DIRECTORY + 
                                                  System.IO.Path.DirectorySeparatorChar + 
                                                  AeroCalc.CONFIG_FILE_NAME;
                
                // DEBUG 
                System.Console.WriteLine("Configuration file: " + configFilePath);
                // END DEBUG

                // Lecture et exploitation du fichier XML de configuration
                ConnectorXML configFile = new ConnectorXML("", configFilePath);

                // Répertoire des modèles de performance
                xmlNames[2] = ConnectorXML.XML_DIRECTORY;
                modelsDirectory = baseDir;
                modelsDirectory += configFile.getAttribute(ConnectorXML.XML_RELATIVE_PATH, xmlNames);
                modelsDirectory = baseDir;
                modelsDirectory += configFile.getValue(ConnectorXML.XML_NODE_DIR, ConnectorXML.XML_ATTR_NAME, ConnectorXML.XML_MODELS);

                // DEBUG 
                System.Console.WriteLine("Models directory: " + modelsDirectory);
                // END DEBUG
                // DEBUG
                // modelsDirectory = AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar + "data";
                // END DEBUG

                Container.setDataModelsDirectory(modelsDirectory);

                // Dictionnaire des unités employées
                /*
                xmlNames[2] = ConnectorXML.XML_UNITS;
                unitsFilePath = baseDir;
                unitsFilePath += configFile.getAttribute(ConnectorXML.XML_RELATIVE_PATH, xmlNames) + System.IO.Path.DirectorySeparatorChar;
                unitsFilePath += configFile.getAttribute(ConnectorXML.XML_NODE_FILE, xmlNames);
                */

                // DEBUG
                // unitsFilePath = modelsDirectory + System.IO.Path.DirectorySeparatorChar + "UnitsDictionary.csv";
                // END DEBUG
                /*
                unitsFilePath = modelsDirectory + 
                                System.IO.Path.DirectorySeparatorChar + 
                                configFile.getValue(ConnectorXML.XML_NODE_FILE, ConnectorXML.XML_ATTR_NAME, ConnectorXML.XML_UNITS);
                */
                // DEBUG 
                // System.Console.WriteLine("Units file: " + unitsFilePath);
                // END DEBUG

                // Container.dataUnits = new ConnectorUnitCSVFile().readFile(unitsFilePath);
                //Units = new ConnectorUnitCSVFile().readFile(unitsFilePath);

                // Mode Verbose
                /*
                xmlNames[2] = ConnectorXML.XML_VERBOSE;
                if (!Boolean.TryParse(configFile.getAttribute(ConnectorXML.XML_ALLOWED, xmlNames),
                                      out verboseSetting)) {
                    verboseAllowed = false;
                }
                else {
                    verboseAllowed = verboseSetting ? true : false;
                }
                */

                /*
                if (!Boolean.TryParse(configFile.getValue(ConnectorXML.XML_NODE_SETTING, 
                                                          ConnectorXML.XML_ATTR_NAME, 
                                                          ConnectorXML.XML_VERBOSE_ALLOWED),
                                      out verboseSetting)) {
                    verboseAllowed = false;
                }
                else {
                    verboseAllowed = verboseSetting ? true : false;
                }
                */


                // Fin du processus d'initialisation
                //initSuccessfull = true;
                initialized = true;
                return true;
            }
            return false;
            // Already initialized
        }



        /*
        /// <summary>
        /// Traitement de la commande de création d'une liste des unités de mesures
        /// disponibles dans le container de performances
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>True si le traitement n'a pas généré d'erreur</returns>
        /// <remarks>DEBUG: Objectif non atteint, on doit pouvoir lister les unités avec filtrage</remarks>
        /// 
        private bool cmd_LIST_UNITS(AeroCalcCommand Cmd) {
            // Liste des unités enregitrées
            Cmd.setResultText("");
            string msg = "";
            foreach (UnitItem item in Units.getUnits()) {
                msg += item.ToString() + "\n";
                Cmd.setResultText(msg);
            }
            Cmd.postProcess(AeroCalcCommand.EVENTCODE_PROCESS_SUCCESSFULL);
            return true;
        }
        */



        /*
        /// <summary>
        /// Traitement d'une commande de calcul de performance.
        /// Retourne True si le traitement de la commande est complet.
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// 
        ///</remarks>
        /// 
        private bool cmd_CALCULATE(AeroCalcCommand Cmd) {
            // Filtre des commandes inexploitables
            if (Cmd == null) {
                // On ne devrait pas recevoir une référence nulle !
                Cmd.postProcess(AeroCalcCommand.EVENTCODE_PROCESSOR_ERROR);
                return true;
            }
            if (Cmd.action != AeroCalcCommand.ACTION_CALCULATE) {
                // On ne devrait pas recevoir un autre type d'opération qu'un calcul !
                Cmd.postProcess(AeroCalcCommand.EVENTCODE_PROCESSOR_ERROR);
                return true;
            }
            // Initialisation
            double numResult = double.NaN;
            // constitution de la Liste des facteurs communiqués dans la commande
            List<CommandFactor> factorList = new List<CommandFactor>();
            for (int count = 1; count < Cmd.subs.Length; count++) {
                factorList.Add(getFactor(Cmd.subs[count]));
            }
            try {
                numResult = Container.compute(Cmd.subs[0], factorList);
            } catch (AeroCalcException e) {
                // La commande a échouée pendant le calcul
                Cmd.setEventCode(AeroCalcCommand.EVENTCODE_PROCESSOR_ERROR);
                // DEBUG, revoir le formatage du message d'erreur, en utilisant aussi la nature de l'exception
                Cmd.setCommentText("Erreur lors du calcul de " + e.modelName + " : " + e.factor);
            }
            if (!double.IsNaN(numResult)) {
                // Réussite du calcul
                Cmd.setEventCode(AeroCalcCommand.EVENTCODE_CALCULATE_SUCCESSFULL);
                Cmd.setCommentText(AeroCalcCommand.RESULT_CALCULATE_SUCCESSFULL);
                Cmd.setNumericResult(numResult);
                Cmd.setResultText(Cmd.rawTxtCommand + " = " + Cmd.numericResult);
            }
                else {
                // La commande a échouée
                // DEBUG revoir les cas ou on peut avoir un résultat NaN sans exception générée
                Cmd.eventCode = AeroCalcCommand.EVENTCODE_UNKNOWN_COMMAND_WORD;
                Cmd.txtComment = AeroCalcCommand.RESULT_ERROR_UNKNOWN_COMMAND_WORD;
            }
            return true;
        }
        */



        /// <summary>
        /// Met en marche le mode verbose
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <param name="verboseModeRequested">Etat du mode verbose demandé par l'utilisateur</param>
        /// <returns>
        /// True, si tout s'est bien passé
        /// </returns>
        private bool setVerboseMode(AeroCalcCommand Cmd, bool verboseModeRequested) {


            Cmd.setNumericResult(double.NaN);

            if (!EnvContext.verboseAllowed) {
                Cmd.setEventCode(AeroCalcCommand.EVENTCODE_UNABLE_VERBOSE_MODIFICATION);
                Cmd.setResultText(AeroCalcCommand.RESULT_UNABLE_VERBOSE_MODIFICATION);
            }
            else {
                if (verboseModeRequested) {
                    // Commande VERBOSE
                    EnvContext.setVerbose(true);
                    Cmd.setEventCode(AeroCalcCommand.EVENTCODE_VERBOSE_ACTIVE);
                    Cmd.setResultText(AeroCalcCommand.RESULT_VERBOSE_ACTIVE);
                }
                else {
                    // Commande STOP VERBOSE
                    EnvContext.setVerbose(false);
                    Cmd.setEventCode(AeroCalcCommand.EVENTCODE_VERBOSE_INACTIVE);
                    Cmd.setResultText(AeroCalcCommand.RESULT_VERBOSE_INACTIVE);
                }
            }
            return true;
        }



        /*
        /// <summary>
        /// Stoppe le mode verbose
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>1, si tout s'est bien passé</returns>
        public int cmd_STOP_VERBOSE() {
            // Arrêt du mode verbose
            verbose = false;
            //Cmd.postProcess(AeroCalcCommand.EVENTCODE_VERBOSE_INACTIVE);
            return AeroCalcCommand.EVENTCODE_VERBOSE_INACTIVE;
        }
        */



        /// <summary>
        /// Retourne un objet commandFactor sur la base d'une saisie texte par l'utilisateur
        /// </summary>
        /// <param name="subString">Chaine à analyser comme facteur d'un calcul multi-dimensionnel</param>
        /// <returns>objet commandFactor contenant les éléments du facteur</returns>
        /// <remarks>
        /// DEBUG: OLD doit être remplacé par un constructeur d'objet commandFactor
        /// </remarks>
        /// 
        private CommandFactor getFactor(string subString) {

            string name = "";
            int unitDictionaryIndex = 0;
            double val = double.NaN;
            // DEBUG les séparateurs ne doit pas être locaux
            char[] separators = { '=', ':' };
            string[] words = subString.Split(separators);

            if (words.Length == 2) {
                // Facteur sans unité
                if (double.TryParse(words[1], out val)) {
                    name = words[0];
                    unitDictionaryIndex = AeroCalc.UNIT_UNDETERMINED;
                }
            }
            else if (words.Length == 3) {
                // Facteur avec unité
                if (double.TryParse(words[1], out val)) {
                    unitDictionaryIndex = Units.getIndexByAlias(words[2]);
                    if (unitDictionaryIndex != AeroCalc.UNIT_UNDETERMINED) {
                        name = words[0];
                    }
                }
            }
            return new CommandFactor(name, val, unitDictionaryIndex);
        }
        /// <summary>
        /// Génère la chaine de caractère en réponse à la demande d'aide
        /// </summary>
        /// <returns>Chaine de caractère d'aide à l'utilisateur</returns>
        /// 
        private string helpMsg() {

            string msg = "";

            msg += "************  AeroCalc  HELP  ************\n";
            msg += "Commands:\n";
            msg += "HELP         : Get some help about AeroCalc commands\n";
            msg += "VERBOSE      : Interpreter issue extended information on command process\n";
            msg += "CONVERT      : Conversion between various units\n";
            msg += "LOAD PERF    : Load flight performance models from current data directory\n";
            msg += "LIST         : List performance models currently loaded\n";
            msg += "PRINT PERF   : Print full flight performance data in memory\n";
            msg += "CATALOG      : Print all flight performances model names loadable from current\n";
            msg += "data directory\n";
            msg += "\n";
            msg += "EXIT         : End AeroCalc\n";
            return msg;
        }



        /// <summary>
        /// Génère la chaine de caractère du message d'accueil
        /// </summary>
        /// <returns>Chaine de caractère du message d'accueil</returns>
        /// 
        private string initMsg() {

            string msg = "";

            msg += "*****************************************\n";
            msg += "*                                       *\n";
            msg += "*            AeroCalc2017 CL             *\n";
            msg += "*                                       *\n";
            msg += "* Flight Performance Computer           *\n";
            msg += "* Command Line version                  *\n";
            msg += "*                                       *\n";
            msg += "* PetitLu AeroDevelopment (c) 2017      *\n";
            msg += "*                                       *\n";
            msg += "*****************************************\n";
            msg += "\n";
            return msg;
        }

    }

}