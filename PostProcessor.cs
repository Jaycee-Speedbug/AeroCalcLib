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
        readonly string[] fields = { FIELD_RAW_TXT_CMD, FIELD_MODEL_NAME, FIELD_MODEL_FACTOR, FIELD_MODELS_IN_MEM };



        /*
         * MEMBRES
         */
        private EventMessages EMsgLib;



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



        }



        /*
         * SERVICES
         */
        public void postProcess(AeroCalcCommand Cmd)
        {

            if (Cmd.eventCode == AeroCalcCommand.EVENTCODE_INITIAL)
            {
                // Command totally unprocessed, not a normal situation
                string msg = "[" + Cmd.eventCode + "]" + " POSTPROC:UNPROCESSED COMMAND";
                Cmd.setResultText(msg);
                // TODO Faut-il autoriser la demande EXIT par le PostProcessor ?
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
                    string msg = formatMsg(EMsgLib.getMessageWith(Cmd.eventCode), Cmd.info);
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
                string msg = formatMsg(EMsgLib.getMessageWith(Cmd.eventCode), Cmd.info);
                if (string.IsNullOrEmpty(msg))
                {
                    // Nothing returned
                    msg = "[" + Cmd.eventCode + "]" + " POSTPROC:ERROR MSG NOT IMPLEMENTED";
                }
                Cmd.setResultText(msg);
            }


        }



        /*
         * METHODES
         */

        private string formatMsg(string message, string[] info)
        {
            if (info != null)
            {
                if (message.Contains("$0"))
                {
                    // Un code à remplacer est identifié
                    for (int index = 0; index < info.Length; index++)
                    {
                        // System.Console.WriteLine("Looking for : " + string.Concat("$", index.ToString()));
                        message = message.Replace(string.Concat("$", index.ToString()), info[index]);
                        //message.Replace("$", info[index]);
                        // /!\ BUG
                    }

                }
            }
            return message;
        }
        /// <summary>
        /// Accesseur de Test
        /// </summary>
        public string _T_formatMsg(string message, string[] info)
        {
            return formatMsg(message, info);
        }

    }

}