using System;
using System.IO; // TODO remove when messages filename integrated with EnvContext
using System.Reflection.Emit;

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



        /*
         * MEMBRES
         */
        // TODO La bibliothèque doit être au minimum chargée avec les messages nécessaires pour indiquer
        // TODO le problème à l'utilisateur, dans la 'langue de base', l'anglais.
        // TODO Cette bibliothèque minimale est ensuite remplacée par celle chargée via un pack de langue.
        // TODO Le remplacement d'une bibliothèque de messages ne se fait qu'après validation de la nouvelle
        // TODO par num de version?, contrôle de la présence de certains messages clés ? A DEFINIR

        /// <summary>
        /// Library stockant les messages à l'utilisateur
        /// </summary>
        private EventMessages EMsgLib;



        /*
         * CONSTRUCTEUR
         */
        public PostProcessor(EnvironmentContext EC) {
            EMsgLib = new EventMessages();
            changeLanguage(EC.getActiveLanguage());
        }



        /*
         * SERVICES
         */
        /// <summary>
        /// Réalise le post traitement d'une commande AeroCalcCommand, pour implémenter les messages à l'utilisateur
        /// </summary>
        /// <param name="Cmd">AeroCalcCommand à traiter</param>
        /// <remarks>
        //  TODO L'implémentation d'une bibliothèque minimale de messages d'erreur doit être envisagée
        //  TODO A faire au niveau du constructeur de EventMessages
        /// </remarks>
        public void postProcess(AeroCalcCommand Cmd) {
            if (Cmd.eventCode == AeroCalcCommand.ECODE_INITIAL_VALUE) {
                // Command totally unprocessed, not a normal situation
                string msg = "[" + Cmd.eventCode + "]" + " POSTPROC:UNPROCESSED COMMAND";
                Cmd.setResultText(msg);
                // TODO Faut-il autoriser la demande EXIT par PostProcessor ?
                Cmd.isExit();
            }
            if (Cmd.eventCode > 0) {
                // Successfull operations
                if (!double.IsNaN(Cmd.numericResult)) {
                    // A numeric value has been produced
                    Cmd.setResultText(Cmd.rawTxtCommand + " = " + Cmd.numericResult);
                }
                else {
                    // No numeric value to expose
                    string msg = swapFields(EMsgLib.getMessageWith(Cmd.eventCode), Cmd.info);
                    if (!string.IsNullOrEmpty(Cmd.txtResult)) {
                        // A message has been prepared, message from library is added, if it exists
                        if (!string.IsNullOrEmpty(msg)) {
                            // A msg from library is found
                            Cmd.setResultText(Cmd.txtResult + Environment.NewLine + msg);
                        }
                    }
                    else {
                        // No message prepared by the command
                        if (string.IsNullOrEmpty(msg)) {
                            // No msg from library
                            msg = "[" + Cmd.eventCode + "]" + " POSTPROC:RESULT MSG NOT IMPLEMENTED";
                        }
                        Cmd.setResultText(msg);
                    }
                }
            }
            if (Cmd.eventCode < 0) {
                // Error
                string msg = swapFields(EMsgLib.getMessageWith(Cmd.eventCode), Cmd.info);
                if (string.IsNullOrEmpty(msg)) {
                    // Nothing returned
                    msg = "[" + Cmd.eventCode + "]" + " POSTPROC:ERROR MSG NOT IMPLEMENTED";
                }
                Cmd.setResultText(msg);
            }
            // Verbose
            if (Cmd.verbosed) {
                verboseCommand(Cmd);
            }
        }



        /// <summary>
        /// Load a language pack and update the event messages library
        /// </summary>
        /// <param name="fileAbsolutePath"></param>
        /// <returns>AeroCalcCommand.ECODE_ERR_LANGFILE_*</returns>
        /// <remarks>
        /// The current lang pack is considered valid and the new one only replaces the current when
        /// checks completed
        /// </remarks>
        public int changeLanguage(Language Lang) {

            //EventMessagesXMLFile xmlFile = new EventMessagesXMLFile(fileAbsolutePath);
            EventMessagesXMLFile xmlFile = new EventMessagesXMLFile(Lang.fileAbsolutePath);

            switch (xmlFile.IOStatus) {

                case FileIO.FILEOP_SUCCESSFUL:
                    // Load it then !
                    EventMessages msgs = xmlFile.getEventMessagesFromXML();
                    if (msgs == null) {
                        // Problem when forging messages library
                        return AeroCalcCommand.ECODE_ERR_LANG_UNDETERMINED;
                    }
                    else {
                        // TODO Is this new library ok ?
                        // TODO What kind of test should be implemented ? A package should refer to a 'standard' number ?
                        if (msgs.langIsoCode != Lang.isoCode) {
                            // Ce n'est pas le language demandé...
                            return AeroCalcCommand.ECODE_ERR_LANG_UNDETERMINED;
                        }
                        else {
                            // OK !!
                            EMsgLib = msgs;

                            return AeroCalcCommand.ECODE_LANG_CHANGED_SUCCESSFULL;
                        }

                    }

                case FileIO.FILEOP_INVALID_PATH:
                    return AeroCalcCommand.ECODE_ERR_LANGFILE_PATH;

                case FileIO.FILEOP_IO_ERROR:
                    return AeroCalcCommand.ECODE_ERR_LANGFILE_IO_ERROR;

                case FileIO.FILEOP_FILE_DOES_NOT_EXIST:
                    return AeroCalcCommand.ECODE_ERR_LANGFILE_DOES_NOT_EXIST;

                case FileIO.FILEOP_INPUT_FILE_IS_LOCKED:
                    return AeroCalcCommand.ECODE_ERR_LANGFILE_SECURITY;

                case FileIO.FILEOP_FILE_INVALID_CONTENT:
                    return AeroCalcCommand.ECODE_ERR_LANGFILE_CONTENT;

                default:
                    return AeroCalcCommand.ECODE_ERR_LANGFILE_UKN_ERROR;
            }
        }



        /*
         * METHODES
         */
        private void verboseCommand(AeroCalcCommand Cmd) {
            string msg = "";
            msg += "raw command  { " + Cmd.rawTxtCommand;
            msg += " }  action code { " + Cmd.action;
            msg += " }  event code { " + Cmd.eventCode;
            msg += " }  result { " + Cmd.numericResult;
            msg += " }  duration { " + Cmd.durationMilliSecond + " ms }" + Environment.NewLine;
            Cmd.setResultText(Cmd.txtResult + msg);
        }



        private string swapFields(string message, string[] info) {
            if (info != null && message.Contains("$0")) {
                // Un code à remplacer est identifié
                for (int index = 0; index < info.Length; index++) {
                    message = message.Replace(string.Concat("$", index.ToString()), info[index]);
                }
            }
            return message;
        }
        /// <summary>
        /// Accesseur de Test
        /// </summary>
        public string _A_formatMsg(string message, string[] info) {
            return swapFields(message, info);
        }

    }

}