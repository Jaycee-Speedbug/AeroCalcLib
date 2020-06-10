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

        public MemoryStack MemStack { get; private set; }



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
            // Construction de la pile mémoire
            MemStack = new MemoryStack();
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
        /// Seules quelques commandes sont traitées ici, l'essentiel est fait via le constructeur AeroCalcCommand
        /// </remarks>
        public AeroCalcCommand process(string txtCommand)
        {
            AeroCalcCommand Cmd = new AeroCalcCommand(txtCommand, ModelLib, EnvContext, MemStack);

            // Certaines commandes rendent la main pour être traitées ici, dans le processeur
            switch (Cmd.action)
            {
                case AeroCalcCommand.ACTION_INIT_INTERPRETER:
                    // Initialisation
                    initProcessor(Cmd);
                    break;

                case AeroCalcCommand.ACTION_EXIT:
                    // Exit
                    Cmd.setEventCode(AeroCalcCommand.EVENTCODE_EXIT_REQUESTED);
                    break;

                case AeroCalcCommand.ACTION_SCRIPTFILE:
                    // Lecture de fichier de script
                    readScriptFile(Cmd);
                    break;

                case AeroCalcCommand.ACTION_HELP:
                    // Aide
                    Cmd.setEventCode(AeroCalcCommand.EVENTCODE_HELP_REQUESTED);
                    break;
            }
            // Post process: Génération de tous les messages vers l'utilisateur
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

            if (Cmd.subs.Length >= 2)
            {
                // Constitution du path
                ScriptConnect.setWorkDirectory(EnvContext.scriptsDirPath);
                ScriptConnect.setInputFileWithRelPath(Cmd.subs[1]);
                ScriptConnect.readFile();
                switch (ScriptConnect.IOStatus)
                {
                    case FileIO.FILEOP_SUCCESSFUL:
                        // Le fichier de script a été lu avec succès

                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_SCRIPTFILE_SUCCESSFULL);
                        break;

                    case FileIO.FILEOP_FILE_DOES_NOT_EXIST:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_ERROR_SCRIPTFILE_DOES_NOT_EXIST);
                        break;

                    case FileIO.FILEOP_INVALID_PATH:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_ERROR_SCRIPT_PATH);
                        break;

                    case FileIO.FILEOP_IO_ERROR:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_ERROR_SCRIPT_IO_ERROR);
                        break;

                    case FileIO.FILEOP_UNKNOWN_ERROR:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_ERROR_SCRIPT_UKN_ERROR);
                        break;

                    case FileIO.FILEOP_INPUT_FILE_IS_LOCKED:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_ERROR_SCRIPT_SECURITY);
                        break;

                    default:
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_ERROR_SCRIPT_GENERIC);
                        break;
                }
            }

            // COMMANDE NON SUPPORTEE
            //Cmd.setEventCode(AeroCalcCommand.EVENTCODE_UNSUPPORTED_COMMAND);
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
                        // Chargement du dictionnaire des unités
                        if (EnvContext.unitsEnabled)
                        {
                            UnitLib = unitsFile.getUnitsFromXML(EnvContext.unitsFileName);
                            ModelLib.setUnitsLibrary(UnitLib);
                        }
                        ModelLib.setDataModelsDirectory(EnvContext.modelsDirPath);
                        // TODO Traiter le false de setDataModelsDirectory
                        // Construction de PostProcessor et de la librairie des messages
                        PostProc = new PostProcessor(EnvContext);
                        Cmd.setEventCode(AeroCalcCommand.EVENTCODE_INIT_SUCCESSFULL);
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

    }

}