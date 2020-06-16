using System;
using System.IO; // TODO remove when messages filename integrated with EnvContext


namespace AeroCalcCore
{



    /// <summary>
    /// Classe destinée uniquement au traitement 'prost process' des commandes, pour mise en forme des messages textes.
    /// Utilisera le package de langue
    /// </summary>
    public class PostProcessor
    {
        /*
         * CONSTANTES
         */
        const string FIELD_RAW_TXT_CMD = "{TXT_CMD}";
        const string FIELD_MODEL_NAME = "{MODEL_NAME}";
        const string FIELD_MODELS_IN_MEM = "{MODELS_IN_MEM}";
        const string FIELD_MODEL_FACTOR = "{MODEL_FACTOR}";
        const string FIELD_CONFIG_FILE_NAME = "{CONFIG_FILE_NAME}";



        /*
         * PROPRIETES
         */
        //readonly string[] fields = { FIELD_RAW_TXT_CMD, FIELD_MODEL_NAME, FIELD_MODEL_FACTOR, FIELD_MODELS_IN_MEM };



        /*
         * MEMBRES
         */
        // TODO La bibliothèque doit être au minimum chargée avec le minimum de messages nécessaires pour indiquer
        // TODO le problème à l'utilisateur, dans la 'langue de base', l'anglais.
        // TODO Cette bibliothèque minimale est ensuite remplacée par celle chargée via un pack de langue.
        // TODO Le remplacement d'une bibliothèque de messages ne se fait qu'après validation de la nouvelle
        // TODO par num de version?, contrôle de la présence de certains messages clés ? A DEFINIR
        
        /// <summary>
        /// Library stockant les messages à l'utilisateur
        /// </summary>
        private EventMessages EMsgLib;

        //private bool verbose;



        /*
         * CONSTRUCTEUR
         */
        public PostProcessor(EnvironmentContext EC)
        {
            // TODO A intégrer pleinement à l'object EnvironmentContext pour éviter le codage en dur du nom de fichier
            //! Remove when possible
            //string fileName = EC.configDirPath + Path.AltDirectorySeparatorChar + "fr.xml";
            string fileName = EC.Langs.Library.Find(x => x.shortName.Equals(EC.activeLang)).fileAbsolutePath;

            // TODO Utiliser la fonction changeLanguage() pour charger la bibliothèque de messages
            //EventMessagesXMLFile xmlFile = new EventMessagesXMLFile(fileName);

            changeLanguage(fileName);

            /*
            if (xmlFile.IOStatus == FileIO.FILEOP_SUCCESSFUL) {
                EMsgLib = xmlFile.getEventMessagesFromXML();
            }
            */

            //verbose = EC.verbose;

        }



        /*
         * SERVICES
         */
        public void postProcess(AeroCalcCommand Cmd)
        {
            if (Cmd.eventCode == AeroCalcCommand.ECODE_INITIAL_VALUE)
            {
                // Command totally unprocessed, not a normal situation
                string msg = "[" + Cmd.eventCode + "]" + " POSTPROC:UNPROCESSED COMMAND";
                Cmd.setResultText(msg);
                // TODO Faut-il autoriser la demande EXIT par PostProcessor ?
                Cmd.isExit();
            }
            if (Cmd.eventCode > 0)
            {
                // Successfull operations
                if (!double.IsNaN(Cmd.numericResult))
                {
                    // A numeric value has been produced
                    Cmd.setResultText(Cmd.rawTxtCommand + " = " + Cmd.numericResult);
                }
                else
                {
                    // No numeric value to expose
                    string msg = swapFields(EMsgLib.getMessageWith(Cmd.eventCode), Cmd.info);
                    if (!string.IsNullOrEmpty(Cmd.txtResult))
                    {
                        // A message has been prepared, message from library is added, if it exists
                        if (!string.IsNullOrEmpty(msg))
                        {
                            // A msg from library is found
                            Cmd.setResultText(Cmd.txtResult + Environment.NewLine + msg);
                        }
                    }
                    else
                    {
                        // No message prepared by the command
                        if (string.IsNullOrEmpty(msg))
                        {
                            // No msg from library
                            msg = "[" + Cmd.eventCode + "]" + " POSTPROC:RESULT MSG NOT IMPLEMENTED";
                        }
                        Cmd.setResultText(msg);
                    }
                }
            }
            if (Cmd.eventCode < 0)
            {
                // Error
                string msg = swapFields(EMsgLib.getMessageWith(Cmd.eventCode), Cmd.info);
                if (string.IsNullOrEmpty(msg))
                {
                    // Nothing returned
                    msg = "[" + Cmd.eventCode + "]" + " POSTPROC:ERROR MSG NOT IMPLEMENTED";
                }
                Cmd.setResultText(msg);
            }
            // Verbose
            if (Cmd.verbosed)
            {
                verboseCommand(Cmd);
            }
        }



        public int changeLanguage(string fileAbsolutePath) {

            EventMessagesXMLFile xmlFile = new EventMessagesXMLFile(fileAbsolutePath);
            if (xmlFile.IOStatus == FileIO.FILEOP_SUCCESSFUL) {
                // Load it then !
                EventMessages msgs = xmlFile.getEventMessagesFromXML();
                if (msgs == null) {
                    // Problem when forging messages library
                    return AeroCalcCommand.ECODE_ERR_LANG_UNDETERMINED;
                }
                else {
                    // This new lib is ok
                    EMsgLib = msgs;
                    return AeroCalcCommand.ECODE_LANG_CHANGED_SUCCESSFULL;
                }
            }
            else {
                // Error from file reading
                // TODO Il faut renvoyer un int homogène !!! ECODE ou IOStatus ???
                return xmlFile.IOStatus;
            }
        }



        /*
         * METHODES
         */
        private void verboseCommand(AeroCalcCommand Cmd)
        {
            string msg = "";
            msg += "raw command  { " + Cmd.rawTxtCommand;
            msg += " }  action code { " + Cmd.action;
            // if (!string.IsNullOrEmpty(Cmd.workDirectory)) { msg += " }  work directory { " + Cmd.workDirectory; }
            // if (!string.IsNullOrEmpty(Cmd.inputFileName)) { msg += " }  input file { " + Cmd.inputFileName; }
            // if (!string.IsNullOrEmpty(Cmd.outputFileName)) { msg += " }  output file { " + Cmd.outputFileName; }
            msg += " }  event code { " + Cmd.eventCode;
            msg += " }  result { " + Cmd.numericResult;
            msg += " }  duration { " + Cmd.durationMilliSecond + " ms }" + Environment.NewLine;
            Cmd.setResultText(Cmd.txtResult + msg);
        }



        private string swapFields(string message, string[] info)
        {
            if (info != null && message.Contains("$0"))
            {
                // Un code à remplacer est identifié
                for (int index = 0; index < info.Length; index++)
                {
                    message = message.Replace(string.Concat("$", index.ToString()), info[index]);
                }
            }
            return message;
        }
        /// <summary>
        /// Accesseur de Test
        /// </summary>
        public string _A_formatMsg(string message, string[] info)
        {
            return swapFields(message, info);
        }

    }

}