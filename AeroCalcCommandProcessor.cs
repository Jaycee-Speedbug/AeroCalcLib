using System;



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



        /*
         * PROPRIETES
         */

        public bool initialized { get; private set; }

        public EnvironmentContext EnvContext {get; private set;}

        public ScriptFile ScriptConnect { get; private set; }

        public DataModelContainer ModelLib { get; private set; }

        public Units UnitLib { get; private set; }


        /*
         * MEMBRES
         */




        /*
         * CONSTRUCTEURS
         */

        /// <summary>
        /// Construit un objet de traitement des commandes
        /// </summary>
        /// 
        public AeroCalcCommandProcessor() {
            // Construction du container de données de performances
            ModelLib = new DataModelContainer();
            // Construction de l'objet de connexion aux fichiers de Script
            ScriptConnect = new ScriptFile();
            // Construction de l'objet d'environnement
            EnvContext = new EnvironmentContext();
            // Construction du dictionnaire des unités 
            UnitLib = new Units();
            // Reglage intial du flag initialized
            initialized = false;
        }



        /*
        public AeroCalcCommandProcessor(string configFileRelativePath) {
            // Création du container de données de performances
            Container = new DataModelContainer();
            // Création de l'objet de connexion aux fichiers de Script
            ScriptConnect = new ScriptFile();
            // Création de l'objet d'environnement
            EnvContext = new EnvironmentContext(configFileRelativePath);
            // Reglage intial du flag initialized
            initialized = false;
        }*/



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

            AeroCalcCommand Cmd = new AeroCalcCommand(txtCommand, ModelLib, EnvContext);

            // Certaines commandes rendent la main pour être traitées ici, dans le processeur
            switch (Cmd.action) {

                case AeroCalcCommand.ACTION_INIT_INTERPRETER:
                // Initialisation
                if (initProcessor(Cmd)) {
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



        /// <summary>
        /// Traite la commande d'initialisation de l'interpreteur
        /// Première (et unique) commande à pouvoir être lançée
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>Etat de réussite de la commande</returns>
        private bool initProcessor(AeroCalcCommand Cmd) {
            
            int loadStatus;
            UnitsXMLFile unitsFile = new UnitsXMLFile("");

            if (!initialized) {
                //
                // TODO: Ici, coder l'initialisation
                //
                loadStatus = EnvContext.loadConfigFile(Cmd.subs[1]);

                // Chargement du dictionnaire des unités
                UnitLib = unitsFile.getUnitsFromXML(EnvContext.unitsFileName);
                

                switch (loadStatus)
                {
                    case FileIO.FILEOP_SUCCESSFUL:
                        Cmd.setResultText(AeroCalcCommand.RESULT_INIT_SUCCESSFULL);
                        Cmd.setCommentText("");
                        break;

                    case FileIO.FILEOP_INVALID_PATH:
                        Cmd.setResultText(AeroCalcCommand.RESULT_ERROR_INIT_ERROR_CONFIGFILE_PATH);
                        Cmd.setCommentText(AeroCalcCommand.COMMENT_ERROR_INIT_CONFIGFILE_PATH);
                        break;

                    case FileIO.FILEOP_IO_ERROR:
                        Cmd.setResultText(AeroCalcCommand.RESULT_ERROR_INIT_IO_ERROR);
                        Cmd.setCommentText(AeroCalcCommand.COMMENT_ERROR_INIT_IO_ERROR);
                        break;

                    case FileIO.FILEOP_UNKNOWN_ERROR:
                        Cmd.setResultText(AeroCalcCommand.RESULT_ERROR_INIT_UKN_FILE_ERROR);
                        Cmd.setCommentText(AeroCalcCommand.COMMENT_ERROR_INIT_UKN_FILE_ERROR);
                        break;

                    default:
                        Cmd.setResultText(AeroCalcCommand.RESULT_ERROR_INIT_UKN_FILE_ERROR);
                        Cmd.setCommentText(AeroCalcCommand.COMMENT_ERROR_INIT_UKN_FILE_ERROR);
                        break;
                }

                // TODO remove ater use
                Console.WriteLine(EnvContext.ToString());
                
                // Fin du processus d'initialisation
                initialized = true;
                return true;
            }
            return false;
            // Already initialized
        }



        /// <summary>
        /// Set Verbose mode
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <param name="verboseModeRequested">Verbose mode status requested by user</param>
        /// <returns>
        /// True, when mode is set according user's request
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



        /// <summary>
        /// Retourne un objet commandFactor sur la base d'une saisie texte par l'utilisateur
        /// </summary>
        /// <param name="subString">Chaine à analyser comme facteur d'un calcul multi-dimensionnel</param>
        /// <returns>objet commandFactor contenant les éléments du facteur</returns>
        /// <remarks>
        /// : OLD doit être remplacé par un constructeur d'objet commandFactor
        /// </remarks>
        /// 
        private CommandFactor getFactor(string subString) {

            string name = "";
            int unitDictionaryIndex = 0;
            double val = double.NaN;
            // TODO les séparateurs ne doit pas être locaux
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
                    unitDictionaryIndex = UnitLib.getIndexByAlias(words[2]);
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
            msg += "LOAD MODEL   : Load flight performance models from current data directory\n";
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
            msg += "*            AeroCalc CL                *\n";
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