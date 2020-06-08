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
    /// post traitements sont à réaliser par PostProcessor
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
        public const int ACTION_INITIAL_VALUE = 0;
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
        public const int EVENTCODE_VERBOSE_ALREADY = 912;
        public const int EVENTCODE_HELP_REQUESTED = 900;
        public const int EVENTCODE_LIST_UNITS_SUCCESSFULL = 550;
        public const int EVENTCODE_LOAD_MODELS_SUCCESSFULL = 500;
        public const int EVENTCODE_CALCULATE_SUCCESSFULL = 200;
        public const int EVENTCODE_PROCESS_SUCCESSFULL = 100;
        public const int EVENTCODE_INIT_SUCCESSFULL = 110;
        public const int EVENTCODE_CMD_HANDOVER = 20;
        public const int EVENTCODE_EXIT_REQUESTED = 10;

        public const int EVENTCODE_INITIAL_VALUE = 0; // Valeur d'initialisation

        public const int EVENTCODE_INIT_UNSUCCESSFULL = -1;
        public const int EVENTCODE_REINIT_NOT_ALLOWED = -2;
        public const int EVENTCODE_ERROR_INIT_CONFIGFILE_PATH = -3;
        public const int EVENTCODE_ERROR_INIT_IO_ERROR = -4;
        public const int EVENTCODE_ERROR_INIT_UKN_FILE_ERROR = -5;
        public const int EVENTCODE_ERROR_INIT_UKN_ERROR = -6;
        public const int EVENTCODE_ERROR_INIT_UNITS_FILE = -7;
        public const int EVENTCODE_ERROR_INIT_LANGUAGE_FILE = -8;
        public const int EVENTCODE_COMMAND_UNPROCESSED = -10;

        public const int EVENTCODE_COMMAND_VOID = -21;

        public const int EVENTCODE_UNKNOWN_COMMAND_WORD = -22;
        public const int EVENTCODE_UNSUPPORTED_COMMAND = -23;
        public const int EVENTCODE_NO_UNIT_DATA_AVAILABLE = -30;
        public const int EVENTCODE_UNABLE_VERBOSE_MODIFICATION = -50;

        public const int EVENTCODE_PROCESSOR_FAILURE = -100;
        public const int EVENTCODE_CALC_PROCESS_ERROR = -110;
        public const int EVENTCODE_CALC_PROCESSOR_MISSING_FACTOR = -111;
        public const int EVENTCODE_CALC_PROCESSOR_MISSING_MODEL = -112;
        public const int EVENTCODE_NO_MODEL_LOADED = -500;


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
        public string workDirectory { get; private set; }

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
        /// Code de l'évènement rencontré pendant le traitement de la commande
        /// </summary>
        public int eventCode { get; private set; }

        /// <summary>
        /// Data texte, à disposition du PostProcessor pour préparer le message à renvoyer à l'utilisateur
        /// </summary>
        // TODO DRAFT of a report system
        public string[] info { get; private set; }

        public bool verbosed { get; private set; }



        /*
         * MEMBRES
         */

        /// <summary>
        /// Tableau des caractères utilisés comme séparateur de mots dans une commande en mode texte
        /// </summary>
        private char[] commandSeparators = { CMD_SEPARATOR, CMD_SEPARATOR_3 };

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
        /// <param name="inputText">string contenant la commande en mode texte, sans traitement préalable.
        /// </param>
        /// 
        public AeroCalcCommand(string inputText, DataModelContainer DMContainer, EnvironmentContext EC)
        {
            // Initialisation des propriétés
            startOfProcess = new DateTime(DateTime.Now.Ticks, DateTimeKind.Utc);
            action = ACTION_INITIAL_VALUE;
            workDirectory = "";
            inputFileName = "";
            outputFileName = "";
            eventCode = EVENTCODE_INITIAL_VALUE;
            numericResult = Double.NaN;
            subs = null;
            Container = DMContainer;
            Factors = new List<CommandFactor>();
            EnvContext = EC;
            if (EnvContext.verbose) verbosed = true;

            if (string.IsNullOrEmpty(inputText))
            {
                // Cas particulier de la chaine nulle
                rawTxtCommand = "";
                action = ACTION_UNDETERMINED;
                eventCode = EVENTCODE_COMMAND_VOID;
            }
            else
            {
                // La chaine de texte n'est pas vide, traitement de la commande
                rawTxtCommand = inputText;
                if (!execute())
                {
                    // Un problème majeur s'est produit dans le traitement de la commande
                    eventCode = EVENTCODE_PROCESSOR_FAILURE;
                    verbosed = true;
                }
                else
                {
                    // Commande traitée
                    if (EC.verbose)
                    {
                        verbosed = true;
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
        /// Enregistre EXIT comme action à mener pour cette commande
        /// Permet de demander à Command Line Tool de fermer, en cas d'erreur majeure à l'initialisation par exemple
        /// </summary>
        public void setExit()
        {
            action = ACTION_EXIT;
        }



        /// <summary>
        /// Renvoie True si l'action de la commande est EXIT, False sinon
        /// </summary>
        public bool isExit() { return action == ACTION_EXIT ? true : false; }



        /*
         * METHODES
         */

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
                    cmd_VERBOSE();
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

                // Default
                if (action == ACTION_INITIAL_VALUE)
                {
                    // Pas une action à un seul keyword n'a été identifiée
                    action = ACTION_UNDETERMINED;
                    eventCode = EVENTCODE_UNKNOWN_COMMAND_WORD;
                }

            }

            // Commandes à mots multiples
            if (subs.Length > 1)
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
                    cmd_VERBOSE();
                }

                if (subs[0].Equals(CMD_WORD_SCRIPTFILE, StrCompOpt))
                {
                    // Chargement et exécution d'un fichier de script, on repasse la main au processeur
                    // Action laissée au processeur
                    action = ACTION_SCRIPTFILE;
                    // TODO: Define cmd_SCRIPTFILE();
                }

                if (subs[0].Equals(CMD_WORD_CONVERT, StrCompOpt))
                {
                    action = ACTION_CONVERT;
                    cmd_CONVERT();
                }

                // En dernier ressort, on considère une commande de calcul
                if (action == ACTION_INITIAL_VALUE)
                {
                    // Commande à mots multiples et non reconnues précédement
                    action = ACTION_CALCULATE;
                    eventCode = EVENTCODE_COMMAND_UNPROCESSED;
                    cmd_CALCULATE();
                }

            }
            if (EnvContext.verbose) verbosed=true;
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
            // Exécution du calcul
            try
            {
                numResult = Container.compute(subs[0], factorList);
            }
            catch (ModelException e)
            {
                // La commande a échoué pendant le calcul 
                eventCode = EVENTCODE_CALC_PROCESS_ERROR;
                addInfo(new string[] { e.modelName, e.factorName, e.factorValue.ToString() });
            }
            // Tratement post calcul
            if (!double.IsNaN(numResult))
            {
                // Réussite du calcul
                eventCode = EVENTCODE_CALCULATE_SUCCESSFULL;
                numericResult = numResult;
            }
            else
            {
                // Echec du calcul, sans génération d'exception...
                eventCode = EVENTCODE_UNKNOWN_COMMAND_WORD;
            }
            return true;
        }



        private bool cmd_CONVERT()
        {
            // COMMANDE NON SUPPORTEE
            eventCode = EVENTCODE_UNSUPPORTED_COMMAND;
            return true;
        }



        private bool cmd_EXIT()
        {
            // Commande traitée par le processeur
            // TODO eventCode à générer ds le processeur
            eventCode = EVENTCODE_CMD_HANDOVER;
            return true;
        }



        private bool cmd_HELP()
        {
            // Commande traitée par le processeur
            // TODO eventCode à générer ds le processeur
            eventCode = EVENTCODE_CMD_HANDOVER;
            return true;
        }



        private bool cmd_INIT()
        {
            // Commande traitée par le processeur
            eventCode = EVENTCODE_CMD_HANDOVER;
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
            string models;
            models = Container.dataModelSignatures();
            if (string.IsNullOrEmpty(models))
            {
                eventCode = AeroCalcCommand.EVENTCODE_NO_MODEL_LOADED;
            }
            else
            {
                eventCode = AeroCalcCommand.EVENTCODE_PROCESS_SUCCESSFULL;
                txtResult = models;
            }
            return true;
        }



        /// <summary>
        /// Traitement de la commande de création d'une liste des unités de mesures
        /// disponibles dans le container de performances
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>True si le traitement n'a pas généré d'erreur</returns>
        /// <remarks></remarks>
        /// TODO: Objectif non encore atteint, on doit pouvoir lister les unités avec filtrage
        private bool cmd_LIST_UNITS()
        {
            // Liste des unités de la bibliothèque
            string msg = "";
            List<Unit> lu = Container.UnitsLib.getUnits();
            if (lu != null)
            {
                foreach (Unit item in lu)
                {
                    msg += item.ToString() + Environment.NewLine;
                }
                txtResult = msg;
                addInfo(lu.Count.ToString());
                eventCode = EVENTCODE_LIST_UNITS_SUCCESSFULL;
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
        /// <returns></returns>
        private bool cmd_LOAD_CATALOG()
        {
            // COMMANDE NON SUPPORTEE
            eventCode = EVENTCODE_UNSUPPORTED_COMMAND;
            return true;
        }



        /// <summary>
        /// Traite la commande de chargement d'un ou plusieurs modèles de données
        /// </summary>
        /// <returns>Code du traitement de l'opération</returns>
        /// <remarks>TO BE DEVELOPPED</remarks>
        /// 
        private bool cmd_LOAD_MODELS()
        {
            int counter = 0;

            foreach (string filter in subs)
            {
                counter += Container.loadDataModels("", filter);
            }
            if (counter > 0)
            {
                addInfo(counter.ToString());
                eventCode = EVENTCODE_LOAD_MODELS_SUCCESSFULL;
            }
            else
            {
                eventCode = EVENTCODE_NO_MODEL_LOADED;
            }
            return true;
        }



        /// <summary>
        /// Traite la commande de changement de mode VERBOSE
        /// </summary>
        /// <param name="Cmd">Commande active</param>
        /// <returns>Code du traitement de l'opération</returns>
        /// <remarks>TO BE DEVELOPPED</remarks>
        private void cmd_VERBOSE()
        {
            if (action == ACTION_VERBOSE)
            {
                // VERBOSE
                if (EnvContext.verboseAllowed)
                {
                    if (EnvContext.verbose)
                    {
                        // VERBOSE is already set
                        eventCode = EVENTCODE_VERBOSE_ALREADY;
                    }
                    else
                    {
                        // VERBOSE to be set
                        EnvContext.setVerbose(true);
                        verbosed = true;
                        eventCode = EVENTCODE_VERBOSE_ACTIVE;
                    }
                }
                else
                {
                    // Not allowed !
                    eventCode = EVENTCODE_UNABLE_VERBOSE_MODIFICATION;
                }
            }
            else
            {
                // STOP VERBOSE
                if (EnvContext.verboseAllowed)
                {
                    EnvContext.setVerbose(false);
                    verbosed = false;
                    eventCode = EVENTCODE_VERBOSE_INACTIVE;
                }
                else
                {
                    // Not allowed !
                    eventCode = EVENTCODE_UNABLE_VERBOSE_MODIFICATION;
                }
            }
        }



        /// <summary>
        /// Ajoute une information qui sera utilisée pour remplacer un marqueur dans le message
        /// à destination de l'utilisateur
        /// </summary>
        /// <param name="information">Information à communiquer</param>
        private void addInfo(string information)
        {
            if (!string.IsNullOrEmpty(information))
            {
                if (info != null)
                {
                    string[] newArray = new string[info.Length];
                    for (int index = 0; index < info.Length; index++)
                    {
                        // Copie de l'ancienne table
                        newArray[index] = info[index];
                    }
                    // Copie du nouvel élément
                    newArray[newArray.Length - 1] = information;
                    info = newArray;
                }
                else
                {
                    info = new string[1];
                    info[0] = information;
                }
            }
        }



        /// <summary>
        /// Ajoute une série d'informations qui seront utilisées pour remplacer un marqueur dans le message
        /// à destination de l'utilisateur
        /// </summary>
        /// <param name="information">Informations à communiquer</param>
        private void addInfo(string[] informations)
        {
            if (informations != null)
            {
                foreach (string item in informations)
                {
                    addInfo(item);
                }
            }
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
                    unitDictionaryIndex = Container.UnitsLib.getIndexByAlias(words[2]);
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
        /// TODO To be replaced by a formating function
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

    }

}