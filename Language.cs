using System;



namespace AeroCalcCore
{
    public class Language : IEquatable<Language>
    {
        /*
         * PROPRIETES
         */
        /// <summary>Nom complet du language</summary>
        public string name { get; private set; }

        /// <summary>Nom abrégé en deux lettres, constitue l'ID qui doit être unique dans la liste</summary>
        public string shortName { get; private set; }

        /// <summary>Chemin absolu vers le fichier de définition du language</summary>
        public string fileAbsolutePath { get; private set; }

        /// <summary>Flag d'activation du language</summary>
        public bool enabled { get; private set; }


        /*
         * CONSTRUCTEURS
         */
        /// <summary>Construit un objet Language</summary>
        public Language(string name, string shortName, string fileAbsolutePath, bool enabled)
        {
            this.name = name;
            this.shortName = shortName;
            this.fileAbsolutePath = fileAbsolutePath;
            this.enabled = enabled;
        }
        /// <summary>Construit un objet Language par copie</summary>
        public Language(Language lang)
        {
            this.name = lang.name;
            this.shortName = lang.shortName;
            this.fileAbsolutePath = lang.fileAbsolutePath;
            this.enabled = lang.enabled;
        }



        /*
         * SERVICES
         */
        public void disableLanguage() { enabled = false; }

        public void enableLanguage() { enabled = true; }



        /*
         * INTERFACE
         */
         
        // IEquatable
        public bool Equals(Language lang)
        {
            if (lang == null)
            {
                return false;
            }
            if (this.shortName == lang.shortName)
            {
                return true;
            }
            else
            {
                return false;
            }

        }



        // IEquatable
        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                Language lang = (Language)obj;
                return (shortName == lang.shortName);
            }
        }



        // IEquatable
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            // throw new System.NotImplementedException();

            string fullName = this.shortName + this.name;
            int hashCode=0;

            foreach (char c in fullName)
            {
                hashCode += (int)c;
                if (hashCode > int.MaxValue - 256) break;
            }
            return hashCode;
        }

    }

}