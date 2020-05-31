using System;
using System.Collections.Generic;



namespace AeroCalcCore
{



    public class Languages
    {
        /*
         * PROPRIETES
         */

        public List<Language> LanguageLibrary { get; private set; }
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
        public void Clear() => LanguageLibrary.Clear();

        public void Add(Language lang)
        {
            if (!LanguageLibrary.Exists(x => x.shortName == lang.shortName))
            {
                LanguageLibrary.Add(lang);
            }
        }


        public Language Find(string langShortName)
        {
            return (LanguageLibrary.Find(item => item.shortName == langShortName));
        }

        public int languageIndex(string langShortName)
        {
            Language lang = LanguageLibrary.Find(item => item.shortName == langShortName);
            if (lang is null)
            {
                return -1;
            }
            return LanguageLibrary.IndexOf(lang);
        }
    }
}