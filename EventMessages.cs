using System;
using System.Collections.Generic;



namespace AeroCalcCore
{



    public class EventMessages
    {
        /*
         * PROPRIETES
         */
        List<EventMessage> eMsgList;


        public int Count => eMsgList.Count;
        public int Capacity => eMsgList.Capacity;



        /*
         * CONSTRUCTEURS
         */
        /// <summary>
        /// Construit un objet EventMessage
        /// </summary>
        public EventMessages()
        {
            eMsgList = new List<EventMessage>();
            // Minimal Library to be implemented

        }


        /*
         * SERVICES
         */

        public void Clear() => eMsgList.Clear();



        public void Add(EventMessage em) => addItem(em.msgID,em.msgStr);



        public bool addItem(int eventCode, string eventMessage)
        {
            EventMessage eMsg = new EventMessage(eventCode, eventMessage);
            if (eMsgList.Exists(elem => elem.msgID == eventCode))
            {
                /// No two EventMessage with the same id
                return false;
            }
            else
            {
                eMsgList.Add(eMsg);
                return true;
            }
        }



        /// <summary>
        /// Retourne la string contenant le message correspondant à l'eventCode passé en argument
        /// </summary>
        public string getMessageWith(int eventCode)
        {
            EventMessage EMList = getEventMessage(eventCode);

            if (EMList != null)
            {
                return EMList.ToString();
            }
            else return "";
        }



        /*
         * METHODES
         */

        /// <summary>
        /// Retourne un objet EventMessage correspondant à l'eventCode passé en argument
        /// </summary>
        private EventMessage getEventMessage(int eventCode)
        {
            int messageID;
            // eventCode to messageID, les codes d'événement négatifs (erreurs) sont changés en ajoutant 10000 à leur valeur absolue
            if (eventCode < 0)
            {
                messageID = 10000 + Math.Abs(eventCode);
            }
            else
            {
                messageID = eventCode;
            }
            return eMsgList.Find(em => em.msgID == messageID);
        }
        public EventMessage _A_getEventMessage(int eventCode) { return getEventMessage(eventCode); }

    }

}