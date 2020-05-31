using System;
using System.Collections.Generic;



namespace AeroCalcCore
{



    public class Languages
    {


        /*
         * PROPRIETES
         */

        List<Language> LanguageLibrary;

        public void Clear() => LanguageLibrary.Clear();

        public int Count => LanguageLibrary.Count;
        public int Capacity => LanguageLibrary.Capacity;



        /*
         * CONSTRUCTEURS
         */
        public Languages()
        {
            LanguageLibrary = new List<Language>();
        }


        /*
         * SERVICES
         */
        public void Add(Language lang)
        {
            if (!LanguageLibrary.Exists(x => x.shortName == lang.shortName))
            {
                LanguageLibrary.Add(lang);
            }
        }
    }
}