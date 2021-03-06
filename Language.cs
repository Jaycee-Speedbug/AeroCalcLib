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

        /// <summary>
        /// Nom abrégé en deux lettres, constitue l'ID qui doit être unique dans la liste
        /// Doit se conformer à ISO 639-1 Code
        /// </summary>
        public string isoCode { get; private set; }

        /// <summary>Chemin absolu vers le fichier de définition du language</summary>
        public string fileAbsolutePath { get; private set; }

        /// <summary>Flag d'activation du language</summary>
        public bool enabled { get; private set; }


        /*
         * CONSTRUCTEURS
         */
        /// <summary>
        /// Construit un objet Language
        /// </summary>
        public Language(string name, string isoCode, string fileAbsolutePath, bool enabled)
        {
            this.name = name;
            this.isoCode = isoCode;
            this.fileAbsolutePath = fileAbsolutePath;
            this.enabled = enabled;
        }

        /// <summary>
        /// Construit un objet Language par copie
        /// </summary>
        public Language(Language lang)
        {
            name = lang.name;
            isoCode = lang.isoCode;
            fileAbsolutePath = lang.fileAbsolutePath;
            enabled = lang.enabled;
        }



        /*
         * SERVICES
         */
        public void disableLanguage() { enabled = false; }

        public void enableLanguage() { enabled = true; }



        /// <summary>
        /// Surcharge la fonction ToString et produit une string permettant l'utilisation dans un message à l'utilisateur
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            string msg = "";
            msg += isoCode + " ";
            msg += name + " ";
            msg += enabled ? "ENABLED " : "DISABLED ";
            msg += fileAbsolutePath + " ";
            return msg;
        }


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
            if (this.isoCode == lang.isoCode)
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
                return (isoCode == lang.isoCode);
            }
        }



        // IEquatable
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            // throw new System.NotImplementedException();

            string fullName = this.isoCode + this.name;
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