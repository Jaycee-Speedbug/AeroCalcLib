using System;
using System.Collections.Generic;



namespace AeroCalcCore
{



    public class EventMessages
    {

        List<EventMessage> eMsgList;


        /*
         * PROPRIETES
         */

        public int Count => eMsgList.Count;
        public int Capacity => eMsgList.Capacity;



        /*
         * CONSTRUCTEURS
         */
        public EventMessages()
        {
            eMsgList = new List<EventMessage>();




        }



        public void Clear() => eMsgList.Clear();



        public void Add(EventMessage em) => eMsgList.Add(em);



        public bool addItem(int eventCode, string eventMessage)
        {
            EventMessage eMsg = new EventMessage(eventCode, eventMessage);
            if (eMsgList.Exists(eMsg => eMsg.eCode == eventCode))
            {
                /// No double EventMessage with the same id
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
            EventMessage EM = eMsgList.Find(em => em.eCode == eventCode);
            if (EM != null)
            {
                return EM.ToString();
            }
            else return "";
        }



        public EventMessage getEventMessage(int eventCode) {
            return eMsgList.Find(em => em.eCode == eventCode);
        }

    }


}