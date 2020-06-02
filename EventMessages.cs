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
        }


        /*
         * SERVICES
         */

        public void Clear() => eMsgList.Clear();



        public void Add(EventMessage em) => eMsgList.Add(em);



        public bool addItem(int eventCode, string eventMessage)
        {
            EventMessage eMsg = new EventMessage(eventCode, eventMessage);
            if (eMsgList.Exists(eMsg => eMsg.msgID == eventCode))
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



        public string getMessageWith(int eventCode)
        {
            EventMessage EM = getEventMessage(eventCode);
            if (EM != null)
            {
                return EM.ToString();
            }
            else return "";
        }



        /*
         * METHODES
         */

        private EventMessage getEventMessage(int eventCode)
        {
            // eventCode to messageID, les codes d'événement négatifs (erreurs) sont changés en ajoutant 10000 à leur valeur absolue
            if (eventCode < 0)
            {
                int messageID = 10000 + Math.Abs(eventCode);
            }

            return eMsgList.Find(em => em.msgID == eventCode);
        }

    }

}