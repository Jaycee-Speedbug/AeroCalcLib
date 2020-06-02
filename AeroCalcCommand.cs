using System;
using System.Collections.Generic;
using System.Linq;




namespace AeroCalcCore
{




    /// <summary>
    /// Classe définissant l'objet commande de travail du processeur AeroCalc
    /// Structure utilisée publiquement pour accéder aux différents membres intervenants
    /// dans le traitement de cette commande
    ///
    /// Il ne doit pas y avoir de manipulation de messages utilisateurs ici, ces
    /// post traitements sont à réaliser par le processeur
    /// 
    /// </summary>
    public class AeroCalcCommand
    {

        /*
         * CONSTANTES
         */

        /// <summary>
        /// Caractères de séparation du langage de script
        /// </summary>
        public const char CMD_SEPARATOR = ' ';
        public const char CMD_SEPARATOR_2 = '²';
        public const char CMD_SEPARATOR_3 = '_';
        public const char CMD_WORD_WHITE_CARD = '*';
        public const char CMD_CHAR_WHITE_CARD = '?';

        /// <summary>
        /// Opérateurs du langage de script
        /// </summary>
        public const char CMD_OPERATOR_SPLITTER = '.';
        public const char CMD_OPERATOR_AFFECT = '=';
        public const char CMD_OPERATOR_UNIT = ':';
        public const char CMD_OPERATOR_COMMENT = '#';

        /// <summary>
        /// Mots codes du langage de script
        /// </summary>
        public const string CMD_WORD_CALCULATE = "CALCULATE";
        public const string CMD_WORD_CATALOG = "CATALOG";
        public const string CMD_WORD_CONVERT = "CONVERT";
        public const string CMD_WORD_EXIT = "EXIT";
        public const string CMD_WORD_HELP = "HELP";
        public const string CMD_WORD_INIT_INTERPRETER = "INIT";
        public const string CMD_WORD_LIST = "LIST";
        public const string CMD_WORD_LOAD = "LOAD";
        public const string CMD_WORD_MODEL = "MODEL";
        public const string CMD_WORD_PRINT = "PRINT";
        public const string CMD_WORD_SCRIPTFILE = "SCRIPTFILE";
        public const string CMD_WORD_STOP = "STOP";
        public const string CMD_WORD_UNIT = "UNIT";
        public const string CMD_WORD_VERBOSE = "VERBOSE";


        /// <summary>
        /// Constantes d'identification de l'action à mener
        /// -> action
        /// </summary>
        public const int ACTION_INIT_VALUE = 0;
        public const int ACTION_UNDETERMINED = -1;
        public const int ACTION_INIT_INTERPRETER = 2;
        public const int ACTION_FILE_OPERATION = 10;
        public const int ACTION_SCRIPTFILE = 11;
        public const int ACTION_PRINT = 20;
        public const int ACTION_CONVERT = 30;
        public const int ACTION_CALCULATE = 40;
        public const int ACTION_LOAD_MODELS = 50;
        public const int ACTION_LOAD_UNITS = 51;
        public const int ACTION_CATALOG = 52;
        public const int ACTION_LIST_MODELS = 53;
        public const int ACTION_LIST_UNITS = 54;
        public const int ACTION_EXIT = 60;
        public const int ACTION_HELP = 70;
        public const int ACTION_VERBOSE = 90;
        public const int ACTION_STOP_VERBOSE = 91;



        /// <summary>
        /// Constantes d'identification de l'évènement final rencontré dans le traitement de la commande
        /// -> eventCode
        /// Un eventCode positif correspond à un traitement conforme à la définition de la commande
        /// Un eventCode négatif signale un événement non conforme à la définition de la commande
        /// 
        /// Message texte retour vers l'utilisateur
        /// -> txtResult
        /// 
        /// N'existe qu'en cas de présence d'une erreur
        /// </summary>
        ///
        public const int EVENTCODE_VERBOSE_ACTIVE = 910;
        public const int EVENTCODE_VERBOSE_INACTIVE = 911;
        public const int EVENTCODE_HELP_REQUESTED = 900;
        public const int EVENTCODE_LOAD_MODELS_SUCCESSFULL = 500;
        public const int EVENTCODE_CALCULATE_SUCCESSFULL = 200;
        public const int EVENTCODE_PROCESS_SUCCESSFULL = 100;
        public const int EVENTCODE_INIT_SUCCESSFULL = 110;
        public const int EVENTCODE_CMD_HANDOVER = 20;
        public const int EVENTCODE_EXIT_REQUESTED = 10;

        public const int EVENTCODE_INITIAL = 0; // Valeur d'initialisation

        public const int EVENTCODE_INIT_UNSUCCESSFULL = -1;
        public const int EVENTCODE_REINIT_NOT_ALLOWED = -2;
        public const int EVENTCODE_ERROR_INIT_CONFIGFILE_PATH = -3;
        public const int EVENTCODE_ERROR_INIT_IO_ERROR = -4;
        public const int EVENTCODE_ERROR_INIT_UKN_FILE_ERROR = -5;
        public const int EVENTCODE_ERROR_INIT_UKN_ERROR = -6;
        public const int EVENTCODE_ERROR_INIT_UNITS_FILE = -7;
        public const int EVENTCODE_ERROR_INIT_LANGUAGE_FILE = -8;
        public const int EVENTCODE_COMMAND_UNPROCESSED = -10;
        public const string RESULT_ERROR_COMMAND_UNPROCESSED = "La commande n'a pas pu être traitée";

        public const int EVENTCODE_COMMAND_VOID = -21;
        public const string RESULT_ERROR_COMMAND_VOID = "La commande traitée est vide";

        public const int EVENTCODE_UNKNOWN_COMMAND_WORD = -22;
        public const int EVENTCODE_UNSUPPORTED_COMMAND = -23;
        public const int EVENTCODE_NO_UNIT_DATA_AVAILABLE=-30;
        public const int EVENTCODE_UNABLE_VERBOSE_MODIFICATION = -50;

        public const int EVENTCODE_PROCESSOR_FAILURE = -100;
        public const int EVENTCODE_CALC_PROCESS_ERROR = -110;
        public const int EVENTCODE_CALC_PROCESSOR_MISSING_FACTOR = -111;
        public const int EVENTCODE_CALC_PROCESSOR_MISSING_MODEL = -112;
        public const int EVENTCODE_NO_MODEL_LOADED = -500;
        public const string RESULT_ERROR_NO_MODEL_LOADED = "Aucun modèle chargé en mémoire";
        public const string COMMENT_ERROR_NO_MODEL_LOADED = "Le(s) modèle(s) demandé(s) n'ont pas été trouvé dans le répertoire des modèles";


        /*
         * PROPRIETES
         */

        /// <summary>
        /// Action requise identifiée dans la commande texte
        /// </summary>
        public int action { get; private set; }

        /// <summary>
        /// Durée du traitement, au moment de l'appel à la propriété
        /// </summary>
        public long durationMilliSecond
        {
            get
            {
                return (DateTimeOffset.Now.Ticks - startOfProcess.Ticks) / TimeSpan.TicksPerMillisecond;
            }
        }

        /// <summary>
        /// Nom du dossier de travail, si un tel dossier est nécessaire
        /// </summary>
        public string directory { get; private set; }

        /// <summary>
        /// Nom du fichier source, si un tel fichier est utilisé
        /// </summary>
        public string inputFileName { get; private set; }

        /// <summary>
        /// Nom du fichier de sortie, si un tel fichier est créé
        /// </summary>
        public string outputFileName { get; private set; }

        /// <summary>
        /// Table des mots de la commande au format texte
        /// </summary>
        public string[] subs { get; private set; }

        /// <summary>
        /// Liste des facteurs de la commande
        /// </summary>
        public List<CommandFactor> Factors { get; private set; }

        /// <summary>
        /// 
        /// Commande telle que reçue, au fomat texte
        /// </summary>
        public string rawTxtCommand { get; private set; }

        /// <summary>
        /// Résultat numérique de la commande au format double
        /// </summary>
        public double numericResult { get; private set; }

        /// <summary>
        /// Résultat de la commande, en version texte (pour affichage par interpéteur)
        /// </summary>
        public string txtResult { get; private set; }

        /// <summary>
        /// Commentaire texte sur le traitement de la commande
        /// </summary>
        public string txtComment { get; private set; }

        /// <summary>
        /// Code de l'évènement rencontré pendant le traitement de la commande
        /// </summary>
        public int eventCode { get; private set; }

        /// <summary>
        /// Tableau des caractères utilisés comme séparateur de mots dans une commande en mode texte
        /// </summary>
        private char[] commandSeparators = { CMD_SEPARATOR, CMD_SEPARATOR_2, CMD_SEPARATOR_3 };

        /*
         * MEMBRES
         */

        /// <summary>
        /// Heure du début de traitement de la commande
        /// </summary>
        private DateTime startOfProcess;

        /// <summary>
        /// Container des modèles de calculs
        /// </summary>
        private DataModelContainer Container;

        private EnvironmentContext EnvContext;


        /*
         * CONSTRUCTEURS
         */

        /// <summary>
        /// Constructeur d'objet Commande
        /// 
        /// </summary>
        /// <param name="txtCommand">string contenant la commande en mode texte, sans traitement préalable.
        /// </param>
        /// 
        public AeroCalcCommand(string txtCommand, DataModelContainer DMContainer, EnvironmentContext EC)
        {
            //
            // Initialisation des propriétés
            //
            startOfProcess = new DateTime(DateTime.Now.Ticks, DateTimeKind.Utc);
            action = ACTION_INIT_VALUE;
            txtComment = "";
            directory = "";
            inputFileName = "";
            outputFileName = "";
            eventCode = EVENTCODE_INITIAL;
            numericResult = Double.NaN;
            subs = null;
            Container = DMContainer;
            Factors = new List<CommandFactor>();
            EnvContext = EC;

            if (string.IsNullOrEmpty(txtCommand))
            {
                // Cas particulier de la chaine nulle
                rawTxtCommand = "";
                action = ACTION_UNDETERMINED;
                eventCode = EVENTCODE_COMMAND_VOID;
            }
            else
            {
                // La chaine de texte n'est pas vide, traitement de la commande
                rawTxtCommand = txtCommand;
                if (!execute())
                {
                    // Un problème majeur s'est produit dans le traitement de la commande
                    eventCode = EVENTCODE_PROCESSOR_FAILURE;
                    verboseMe();
                }
                else
                {
                    // Commande traitée
                    if (EC.verbose)
                    {
                        verboseMe();
                    }
                }
            }

        }



        /*
         * SERVICES
         */

        /// <summary>
        /// Retourne True si la valeur du facteur dont le nom a été passé en argument a bien été
        /// affecté à l'argument value.
        /// </summary>
        /// <param name="factorName">Nom du facteur</param>
        /// <param name="value">Valeur du facteur</param>
        /// <returns></returns>
        /// 
        // TODO, Pas normal de renvoyer true is le factorName est invalide !!! 
        public bool factor(string factorName, out double value)
        {
            if (string.IsNullOrEmpty(factorName))
            {
                value = AeroCalc.MODEL_DIMENSION_DEFAULT_VALUE;
                return true;
            }
            int index = this.Factors.FindIndex(x => x.name == factorName);
            if (index < 0)
            {
                // Pas de facteur trouvé
                value = double.NaN;
                return false;
            }
            value = Factors.ElementAt(index).value;
            return true;
        }



        /// <summary>
        /// Enregistre le code de l'événement passé en argument
        /// </summary>
        /// <param name="eventCode"></param>
        /// 
        public void setEventCode(int eventCode)
        {
            this.eventCode = eventCode;
        }



        /// <summary>
        /// Enregistre le résultat numérique du calcul réalisé
        /// </summary>
        /// <param name="result">
        /// Double, valeur numérique résultat du calcul demandé</param>
        public void setNumericResult(double result)
        {
            this.numericResult = result;
        }



        /// <summary>
        /// Enregistre le texte passé en argument comme résultat au format texte
        /// </summary>
        /// <param name="txtResult">
        /// Résultat au format texte qui sera communiqué à un utilisateur utilisant la console texte
        /// </param>
        public void setResultText(string txtResult)
        {
            this.txtResult = txtResult;
        }



        /// <summary>
        /// Enregistre le texte passé en argument comme commentaire au format texte
        /// </summary>
        /// <param name="comment">
        /// Texte de commentaire sur la  commande qui sera communiqué à un utilisateur utilisant la console texte
        /// </param>
        public void setCommentText(string comment)
        {
            this.txtComment = comment;
        }



        /// <summary>
        /// Analyse la requête texte pour définir l'action à réaliser, puis lance l'action
        /// </summary>
        /// <returns>True si l'action a pu être déterminée et menée</returns>
        /// <remarks>Créée spécialement pour respecter un principe d'architecture POO, le constructeur ne doit pas
        /// comporter de traitement métier.
        /// 
        /// TODO: Traitement de la chaine de caractère pour localisation.
        /// </remarks>
        private bool execute()
        {

            StringComparison StrCompOpt = StringComparison.CurrentCultureIgnoreCase;
            action = ACTION_UNDETERMINED;
            subs = rawTxtCommand.Split(commandSeparators, StringSplitOptions.RemoveEmptyEntries);

            // Commandes à mot unique
            if (subs.Length == 1)
            {

                if (subs[0].Equals(CMD_WORD_EXIT, StrCompOpt))
                {
                    action = ACTION_EXIT;
                    cmd_EXIT();
                }

                if (subs[0].Equals(CMD_WORD_CONVERT, StrCompOpt))
                {
                    action = ACTION_CONVERT;
                    cmd_CONVERT();
                }

                if (subs[0].Equals(CMD_WORD_VERBOSE, StrCompOpt))
                {
                    action = ACTION_VERBOSE;
                    eventCode = EVENTCODE_CMD_HANDOVER;
                    //cmd_VERBOSE(true);
                }

                if (subs[0].Equals(CMD_WORD_CATALOG, StrCompOpt))
                {
                    action = ACTION_CATALOG;
                    cmd_LOAD_CATALOG();
                }

                if (subs[0].Equals(CMD_WORD_LIST, StrCompOpt))
                {
                    action = ACTION_LIST_MODELS;
                    // TODO: S'arranger pour que la commande à mot unique LIST soit équivalente à LIST *
                    // Traitement des ALIAS potentiels
                    //useAlias();
                    cmd_LIST_MODELS();
                }

                if (subs[0].Equals(CMD_WORD_LOAD, StrCompOpt))
                {
                    action = ACTION_LOAD_MODELS;
                    // TODO: S'arranger pour que la commande à mot unique LOAD soit équivalente à LOAD *
                    // Traitement des ALIAS potentiels
                    //useAlias();
                    cmd_LOAD_MODELS();
                }

                if (subs[0].Equals(CMD_WORD_HELP, StrCompOpt))
                {
                    // Action laissée au processeur
                    action = ACTION_HELP;
                    cmd_HELP();
                }

            }

            // Commandes à mots multiples
            if (subs.Length >= 2)
            {

                if (subs[0].Equals(CMD_WORD_INIT_INTERPRETER, StrCompOpt))
                {
                    // Action laissée au processeur
                    action = ACTION_INIT_INTERPRETER;
                    cmd_INIT();
                }

                if (subs[0].Equals(CMD_WORD_LOAD, StrCompOpt))
                {
                    if (subs[1].Equals(CMD_WORD_MODEL, StrCompOpt))
                    {
                        action = ACTION_LOAD_MODELS;
                        cmd_LOAD_MODELS();
                    }
                    else if (subs[1].Equals(CMD_WORD_UNIT, StrCompOpt))
                    {
                        action = ACTION_LOAD_UNITS;
                        // TODO: Define cmd_LOAD_UNITS()
                    }
                }

                if (subs[0].Equals(CMD_WORD_LIST, StrCompOpt))
                {
                    if (subs[1].Equals(CMD_WORD_MODEL, StrCompOpt))
                    {
                        action = ACTION_LIST_MODELS;
                        cmd_LIST_MODELS();
                    }
                    else if (subs[1].Equals(CMD_WORD_UNIT, StrCompOpt))
                    {
                        action = ACTION_LIST_UNITS;
                        cmd_LIST_UNITS();
                    }
                }

                if (subs[0].Equals(CMD_WORD_STOP, StrCompOpt) &&
                    subs[1].Equals(CMD_WORD_VERBOSE, StrCompOpt))
                {
                    // Action laissée au processeur
                    action = ACTION_STOP_VERBOSE;
                    //cmd_VERBOSE(false);
                }

                if (subs[0].Equals(CMD_WORD_SCRIPTFILE, StrCompOpt))
                {
                    // Chargement et exécution d'un fichier de script, on repasse la main au processeur
                    // Action laissée au processeur
                    action = ACTION_SCRIPTFILE;
                    // TODO: Define cmd_SCRIPTFILE();
                }

                // En dernier ressort, on considère une commande de calcul
                if (action == ACTION_UNDETERMINED)
                {
                    // Commande à mots multiples et non reconnues précédement
                    action = ACTION_CALCULATE;
                    eventCode = EVENTCODE_COMMAND_UNPROCESSED;
                    cmd_CALCULATE();
                }

            }
            return true;
        }



        /// <summary>
        /// Traitement d'une commande de calcul de performance.
        /// Retourne True si le traitement de la commande est complet.
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// NEW PROCESSOR STRUCTURE
        ///</remarks>
        /// 
        private bool cmd_CALCULATE()
        {

            // Initialisation
            double numResult = double.NaN;
            // constitution de la Liste des facteurs communiqués dans la commande
            List<CommandFactor> factorList = new List<CommandFactor>();
            for (int count = 1; count < this.subs.Length; count++)
            {
                factorList.Add(getFactor(subs[count]));
            }

            try
            {
                numResult = Container.compute(subs[0], factorList);
            }
            catch (ModelException e)
            {
                // La commande a échouée pendant le calcul 
                setEventCode(EVENTCODE_CALC_PROCESS_ERROR);
                // TODO, revoir le formatage du message d'erreur, en utilisant aussi la nature de l'exception
                //setCommentText("Erreur lors du calcul de " + e.modelName + " : " + e.factor);
            }

            if (!double.IsNaN(numResult))
            {
                // Réussite du calcul
                setEventCode(EVENTCODE_CALCULATE_SUCCESSFULL);
                setNumericResult(numResult);
            }
            return true;
        }



        private bool cmd_CONVERT()
        {
            // COMMANDE NON SUPPORTEE
            numericResult = Double.NaN;
            eventCode = EVENTCODE_UNSUPPORTED_COMMAND;
            return true;
        }



        private bool cmd_EXIT()
        {
            // Commande traitée par le processeur
            // TODO eventCode à générer ds le processeur
            eventCode = EVENTCODE_EXIT_REQUESTED;
            return true;
        }



        private bool cmd_HELP()
        {
            // Commande traitée par le processeur
            numericResult = double.NaN;
            // TODO eventCode à générer ds le processeur
            eventCode = EVENTCODE_HELP_REQUESTED;
            return true;
        }



        private bool cmd_INIT()
        {
            // Commande traitée par le processeur
            numericResult = double.NaN;
            return true;
        }



        /// <summary>
        /// Traitement de la commande de création d'une liste des modèles de performance
        /// disponibles dans le container de performances
        /// </summary>
        /// <returns>Bool résultat du traitement</returns>
        /// <remarks></remarks>
        /// TODO Objectif non atteint, on doit pouvoir lister les modèles avec filtrage
        private bool cmd_LIST_MODELS()
        {
            // Recherche des noms de modèles de performance qui match
            setResultText(Container.dataModelSignatures());
            eventCode = AeroCalcCommand.EVENTCODE_PROCESS_SUCCESSFULL;
            numericResult = Double.NaN;
            return true;
        }



        /// <summary>
        /// Traitement de la commande de création d'une liste des unités de mesures
        /// disponibles dans le container de performances
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>True si le traitement n'a pas généré d'erreur</returns>
        /// <remarks></remarks>
        /// TODO: Objectif non atteint, on doit pouvoir lister les unités avec filtrage
        private bool cmd_LIST_UNITS()
        {
            // Liste des unités enregitrées
            string msg = "";
            //! BUG A corriger ici ! Object Reference null
            List<Unit> lu = Container.dataUnits.getUnits();
            if (lu != null)
            {
                foreach (Unit item in lu)
                {
                    msg += item.ToString() + "\n";
                    setResultText(msg);
                    eventCode = EVENTCODE_PROCESS_SUCCESSFULL;
                }
            }
            else
            {
                // No units available!
                eventCode = EVENTCODE_NO_UNIT_DATA_AVAILABLE;
            }
            return true;
        }



        /// <summary>
        /// Traite la commande de chargement du catalogue
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns></returns>
        private bool cmd_LOAD_CATALOG()
        {
            // COMMANDE NON SUPPORTEE
            eventCode = EVENTCODE_UNSUPPORTED_COMMAND;
            numericResult = Double.NaN;
            return true;
        }



        /// <summary>
        /// Traite la commande de chargement d'un ou plusieurs modèles de données
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>Code du traitement de l'opération</returns>
        /// <remarks>TO BE DEVELOPPED</remarks>
        /// 
        private bool cmd_LOAD_MODELS()
        {

            int counter = 0;

            foreach (String filter in subs)
            {
                counter += Container.loadDataModels(filter);
            }
            if (counter > 0)
            {
                eventCode = EVENTCODE_LOAD_MODELS_SUCCESSFULL;
                // TODO Ce résultat doit être maintenu à travers le post process
                txtResult = String.Format("{0} modèle(s) ont été chargés en mémoire.", counter);
                numericResult = Double.NaN;
            }
            else
            {
                eventCode = EVENTCODE_NO_MODEL_LOADED;
                numericResult = Double.NaN;
            }
            return true;
        }



        /// <summary>
        /// Retourne un objet commandFactor sur la base d'une saisie texte par l'utilisateur
        /// </summary>
        /// <param name="subString">Chaine à analyser comme facteur d'un calcul multi-dimensionnel</param>
        /// <returns>objet commandFactor contenant les éléments du facteur</returns>
        /// <remarks>
        /// DEBUG: OLD doit être remplacé par un constructeur d'objet commandFactor
        /// </remarks>
        /// 
        private CommandFactor getFactor(string subString)
        {

            string name = "";
            int unitDictionaryIndex = 0;
            double val = double.NaN;
            // TODO les séparateurs ne doit pas être locaux
            char[] separators = { CMD_OPERATOR_AFFECT, CMD_OPERATOR_UNIT };
            string[] words = subString.Split(separators);

            if (words.Length == 2)
            {
                // Facteur sans unité
                if (double.TryParse(words[1], out val))
                {
                    name = words[0];
                    unitDictionaryIndex = AeroCalc.UNIT_UNDETERMINED;
                }
            }
            else if (words.Length == 3)
            {
                // Facteur avec unité
                if (double.TryParse(words[1], out val))
                {
                    unitDictionaryIndex = Container.dataUnits.getIndexByAlias(words[2]);
                    if (unitDictionaryIndex != AeroCalc.UNIT_UNDETERMINED)
                    {
                        name = words[0];
                    }
                }
            }
            return new CommandFactor(name, val, unitDictionaryIndex);
        }



        /// <summary>
        /// Remplace les ALIAS des commandes par les commandes étendues
        /// Seul le remplacement pur et simple est autorisé
        /// La commande texte à l'état brut n'est pas modifiée, mais le tableau de séquençage
        /// </summary>
        /// <remarks>
        /// A remplacer par un traitement plus automatisé, en utilisant une List<> ou 
        /// </remarks>
        /// 
        private void useAlias()
        {

            bool aliasUsed = false;
            string aliasCmd = "";

            if (subs.Length == 1 && this.action == ACTION_LIST_MODELS)
            {
                // LIST  ->  LIST *
                aliasCmd = "LIST *";
                aliasUsed = true;
            }
            if (subs.Length == 1 && this.action == ACTION_LOAD_MODELS)
            {
                // LOAD  ->  LOAD *
                aliasCmd = "LOAD *";
                aliasUsed = true;
            }
            // Si remplacement, nouveau séquençage en mots
            if (aliasUsed)
            {
                subs = aliasCmd.Split(commandSeparators, StringSplitOptions.RemoveEmptyEntries);
            }
        }



        /// <summary>
        /// Transforme le comment de la commande ainsi que txtResult en version étendue, 
        /// comprenant tous les champs de l'objet commande.
        /// </summary>
        /// <returns>Etat de l'opération</returns>
        /// <remarks>
        /// Doit être appelée en tout dernier, afin que les champs de l'objet soient prêt à être utilisés
        /// </remarks>
        // TODO Doit disparaitre à terme
        private bool verboseMe()
        {

            string msg = "";

            msg += "\n---------------- VERBOSE ----------------\n";
            msg += "\nRaw command  : " + rawTxtCommand;
            msg += "\nAction code  : " + action;
            msg += "\nDirectory    : " + directory;
            msg += "\nFile name    : " + inputFileName;
            msg += "\nOutput name  : " + outputFileName;
            msg += "\nEvent code   : " + eventCode;
            msg += "\nNum result   : " + numericResult;
            msg += "\nTxt result   : " + txtResult;
            msg += "\nComment      : " + txtComment;
            msg += "\nDuration     : " + durationMilliSecond + " ms\n\n";
            txtResult = msg;
            txtComment = txtResult;
            return true;
        }

    }

}