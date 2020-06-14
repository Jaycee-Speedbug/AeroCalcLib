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



        /*
         * MEMBRES
         */
        private PostProcessor PostProc;

        private EnvironmentContext EnvContext;

        private DataModelContainer ModelsLib;

        private Units UnitsLib;

        private MemoryStack MemStack;



        /*
         * CONSTRUCTEURS
         */

        /// <summary>
        /// Construit un objet de traitement des commandes
        /// </summary>
        /// 
        public AeroCalcCommandProcessor() {
            // Construction du container de données de performances
            ModelsLib = new DataModelContainer();
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
        public AeroCalcCommand process(string txtCommand) {
            AeroCalcCommand Cmd = new AeroCalcCommand(txtCommand, ModelsLib, EnvContext, MemStack);

            // Certaines commandes rendent la main pour être traitées ici, dans le processeur
            switch (Cmd.action) {
                case AeroCalcCommand.ACTION_INIT_INTERPRETER:
                    // Initialisation
                    initProcessor(Cmd);
                    break;

                case AeroCalcCommand.ACTION_EXIT:
                    // Exit
                    Cmd.setEventCode(AeroCalcCommand.ECODE_EXIT_REQUESTED);
                    break;

                case AeroCalcCommand.ACTION_SCRIPTFILE:
                    // Lecture de fichier de script
                    readScriptFile(Cmd);
                    break;

                case AeroCalcCommand.ACTION_HELP:
                    // Aide
                    Cmd.setEventCode(AeroCalcCommand.ECODE_HELP_REQUESTED);
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
        /// Les instructions exclusives d'AeroCalcCommandProcessor ne peuvent pas être executées
        /// SCRIPTFILE, HELP, EXIT, INIT
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>
        /// True, si la commande a été traitée sans erreur
        /// </returns>
        private bool readScriptFile(AeroCalcCommand Cmd) {
            if (Cmd.subs.Length >= 2) {

                ScriptFile SF = new ScriptFile();

                // Constitution du path
                SF.setWorkDirectory(EnvContext.scriptsDirPath);
                SF.setInputFileWithRelPath(Cmd.subs[1]);
                switch (SF.readFile()) {

                    case FileIO.FILEOP_SUCCESSFUL:
                        // Le fichier de script a été lu avec succès
                        if (SF.Count > 0) {
                            string outputLn = "";
                            for (int index = 0; index < SF.Count; index++) {
                                string ln = SF.readNextLine();
                                outputLn += ln + Environment.NewLine;
                                AeroCalcCommand ScriptCmd = process(ln);
                                outputLn += ScriptCmd.txtResult + Environment.NewLine;
                            }
                            Cmd.setResultText(outputLn);
                            Cmd.setEventCode(AeroCalcCommand.ECODE_SCRIPTFILE_SUCCESSFULL);
                        }
                        else {
                            Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_SCRIPTFILE_VOID);
                        }
                        break;

                    case FileIO.FILEOP_FILE_DOES_NOT_EXIST:
                        Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_SCRIPTFILE_DOES_NOT_EXIST);
                        break;

                    case FileIO.FILEOP_INVALID_PATH:
                        Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_SCRIPT_PATH);
                        break;

                    case FileIO.FILEOP_IO_ERROR:
                        Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_SCRIPT_IO_ERROR);
                        break;

                    case FileIO.FILEOP_UNKNOWN_ERROR:
                        Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_SCRIPT_UKN_ERROR);
                        break;

                    case FileIO.FILEOP_INPUT_FILE_IS_LOCKED:
                        Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_SCRIPT_SECURITY);
                        break;

                    default:
                        Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_SCRIPT_GENERIC);
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
        private bool initProcessor(AeroCalcCommand Cmd) {
            int loadStatus;
            UnitsXMLFile unitsFile = new UnitsXMLFile("");

            if (!initialized) {
                loadStatus = EnvContext.loadConfigFile(Cmd.subs[1]);
                switch (loadStatus) {
                    case FileIO.FILEOP_SUCCESSFUL:
                        // Chargement du dictionnaire des unités
                        if (EnvContext.unitsEnabled) {
                            UnitsLib = unitsFile.getUnitsFromXML(EnvContext.unitsFileName);
                            ModelsLib.setUnitsLibrary(UnitsLib);
                        }
                        ModelsLib.setDataModelsDirectory(EnvContext.modelsDirPath);
                        // TODO Traiter le false de setDataModelsDirectory
                        // Construction de PostProcessor et de la librairie des messages
                        PostProc = new PostProcessor(EnvContext);
                        Cmd.setEventCode(AeroCalcCommand.ECODE_INIT_SUCCESSFULL);
                        break;

                    case FileIO.FILEOP_INVALID_PATH:
                        Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_INIT_CONFIGFILE_PATH);
                        Cmd.setExit();
                        break;

                    case FileIO.FILEOP_IO_ERROR:
                        Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_INIT_IO_ERROR);
                        Cmd.setExit();
                        break;

                    case FileIO.FILEOP_UNKNOWN_ERROR:
                        Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_INIT_UKN_FILE_ERROR);
                        Cmd.setExit();
                        break;

                    default:
                        Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_INIT_UKN_ERROR);
                        Cmd.setExit();
                        break;
                }
                // Fin du processus d'initialisation
                initialized = true;
                return true;
            }
            else {
                // Already initialized
                Cmd.setEventCode(AeroCalcCommand.ECODE_ERR_REINIT_NOT_ALLOWED);
            }
            return false;
        }

    }

}