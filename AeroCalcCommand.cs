using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
        public const string CMD_WORD_LANG = "LANG";
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
        public const int ACTION_DECLARE = 45;
        public const int ACTION_LOAD_MODELS = 50;
        public const int ACTION_LOAD_UNITS = 51;
        public const int ACTION_CATALOG = 52;
        public const int ACTION_LIST_MODELS = 53;
        public const int ACTION_LIST_UNITS = 54;
        public const int ACTION_LIST_LANG = 55;
        public const int ACTION_EXIT = 60;
        public const int ACTION_HELP = 70;
        public const int ACTION_VERBOSE = 90;
        public const int ACTION_STOP_VERBOSE = 91;
        public const int ACTION_LANG = 95;
        public const int ACTION_LANG_CHANGE = 96;



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
        public const int ECODE_VERBOSE_ACTIVE = 910;
        public const int ECODE_VERBOSE_INACTIVE = 911;
        public const int ECODE_VERBOSE_ALREADY = 912;
        public const int ECODE_HELP_REQUESTED = 900;
        public const int ECODE_LIST_LANG_SUCCESSFULL = 560;
        public const int ECODE_LIST_UNITS_SUCCESSFULL = 550;
        public const int ECODE_SCRIPTFILE_SUCCESSFULL = 500;
        public const int ECODE_LOAD_MODELS_SUCCESSFULL = 400;
        public const int ECODE_LANG_CHANGED_SUCCESSFULL = 301;
        public const int ECODE_ACTIVE_LANG = 300;
        public const int ECODE_MEM_SUCCESSFULL = 210;
        public const int ECODE_CALC_SUCCESSFULL = 200;
        public const int ECODE_INIT_SUCCESSFULL = 110;
        public const int ECODE_PROCESS_SUCCESSFULL = 100;
        public const int ECODE_CMD_HANDOVER = 20;
        public const int ECODE_EXIT_REQUESTED = 10;

        public const int ECODE_INITIAL_VALUE = 0; // Valeur d'initialisation

        public const int ECODE_ERR_INIT_UNSUCCESSFULL = -1;
        public const int ECODE_ERR_REINIT_NOT_ALLOWED = -2;
        public const int ECODE_ERR_INIT_CONFIGFILE_PATH = -3;
        public const int ECODE_ERR_INIT_IO_ERROR = -4;
        public const int ECODE_ERR_INIT_UKN_FILE_ERROR = -5;
        public const int ECODE_ERR_INIT_UKN_ERROR = -6;
        public const int ECODE_ERR_INIT_UNITS_FILE = -7;
        public const int ECODE_ERR_INIT_LANGUAGE_FILE = -8;
        public const int ECODE_ERR_CMD_UNPROCESSED = -10; // MINIMAL ERR MSG LIB

        public const int ECODE_ERR_SCRIPTFILE_DOES_NOT_EXIST = -12;
        public const int ECODE_ERR_SCRIPT_PATH = -13;
        public const int ECODE_ERR_SCRIPT_IO_ERROR = -14;
        public const int ECODE_ERR_SCRIPT_UKN_FILE_ERROR = -15;
        public const int ECODE_ERR_SCRIPT_UKN_ERROR = -16;
        public const int ECODE_ERR_SCRIPT_SECURITY = -17;
        public const int ECODE_ERR_SCRIPT_GENERIC = -18;
        public const int ECODE_ERR_SCRIPTFILE_VOID = -19;

        public const int ECODE_ERR_CMD_VOID = -21;

        public const int ECODE_ERR_UKN_CMD_WORD = -22;
        public const int ECODE_ERR_UNSUPPORTED_CMD = -23;
        public const int ECODE_ERR_NO_UNIT_DATA_AVAILABLE = -30;
        public const int ECODE_ERR_UNABLE_VERBOSE_MOD = -50;

        public const int ECODE_ERR_PROCESSOR_FAILURE = -100;
        public const int ECODE_ERR_CALC_PROCESS = -110;
        public const int ECODE_ERR_CALC_MISSING_FACTOR = -111;
        public const int ECODE_ERR_CALC_MISSING_MODEL = -112;
        public const int ECODE_ERR_MEM_DECLARATION = -115;

        public const int ECODE_ERR_LANG_UNDETERMINED = -301;
        public const int ECODE_ERR_LANG_DOES_NOT_EXIST = -302;
        public const int ECODE_ERR_LANG_DISABLED = -303;
        public const int ECODE_ERR_LANG_ALREADY_SET = -304;

        public const int ECODE_ERR_LANGFILE_CONTENT_MISSING = -311;
        public const int ECODE_ERR_LANGFILE_DOES_NOT_EXIST = -312;
        public const int ECODE_ERR_LANGFILE_PATH = -313;
        public const int ECODE_ERR_LANGFILE_IO_ERROR = -314;
        public const int ECODE_ERR_LANGFILE_UKN_FILE_ERROR = -315;
        public const int ECODE_ERR_LANGFILE_UKN_ERROR = -316;
        public const int ECODE_ERR_LANGFILE_SECURITY = -317;
        public const int ECODE_ERR_LANGFILE_GENERIC = -318;
        public const int ECODE_ERR_LANGFILE_VOID = -319;
        public const int ECODE_ERR_LANGFILE_OLD_VERSION = -320;
        public const int ECODE_ERR_LANGFILE_ID = -321;

        public const int ECODE_ERR_NO_MODEL_LOADED = -500;


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
        /// Table des mots de la commande au format texte
        /// </summary>
        public string[] subs { get; private set; }

        /// <summary>
        /// Liste des facteurs de la commande
        /// </summary>
        public List<CommandFactor> Factors { get; private set; }

        /// <summary>
        /// Pile mémoire
        /// </summary>
        public MemoryStack MemStack { get; private set; }

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

        /// <summary>
        /// Flag du mode Verbose
        /// </summary>
        public bool verbosed { get; private set; }
        
        /// <summary>
        /// Index servant dans certaines commandes (LANG CHANGE)
        /// </summary>
        public int index { get; private set; }


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
        /// </summary>
        /// <param name="inputText">string contenant la commande en mode texte, sans traitement préalable.</param>
        /// <param name="DMC">DataModelContainer, Conteneur de modèles de calculs</param>
        /// <param name="EC">EnvironmentContext, Objet contenant toutes les données de contexte</param>
        /// <param name="MS">MemoryStack, Pile de mémoire</param>
        public AeroCalcCommand(string inputText, DataModelContainer DMC, EnvironmentContext EC, MemoryStack MS) {
            // Initialisation des propriétés
            startOfProcess = new DateTime(DateTime.Now.Ticks, DateTimeKind.Utc);
            action = ACTION_INITIAL_VALUE;
            eventCode = ECODE_INITIAL_VALUE;
            numericResult = Double.NaN;
            subs = null;
            Factors = new List<CommandFactor>();
            Container = DMC;
            EnvContext = EC;
            MemStack = MS;

            if (EnvContext.verbose) verbosed = true;

            if (string.IsNullOrEmpty(inputText)) {
                // Cas particulier de la chaine nulle
                rawTxtCommand = "";
                action = ACTION_UNDETERMINED;
                eventCode = ECODE_ERR_CMD_VOID;
            }
            else {
                // La chaine de texte n'est pas vide, traitement de la commande
                rawTxtCommand = inputText;
                if (!execute()) {
                    // Un problème majeur s'est produit dans le traitement de la commande
                    eventCode = ECODE_ERR_PROCESSOR_FAILURE;
                    verbosed = true;
                }
                else {
                    // Commande traitée
                    if (EC.verbose) {
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
        public bool factor(string factorName, out double value) {
            if (string.IsNullOrEmpty(factorName)) {
                value = AeroCalc.MODEL_DIMENSION_DEFAULT_VALUE;
                return true;
            }
            int index = this.Factors.FindIndex(x => x.name == factorName);
            if (index < 0) {
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
        public void setEventCode(int eventCode) {
            this.eventCode = eventCode;
        }



        /// <summary>
        /// Enregistre le résultat numérique du calcul réalisé
        /// </summary>
        /// <param name="result">Double, valeur numérique résultat du calcul demandé</param>
        public void setNumericResult(double result) {
            this.numericResult = result;
        }



        /// <summary>
        /// Enregistre le texte passé en argument comme résultat au format texte
        /// </summary>
        /// <param name="txtResult">Résultat au format texte qui sera communiqué à un utilisateur utilisant la console texte
        /// </param>
        public void setResultText(string txtResult) {
            this.txtResult = txtResult;
        }



        /// <summary>
        /// Enregistre EXIT comme action à mener pour cette commande
        /// Permet de demander à Command Line Tool de fermer, en cas d'erreur majeure à l'initialisation par exemple
        /// </summary>
        public void setExit() {
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
        /// <returns>True si l'action a pu être identifiée et traitée</returns>
        /// <remarks>
        /// Créée spécialement pour respecter un principe d'architecture POO, le constructeur ne doit pas
        /// comporter de traitement métier.
        //  TODO: Traitement de la chaine de caractère pour localisation.
        /// </remarks>
        private bool execute() {
            StringComparison StrCompOpt = StringComparison.CurrentCultureIgnoreCase;
            subs = rawTxtCommand.Split(commandSeparators, StringSplitOptions.RemoveEmptyEntries);

            // Commandes à mot unique
            if (subs.Length == 1) {
                if (subs[0].Equals(CMD_WORD_EXIT, StrCompOpt)) {
                    action = ACTION_EXIT;
                    cmd_EXIT();
                }

                if (subs[0].Contains(CMD_OPERATOR_AFFECT)) {
                    action = ACTION_DECLARE;
                    cmd_DECLARE();
                }

                if (subs[0].Equals(CMD_WORD_CONVERT, StrCompOpt)) {
                    action = ACTION_CONVERT;
                    cmd_CONVERT();
                }

                if (subs[0].Equals(CMD_WORD_VERBOSE, StrCompOpt)) {
                    action = ACTION_VERBOSE;
                    cmd_VERBOSE();
                }

                if (subs[0].Equals(CMD_WORD_CATALOG, StrCompOpt)) {
                    action = ACTION_CATALOG;
                    cmd_LOAD_CATALOG();
                }

                if (subs[0].Equals(CMD_WORD_LIST, StrCompOpt)) {
                    action = ACTION_LIST_MODELS;
                    // TODO: S'arranger pour que la commande à mot unique LIST soit équivalente à LIST MODEL *
                    // Traitement des ALIAS potentiels
                    //useAlias();
                    cmd_LIST_MODELS();
                }

                if (subs[0].Equals(CMD_WORD_LOAD, StrCompOpt)) {
                    action = ACTION_LOAD_MODELS;
                    // TODO: S'arranger pour que la commande à mot unique LOAD soit équivalente à LOAD *
                    // Traitement des ALIAS potentiels
                    //useAlias();
                    cmd_LOAD_MODELS();
                }

                if (subs[0].Equals(CMD_WORD_HELP, StrCompOpt)) {
                    // Action laissée au processeur
                    action = ACTION_HELP;
                    cmd_HELP();
                }

                if (subs[0].Equals(CMD_WORD_LANG, StrCompOpt)) {
                    action = ACTION_LANG;
                    cmd_LANG();
                }

                // Default
                if (action == ACTION_INITIAL_VALUE) {
                    // Pas une action à un seul keyword n'a été identifiée
                    action = ACTION_UNDETERMINED;
                    eventCode = ECODE_ERR_UKN_CMD_WORD;
                }

            }

            // Commandes à mots multiples
            if (subs.Length > 1) {
                if (subs[0].Equals(CMD_WORD_INIT_INTERPRETER, StrCompOpt)) {
                    // Action laissée au processeur
                    action = ACTION_INIT_INTERPRETER;
                    cmd_INIT();
                }

                if (subs[0].Equals(CMD_WORD_LOAD, StrCompOpt)) {
                    if (subs[1].Equals(CMD_WORD_MODEL, StrCompOpt)) {
                        action = ACTION_LOAD_MODELS;
                        cmd_LOAD_MODELS();
                    }
                    else if (subs[1].Equals(CMD_WORD_UNIT, StrCompOpt)) {
                        action = ACTION_LOAD_UNITS;
                        // TODO: Define cmd_LOAD_UNITS()
                    }
                }

                if (subs[0].Equals(CMD_WORD_LIST, StrCompOpt)) {
                    if (subs[1].Equals(CMD_WORD_MODEL, StrCompOpt)) {
                        action = ACTION_LIST_MODELS;
                        cmd_LIST_MODELS();
                    }
                    else if (subs[1].Equals(CMD_WORD_UNIT, StrCompOpt)) {
                        action = ACTION_LIST_UNITS;
                        cmd_LIST_UNITS();
                    }
                    else if (subs[1].Equals(CMD_WORD_LANG, StrCompOpt)) {
                        action = ACTION_LIST_LANG;
                        cmd_LIST_LANG();
                    }
                }

                if (subs[0].Equals(CMD_WORD_STOP, StrCompOpt) &&
                    subs[1].Equals(CMD_WORD_VERBOSE, StrCompOpt)) {
                    // Action laissée au processeur
                    action = ACTION_STOP_VERBOSE;
                    cmd_VERBOSE();
                }

                if (subs[0].Equals(CMD_WORD_SCRIPTFILE, StrCompOpt)) {
                    // Chargement et exécution d'un fichier de script, on repasse la main au processeur
                    action = ACTION_SCRIPTFILE;
                    cmd_SCRIPTFILE();
                }

                if (subs[0].Equals(CMD_WORD_CONVERT, StrCompOpt)) {
                    action = ACTION_CONVERT;
                    cmd_CONVERT();
                }

                if (subs[0].Equals(CMD_WORD_LANG, StrCompOpt)) {
                    action = ACTION_LANG_CHANGE;
                    cmd_LANG();
                }

                // En dernier ressort, on considère une commande de calcul
                if (action == ACTION_INITIAL_VALUE) {
                    // Commande à mots multiples et non reconnues précédement
                    action = ACTION_CALCULATE;
                    eventCode = ECODE_ERR_CMD_UNPROCESSED;
                    cmd_CALCULATE();
                }

            }
            if (EnvContext.verbose) verbosed = true;
            return true;
        }



        /// <summary>
        /// Traitement d'une commande de calcul de performance.
        /// </summary>
        /// <returns>
        /// Retourne True si le traitement de la commande est complet.
        /// </returns>
        /// <remarks>
        /// </remarks>
        private bool cmd_CALCULATE() {
            // Initialisation
            double numResult = double.NaN;
            // constitution de la Liste des facteurs communiqués dans la commande
            List<CommandFactor> factorList = new List<CommandFactor>();
            for (int count = 1; count < this.subs.Length; count++) {
                factorList.Add(getFactor(subs[count]));
            }
            // Exécution du calcul
            try {
                numResult = Container.compute(subs[0], factorList);
            }
            catch (ModelException e) {
                // La commande a échoué pendant le calcul 
                eventCode = ECODE_ERR_CALC_PROCESS;
                addInfo(new string[] { e.modelName, e.factorName, e.factorValue.ToString() });
            }
            // Tratement post calcul
            if (!double.IsNaN(numResult)) {
                // Réussite du calcul
                eventCode = ECODE_CALC_SUCCESSFULL;
                numericResult = numResult;
            }
            else {
                // Echec du calcul, sans génération d'exception...
                eventCode = ECODE_ERR_UKN_CMD_WORD;
            }
            return true;
        }



        /// <summary>
        /// DRAFT Mise en mémoire de valeurs
        /// </summary>
        /// <returns></returns>
        private bool cmd_DECLARE() {
            // Initialisation
            bool success;
            CommandFactor factor;
            factor = new CommandFactor(subs[0]);
            success = MemStack.addFactor(factor);

            if (success) {
                addInfo(new string[] { factor.name, factor.value.ToString() });
                eventCode = ECODE_MEM_SUCCESSFULL;
                return true;
            }
            else {
                eventCode = ECODE_ERR_MEM_DECLARATION;
                return false;
            }
        }



        /// <summary>
        /// Traitement de la commande de conversion entre unités
        /// </summary>
        /// <returns></returns>
        private bool cmd_CONVERT() {
            // COMMANDE NON SUPPORTEE
            eventCode = ECODE_ERR_UNSUPPORTED_CMD;
            return true;
        }



        /// <summary>
        ///  Traitement de la commande d'exécution d'un script
        /// </summary>
        /// <returns></returns>
        private bool cmd_SCRIPTFILE() {
            // Commande traitée par le processeur
            addInfo(subs[1]);
            eventCode = ECODE_CMD_HANDOVER;
            return true;
        }



        /// <summary>
        /// Traitement de la commande de gestion du pack de langue
        /// </summary>
        /// <returns></returns>
        private bool cmd_LANG() {
            if (action == ACTION_LANG) {
                // Language currently used
                addInfo(EnvContext.Langs.Library[EnvContext.activeLangIndex].name);
                addInfo(EnvContext.Langs.Library[EnvContext.activeLangIndex].isoCode);
                eventCode = ECODE_ACTIVE_LANG;
            }
            else {
                // Set a language
                Language l = EnvContext.Langs.Library.Find(x => x.isoCode.Equals(subs[1]));
                if (l == null) {
                    addInfo(subs[1]);
                    eventCode = ECODE_ERR_LANG_DOES_NOT_EXIST;
                    return true;
                }
                if (!l.enabled) {
                    addInfo(l.isoCode);
                    eventCode = ECODE_ERR_LANG_DISABLED;
                    return true;
                }
                if (l.isoCode.Equals(EnvContext.activeLang)) {
                    addInfo(subs[1]);
                    eventCode = ECODE_ERR_LANG_ALREADY_SET;
                    return true;
                }
                index = EnvContext.Langs.Library.IndexOf(l);
                eventCode = ECODE_CMD_HANDOVER;
                addInfo(new string[] { l.name, l.isoCode });
            }
            return true;
        }



        /// <summary>
        ///  Traitement de la commande de fermeture de l'interpréteur de commande
        /// </summary>
        /// <returns></returns>
        private bool cmd_EXIT() {
            // Commande traitée par le processeur
            eventCode = ECODE_CMD_HANDOVER;
            return true;
        }



        /// <summary>
        /// Traitement de la commande de demande d'aide
        /// </summary>
        /// <returns></returns>
        private bool cmd_HELP() {
            // Commande traitée par le processeur
            eventCode = ECODE_CMD_HANDOVER;
            return true;
        }



        /// <summary>
        /// Traitement de la commande d'initialisation de l'interpréteur de commande
        /// </summary>
        /// <returns></returns>
        private bool cmd_INIT() {
            // Commande traitée par le processeur
            eventCode = ECODE_CMD_HANDOVER;
            return true;
        }



        /// <summary>
        /// Traitement de la commande de création d'une liste des modèles de performance
        /// disponibles dans le container de performances
        /// </summary>
        /// <returns>Bool résultat du traitement</returns>
        /// <remarks>
        // TODO Objectif partiellement atteint, on doit pouvoir lister les modèles avec filtrage
        /// </remarks>
        private bool cmd_LIST_MODELS() {
            // Recherche des noms de modèles de performance qui match
            string models;
            models = Container.dataModelSignatures();
            if (string.IsNullOrEmpty(models)) {
                eventCode = AeroCalcCommand.ECODE_ERR_NO_MODEL_LOADED;
            }
            else {
                eventCode = AeroCalcCommand.ECODE_PROCESS_SUCCESSFULL;
                txtResult = models;
            }
            return true;
        }



        /// <summary>
        /// Traitement de la commande de création d'une liste des unités de mesures
        /// disponibles dans le container de performances
        /// </summary>
        /// <returns>True si le traitement n'a pas généré d'erreur</returns>
        /// <remarks>
        // TODO Objectif partiellement atteint, on doit pouvoir lister les unités avec filtrage
        /// </remarks>
        private bool cmd_LIST_UNITS() {
            // Liste des unités de la bibliothèque
            string msg = "";
            List<Unit> lu = Container.UnitsLib.getUnits();
            if (lu != null) {
                foreach (Unit item in lu) {
                    msg += item.ToString() + Environment.NewLine;
                }
                txtResult = msg;
                addInfo(lu.Count.ToString());
                eventCode = ECODE_LIST_UNITS_SUCCESSFULL;
            }
            else {
                // No units available!
                eventCode = ECODE_ERR_NO_UNIT_DATA_AVAILABLE;
            }
            return true;
        }



        /// <summary>
        /// Traitement de la commande de liste des packs de langue enregistrés
        /// </summary>
        /// <returns></returns>
        private bool cmd_LIST_LANG() {
            string listMsg = "";
            foreach (Language lang in EnvContext.Langs.Library) {
                listMsg += lang.ToString() + Environment.NewLine;
            }
            txtResult = listMsg;
            addInfo(EnvContext.Langs.Count.ToString());
            eventCode = ECODE_LIST_LANG_SUCCESSFULL;
            return true;
        }



        /// <summary>
        /// Traite la commande de chargement du catalogue
        /// </summary>
        /// <returns></returns>
        private bool cmd_LOAD_CATALOG() {
            // COMMANDE NON SUPPORTEE
            eventCode = ECODE_ERR_UNSUPPORTED_CMD;
            return true;
        }



        /// <summary>
        /// Traite la commande de chargement d'un ou plusieurs modèles de données
        /// </summary>
        /// <returns>Code du traitement de l'opération</returns>
        /// <remarks>TO BE DEVELOPPED</remarks>
        /// 
        private bool cmd_LOAD_MODELS() {
            int counter = 0;

            foreach (string filter in subs) {
                counter += Container.loadDataModels("", filter);
            }
            if (counter > 0) {
                addInfo(counter.ToString());
                eventCode = ECODE_LOAD_MODELS_SUCCESSFULL;
            }
            else {
                eventCode = ECODE_ERR_NO_MODEL_LOADED;
            }
            return true;
        }



        /// <summary>
        /// Traite la commande de changement de mode VERBOSE
        /// </summary>
        /// <returns>Code du traitement de l'opération</returns>
        /// <remarks>TO BE DEVELOPPED</remarks>
        private void cmd_VERBOSE() {
            if (action == ACTION_VERBOSE) {
                // VERBOSE
                if (EnvContext.verboseAllowed) {
                    if (EnvContext.verbose) {
                        // VERBOSE is already set
                        eventCode = ECODE_VERBOSE_ALREADY;
                    }
                    else {
                        // VERBOSE to be set
                        EnvContext.setVerbose(true);
                        verbosed = true;
                        eventCode = ECODE_VERBOSE_ACTIVE;
                    }
                }
                else {
                    // Not allowed !
                    eventCode = ECODE_ERR_UNABLE_VERBOSE_MOD;
                }
            }
            else {
                // STOP VERBOSE
                if (EnvContext.verboseAllowed) {
                    EnvContext.setVerbose(false);
                    verbosed = false;
                    eventCode = ECODE_VERBOSE_INACTIVE;
                }
                else {
                    // Not allowed !
                    eventCode = ECODE_ERR_UNABLE_VERBOSE_MOD;
                }
            }
        }



        /// <summary>
        /// Ajoute une information qui sera utilisée pour remplacer un marqueur dans le message
        /// à destination de l'utilisateur
        /// </summary>
        /// <param name="information">Information à communiquer</param>
        private void addInfo(string information) {
            if (!string.IsNullOrEmpty(information)) {
                if (info != null) {
                    string[] newArray = new string[info.Length + 1];
                    for (int index = 0; index < info.Length; index++) {
                        // Copie de l'ancienne table
                        newArray[index] = info[index];
                    }
                    // Copie du nouvel élément
                    newArray[newArray.Length - 1] = information;
                    info = newArray;
                }
                else {
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
        private void addInfo(string[] informations) {
            if (informations != null) {
                foreach (string item in informations) {
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
        private CommandFactor getFactor(string subString) {
            string name = "";
            int unitDictionaryIndex = 0;
            double val = double.NaN;
            char[] separators = { CMD_OPERATOR_AFFECT, CMD_OPERATOR_UNIT };
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
                    unitDictionaryIndex = Container.UnitsLib.getIndexByAlias(words[2]);
                    if (unitDictionaryIndex != AeroCalc.UNIT_UNDETERMINED) {
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
        // TODO To be replaced by a formating function
        private void useAlias() {
            bool aliasUsed = false;
            string aliasCmd = "";

            if (subs.Length == 1 && this.action == ACTION_LIST_MODELS) {
                // LIST  ->  LIST *
                aliasCmd = "LIST *";
                aliasUsed = true;
            }
            if (subs.Length == 1 && this.action == ACTION_LOAD_MODELS) {
                // LOAD  ->  LOAD *
                aliasCmd = "LOAD *";
                aliasUsed = true;
            }
            // Si remplacement, nouveau séquençage en mots
            if (aliasUsed) {
                subs = aliasCmd.Split(commandSeparators, StringSplitOptions.RemoveEmptyEntries);
            }
        }

    }

}