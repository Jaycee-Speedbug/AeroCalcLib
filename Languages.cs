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
            if (!Library.Exists(x => x.isoCode == lang.isoCode))
            {
                Library.Add(lang);
            }
        }


        public Language Find(string languageISOCode)
        {
            return (Library.Find(item => item.isoCode == languageISOCode));
        }

        public int languageIndex(string languageISOCode)
        {
            Language lang = Library.Find(item => item.isoCode == languageISOCode);
            if (lang is null)
            {
                return -1;
            }
            return Library.IndexOf(lang);
        }
    }
}