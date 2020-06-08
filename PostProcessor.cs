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
        private EventMessages EMsgLib;

        //private bool verbose;



        /*
         * CONSTRUCTEUR
         */
        public PostProcessor(EnvironmentContext EC)
        {
            // TODO A intégrer pleinement à l'object EnvironmentContext
            //! Remove when possible
            string fileName = EC.configDirPath + Path.AltDirectorySeparatorChar + "fr.xml";

            EventMessagesXMLFile xmlFile = new EventMessagesXMLFile(fileName);

            EMsgLib = xmlFile.getEventMessagesFromXML();

            //verbose = EC.verbose;

        }



        /*
         * SERVICES
         */
        public void postProcess(AeroCalcCommand Cmd)
        {
            if (Cmd.eventCode == AeroCalcCommand.EVENTCODE_INITIAL_VALUE)
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



        /*
         * METHODES
         */
        private void verboseCommand(AeroCalcCommand Cmd)
        {
            string msg = "";
            msg += Environment.NewLine + "raw command  { " + Cmd.rawTxtCommand;
            msg += " }  action code { " + Cmd.action;
            if (!string.IsNullOrEmpty(Cmd.workDirectory)) { msg += " }  work directory { " + Cmd.workDirectory; }
            if (!string.IsNullOrEmpty(Cmd.inputFileName)) { msg += " }  input file { " + Cmd.inputFileName; }
            if (!string.IsNullOrEmpty(Cmd.outputFileName)) { msg += " }  output file { " + Cmd.outputFileName; }
            msg += " }  event code { " + Cmd.eventCode;
            msg += " }  result { " + Cmd.numericResult;
            msg += " }  duration { " + Cmd.durationMilliSecond + " ms }" + Environment.NewLine;
            Cmd.setResultText(Cmd.txtResult + Environment.NewLine + msg);
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
        public string _T_formatMsg(string message, string[] info)
        {
            return swapFields(message, info);
        }

    }

}