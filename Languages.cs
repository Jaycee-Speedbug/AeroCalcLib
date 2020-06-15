using System;
using System.Collections.Generic;



namespace AeroCalcCore
{



    public class Languages
    {
        /*
         * PROPRIETES
         */

        public List<Language> Library { get; private set; }
        public int Count => Library.Count;
        public int Capacity => Library.Capacity;



        /*
         * CONSTRUCTEURS
         */
        public Languages()
        {
            Library = new List<Language>();
        }



        /*
         * SERVICES
         */
        public void Clear() => Library.Clear();

        public void Add(Language lang)
        {
            if (!Library.Exists(x => x.shortName == lang.shortName))
            {
                Library.Add(lang);
            }
        }


        public Language Find(string langShortName)
        {
            return (Library.Find(item => item.shortName == langShortName));
        }

        public int languageIndex(string langShortName)
        {
            Language lang = Library.Find(item => item.shortName == langShortName);
            if (lang is null)
            {
                return -1;
            }
            return Library.IndexOf(lang);
        }
    }
}