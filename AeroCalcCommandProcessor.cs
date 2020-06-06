using System;



namespace AeroCalcCore
{


    /// <summary>
    /// Classe définissant le processeur de commande AeroCalcCommand
    /// Le processeur travaille sur la base d'une commande pré-traitée par un préprocesseur
    /// et n'accepte donc pas de commande en ligne de commande au format texte.
    /// 
    /// Le processeur est principalement en charge des traitements numériques.
    /// 
    /// </summary>
    public class AeroCalcCommandProcessor
    {

        /*
         * CONSTANTES
         */



        /*
         * PROPRIETES
         */
        // ! Certaines propriétés devraient être reclassées private
        public bool initialized { get; private set; }

        public EnvironmentContext EnvContext { get; private set; }

        public ScriptFile ScriptConnect { get; private set; }

        public DataModelContainer ModelLib { get; private set; }

        public Units UnitLib { get; private set; }



        /*
         * MEMBRES
         */
        private PostProcessor PostProc;



        /*
         * CONSTRUCTEURS
         */

        /// <summary>
        /// Construit un objet de traitement des commandes
        /// </summary>
        /// 
        public AeroCalcCommandProcessor()
        {
            // Construction du container de données de performances
            ModelLib = new DataModelContainer();
            // Construction de l'objet de connexion aux fichiers de Script
            ScriptConnect = new ScriptFile();
            // Construction de l'objet d'environnement
            EnvContext = new EnvironmentContext();
            // Reglage intial du flag initialized
            initialized = false;
        }



        /*
         * SERVICES
         */

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
        // TODO Un constructeur ne devrait pas réaliser de traitements: A transplanter dans une fonction
        /// </remarks>
        /// 
        public AeroCalcCommand process(string txtCommand)
        {
            // Construction d'un objet AeroCalcCommand et exécution de la commande simple
            // Une commande complexe (nécessitant l'exécution de plus d'une commande simple) doit
            // d'abord être décomposée

            AeroCalcCommand Cmd = new AeroCalcCommand(txtCommand, ModelLib, EnvContext);
            // Certaines commandes rendent la main pour être traitées ici, dans le processeur
            switch (Cmd.action)
            {
                case AeroCalcCommand.ACTION_INIT_INTERPRETER:
                    // Initialisation
                    initProcessor(Cmd);
                    break;

                case AeroCalcCommand.ACTION_SCRIPTFILE:
                    readScriptFile(Cmd);
                    break;

                case AeroCalcCommand.ACTION_HELP:
                    Cmd.setEventCode(AeroCalcCommand.EVENTCODE_HELP_REQUESTED);
                    break;
            }
            // Post process
            PostProc.postProcess(Cmd);
            return Cmd;
        }



        /*
         * METHODES
         */

        /// <summary>
        /// Traitement de la commande de lecture et d'exécution d'un fichier de script
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>
        /// True, si la commande a été traitée sans erreur
        /// </returns>
        private bool readScriptFile(AeroCalcCommand Cmd)
        {
            // COMMANDE NON SUPPORTEE
            Cmd.setEventCode(AeroCalcCommand.EVENTCODE_UNSUPPORTED_COMMAND);
            return true;
        }



        /// <summary>
        /// Traite la commande d'initialisation de l'interpreteur
        /// Première (et unique) commande à pouvoir être lançée
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>Etat de réussite de la commande</returns>
        private bool initProcessor(AeroCalcCommand Cmd)
        {
            int loadStatus;
            UnitsXMLFile unitsFile = new UnitsXMLFile("");

            if (!initialized)
            {
                //
                // TODO: Ici, coder l'initialisation
                // eventCode à inscrire ici
                //
                loadStatus = EnvContext.loadConfigFile(Cmd.subs[1]);
                switch (loadStatus)
                {
                    case FileIO.FILEOP_SUCCESSFUL:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_INIT_SUCCESSFULL);
                        // Chargement du dictionnaire des unités
                        if (EnvContext.unitsEnabled)
                        {
                            UnitLib = unitsFile.getUnitsFromXML(EnvContext.unitsFileName);
                            ModelLib.setUnitsLibrary(UnitLib);
                            // TODO Remove
                            Console.WriteLine("Units loaded : " + UnitLib.units.Count);
                        }
                        // Chargement de la librairie des messages
                        PostProc = new PostProcessor(EnvContext);
                        break;

                    case FileIO.FILEOP_INVALID_PATH:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_ERROR_INIT_CONFIGFILE_PATH);
                        Cmd.setExit();
                        break;

                    case FileIO.FILEOP_IO_ERROR:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_ERROR_INIT_IO_ERROR);
                        Cmd.setExit();
                        break;

                    case FileIO.FILEOP_UNKNOWN_ERROR:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_ERROR_INIT_UKN_FILE_ERROR);
                        Cmd.setExit();
                        break;

                    default:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_ERROR_INIT_UKN_ERROR);
                        Cmd.setExit();
                        break;
                }
                // TODO remove after use
                // Console.WriteLine(EnvContext.ToString());
                // Fin du processus d'initialisation
                initialized = true;
                return true;
            }
            else
            {
                // Already initialized
                Cmd.setEventCode(AeroCalcCommand.EVENTCODE_REINIT_NOT_ALLOWED);
            }
            return false;
        }



        /// <summary>
        /// Set Verbose mode
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <param name="verboseModeRequested">Verbose mode status requested by user</param>
        /// <returns>
        /// True, when mode is set according user's request
        /// </returns>
        private bool setVerboseMode(AeroCalcCommand Cmd, bool verboseModeRequested)
        {

            Cmd.setNumericResult(double.NaN);

            if (!EnvContext.verboseAllowed)
            {
                Cmd.setEventCode(AeroCalcCommand.EVENTCODE_UNABLE_VERBOSE_MODIFICATION);
            }
            else
            {
                if (verboseModeRequested)
                {
                    // Commande VERBOSE
                    EnvContext.setVerbose(true);
                    Cmd.setEventCode(AeroCalcCommand.EVENTCODE_VERBOSE_ACTIVE);
                }
                else
                {
                    // Commande STOP VERBOSE
                    EnvContext.setVerbose(false);
                    Cmd.setEventCode(AeroCalcCommand.EVENTCODE_VERBOSE_INACTIVE);
                }
            }
            return true;
        }

    }

}